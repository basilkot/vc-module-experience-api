using AutoMapper;
using GraphQL.Server;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog;
using VirtoCommerce.ExperienceApiModule.XProfile;
using VirtoCommerce.ExperienceApiModule.XProfile.Services;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.ExperienceApiModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection services)
        {

            services.AddMediatR(typeof(Anchor));
            services.AddMediatR(typeof(XProfileAnchor));


            //services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));
            //serviceCollection.AddSingleton(typeof(IRequestPreProcessor<>), typeof(GenericRequestPreProcessor<>));

            //Discover the assembly and  register all mapping profiles through reflection
            services.AddAutoMapper(typeof(Anchor));
            //services.AddAutoMapper(typeof(XProfileAnchor));


            //Register .NET GraphQL server
            services.AddGraphQL(_ =>
            {
                _.EnableMetrics = true;
                _.ExposeExceptions = true;
                _.ExposeExceptions = true;
            }).AddNewtonsoftJson(deserializerSettings => { }, serializerSettings => { })
            .AddUserContextBuilder(context => new GraphQLUserContext { User = context.User })
            .AddRelayGraphTypes()
            .AddGraphTypes(typeof(Anchor))
            .AddGraphTypes(typeof(XProfileAnchor))
            .AddDataLoader();


            //Register custom GraphQL dependencies
            services.AddPermissionAuthorization();
            //services.AddGraphShemaBuilders(typeof(Anchor));
            services.AddGraphShemaBuilders(typeof(XProfileAnchor));
            services.AddTransient<IMemberServiceX, MemberServiceX>();

            services.AddAutoMapper(typeof(Module));
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {

            // add http for Schema at default url /graphql
            appBuilder.UseGraphQL<ISchema>();

            // use graphql-playground at default url /ui/playground
            appBuilder.UseGraphQLPlayground();



        }

        public void Uninstall()
        {
        }

    }
}

