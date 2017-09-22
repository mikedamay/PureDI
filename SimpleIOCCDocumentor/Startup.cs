using System;
using System.IO;
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
        private IOCCDiagnostics diagnostics2;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            SimpleIOCContainer sic = new SimpleIOCContainer(new string[] {"authoring"});
            services.Add(new ServiceDescriptor(typeof(IDocumentProcessor)
              , sic.CreateAndInjectDependencies<
              IDocumentProcessor>(out diagnostics)));
            services.Add(new ServiceDescriptor(typeof(IDocumentationSiteGenerator)
                , sic.CreateAndInjectDependencies<IDocumentationSiteGenerator>(out diagnostics2)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env
          , IDocumentProcessor documentProcessor, IDocumentationSiteGenerator siteGenerator)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseFileServer();

            app.Run(async (context) =>
            {
                siteGenerator.GenerateSite();
                if (diagnostics.HasWarnings)
                {
                    await context.Response.WriteAsync(diagnostics.ToString());
                    return;
                }
                (string document, string fragment) = GetRouteFromRequest(context.Request.Path.Value);
 
                string str = documentProcessor.ProcessDocument(
                  document, fragment);
                await context.Response.WriteAsync(str);
            });
        }
        /// <summary>
        /// TODO we should really examine how routes work
        /// </summary>
        /// <param name="pathValue">e.g. "/Simple/UserGuide/Introduction.html"</param>
        /// <returns>e.g. ("UserGuide", "Introduction")</returns>
        private (string document, string fragment) GetRouteFromRequest(string pathValue)
        {
            //const int SITE_PART = 1;
            const int DOCUMENT_PART = 2;
            const int FRAGMENT_PART = 3;
            string[] parts = pathValue.Split("/");
            if (parts.Length <= DOCUMENT_PART)
            {
                return (string.Empty, string.Empty);
            }
            string document = parts[DOCUMENT_PART];
            string fragment = parts.Length > FRAGMENT_PART ? parts[FRAGMENT_PART] : string.Empty;
            return (document, Path.ChangeExtension(fragment, null));
        }
    }

    public interface IMarkdownProcessor

    {
        string ProcessFragment(string someMarkdown);
    }
}
