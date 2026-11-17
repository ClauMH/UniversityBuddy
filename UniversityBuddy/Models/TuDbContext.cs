using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;
using UniversityBuddy.Models;

namespace UniversityBuddy.Models
{
    public class TuDbContext : DbContext
    {
        public TuDbContext() : base("name=BaseAmigo")
        {
        }
        public DbSet<Actividad> Actividades { get; set; }
        //public DbSet<Materia> Materias { get; set; }
        //public DbSet<Usuario> Usuarios { get; set; }
    }
}