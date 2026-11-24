using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversityBuddy.Models;

namespace UniversityBuddy.Controllers
{
    public class HomeController : Controller
    {
        private readonly TuDbContext db = new TuDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Asignaciones()
        {
            return View();
        }

        public ActionResult Horario()
        {
            return View();
        }

        public ActionResult Calendario()
        {
            return View();
        }


        public ActionResult Pomodoro()
        {
            return View();
        }

        public ActionResult SOS()
        {
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



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
