namespace ClinicaDental.Contrato.Respuestas.Pago
{
    public class DeudaRespuesta
    {
        public Guid IdTratamiento { get; set; }
        public Guid IdPaciente { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public string PacienteCI { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public DateOnly Fecha { get; set; }
        public decimal Costo { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal Saldo { get; set; }
    }
}
