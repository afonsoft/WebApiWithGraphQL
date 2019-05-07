using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using WebApiWithGraphQL.Data;

namespace WebApiWithGraphQL.GraphQL
{
    /// <summary>
    /// Para que nossa API GraphQL funcione precisamos configurar o nosso schema de consulta e seu objetos
    /// </summary>
    public class StarWarsQuery : ObjectGraphType
    {
        public StarWarsQuery(StarWarsData data)
        {

            Name = "Query";
            Field<CharacterInterface>("hero", resolve: context => data.GetDroidByIdAsync("3"));
            Field<HumanType>(
                "human",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the human" }
                ),
                resolve: context => data.GetHumanByIdAsync(context.GetArgument<string>("id"))
            );
            Field<DroidType>(
                "droid",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the droid" }
                ),
                resolve: context => data.GetDroidByIdAsync(context.GetArgument<string>("id"))
            );
        }
    }
}
