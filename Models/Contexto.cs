using Microsoft.EntityFrameworkCore;

namespace API_Integradora.Models
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options) { }

        public DbSet<Log> Logs { get; set; }
    }
}
