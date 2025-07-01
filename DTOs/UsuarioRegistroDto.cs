namespace OSControlSystem.DTOs
{
    public class UsuarioRegistroDto
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; } // "Administrador", "Técnico", "Administrativo"
        public string Password { get; set; }
    }
}
