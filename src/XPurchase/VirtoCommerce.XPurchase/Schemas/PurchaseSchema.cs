using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.XPurchase.Commands;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class PurchaseSchema : ISchemaBuilder
    {
        private readonly ICartAggregateRepository _cartAggrRepository;
        private readonly IMediator _mediator;
        public const string _commandName = "command";

        public PurchaseSchema(IMediator mediator, ICartAggregateRepository cartAggrFactory)
        {
            _mediator = mediator;
            _cartAggrRepository = cartAggrFactory;
        }

        public void Build(ISchema schema)
        {
            //Queries
            //We can't use the fluent syntax for new types registration provided by dotnet graphql here, because we have the strict requirement for underlying types extensions
            //and must use GraphTypeExtenstionHelper to resolve the effective type on execution time
            var cartField = new FieldType
            {
                Name = "cart",
                Arguments = new QueryArguments(
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "storeId" },
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "cartName" },
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "userId" },
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "cultureName" },
                        new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "currencyCode" },
                        new QueryArgument<StringGraphType> { Name = "type" }),
                Type = GraphTypeExtenstionHelper.GetActualType<CartType>(),
                Resolver = new AsyncFieldResolver<object>(async context =>
                {
                    //TODO: Move to extension methods
                    var storeId = context.GetArgument<string>("storeId");
                    var cartName = context.GetArgument<string>("cartName");
                    var userId = context.GetArgument<string>("userId");
                    var cultureName = context.GetArgument<string>("cultureName");
                    var currencyCode = context.GetArgument<string>("currencyCode");
                    var type = context.GetArgument<string>("type");

                    var cartAggregate = await _cartAggrRepository.GetOrCreateAsync(cartName, storeId, userId, cultureName, currencyCode, type);

                    await cartAggregate.ValidateAsync();
                    await cartAggregate.RecalculateAsync();

                    //store cart aggregate in the user context for future usage in the graph types resolvers
                    context.UserContext.Add("cartAggregate", cartAggregate);

                    return cartAggregate;
                })
            };
            schema.Query.AddField(cartField);

            //Mutations
            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation (command:InputAddItemType!){ addItem(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": "",
            ///          "productId": "9cbd8f316e254a679ba34a900fccb076",
            ///          "quantity": 1
            ///      }
            ///   }
            /// }
            /// </example>
            var addItemField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                           .Name("addItem")
                                           .Argument<NonNullGraphType<InputAddItemType>>(_commandName)
                                           //TODO: Write the unit-tests for successfully mapping input variable to the command
                                           .ResolveAsync(async context => await _mediator.Send(context.GetCartCommand<AddCartItemCommand>()))
                                           .FieldType;

            schema.Mutation.AddField(addItemField);

            /// <example>
            /// This is an example JSON request for a mutation
            /// {
            ///   "query": "mutation (command:InputClearCartType!){ clearCart(command: $command) {  total { formatedAmount } } }",
            ///   "variables": {
            ///      "command": {
            ///          "storeId": "Electronics",
            ///          "cartName": "default",
            ///          "userId": "b57d06db-1638-4d37-9734-fd01a9bc59aa",
            ///          "language": "en-US",
            ///          "currency": "USD",
            ///          "cartType": ""
            ///      }
            ///   }
            /// }
            /// </example>
            var clearCartField = FieldBuilder.Create<CartAggregate, CartAggregate>(typeof(CartType))
                                             .Name("clearCart")
                                             .Argument<NonNullGraphType<InputClearCartType>>(_commandName)
                                             .ResolveAsync(async context => await _mediator.Send(context.GetCartCommand<ClearCartCommand>()))
                                             .FieldType;

            schema.Mutation.AddField(clearCartField);
        }
    }
}