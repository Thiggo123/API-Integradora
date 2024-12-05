namespace API_Integradora.Models
{
    public class Log
    {
        public int Id { get; set; }
        public int Codigo { get; set; }
        public int Status { get; set; }
        public string Acao { get; set; }
        public double Tempo { get; set; }

        public string LogOriginal { get; set; }

        public string LogConvertido { get; set; }
    }
}
