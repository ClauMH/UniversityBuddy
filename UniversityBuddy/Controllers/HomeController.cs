using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversityBuddy.Models;

namespace UniversityBuddy.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly TuDbContext db = new TuDbContext();
        public ActionResult Index()
        {
            if (Session["ID_Usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int idUsuario = Convert.ToInt32(Session["ID_Usuario"]);
            string nombreUsuario = Convert.ToString(Session["Nombre"] ?? "Estudiante");

            var model = new DashboardViewModel
            {
                NombreUsuario = nombreUsuario,
                Materias = db.Materias.Where(m => m.ID_Usuario == idUsuario).ToList(),
                ActividadesProximas = db.Actividades
                                        .Include(a => a.Materias)
                                        .Where(a => a.ID_Usuario == idUsuario && a.Estado != "Completada" && a.Estado != "No hecha")
                                        .OrderBy(a => a.Fecha)
                                        .Take(3)
                                        .ToList()
            };

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Actividades()
        {
            if (Session["ID_Usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int idUsuario = Convert.ToInt32(Session["ID_Usuario"]);
            DateTime hoy = DateTime.Today;

            // REGLA DE NEGOCIO: Pasar a "No hecha" las actividades cuya fecha ya venció
            var vencidas = db.Actividades
                             .Where(a => a.ID_Usuario == idUsuario && a.Fecha < hoy && a.Estado != "Completada" && a.Estado != "No hecha")
                             .ToList();

            if (vencidas.Any())
            {
                foreach (var item in vencidas)
                {
                    item.Estado = "No hecha";
                }
                db.SaveChanges();
            }

            var materiasUsuario = db.Materias.Where(m => m.ID_Usuario == idUsuario).OrderBy(m => m.Nombre).ToList();
            ViewBag.Materias = new SelectList(materiasUsuario, "ID_Materia", "Nombre");
            ViewBag.TieneMaterias = materiasUsuario.Any();

            var lista = db.Actividades
                          .Include(a => a.Materias)
                          .Where(a => a.ID_Usuario == idUsuario)
                          .OrderBy(a => a.Fecha)
                          .ToList();

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarActividad(Actividades actividad)
        {
            if (Session["ID_Usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int idUsuario = Convert.ToInt32(Session["ID_Usuario"]);
            actividad.ID_Usuario = idUsuario;

            // Limpiar validaciones automáticas de campos generados en servidor
            ModelState.Remove("ID_Usuario");

            if (actividad.ID_Actividad == 0)
            {
                actividad.Estado = "Pendiente";
                ModelState.Remove("Estado");
            }

            // Validaciones personalizadas
            if (actividad.ID_Actividad == 0 && actividad.Fecha.Date < DateTime.Today)
            {
                ModelState.AddModelError("Fecha", "La fecha no puede ser menor a la fecha actual.");
            }

            bool materiaValida = db.Materias.Any(m => m.ID_Materia == actividad.ID_Materia && m.ID_Usuario == idUsuario);
            if (!materiaValida)
            {
                ModelState.AddModelError("ID_Materia", "La materia seleccionada no es válida.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (actividad.ID_Actividad == 0)
                    {
                        db.Actividades.Add(actividad);
                    }
                    else
                    {
                        var actDB = db.Actividades.FirstOrDefault(a => a.ID_Actividad == actividad.ID_Actividad && a.ID_Usuario == idUsuario);
                        if (actDB != null)
                        {
                            actDB.Nombre = actividad.Nombre;
                            actDB.Categoria = actividad.Categoria;
                            actDB.ID_Materia = actividad.ID_Materia;
                            actDB.Descripcion = actividad.Descripcion;
                            actDB.Fecha = actividad.Fecha;

                            if (actividad.Fecha.Date >= DateTime.Today && actDB.Estado == "No hecha")
                            {
                                actDB.Estado = "Pendiente";
                            }
                        }
                    }

                    db.SaveChanges();
                    return RedirectToAction("Actividades");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar en base de datos: " + ex.GetBaseException().Message);
                }
            }

            var recargaMaterias = db.Materias.Where(m => m.ID_Usuario == idUsuario).ToList();
            ViewBag.Materias = new SelectList(recargaMaterias, "ID_Materia", "Nombre", actividad.ID_Materia);
            ViewBag.TieneMaterias = recargaMaterias.Any();

            var lista = db.Actividades.Include(a => a.Materias).Where(a => a.ID_Usuario == idUsuario).ToList();
            return View("Actividades", lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarEstadoDirecto(int id)
        {
            if (Session["ID_Usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int idUsuario = Convert.ToInt32(Session["ID_Usuario"]);
            var act = db.Actividades.FirstOrDefault(a => a.ID_Actividad == id && a.ID_Usuario == idUsuario);

            if (act != null)
            {
                // Alternar estado: Si ya estaba completada, la regresa a Pendiente/No hecha. Si no, la completa.
                if (act.Estado == "Completada")
                {
                    act.Estado = (act.Fecha.Date < DateTime.Today) ? "No hecha" : "Pendiente";
                }
                else
                {
                    act.Estado = "Completada";
                }

                db.SaveChanges(); // 💾 Guarda directo en SQL Server
            }

            return RedirectToAction("Actividades"); // Recarga la tabla con los datos actualizados
        }

        [HttpGet]
        public JsonResult ObtenerActividad(int id)
        {
            if (Session["ID_Usuario"] == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            int idUsuario = Convert.ToInt32(Session["ID_Usuario"]);

            // 1. Obtener la entidad de SQL primero
            var act = db.Actividades.FirstOrDefault(a => a.ID_Actividad == id && a.ID_Usuario == idUsuario);

            if (act == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            // 2. Mapear y formatear la fecha en memoria
            return Json(new
            {
                act.ID_Actividad,
                act.Nombre,
                act.Categoria,
                act.ID_Materia,
                act.Descripcion,
                Fecha = act.Fecha.ToString("yyyy-MM-dd"),
                act.Estado
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarActividad(int id)
        {
            if (Session["ID_Usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int idUsuario = Convert.ToInt32(Session["ID_Usuario"]);
            var act = db.Actividades.FirstOrDefault(a => a.ID_Actividad == id && a.ID_Usuario == idUsuario);

            if (act != null)
            {
                db.Actividades.Remove(act);
                db.SaveChanges();
            }

            return RedirectToAction("Actividades");
        }

        public ActionResult Horario()
        {
            if (Session["ID_Usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public ActionResult Calendario()
        {
            return View();
        }


        public ActionResult Pomodoro()
        {
            if (Session["ID_Usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public ActionResult SOS()
        {
            if (Session["ID_Usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public ActionResult Materias(int? idEditar)
        {
            if (Session["ID_Usuario"] == null)
                return RedirectToAction("Login", "Account");

            int idUsuario = Convert.ToInt32(Session["ID_Usuario"]);

            List<Materias> lista = db.Materias
                .Where(m => m.ID_Usuario == idUsuario)
                .OrderByDescending(m => m.ID_Materia)
                .ToList();

            Materias materiaFormulario = new Materias();
            if (idEditar.HasValue)
            {
                materiaFormulario = lista.FirstOrDefault(m => m.ID_Materia == idEditar.Value) ?? new Materias();
            }

            ViewBag.MateriaActual = materiaFormulario;
            return View(lista);
        }

        // POST: Home/GuardarMateria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarMateria(Materias model)
        {
            if (Session["ID_Usuario"] == null)
                return RedirectToAction("Login", "Account");

            int idUsuario = Convert.ToInt32(Session["ID_Usuario"]);

            if (string.IsNullOrWhiteSpace(model.Nombre))
                return RedirectToAction("Materias");

            try
            {
                if (model.ID_Materia == 0)
                {
                    model.ID_Usuario = idUsuario;
                    model.Nombre = model.Nombre.Trim();
                    model.Maestro = model.Maestro?.Trim();
                    model.Contacto = model.Contacto?.Trim();
                    model.Aula = model.Aula?.Trim();
                    model.ImagenMateria = model.ImagenMateria?.Trim();

                    db.Materias.Add(model);
                    db.SaveChanges();
                }
                else
                {
                    var existente = db.Materias
                        .FirstOrDefault(m => m.ID_Materia == model.ID_Materia && m.ID_Usuario == idUsuario);

                    if (existente != null)
                    {
                        existente.Nombre = model.Nombre.Trim();
                        existente.Maestro = model.Maestro?.Trim();
                        existente.Contacto = model.Contacto?.Trim();
                        existente.Aula = model.Aula?.Trim();
                        existente.Horario = model.Horario;
                        existente.ImagenMateria = model.ImagenMateria?.Trim();

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMateria"] = ex.InnerException?.InnerException?.Message ?? ex.Message;
            }

            return RedirectToAction("Materias");
        }

        // POST: Home/EliminarMateria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarMateria(int id)
        {
            if (Session["ID_Usuario"] == null)
                return RedirectToAction("Login", "Account");

            int idUsuario = Convert.ToInt32(Session["ID_Usuario"]);

            try
            {
                var existente = db.Materias
                    .FirstOrDefault(m => m.ID_Materia == id && m.ID_Usuario == idUsuario);

                if (existente != null)
                {
                    db.Materias.Remove(existente);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMateria"] = ex.InnerException?.InnerException?.Message ?? ex.Message;
            }

            return RedirectToAction("Materias");
        }


    }
}
