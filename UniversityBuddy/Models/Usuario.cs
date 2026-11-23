using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityBuddy.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Usuario { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(10, ErrorMessage = "El usuario no puede tener más de 10 caracteres.")]
        [Display(Name = "Usuario")]
        public string NomUsuario { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 9, ErrorMessage = "La contraseña debe tener más de 8 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        [StringLength(100, ErrorMessage = "La universidad no puede superar los 100 caracteres.")]
        [Display(Name = "Universidad")]
        public string Universidad { get; set; }

        [StringLength(100)]
        [Display(Name = "Foto de Perfil")]
        public string FotoPerfil { get; set; }
    }
}