using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityBuddy.Models
{
    [Table("Materias")]
    public class Materias
    {
        [Key]
        public int ID_Materia { get; set; }
        public int ID_Usuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(100)]
        public string Maestro { get; set; }

        [StringLength(100)]
        public string Contacto { get; set; }

        [StringLength(100)]
        public string Aula { get; set; }

        public TimeSpan? Horario { get; set; }

        [StringLength(100)]
        public string ImagenMateria { get; set; }
    }
}

