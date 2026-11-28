using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversityBuddy.Models
{
    public class DashboardViewModel
    {
        public string NombreUsuario { get; set; }
        public List<Materias> Materias { get; set; }
        public List<Actividades> ActividadesProximas { get; set; }
    }
}