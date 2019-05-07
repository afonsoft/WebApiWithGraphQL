using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiWithGraphQL.Model
{
    public class Human : StarWarsCharacter
    {
        public string HomePlanet { get; set; }
    }
}
