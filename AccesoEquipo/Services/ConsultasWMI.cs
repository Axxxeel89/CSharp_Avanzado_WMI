using AccesoEquipo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace AccesoEquipo.Services
{
    public static class ConsultasWMI
    {
        [DllImport("wtsapi32.dll", SetLastError = true)]
        static extern bool WTSLogoffSession(IntPtr hServer, int SessionId, bool bWait);


        public static void EspecificacionesEquipo(ManagementObjectCollection queryCollection)
        {
            foreach (ManagementObject m in queryCollection)
            {
                Console.WriteLine("\n---------------------\n");
                Console.WriteLine("Nombre del dispositivo: " + m["Name"]);
                Console.WriteLine("Fabricante: " + m["Manufacturer"]);
                Console.WriteLine("Modelo: " + m["Model"]);
                Console.WriteLine("Tipo del sistema: " + m["SystemType"]);
                Console.WriteLine("Dominio: " + m["Domain"]);
                Console.WriteLine("Usuario: " + m["UserName"]);
                Console.WriteLine("\n---------------------\n");
            }
        }

        public static void EspecificacionesSisteOperativo(ManagementObjectCollection queryCollection)
        {
            foreach (ManagementObject equipo in queryCollection)
            {
                Console.WriteLine("\n---------------------\n");
                Console.WriteLine($"Nombre del sistema operativo: {equipo["Caption"]}");
                Console.WriteLine($"Version del sistema operativo: {equipo["Version"]}");
                Console.WriteLine($"Arquitectura del sistema operativo: {equipo["OSArchitecture"]}");
                // Imprimir los idiomas del sistema operativo
                Console.Write("Idiomas del sistema operativo: ");
                string[] languages = (string[])equipo["MUILanguages"];
                foreach (string language in languages)
                {
                    Console.Write(language + ", ");
                }
                Console.WriteLine();
                Console.WriteLine("\n---------------------\n");
            }
        }

        public static void EspecificacionesProcesador(ManagementObjectCollection queryCollection)
        {

            foreach (ManagementObject procesador in queryCollection)
            {
                Console.WriteLine("\n---------------------\n");
                Console.WriteLine("Nombre del dispositivo: " + procesador["Name"]);
                Console.WriteLine("Numero de nucleos: " + procesador["NumberOfCores"]);
                Console.WriteLine("Velocidad Maxima: " + procesador["MaxClockSpeed"]);
                Console.WriteLine("Numero de hilos: " + procesador["NumberOfLogicalProcessors"]);
                Console.WriteLine("\n---------------------\n");
            }
        }

        public static void EspecificacionesMemoriaRam(ManagementObjectCollection queryCollection)
        {
            foreach (ManagementObject m in queryCollection)
            {
                ulong capacidadBytes = (ulong)m["Capacity"];
                double capacidadGB = (double)capacidadBytes / 1073741824; // Dividir por 1,073,741,824 para obtener GB
                Console.WriteLine("Capacidad: " + capacidadGB.ToString("F2") + " GigaBytes");
                string memoryType = "Desconocido";
                int memorySpeed = Convert.ToInt32(m["ConfiguredClockSpeed"]); // Velocidad en MHz

                if (memorySpeed >= 1600 && memorySpeed <= 2133)
                {
                    memoryType = "DDR3";
                }
                else if (memorySpeed >= 2133 && memorySpeed <= 3200)
                {
                    memoryType = "DDR4";
                }
                // Agrega más condiciones para otros tipos de memoria

                Console.WriteLine("Tipo de memoria: " + memoryType);
                Console.WriteLine("Fabricante: " + m["Manufacturer"]);
                Console.WriteLine("Número de serie: " + m["SerialNumber"]);
                Console.WriteLine("Velocidad: " + m["Speed"] + " MHz");
                Console.WriteLine("---------------------");
            }
            Console.WriteLine("\n---------------------\n");
        }

        public static void EspecificacionesTarjetaVideo(ManagementObjectCollection queryCollection)
        {
            foreach (ManagementObject m in queryCollection)
            {
                Console.WriteLine("Nombre: " + m["Name"]);
                Console.WriteLine("Fabricante: " + m["AdapterCompatibility"]);
                ulong capacidadGb = (uint)m["AdapterRAM"];
                double memoryVideo = capacidadGb / 1073741824;
                Console.WriteLine("Memoria de Video: " + memoryVideo + " GigaBytes");
                Console.WriteLine("Resolución: " + m["CurrentHorizontalResolution"] + "x" + m["CurrentVerticalResolution"]);
                Console.WriteLine("---------------------");
            }
            Console.WriteLine("\n---------------------\n");
        }

        public static void EspecificacionesDiscoDuro(ManagementObjectCollection queryCollection)
        {
            //Validamos los datos del disco
            double espacioTotalGB = 0;
            foreach (ManagementObject m in queryCollection)
            {
                Console.WriteLine("\n---------------------\n");
                Console.WriteLine("Modelo: " + m["Model"]);
                ulong espacioTotalBytes = (ulong)m["Size"];
                espacioTotalGB = (double)espacioTotalBytes / 1073741824;

                Console.WriteLine("Espacio Total: " + espacioTotalGB.ToString("F2") + " GigaBytes");
                Console.WriteLine("Tipo de Interfaz: " + m["InterfaceType"]);
                Console.WriteLine("Número de Serie: " + m["SerialNumber"]);
                Console.WriteLine("---------------------");
            }

            const int HARD_DISK = 3;
            string strComputer = ".";

            //Hacemos nueva consulta en Win32_LogicalDisk y nos traemos la capacidad del disco
            ManagementScope namespaceScope = new ManagementScope("\\\\" + strComputer + "\\ROOT\\CIMV2");
            ObjectQuery diskQuery = new ObjectQuery("SELECT * FROM Win32_LogicalDisk WHERE DriveType = " + HARD_DISK + "");
            ManagementObjectSearcher mgmtObjSearcher = new ManagementObjectSearcher(namespaceScope, diskQuery);
            ManagementObjectCollection colDisks = mgmtObjSearcher.Get();

            foreach (ManagementObject objDisk in colDisks)
            {
                Console.WriteLine("** Datos del disco duro: \n");
                Console.WriteLine("Dispositivo ID : {0}", objDisk["DeviceID"]);

                ulong espacioLibreBytes = (ulong)objDisk["FreeSpace"];
                double espacioLibreGB = (double)espacioLibreBytes / 1073741824;

                double espacioUsadoGB = espacioTotalGB - espacioLibreGB;

                Console.WriteLine("Espacio Usado: " + espacioUsadoGB.ToString("F2") + " GigaBytes");
                Console.WriteLine("Espacio Libre: " + espacioLibreGB.ToString("F2") + " GigaBytes");
                Console.ReadLine();
            }


        }

        public static void ProgramasInstalados(ManagementObjectCollection queryCollection)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Product");

            //Leer la collecion 
            foreach (ManagementObject product in searcher.Get())
            {

                if (product.Properties["Name"] != null && product.Properties["Version"] != null)
                {
                    // Imprimir programas y versiones
                    Console.WriteLine("Programa:{0}   -   Version: {1}", product.Properties["Name"].Value, product.Properties["Version"].Value);
                }
            }
        }

        public static void BusquedaProgramasInstalados(string buscarPrograma)
        {
            string rta = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Product");

            string formatoBusqueda = buscarPrograma.ToLower().Trim();

            foreach (ManagementObject product in searcher.Get())
            {
                // Convertimos el valor de la propiedad "Name" a minúsculas y eliminamos espacios en blanco
                string nombrePrograma = product.Properties["Name"].Value?.ToString().ToLower().Trim();

                // Verificamos si el valor de la propiedad "Name" coincide con el nombre de búsqueda
                if (!string.IsNullOrEmpty(nombrePrograma) && nombrePrograma.Contains(formatoBusqueda))
                {
                    // Imprimimos el programa
                    Console.WriteLine("Programa: {0} - Version: {1}",
                        product.Properties["Name"].Value,
                        product.Properties["Version"].Value);

                    Console.WriteLine("");

                    rta = "encontrado";
                    return;
                }
                else
                {
                    rta = "Noencontrado";
                }

            }

            if (rta == "Noencontrado")
            {
                Console.WriteLine("El programa solicitado no ha sido encontrado");
            }


            Console.WriteLine("\n-------------------------------------------------");
        }

        public static void SesionActiva(ManagementObjectCollection queryCollection)
        {
            string equipoIp = "192.168.0.1";
            int logonId = 0;

            ConnectionOptions connectionOptions = new ConnectionOptions
            {
                Username = "ZonET",
                Password = "1689"
            };

            foreach (ManagementObject system in queryCollection)
            {
                Console.WriteLine("Usuario en sesión: " + system["UserName"]);
            }

            Console.WriteLine("-------------------------------------------");

            //Hacemos nueva consulta en Win32_LogonSession y nos traemos el logon ID
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_LogonSession");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection queryCollection2 = searcher.Get();

            foreach (ManagementObject system in queryCollection2)
            {
                object logonIdObj = system["LogonId"];
                if (logonIdObj != null && int.TryParse(logonIdObj.ToString(), out logonId))
                {
                    Console.WriteLine("Usuario en sesión: " + logonId);
                }
                else
                {
                    Console.WriteLine("No se encontraron sesiones activas.");
                }

            }

            Console.WriteLine("-------------------------------------------\n");

        }

        public static void CerrarSesionActivaEquipoRemoto( int sessionIdToClose)
        {
            string equipoIp = "192.168.0.19"; // IP de la máquina remota

            // Construye el comando para cerrar la sesión específica
            string shutdownCommand = $"shutdown -l -f -m \\\\{equipoIp} -i {sessionIdToClose}";

            // Crea una instancia de ManagementClass para ejecutar el comando
            ConnectionOptions connectionOptions = new ConnectionOptions
            {
                Username = "ZonET",
                Password = "1689"
            };
            ManagementScope scope = new ManagementScope($"\\\\{equipoIp}\\root\\cimv2", connectionOptions);
            ManagementClass processClass = new ManagementClass(scope, new ManagementPath("Win32_Process"), new ObjectGetOptions());

            // Ejecuta el comando para cerrar la sesión
            processClass.InvokeMethod("Create", new object[] { "cmd.exe /c " + shutdownCommand, null, null });

        }

        public static void CerrarSesionPropia()
        {
            int sessionIdToClose = 1; // Cambia esto al ID correcto

            bool success = WTSLogoffSession(IntPtr.Zero, sessionIdToClose, true);

            if (success)
            {
                Console.WriteLine("Sesión cerrada exitosamente.");
            }
            else
            {
                Console.WriteLine("No se pudo cerrar la sesión.");
            }
        }




    }
}
