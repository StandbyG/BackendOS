using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OSControlSystem.Models
{
    public class Notificacion
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("MotivoParada")]
        public int MotivoParadaId { get; set; }
        public MotivoParada MotivoParada { get; set; }

        public string Mensaje { get; set; }

        public bool Personalizable { get; set; } = true;
    }
}
