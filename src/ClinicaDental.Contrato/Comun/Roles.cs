namespace ClinicaDental.Contrato.Comun
{
    /// <summary>Roles válidos (los mismos que ClinicaDental.Web.Models.Roles).</summary>
    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Recepcionista = "Recepcionista";
        public const string Odontologo = "Odontologo";

        public static readonly string[] Todos = [Administrador, Recepcionista, Odontologo];

        public static string Texto(string rol) => rol == Odontologo ? "Odontólogo" : rol;
    }
}
