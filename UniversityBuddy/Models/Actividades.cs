using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UniversityBuddy.Models
{
    [Table("Actividades")]
    public class Actividades
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Actividad { get; set; }

        public int ID_Usuario { get; set; }

        [Required(ErrorMessage = "El nombre de la actividad es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        [Display(Name = "Nombre de la Actividad")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría.")]
        [StringLength(100, ErrorMessage = "La categoría no es válida.")]
        [Display(Name = "Categoría")]
        public string Categoria { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una materia.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una materia válida.")]
        [Display(Name = "Materia")]
        public int ID_Materia { get; set; }

        [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha de entrega es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Entrega")]
        public DateTime Fecha { get; set; }

        [StringLength(100)]
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        // Propiedad de navegación
        [ForeignKey("ID_Materia")]
        public virtual Materias Materias { get; set; }
    }
}
