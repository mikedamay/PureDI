using System;
using System.Linq;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIOCCDocumentor
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMarkdownProcessor, MarkdownProcessor>();
            services.Add(new ServiceDescriptor(typeof(IDiagnosticProcessor)
              , new SimpleIOCContainer().CreateAndInjectDependencies<IDiagnosticProcessor>()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env
          , IDiagnosticProcessor diagnosticProcessor)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseFileServer();

            app.Run(async (context) =>
            {
                string str = diagnosticProcessor.ProcessDiagnostic(
                  new String(context.Request.Path.Value.Skip(1).ToArray()));
                await context.Response.WriteAsync(str);
            });
        }
    }

    public interface IMarkdownProcessor

    {
        string ProcessFragment(string someMarkdown);
    }
}
