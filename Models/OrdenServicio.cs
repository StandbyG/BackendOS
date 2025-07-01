using System;
using System.ComponentModel.DataAnnotations;

namespace OSControlSystem.Models
{
    public class OrdenServicio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(9)]
        public string Codigo { get; set; }

        [Required]
        public string Estado { get; set; } // AST aprobado, IN-PROCESS, DONE

        [Required]
        public string EtapaActual { get; set; } // Desarme o Armado

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }
        public ICollection<Etapa> Etapas { get; set; }

    }
}
