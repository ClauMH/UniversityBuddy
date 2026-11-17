using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;

namespace UniversityBuddy.Models
{
    [Table("Actividades")]
    public class Actividad : IValidatableObject
    {
        [Key]
        public int ID_Actividad { get; set; }

        public int ID_Usuario { get; set; } 

        [Required(ErrorMessage = "El nombre de la asignación es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        [Display(Name = "Nombre de la Asignación")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría.")]
        [StringLength(100)]
        [Display(Name = "Categoría")]
        public string Categoria { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una materia.")]
        [Display(Name = "Materia")]
        public int ID_Materia { get; set; }

        [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha de entrega es obligatoria.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de Entrega")]
        public DateTime Fecha { get; set; }

        [Required]
        [StringLength(100)]
        public string Estado { get; set; } 

        
        //[ForeignKey("ID_Materia")]
        //public virtual Materia { get; set; }

        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Fecha.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "La fecha de la asignación debe ser futura o el día de hoy.",
                    new[] { nameof(Fecha) }
                );
            }
        }
    }
 }