using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class StuffContext : DbContext
    {
        public IDbSet<Dodad> Dodads { get; set; }
        public IDbSet<Thing> Things { get; set; }
    }
}
