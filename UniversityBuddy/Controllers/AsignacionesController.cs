using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversityBuddy.Models;

namespace UniversityBuddy.Controllers
{
    [Authorize]
    public class AsignacionesController : Controller
    {
        private TuDbContext db = new TuDbContext(); // Reemplazar con tu DbContext

        // GET: Asignaciones (Listado con filtros y actualización de atrasos)
        public ActionResult Index(string estadoFiltro = "Todas")
        {
            int idUsuario = Convert.ToInt32(Session["ID_Usuario"] ?? 1); // Obtiene usuario en sesión

            // 1. Obtener actividades del usuario
            var actividades = db.Actividades
                                .Include(a => a.Materia)
                                .Where(a => a.ID_Usuario == idUsuario)
                                .ToList();

            // 2. Actualizar automáticamente a "Atrasada" si la fecha ya pasó y sigue "En proceso"
            bool hayCambios = false;
            foreach (var act in actividades)
            {
                if (act.Fecha.Date < DateTime.Today && act.Estado == "En proceso")
                {
                    act.Estado = "Atrasada";
                    db.Entry(act).State = EntityState.Modified;
                    hayCambios = true;
                }
            }
            if (hayCambios) db.SaveChanges();

            // 3. Aplicar filtro seleccionado
            if (!string.IsNullOrEmpty(estadoFiltro) && estadoFiltro != "Todas")
            {
                actividades = actividades.Where(a => a.Estado == estadoFiltro).ToList();
            }

            ViewBag.EstadoActual = estadoFiltro;
            return View(actividades);
        }

        // GET: Asignaciones/Create
        public ActionResult Create()
        {
            CargarDesplegables();
            return View(new Actividad { Fecha = DateTime.Today.AddDays(1) });
        }

        // POST: Asignaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Actividad actividad)
        {
            int idUsuario = Convert.ToInt32(Session["ID_Usuario"] ?? 1);
            actividad.ID_Usuario = idUsuario;
            actividad.Estado = "En proceso"; // Estado inicial por defecto

            // Validar que la materia exista en la BD
            if (!db.Materias.Any(m => m.ID_Materia == actividad.ID_Materia))
            {
                ModelState.AddModelError("ID_Materia", "La materia seleccionada no es válida o no existe.");
            }

            if (ModelState.IsValid)
            {
                db.Actividades.Add(actividad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            CargarDesplegables();
            return View(actividad);
        }

        // AJAX POST: Cambiar estado desde el Checkbox
        [HttpPost]
        public JsonResult CambiarEstado(int id, bool completada)
        {
            var actividad = db.Actividades.Find(id);
            if (actividad == null)
            {
                return Json(new { success = false, message = "Asignación no encontrada." });
            }

            if (completada)
            {
                actividad.Estado = "Completada";
            }
            else
            {
                // Si desmarca, vuelve a "En proceso" o "Atrasada" según la fecha
                actividad.Estado = actividad.Fecha.Date < DateTime.Today ? "Atrasada" : "En proceso";
            }

            db.Entry(actividad).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true, nuevoEstado = actividad.Estado });
        }

        private void CargarDesplegables()
        {
            // Cargar materias disponibles
            ViewBag.ID_Materia = new SelectList(db.Materias.OrderBy(m => m.Nombre), "ID_Materia", "Nombre");

            // Cargar categorías permitidas
            var categorias = new List<string> { "Tarea", "Examen", "Exposición", "Proyecto final", "Laboratorio" };
            ViewBag.Categorias = new SelectList(categorias);
        }
    }
}