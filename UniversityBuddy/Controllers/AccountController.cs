using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UniversityBuddy.Models;

namespace UniversityBuddy.Controllers
{
    public class AccountController : Controller
    {
        private TuDbContext db = new TuDbContext();

        // GET: Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = db.Usuarios.FirstOrDefault(u => u.NomUsuario == model.NomUsuario && u.Contrasena == model.Contrasena);

            if (user == null)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                return View(model);
            }

            // Iniciar sesión con FormsAuthentication y Variables de Sesión
            FormsAuthentication.SetAuthCookie(user.NomUsuario, model.Recordarme);

            Session["ID_Usuario"] = user.ID_Usuario;
            Session["NomUsuario"] = user.NomUsuario;
            Session["Nombre"] = user.Nombre;
            Session["FotoPerfil"] = user.FotoPerfil;
            Session["Universidad"] = user.Universidad;

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Registro
        [AllowAnonymous]
        public ActionResult Registro()
        {
            return View();
        }

        // POST: Account/Registro
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(Usuario usuario, HttpPostedFileBase fotoFile)
        {
            // Validar si el usuario ya existe en la Base de Datos
            if (db.Usuarios.Any(u => u.NomUsuario.Equals(usuario.NomUsuario, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("NomUsuario", "El usuario ingresado ya existe. Por favor elija otro.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string folderPath = Server.MapPath("~/Content/Images");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // 1. Si el usuario subió un archivo de imagen
                    if (fotoFile != null && fotoFile.ContentLength > 0)
                    {
                        var extensionesValidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                        var ext = Path.GetExtension(fotoFile.FileName).ToLower();

                        if (!extensionesValidas.Contains(ext))
                        {
                            ModelState.AddModelError("FotoPerfil", "Formato no permitido (Use JPG, PNG, GIF o WEBP).");
                            return View(usuario);
                        }

                        string nombreArchivo = Guid.NewGuid().ToString("N") + ext;
                        string rutaFisica = Path.Combine(folderPath, nombreArchivo);
                        fotoFile.SaveAs(rutaFisica);

                        usuario.FotoPerfil = "/Content/Images/" + nombreArchivo;
                    }
                    else
                    {
                        // 2. Si no seleccionó imagen: Crear archivo SVG físico en disco con la inicial
                        string inicial = string.IsNullOrWhiteSpace(usuario.Nombre) ? "U" : usuario.Nombre.Trim().Substring(0, 1).ToUpper();
                        string nombreArchivoSvg = "avatar_" + Guid.NewGuid().ToString("N") + ".svg";
                        string rutaFisicaSvg = Path.Combine(folderPath, nombreArchivoSvg);

                        string contenidoSvg = $"<svg xmlns='http://www.w3.org/2000/svg' width='128' height='128'><rect width='100%' height='100%' fill='%236366f1'/><text x='50%' y='55%' dominant-baseline='middle' text-anchor='middle' font-size='64' font-family='Arial' font-weight='bold' fill='white'>{inicial}</text></svg>";

                        System.IO.File.WriteAllText(rutaFisicaSvg, contenidoSvg);

                        // La ruta corta entra en el espacio NVARCHAR(100)
                        usuario.FotoPerfil = "/Content/Images/" + nombreArchivoSvg;
                    }

                    // Guardar registro en la Base de Datos
                    db.Usuarios.Add(usuario);
                    db.SaveChanges();

                    TempData["MensajeExito"] = "¡Cuenta creada exitosamente! Ingresa con tus credenciales.";
                    return RedirectToAction("Login");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var entityErrors in ex.EntityValidationErrors)
                    {
                        foreach (var error in entityErrors.ValidationErrors)
                        {
                            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var baseEx = ex.GetBaseException();
                    ModelState.AddModelError("", "Error al guardar en la base de datos: " + baseEx.Message);
                }
            }

            return View(usuario);
        }

        // GET: Account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }
    }
}