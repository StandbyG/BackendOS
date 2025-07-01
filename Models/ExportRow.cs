namespace OSControlSystem.Models
{
    public class ExportRow
    {
        public string CodigoOS { get; set; }
        public string EstadoOS { get; set; }
        public string Etapa { get; set; }
        public string TipoRegistro { get; set; } // "Parada" o "Reprogramación"
        public string? TipoParada { get; set; }   // Solo para paradas
        public string Motivo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
