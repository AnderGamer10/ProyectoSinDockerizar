using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TiempoContext : DbContext
{
    public TiempoContext(DbContextOptions<TiempoContext> options)
        : base(options)
    {
    }

    public DbSet<InformacionTiempo> InformacionTiempo { get; set; }
    public string connString { get; private set; }
    public TiempoContext()
    {
        connString = $"Server=185.60.40.210\\SQLEXPRESS,58015;Database=DB11Ander;User Id=sa;Password=Pa88word;";
        //connString = $"Server=(localdb)\\mssqllocaldb;Database=BD11AnderPrueba;Trusted_Connection=True;MultipleActiveResultSets=true";
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlServer(connString);
}
