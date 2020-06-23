using GraphQL.Types;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class UserUpdateInfoInputType : InputObjectGraphType
    {
        public UserUpdateInfoInputType()
        {
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.FirstName));
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.LastName));
            Field<NonNullGraphType<StringGraphType>>(nameof(Contact.FullName));
            Field<NonNullGraphType<StringGraphType>>("Email");
        }
    }
}