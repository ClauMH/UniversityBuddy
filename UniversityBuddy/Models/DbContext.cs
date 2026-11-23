using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;

namespace UniversityBuddy.Models
{
    public class TuDbContext : DbContext
    {
        public TuDbContext() : base("name=BaseAmigo")
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
        //public DbSet<Actividades> Actividades { get; set; }
        //public DbSet<Materias> Materias { get; set; }
    }
}