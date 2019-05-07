using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Http;
using GraphQL.Server;
using GraphQL.Server.Ui.GraphiQL;
using GraphQL.Server.Ui.Playground;
using GraphQL.Server.Ui.Voyager;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApiWithGraphQL.Data;
using WebApiWithGraphQL.GraphQL;

namespace WebApiWithGraphQL
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Bootstrapper bootstrapper = new Bootstrapper();
            services.AddSingleton<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddSingleton<IDependencyResolver>(bootstrapper.Resolver());
            services.AddSingleton<IDocumentExecuter>(new DocumentExecuter());
            services.AddSingleton<IDocumentWriter>(new DocumentWriter(true));
            services.AddSingleton(new StarWarsData());

            services.AddSingleton<HumanType>();
            services.AddSingleton<DroidType>();
            services.AddSingleton<CharacterInterface>();
            //services.AddSingleton<ISchema, StarWarsSchema>();
            services.AddSingleton(new StarWarsSchema(type => (GraphType)bootstrapper.container.Get(type)));

            // Add GraphQL services and configure options
            services.AddGraphQL(options =>
                {
                    options.EnableMetrics = true;
                    options.ExposeExceptions = true;
                })
                .AddWebSockets() // Add required services for web socket support
                .AddDataLoader(); // Add required services for DataLoader support

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            if (!env.IsDevelopment())
                app.UseHsts();

            app.UseDeveloperExceptionPage();

            // this is required for websockets support
            app.UseWebSockets();

            // use websocket middleware for ChatSchema at path /graphql
            app.UseGraphQLWebSockets<StarWarsSchema>("/graphql");

            // use HTTP middleware for ChatSchema at path /graphql
            app.UseGraphQL<StarWarsSchema>("/graphql");

            // use graphiQL middleware at default url /graphiql
            app.UseGraphiQLServer(new GraphiQLOptions());

            // use graphql-playground middleware at default url /ui/playground
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());

            // use voyager middleware at default url /ui/voyager
            app.UseGraphQLVoyager(new GraphQLVoyagerOptions());

       
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseHttpsRedirection();
            //app.UseMvc();
        }
    }
}
