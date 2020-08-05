using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class PropertyExtensions
    {
        public static IList<Property> ExpandByValues(this IEnumerable<Property> properties)
        {
            var result = properties.SelectMany(property =>
            {
                var propertyValues = property.Values.Select(v =>
                {
                    var clonedProperty = (Property)property.Clone();
                    clonedProperty.Values = new List<PropertyValue> { v };
                    return clonedProperty;
                }).ToList();

                if (propertyValues.IsNullOrEmpty())
                {
                    propertyValues = new List<Property> { (Property) property.Clone() };
                }

                return propertyValues;
            });

            return result.ToList();
        }
    }
}
