using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OSControlSystem.Models
{
    public class Reprogramacion
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Etapa")]
        public int EtapaId { get; set; }
        public Etapa Etapa { get; set; }

        public string Motivo { get; set; }

        public DateTime Fecha { get; set; }

        public bool Notificado { get; set; } = false;
    }
}
