using GraphQL;
using GraphQL.Builders;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.XProfile.Commands;
using VirtoCommerce.ExperienceApiModule.XProfile.Queries;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class ProfileSchema : ISchemaBuilder
    {
        public const string _commandName = "command";

        private readonly IMediator _mediator;
        private readonly IDataLoaderContextAccessor _dataLoader;

        public ProfileSchema(IMediator mediator, IDataLoaderContextAccessor dataLoader)
        {
            _mediator = mediator;
            _dataLoader = dataLoader;
        }

        public void Build(ISchema schema)
        {
            //Queries

            /* organization query with contacts connection filtering:
            {
              organization(id: "689a72757c754bef97cde51afc663430"){
                 id contacts(first:10, after: "0", searchPhrase: null){
                  totalCount items {id firstName}
                }
              }
            }
             */
            schema.Query.AddField(new FieldType
            {
                Name = "organization",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" }),
                Type = GraphTypeExtenstionHelper.GetActualType<OrganizationType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var organizationId = context.GetArgument<string>("id");

                    var getOrganizationByIdQuery = new GetOrganizationByIdQuery(organizationId);
                    var organizationAggregate = await _mediator.Send(getOrganizationByIdQuery);

                    //store organization aggregate in the user context for future usage in the graph types resolvers
                    context.UserContext.Add("organizationAggregate", organizationAggregate);

                    return organizationAggregate;
                })
            });

            /// <example>
#pragma warning disable S125 // Sections of code should not be commented out
            /*
                         {
                          contact(id: "51311ae5-371c-453b-9394-e6d352f1cea7"){
                              firstName memberType organizationIds organizations { id businessCategory description emails groups memberType name outerId ownerId parentId phones seoObjectType }
                              addresses { line1 phone }
                         }
                        }
                         */
#pragma warning restore S125 // Sections of code should not be commented out
            /// </example>
            schema.Query.AddField(new FieldType
            {
                Name = "contact",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" }),
                Type = GraphTypeExtenstionHelper.GetActualType<ContactType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var getContactByIdQuery = new GetContactByIdQuery(context.GetArgument<string>("id"));
                    var contactAggregate = await _mediator.Send(getContactByIdQuery);

                    //store organization aggregate in the user context for future usage in the graph types resolvers
                    context.UserContext.Add("contactAggregate", contactAggregate);

                    return contactAggregate;
                })
            });

            /// sample code for updating addresses:
#pragma warning disable S125 // Sections of code should not be commented out
            /*
                        mutation updateAddresses($command: InputUpdateContactAddressType!){
                        contact: updateAddresses(command: $command)
                          {
                            firstName lastName
                            addresses { key city countryCode countryName email firstName  lastName line1 line2 middleName name phone postalCode regionId regionName zip }
                          }
                        }
                        query variables:
                        {
                            "command": {
                              "contactId": "acc3b262-a21e-45f9-a612-b4b1530d27ef",
                              "addresses": [{"addressType": "Shipping", "name": "string", "countryCode": "string", "countryName": "string", "city": "string", "postalCode": "string", "line1": "string", "regionId": "string", "regionName": "string", "firstName": "string", "lastName": "string", "phone": "string", "email": "string", "regionId": "string"
                                }]
                            }
                        }
                         */
#pragma warning restore S125 // Sections of code should not be commented out
            _ = schema.Mutation.AddField(FieldBuilder.Create<ContactAggregate, ContactAggregate>(typeof(ContactType))
                            .Name("updateAddresses")
                            .Argument<NonNullGraphType<InputUpdateContactAddressType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<UpdateContactAddressesCommand>(_commandName)))
                            .FieldType);

            /// <example>
            ///mutation($command: OrganizationInputType!){
            ///    updateOrganization(command: $command){
            ///        name addresses { line1 }
            ///    }
            ///}
            /// </example>
            _ = schema.Mutation.AddField(FieldBuilder.Create<OrganizationAggregate, OrganizationAggregate>(typeof(OrganizationType))
                            .Name("updateOrganization")
                            .Argument<NonNullGraphType<InputUpdateOrganizationType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<UpdateOrganizationCommand>(_commandName)))
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<OrganizationAggregate, OrganizationAggregate>(typeof(OrganizationType))
                            .Name("createOrganization")
                            .Argument<NonNullGraphType<InputCreateOrganizationType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<CreateOrganizationCommand>(_commandName)))
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<ContactAggregate, ContactAggregate>(typeof(ContactType))
                            .Name("createContact")
                            .Argument<NonNullGraphType<InputCreateContactType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<CreateContactCommand>(_commandName)))
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<ContactAggregate, ContactAggregate>(typeof(ContactType))
                            .Name("updateContact")
                            .Argument<NonNullGraphType<InputUpdateContactType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<UpdateContactCommand>(_commandName)))
                            .FieldType);

            _ = schema.Mutation.AddField(FieldBuilder.Create<ContactAggregate, bool>(typeof(BooleanGraphType))
                            .Name("deleteContact")
                            .Argument<NonNullGraphType<InputDeleteContactType>>(_commandName)
                            .ResolveAsync(async context => await _mediator.Send(context.GetArgument<DeleteContactCommand>(_commandName)))
                            .FieldType);

            // Security API fields

