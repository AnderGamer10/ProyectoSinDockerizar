using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using NW = Newtonsoft.Json;
using MS = System.Text.Json;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
class Program
{
    static async Task Main(string[] args)
    {
        TimeSpan interval = new TimeSpan(0, 0, 300);
        while (true)
        {
            await ActualizandoDatos();
            Thread.Sleep(interval);
        }
    }
    static async Task ActualizandoDatos()
    {
        Console.WriteLine("Actualizando/Insertando datos");

        DateTime fecha = DateTime.Now;
        var sDia = "";
        var sMes = "";
        if (fecha.Day < 10)
        {
            sDia = 0 + "" + fecha.Day;
        }
        else
        {
            sDia = Convert.ToString(fecha.Day);
        }

        if (fecha.Month < 10)
        {
            sMes = 0 + "" + fecha.Month;
        }
        else
        {
            sMes = Convert.ToString(fecha.Month);
        }

        Console.WriteLine("****************************" + fecha.TimeOfDay + "****************************");
        var client = new HttpClient { BaseAddress = new Uri($"https://www.euskalmet.euskadi.eus/vamet/stations/stationList/stationList.json") };
        var responseMessage = await client.GetAsync("", HttpCompletionOption.ResponseContentRead);
        var resultData = await responseMessage.Content.ReadAsStringAsync();
        dynamic stationTypeJson = JsonConvert.DeserializeObject(resultData);
        foreach (var Estaciones in stationTypeJson)
        {
            try
            {
                var cliente = new HttpClient { BaseAddress = new Uri($"https://www.euskalmet.euskadi.eus/vamet/stations/readings/{Estaciones.id}/{fecha.Year}/{sMes}/{sDia}/readingsData.json") };
                var responseMessagee = await cliente.GetAsync("", HttpCompletionOption.ResponseContentRead);
                var resultDatae = await responseMessagee.Content.ReadAsStringAsync();
                dynamic stationReadingsJson = JsonConvert.DeserializeObject(resultDatae);

                var sTemperatura = "No hay datos";
                var sPrecipitacion = "No hay datos";
                var sHumedad = "No hay datos";
                var sViento = "No hay datos";

                foreach (var DatosEstaciones in stationReadingsJson)
                {

                    foreach (JObject item in DatosEstaciones)
                    {
                        try
                        {
                            String dataType = item["name"].ToString();
                            JObject preDataJson = JObject.Parse(item["data"].ToString());
                            IList<string> keys = preDataJson.Properties().Select(p => p.Name).ToList();
                            JObject dataJson = JObject.Parse(preDataJson[keys[0]].ToString());
                            switch (dataType)
                            {
                                case "temperature":
                                    List<string> dataJsonTimeList = dataJson.Properties().Select(p => p.Name).ToList();
                                    dataJsonTimeList.Sort();
                                    sTemperatura = Convert.ToString(dataJson[dataJsonTimeList.Last()]);
                                    break;
                                case "precipitation":
                                    List<string> dataJsonPreciList = dataJson.Properties().Select(p => p.Name).ToList();
                                    dataJsonPreciList.Sort();
                                    sPrecipitacion = Convert.ToString(dataJson[dataJsonPreciList.Last()]);
                                    break;
                                case "humidity":
                                    List<string> dataJsonHumiList = dataJson.Properties().Select(p => p.Name).ToList();
                                    dataJsonHumiList.Sort();
                                    sHumedad = Convert.ToString(dataJson[dataJsonHumiList.Last()]);
                                    break;
                                case "mean_speed":
                                    List<string> dataJsonWindList = dataJson.Properties().Select(p => p.Name).ToList();
                                    dataJsonWindList.Sort();
                                    sViento = Convert.ToString(dataJson[dataJsonWindList.Last()]);
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Sin obtener datos 'Error'");
                        }
                    }
                }
                using (var db = new TiempoContext())
                {
                    try
                    {
                        if (sTemperatura == "No hay datos" && sHumedad == "No hay datos" && sPrecipitacion == "No hay datos" && sViento == "No hay datos")
                        {
                            Console.WriteLine("Sin datos");
                        }
                        else
                        {
                            //Zona de actualizacion de datos
                            string id = Estaciones.id;
                            try
                            {
                                var infoNueva = db.InformacionTiempo.Where(a => a.Id == id).Single();
                                Console.WriteLine(Estaciones.id + ": Actualizando los datos");
                                infoNueva.Temperatura = sTemperatura;
                                infoNueva.Humedad = sHumedad;
                                infoNueva.VelocidadViento = sViento;
                                infoNueva.PrecipitacionAcumulada = sPrecipitacion;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Metiendo datos nuevos");
                                var ao1 = new InformacionTiempo
                                {
                                    Id = Estaciones.id,
                                    Nombre = Estaciones.name,
                                    Municipio = Estaciones.municipality,
                                    Temperatura = sTemperatura,
                                    Humedad = sHumedad,
                                    VelocidadViento = sViento,
                                    PrecipitacionAcumulada = sPrecipitacion,
                                    GpxX = Estaciones.x,
                                    GpxY = Estaciones.y,
                                    TipoEstacion = Estaciones.stationType,
                                    Provincia = Estaciones.province
                            };
                                db.InformacionTiempo.Add(ao1);
                            }
                        };
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("No se ha podido guardar" + e);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
