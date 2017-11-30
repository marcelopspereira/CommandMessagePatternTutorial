﻿using System;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using Checkout.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShoppingCart.Application;
using Store.Application;
using Swashbuckle.AspNetCore.Swagger;

namespace OnlineStore.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private Func<IServiceProvider> ServiceProviderFunc { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Online Store API", Version = "v1" });
            });

            ICommandingDependencyResolver commandingDependencyResolver = new CommandingDependencyResolver(
                (type, instance) => services.AddSingleton(type, instance),
                (type, impl) => services.AddTransient(type, impl),
                type => ServiceProviderFunc().GetService(type));
            ICommandRegistry registry = commandingDependencyResolver.UseCommanding();

            services
                .UseShoppingCart(registry)
                .UseStore(registry)
                .UseCheckout(registry);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            ServiceProviderFunc = () => app.ApplicationServices;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Online Store API V1");
            });

            app.UseMvc();
        }
    }
}