using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using WebApiWithGraphQL.Model;

namespace WebApiWithGraphQL.GraphQL
{
    public class CharacterInterface : InterfaceGraphType<StarWarsCharacter>
    {
        public CharacterInterface()
        {
            Name = "Character";
            Field(d => d.Id).Description("Id do Personagem");
            Field(d => d.Name, nullable: true).Description("Nome do Personagem");
            Field<ListGraphType<CharacterInterface>>("friends");
            Field<ListGraphType<EpisodeEnum>>("appearsIn", "Qual filme apareceu");
        }
    }
}
