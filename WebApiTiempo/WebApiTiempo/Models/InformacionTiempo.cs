using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiTiempo.Models
{
    public class InformacionTiempo
    {
        [Key]
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Municipio { get; set; }
        public string Provincia { get; set; }
        public string Temperatura { get; set; }
        public string Humedad { get; set; }
        public string VelocidadViento { get; set; }
        public string PrecipitacionAcumulada { get; set; }
        public string GpxX { get; set; }
        public string GpxY { get; set; }
        public string TipoEstacion { get; set; }
    }
}
