using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputAddressType : InputObjectGraphType<Address>
    {
        public InputAddressType()
        {
            Field(x => x.City, nullable: true).Description("City");
            Field(x => x.CountryCode, nullable: true).Description("Country code");
            Field(x => x.CountryName, nullable: true).Description("Country name");
            Field(x => x.Email, nullable: true).Description("Email");
            Field(x => x.FirstName, nullable: true).Description("First name");
            Field(x => x.Key, nullable: true).Description("Id");
            Field(x => x.LastName, nullable: true).Description("Last name");
            Field(x => x.Line1, nullable: true).Description("Line1");
            Field(x => x.Line2, nullable: true).Description("Line2");
            Field(x => x.MiddleName, nullable: true).Description("Middle name");
            Field(x => x.Name, nullable: true).Description("Name");
            Field(x => x.Organization, nullable: true).Description("Company name");
            Field(x => x.Phone, nullable: true).Description("Phone");
            Field(x => x.PostalCode, nullable: true).Description("Postal code");
            Field(x => x.RegionId, nullable: true).Description("Region id");
            Field(x => x.RegionName, nullable: true).Description("Region name");
            Field(x => x.Zip, nullable: true).Description("Zip");
        }
    }
}
