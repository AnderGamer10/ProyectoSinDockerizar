using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApiTiempo.Models
{
    public class TiempoContext : DbContext
    {
        public TiempoContext(DbContextOptions<TiempoContext> options)
           : base(options)
        {
        }

        public DbSet<InformacionTiempo> TiempoItems { get; set; }
    }
}