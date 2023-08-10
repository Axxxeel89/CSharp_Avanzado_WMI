using AccesoEquipo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoEquipo.Services
{
    public static class Geolocalizacion
    {
        public static async Task CapturaIp()
        {
            string ipPublica = "";
            string geolocalizacion = "";
            const string API = "https://api.ipify.org";


            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(API);

                if (response.IsSuccessStatusCode)
                {
                    ipPublica = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Console.WriteLine("No se pudo resolver la solicitud");
                }

                string APIServicioGeolocalizacion = $"http://ip-api.com/json/{ipPublica}";

                HttpResponseMessage responseGeolocalizacion = await httpClient.GetAsync(APIServicioGeolocalizacion);

                if (responseGeolocalizacion.IsSuccessStatusCode)
                {
                    geolocalizacion = await responseGeolocalizacion.Content.ReadAsStringAsync();
                }
            }

            GeolocalizacionInfo geolocalizacionInfo = JsonConvert.DeserializeObject<GeolocalizacionInfo>(geolocalizacion);

            Console.WriteLine($"La ip publica de la maquina es: {ipPublica}, Otros datos de geolocalización: \n");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine($"Pais: {geolocalizacionInfo.Country}");
            Console.WriteLine($"Cod. Pais: {geolocalizacionInfo.CountryCode}");
            Console.WriteLine($"Region: {geolocalizacionInfo.Region}");
            Console.WriteLine($"Departamento: {geolocalizacionInfo.RegionName}");
            Console.WriteLine($"Ciudad: {geolocalizacionInfo.City}");
            Console.WriteLine($"Codigo Postal: {geolocalizacionInfo.zip}");
            Console.WriteLine($"Latitud: {geolocalizacionInfo.lat}");
            Console.WriteLine($"Longitud: {geolocalizacionInfo.lon}");
            Console.WriteLine($"Zona Horaria: {geolocalizacionInfo.timezone}");
            Console.WriteLine($"ISP del servicio: {geolocalizacionInfo.isp}");
            Console.WriteLine("----------------------------------------------\n");
        }
    }
}
