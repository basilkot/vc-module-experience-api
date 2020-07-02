using System;
using System.Linq;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class ProfileType : ObjectGraphType<Profile>
    {
        public ProfileType(IMediator mediator, ICustomerOrderSearchService orderSearchService)
        {
            Name = "Profile";
            Description = "Currently logged-in customer information";

            Field(x => x.User.Id).Description("User ID");
            Field(x => x.User.StoreId, nullable: true).Description("User store ID (if any)");
            Field(x => x.User.UserName).Description("User login name");
            Field(x => x.User.PhoneNumber, nullable: true).Description("Phone Number");
            Field(x => x.User.PhoneNumberConfirmed).Description("Is Phone Number confirmed");
            Field(x => x.User.Email, nullable: true).Description("User email");
            Field("isRegisteredUser", x => true).Description("Is User Registered");
            Field("isLockedOut", x => x.User.LockoutEnd != null && x.User.LockoutEnd.Value > DateTime.UtcNow).Description("Is User in locked out state");
            Field(x => x.User.IsAdministrator).Description("Is User Administrator");
            Field(d => d.User.UserType).Description("UserType");
            Field<ContactType>("contact", "Customer contact information", resolve: context => context.Source.Contact);
            Field<ListGraphType<RoleType>>("roles", resolve: context => context.Source.User.Roles);

            //Obsolete??:
            Field("permissions", x => x.User.Roles.SelectMany(x => x.Permissions).Select(x => x.Name).Distinct().ToArray()).Description("All User's Permissions");

            //Obsolete:
            //Field<ListGraphType<RoleType>>("externalLogins", resolve: context => context.Source.User.Logins);

            FieldAsync<BooleanGraphType>("isFirstTimeBuyer", "Indicates that user has no orders", resolve: async context =>
            {
                if (context.Source.User != null)
                {
                    var orderSearchResult = await orderSearchService.SearchCustomerOrdersAsync(new CustomerOrderSearchCriteria
                    {
                        CustomerId = context.Source.User.Id,
                        Take = 0
                    });
                    return orderSearchResult.TotalCount == 0;
                }

                return true;
            });
        }
    }
}
