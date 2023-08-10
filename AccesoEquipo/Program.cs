// See https://aka.ms/new-console-template for more information

using AccesoEquipo.Services;
using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

class Program
{

    [DllImport("wtsapi32.dll", SetLastError = true)]
    static extern bool WTSLogoffSession(IntPtr hServer, int SessionId, bool bWait);


    delegate void OperacionesWMI(ManagementObjectCollection queryCollection);
    delegate void ConsultasWMIBusqueda(string busqueda);
    static async Task Main(string[] args)
    {
        int opcMenu = 20;

        while (opcMenu != 0)
        {
            Console.WriteLine("Bienvenido a la aplicación, que deseas hacer hoy, solo se aceptan valores numericos");
            Console.WriteLine("1- Consultar especificaciones del equipo");
            Console.WriteLine("2- Consultar especificaciones del sistema operativo");
            Console.WriteLine("3- Consultar especificaciones del procesador");
            Console.WriteLine("4- Consultar especificaciones de la memoria ram");
            Console.WriteLine("5- Consultar especificaciones de la tarjeta de video.");
            Console.WriteLine("6- Consultar especificaciones del disco duro");
            Console.WriteLine("7- Consulta de programas instalados en el equipo");
            Console.WriteLine("8- Mostrar todos los programas instalados en el equipo");
            Console.WriteLine("9- Mostrar sesiones activas");
            Console.WriteLine("10- Captura Ip Publica");
            Console.WriteLine("11- Cerrar sesion de usuario");
            Console.WriteLine("0- Salir del programa\n");

            try
            {
                opcMenu = int.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("El dato ingresado no es numerico");
            }

            switch (opcMenu)
            {
                case 0:
                    Console.WriteLine("Hasta la proxima");
                    return;

                case 1:
                    Console.WriteLine("\nEspecificaciones del equipo");
                    EjecutarConsultaWMI(1);
                    break;

                case 2:
                    Console.WriteLine("\nEspecificaciones del S.O");
                    EjecutarConsultaWMI(2);
                    break;

                case 3:
                    Console.WriteLine("\nEspecificaciones del Procesador");
                    EjecutarConsultaWMI(3);
                    break;

                case 4:
                    Console.WriteLine("\nEspecificaciones del Memoria Ram\n");
                    EjecutarConsultaWMI(4);
                    break;

                case 5:
                    Console.WriteLine("\nEspecificaciones de la Tarjeta de video\n");
                    EjecutarConsultaWMI(5);
                    break;

                case 6:
                    Console.WriteLine("\nEspecificaciones del Disco duro");
                    EjecutarConsultaWMI(6);
                    break;

                case 7:
                    Console.WriteLine("\nBusqueda programas instalados");
                    EjecutarConsultaWMI(7);
                    break;

                case 8:
                    Console.WriteLine("\nInformación programas instalados");
                    EjecutarConsultaWMI(8);
                    break;

                case 9:
                    Console.WriteLine("\nValidando sesiones");
                    EjecutarConsultaWMI(9);
                    break;

                case 10:
                    Console.WriteLine("Realizando consulta");
                    await Geolocalizacion.CapturaIp();
                    break;

                case 11:
                    int sessionIdToClose = 3; // Cambia esto al ID correcto

                    bool success = WTSLogoffSession(IntPtr.Zero, sessionIdToClose, true);

                    if (success)
                    {
                        Console.WriteLine("Sesión cerrada exitosamente.");
                    }
                    else
                    {
                        Console.WriteLine("No se pudo cerrar la sesión.");
                    }

                    //ConsultasWMI.CerrarSesionPropia();
                    break;

                default:
                    Console.WriteLine("No has elegido una opción valida\n");
                    break;
            }
            
        }
    }

    static void EjecutarConsultaWMI(int operacionSeleccionada)
    {
        //Apuntadores
        OperacionesWMI EspecificacionesEquipo = ConsultasWMI.EspecificacionesEquipo;
        OperacionesWMI EspecificacionesSisteOperativo = ConsultasWMI.EspecificacionesSisteOperativo;
        OperacionesWMI EspecificacionesProcesador = ConsultasWMI.EspecificacionesProcesador;
        OperacionesWMI EspecificacionesMemoriaRam = ConsultasWMI.EspecificacionesMemoriaRam;
        OperacionesWMI EspecificacionesTarjetaVideo = ConsultasWMI.EspecificacionesTarjetaVideo;
        OperacionesWMI EspecificacionesSesionActiva = ConsultasWMI.SesionActiva;
        OperacionesWMI EspecificacionesDiscoDuro = ConsultasWMI.EspecificacionesDiscoDuro;
        OperacionesWMI ProgramasInstalados = ConsultasWMI.ProgramasInstalados;
        ConsultasWMIBusqueda BusquedaProgramasInstalados = ConsultasWMI.BusquedaProgramasInstalados;

        ObjectQuery query = null;
        
        switch (operacionSeleccionada)
        {
            case 1:
                query = new ObjectQuery("SELECT * FROM Win32_ComputerSystem");
                break;

            case 2:
                query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                break;

            case 3:
                query = new ObjectQuery("SELECT * FROM Win32_Processor");
                break;

            case 4:
                query = new ObjectQuery("SELECT * FROM Win32_PhysicalMemory");
                break;

            case 5:
                query = new ObjectQuery("SELECT * FROM Win32_VideoController");
                break;

            case 6:
                query = new ObjectQuery("SELECT * FROM Win32_DiskDrive");
                break;

            case 7:
                query = new ObjectQuery("SELECT * FROM Win32_Product");
                break;

            case 8:
                query = new ObjectQuery("SELECT * FROM Win32_Product");
                break;

            case 9:
                query = new ObjectQuery("SELECT * FROM Win32_ComputerSystem");
                break;

        }

        ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
        ManagementObjectCollection queryCollection = searcher.Get();

        switch (operacionSeleccionada)
        {
            case 1:
                
                EspecificacionesEquipo(queryCollection);
                break;

            case 2:
                EspecificacionesSisteOperativo(queryCollection);
                break;

            case 3:
                EspecificacionesProcesador(queryCollection);
                break;

            case 4:
                EspecificacionesMemoriaRam(queryCollection);
                break;

            case 5:
                EspecificacionesTarjetaVideo(queryCollection);
                break;

            case 6:
                EspecificacionesDiscoDuro(queryCollection);
                break;

            case 7:
                Console.WriteLine("Ingresa el nombre del programa que quieres buscar");
                string buscarPrograma = Console.ReadLine();
                BusquedaProgramasInstalados(buscarPrograma);
                break;

            case 8:
                ProgramasInstalados(queryCollection);
                break;

            case 9:
                EspecificacionesSesionActiva(queryCollection);
                break;
        }

    }
}