#pragma warning disable S125 // Sections of code should not be commented out
            /*
                            {
                                user(id: "1eb2fa8ac6574541afdb525833dadb46"){
                                userName isAdministrator roles { name } userType memberId storeId
                                }
                            }
                         */
#pragma warning restore S125 // Sections of code should not be commented out
            _ = schema.Query.AddField(new FieldType
            {
                Name = "user",
                Arguments = new QueryArguments(
                    new QueryArgument<StringGraphType> { Name = "id" },
                    new QueryArgument<StringGraphType> { Name = "userName" },
                    new QueryArgument<StringGraphType> { Name = "email" },
                    new QueryArgument<StringGraphType> { Name = "loginProvider" },
                    new QueryArgument<StringGraphType> { Name = "providerKey" }),
                Type = GraphTypeExtenstionHelper.GetActualType<UserType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new GetUserQuery(
                        id: context.GetArgument<string>("id"),
                        userName: context.GetArgument<string>("userName"),
                        email: context.GetArgument<string>("email"),
                        loginProvider: context.GetArgument<string>("loginProvider"),
                        providerKey: context.GetArgument<string>("providerKey")));

                    return result;
                })
            });

#pragma warning disable S125 // Sections of code should not be commented out
            /*
                         {
                          getRole(roleName: "Use api"){
                           permissions
                          }
                        }
                         */
#pragma warning restore S125 // Sections of code should not be commented out
            _ = schema.Query.AddField(new FieldType
            {
                Name = "role",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "roleName" }),
                Type = GraphTypeExtenstionHelper.GetActualType<RoleType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = await _mediator.Send(new GetRoleQuery(context.GetArgument<string>("roleName")));

                    return result;
                })
            });

#pragma warning disable S125 // Sections of code should not be commented out
            /*
            mutation ($command: InputCreateUserType!){
                createUser(command: $command){ succeeded errors { code }}
            }
            Query variables:
            {
                "command": {
                "createdBy": "eXp1", "email": "eXp1@mail.com", "password":"eXp1@mail.com", "userName": "eXp1@mail.com", "userType": "Customer"
                }
            }
             */
#pragma warning restore S125 // Sections of code should not be commented out
            _ = schema.Mutation.AddField(FieldBuilder.Create<object, IdentityResult>(typeof(IdentityResultType))
                        .Name("createUser")
                        .Argument<NonNullGraphType<InputCreateUserType>>(_commandName)
                        .ResolveAsync(async context => await _mediator.Send(context.GetArgument<CreateUserCommand>(_commandName)))
                        .FieldType);

#pragma warning disable S125 // Sections of code should not be commented out
            /*
                         mutation ($command: InputUpdateUserType!){
                          updateUser(command: $command){ succeeded errors { description } }
                        }
                        Query variables:
                        {
                         "command":{
                          "isAdministrator": false,
                          "userType": "Customer",
                          "roles": [],
                          "id": "b5d28a83-c296-4212-b89e-046fca3866be",
                          "userName": "_loGIN999",
                          "email": "_loGIN999@gmail.com"
                            }
                        }
                         */
#pragma warning restore S125 // Sections of code should not be commented out
            _ = schema.Mutation.AddField(FieldBuilder.Create<object, IdentityResult>(typeof(IdentityResultType))
                        .Name("updateUser")
                        .Argument<NonNullGraphType<InputUpdateUserType>>(_commandName)
                        .ResolveAsync(async context => await _mediator.Send(context.GetArgument<UpdateUserCommand>(_commandName)))
                        .FieldType);

#pragma warning disable S125 // Sections of code should not be commented out
            /*
             mutation ($command: InputDeleteUserType!){
              deleteUser(command: $command){ succeeded errors { description } }
            }
            Query variables:
            {
              "command": {
                "userNames": ["admin",  "eXp1@mail.com"]
              }
            }
             */
#pragma warning restore S125 // Sections of code should not be commented out
            _ = schema.Mutation.AddField(FieldBuilder.Create<object, IdentityResult>(typeof(IdentityResultType))
                        .Name("deleteUsers")
                        .Argument<NonNullGraphType<InputDeleteUserType>>(_commandName)
                        .ResolveAsync(async context => await _mediator.Send(context.GetArgument<DeleteUserCommand>(_commandName)))
                        .FieldType);

#pragma warning disable S125 // Sections of code should not be commented out
            /*
                         mutation ($command: InputUpdateRoleType!){
                          updateRole(command: $command){ succeeded errors { description } }
                        }
                        Query variables:
                        {
                         "command":{
                         "id": "graphtest",  "name": "graphtest", "permissions": [
                            { "name": "security:call_api", "assignedScopes": [] },
                            { "name": "order:read", "assignedScopes": [{"scope": "{{userId}}", "type": "OnlyOrderResponsibleScope" }] }
                          ]
                         }
                        }
                         */
#pragma warning restore S125 // Sections of code should not be commented out
            _ = schema.Mutation.AddField(FieldBuilder.Create<object, IdentityResult>(typeof(IdentityResultType))
                     .Name("updateRole")
                     .Argument<NonNullGraphType<InputUpdateRoleType>>(_commandName)
                     .ResolveAsync(async context => await _mediator.Send(context.GetArgument<UpdateRoleCommand>(_commandName)))
                     .FieldType);
        }
    }
}