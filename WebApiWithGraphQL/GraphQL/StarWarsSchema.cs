using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;

namespace WebApiWithGraphQL.GraphQL
{
    /// <summary>
    /// ele é o responsável pelo gerenciamento do nosso schema
    /// </summary>
    public class StarWarsSchema : Schema
    {
        public StarWarsSchema(Func<Type, IGraphType> resolveType) : base(resolveType)
        {
            Query = (StarWarsQuery)resolveType(typeof(StarWarsQuery));
        }

       public StarWarsSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<StarWarsQuery>();
        }
    }
}
