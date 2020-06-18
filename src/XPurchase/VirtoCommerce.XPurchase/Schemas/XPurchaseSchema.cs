using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.XPurchase.Domain.Builders;
using VirtoCommerce.XPurchase.Domain.Factories;
using VirtoCommerce.XPurchase.Models.Cart;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class XPurchaseSchema : ISchemaBuilder
    {
        private readonly IMediator _mediator;
        private readonly IDataLoaderContextAccessor _dataLoader;
        private readonly IShoppingCartAggregateFactory _cartFactory;

        public XPurchaseSchema(IMediator mediator, IDataLoaderContextAccessor dataLoader, IShoppingCartAggregateFactory cartFactory)
        {
            _mediator = mediator;
            _dataLoader = dataLoader;
            _cartFactory = cartFactory;
        }

        public void Build(ISchema schema)
        {
            var getCartQuery = new FieldType
            {
                Name = "cart",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "storeId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "cartName" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "cultureName" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "currencyCode" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "type" }
                ),
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Resolver = new AsyncFieldResolver<ShoppingCart>(async context =>
                {
                    var storeId = context.GetArgument<string>("storeId");
                    var cartName = context.GetArgument<string>("cartName");
                    var userId = context.GetArgument<string>("userId");
                    var cultureName = context.GetArgument<string>("cultureName");
                    var currencyCode = context.GetArgument<string>("currencyCode");
                    var type = context.GetArgument<string>("type");

                    var shoppingCartContext = CartContextBuilder.Build()
                                                                .WithStore(storeId)
                                                                .WithCartName(cartName)
                                                                .WithUser(userId)
                                                                .WithCurrencyAndLanguage(currencyCode, cultureName)
                                                                .WithCartType(type)
                                                                .GetContext();

                    var cartAggregate = await _cartFactory.CreateOrGetShoppingCartAggregateAsync(shoppingCartContext);

                    await cartAggregate.ValidateAsync();

                    return cartAggregate.Cart;
                })
            };

            schema.Query.AddField(getCartQuery);
        }
    }
}