using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiWithGraphQL.Model
{
    /// <summary>
    /// Ele será o responsável pelo nosso modelo de dados.
    /// </summary>
    public abstract class StarWarsCharacter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Friends { get; set; }
        public int[] AppearsIn { get; set; }
    }

    public enum Episodes
    {
        NEWHOPE = 4,
        EMPIRE = 5,
        JEDI = 6
    }
}
