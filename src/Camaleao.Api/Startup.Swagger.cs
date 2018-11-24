﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Reflection;

namespace Camaleao.Api {
    public partial class Startup {
        private void ConfigureSwaggerService(IServiceCollection services) {
            services.AddSwaggerGen(options => {
                string basePath = PlatformServices.Default.Application.ApplicationBasePath;
                string moduleName = GetType().GetTypeInfo().Module.Name.Replace(".dll", ".xml");
                string filePath = Path.Combine(basePath, moduleName);
                string readme = File.ReadAllText(Path.Combine(basePath, "Docs", "README.md"));

                ApiKeyScheme scheme = Configuration.GetSection("ApiKeyScheme").Get<ApiKeyScheme>();

                Info info = Configuration.GetSection("Info").Get<Info>();
                info.Description = readme;
                options.SwaggerDoc(info.Version, info);

                options.IncludeXmlComments(filePath);
                options.IncludeXmlComments(Path.Combine(basePath, "Camaleao.Application.xml"));
                options.DescribeAllEnumsAsStrings();
                options.OperationFilter<ExamplesOperationFilter>();


                options.DescribeAllEnumsAsStrings();

            });
        }

        private void ConfigureSwagger(IApplicationBuilder app, IHostingEnvironment env) {
            app.UseStaticFiles();

            if (env.IsDevelopment()) {
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs"));
            }

            app.UseSwagger(c => c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value));
        }
    }
}

