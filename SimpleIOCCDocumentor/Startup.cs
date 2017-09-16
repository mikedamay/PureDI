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
        private IOCCDiagnostics diagnostics;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMarkdownProcessor, MarkdownProcessor>();
            services.Add(new ServiceDescriptor(typeof(IDocumentProcessor)
              , new SimpleIOCContainer().CreateAndInjectDependencies<IDocumentProcessor>(out diagnostics)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env
          , IDocumentProcessor documentProcessor)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseFileServer();

            app.Run(async (context) =>
            {
                if (diagnostics.HasWarnings)
                {
                    await context.Response.WriteAsync(diagnostics.ToString());
                    return;
                }
                int DOCUMENT_PART = 1, FRAGMENT_PART = 2;
                string[] parts = context.Request.Path.Value.Split("/");

                string str = documentProcessor.ProcessDocument(
                  parts[DOCUMENT_PART], parts.Length > FRAGMENT_PART ? parts[FRAGMENT_PART] : string.Empty);
                await context.Response.WriteAsync(str);
            });
        }
    }

    public interface IMarkdownProcessor

    {
        string ProcessFragment(string someMarkdown);
    }
}
