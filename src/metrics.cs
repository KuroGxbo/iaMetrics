using System;
using System.IO;
using GTA;
using GTA.Math;
using GTA.Native;
using System.Windows.Forms;
using System.Collections.Generic;
using iaMetrics.Entidades;

namespace iaMetrics


{
    /// <summary>
    /// Clase que contiene los métodos necesarios para poder obtener información del videojuego Grand Theft Auto V, útiles en la elaboración de simuladores de conducción que permiten la recolección de datos sobre el conductor y el vehículo para procesamiento posterior
    /// </summary>
    public class Metrics : Script
    {
        /// <summary>
        /// <value>Variable pública de la clase Direccion</value>
        /// </summary>
        public Direccion Direccion { get; set; }
        /// <summary>
        /// <value>Variable pública de la clase SeguroDireccionales</value>
        /// </summary>
        public SeguroDireccionales DireccionMetodos { get; set; }
        /// <summary>
        /// <value>Variable pública de la clase Auto</value>
        /// </summary>
        public Auto AutoObjeto { get; set; }
        /// <summary>
        /// <value>Variable pública del tipo Ped de la clase GTA.Ped que almacena el objeto Player que hace referencia al Jugador</value>
        /// </summary>
        public Ped Jugador;
        /// <summary>
        /// <value>Variable pública del tipo Vehicle de la clase GTA.Vehicle que almacena el objeto Vehicle que hace referencia al auto conducido actualmente</value>
        /// </summary>
        public Vehicle Auto;

        /// <summary>
        /// <value>Variable pública del tipo Bool que almacena el objeto Vehicle que hace referencia al valor de True o False del estado de las luces</value>
        /// </summary>
        public bool LucesGlobales;
        /// <summary>
        /// <value>Variable pública del tipo Float que almacena el valor máximo de la velocidad de un vehículo</value>
        /// </summary>
        public float LimiteVelocidad;
        /// <summary>
        /// <value>Variable pública del tipo Vector3 de la clase GTA.Math que almacena las coordenadas de un objeto del Mundo</value>
        /// </summary>
        public Vector3 CoordenadasMundo;
        /// <summary>
        /// <value>Variable pública del tipo Float que almacena los valores de FPS de la sesión de juego</value>
        /// </summary>
        public float Fps { get; set; }
        /// <summary>
        /// <value>Variable pública del tipo DateTime que almacena la fecha actual</value>
        /// </summary>
        public DateTime Fecha { get; set; }
        /// <summary>
        /// <value>Variable pública que almacena la ruta en donde se almacenará el archivo de datos (Por defecto buscará el directorio de instalación del juego)</value>
        /// </summary>
        public string RutaArchivo = Directory.GetCurrentDirectory();
        /// <summary>
        /// <value>Variable pública del tipo List que almacena los puntos de coordenadas del mapa</value>>
        /// </summary>
        public List<String> Coor = new List<String>();
      




        /// <summary>
        /// Constructor que inicializa los métodos de la clase ScriptHookVDotNet3 onKeyDown y onTick
        /// <list type="bullet">
        /// <item>
        /// <description>onKeyDown: Al presionar una técla específica</description>
        /// </item> 
        /// <item>
        /// <description>onKeyUp: Al Levantar una técla específica</description>
        /// </item> 
        /// <item>
        /// <description>onTick: En cada fotograma de la sesión de juego</description>
        /// </item> 
        /// </list>
        /// </summary>
        public Metrics()
        {
            this.KeyUp += onKeyUp;
            //this.KeyDown += onKeyDown;
            this.Tick += onTick;
            Direccion = new Direccion();
            DireccionMetodos = new SeguroDireccionales();
            AutoObjeto = new Auto();

        }

        /// <summary>
        /// Método que inicializa algunas de las variables públicas globales
        /// <list type="bullet">
        /// <item>
        /// <description>jugador: Mediante la clase GTA.Game se obtiene el personaje utilizado por el jugador</description>
        /// </item> 
        /// <item>
        /// <description>auto: Mediante el objeto jugador se obtiene el vehiculo actual</description>
        /// </item>
        /// <item>
        /// <description>Límite de velocidad marcado en 33 m/s</description>
        /// </item>
        /// <item>
        /// <description>Eliminación de Armas al Jugador</description>
        /// </item>
        /// </list>
        /// </summary>

        private void OpcionesGlobales()
        {
            try
            {
                Jugador = Game.Player.Character; //Obtiene al Objeto Jugador
                Auto = Jugador.CurrentVehicle; // Obtener objeto Vehicle del jugador
                Auto.MaxSpeed = 33; //Límite de velocidad del auto
                Jugador.MaxDrivingSpeed = 33; //Establecer en m/s (Límite fijado en 120 km/h)
                Jugador.Weapons.RemoveAll(); //Remover Armas del Jugador
            }
            catch (Exception err)
            {
                Console.WriteLine($@"Error de Inicio:{err.Message}");
            }

        }


        /// <summary>
        /// Método que mediante la utilización de la librería ScriptHookVDotNet3 genera los datos del conductor y del auto, una vez generado los datos se utiliza la librería estandar de C# para escribir los datos en un archivo del tipo CSV.
        /// <list type="bullet">
        /// <item> <description> aceleracion:Mediante el objeto auto del tipo GTA.Vehicle permite saber si se está acelerando</description></item>
        /// <item> <description>rpm:Mediante el objeto auto del tipo GTA.Vehicle se obtiene los RPM del motor</description></item>
        /// <item><description>colision:Mediante el objeto auto del tipo GTA.Vehicle Detecta Colisiones </description></item>
        /// <item><description>embrague:Mediante el objeto auto del tipo GTA.Vehicle Permite saber si el embragüe se está aplastando</description></item>
        /// <item><description>anguloVolante:Mediante el objeto auto del tipo GTA.Vehicle se obtiene el Ángulo de giro del volante</description></item>
        /// <item><description>luces:Mediante el objeto auto del tipo GTA.Vehicle se obtiene el valor de si las luces están encendiadas</description></item>
        /// <item><description>lucesIntensas:Mediante el objeto auto del tipo GTA.Vehicle se obtiene el valor de si las luces intensas están encendiadas</description></item>
        /// <item><description>velocidad:Mediante el objeto auto del tipo GTA.Vehicle se obtiene la Velocidad en Km/h</description></item>
        /// <item><description>bocina:Mediante el objeto auto del tipo GTA.Vehicle Permite saber si el jugador está presionando la Bocina</description></item>
        /// <item><description>rd:Mediante el objeto auto del tipo GTA.Vehicle se obtiene el índice de la estación de radio (Del 1 al 16, si es 255 la radio se encuentra apagada)</description></item>
        /// </list>
        /// </summary>

        private void Recogedor()
        {
            try
            {

                OpcionesGlobales();
                AutoObjeto.Info(AutoObjeto,Auto);
                Fps = Game.FPS;
                Fecha = DateTime.Now;

                if (Jugador.IsInVehicle())
                {
                    Direccion.VerificarAngulo(DireccionMetodos, Auto);

                    if (Game.IsControlJustPressed(GTA.Control.ScriptLB) || Game.IsKeyPressed(Keys.Left))
                    {
                        Direccion.Izq(Jugador, DireccionMetodos, Auto);

                    }
                    if (Game.IsControlJustPressed(GTA.Control.ScriptRB) || Game.IsKeyPressed(Keys.Right))
                    {
                        Direccion.Der(Jugador, DireccionMetodos, Auto);

                    }
                    if (Game.IsControlJustPressed(GTA.Control.ScriptRDown) || Game.IsKeyPressed(Keys.Down))
                    {
                        Direccion.Par(Jugador, DireccionMetodos, Auto);
                    }

                    StreamWriter sw = new StreamWriter(RutaArchivo + "\\Driver_Data.csv", true);
                    sw.WriteLine(AutoObjeto.Aceleracion + ";" + AutoObjeto.Rpm + ";" + AutoObjeto.Colision + ";" + AutoObjeto.Embrague + ";" + Math.Round(DireccionMetodos.AnguloVolante, 0) + ";" + AutoObjeto.Luces + ";" + AutoObjeto.LucesIntensas + ";" + AutoObjeto.Velocidad + ";" + AutoObjeto.Bocina + ";" + AutoObjeto.Radio + ";" + DireccionMetodos.EstadoI + ";" + DireccionMetodos.SeguroI + ";" + DireccionMetodos.EstadoD + ";" + DireccionMetodos.SeguroD + ";"+ Math.Round(Fps,0) +";"+Fecha.ToString("dd/MM/yyyy") + ";" + Fecha.ToString("HH:mm:ss"));
                   
                    sw.Close();

                }

            }
            catch (Exception err)
            {
                Console.WriteLine("Error:" + err);
                GTA.UI.Notification.Show("No está conduciendo");


            }

        }

        /// <summary>
        /// Método que permite obtener y almacenar en la varibale coordenadasMundo las coordenadas en el mundo del objeto jugador y mostrarla por pantalla mediante la clase GTA.UI, útil para extraer puntos para crear rutas personalizadas
        /// </summary>

        private void obtenerCoordenadas()
        {

            CoordenadasMundo = Jugador.Position;

            Coor.Add(CoordenadasMundo.ToString());



        }
        /// <summary>
        /// Método que guarda los puntos almacenados en un archivo de texto, una vez es activado la variable coor será vaciada.
        /// </summary>

        private void almacenarPuntos()
        {
            StreamWriter sw = new StreamWriter(RutaArchivo + "\\PuntosTomados.txt", true);
            foreach (string puntos in Coor)
            {
                sw.WriteLine(puntos);

            };
            sw.Close();
            Coor.Clear();

        }

        ///<summary>
        ///Método detector de semáforos
        ///</summary>
        private void AlmacenarSemaforo()
        {
            //589548997
            
            

        }



        /// <summary>
        /// Método propio de la librería ScriptHookVDotNet que ejecuta acciones en cada frame del juego, en este caso exacto ejecuta la función Recogedor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void onTick(object sender, EventArgs e)
        {
            Recogedor();

        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            try {
                switch (e.KeyCode)
                {
                    case Keys.J:
                        var Vec3 = Jugador.ForwardVector;
                        GTA.UI.Screen.ShowSubtitle($@"Almacenado en: {RutaArchivo} Posición Actual del Jugador: {Vec3}");
                        break;
                    case Keys.Left:
                        Direccion.Izq(Jugador, DireccionMetodos, Auto);
                        break;
                    case Keys.Right:
                        Direccion.Der(Jugador, DireccionMetodos, Auto);
                        break;
                    case Keys.Down:
                        Direccion.Par(Jugador, DireccionMetodos, Auto);
                        break;
                    case Keys.NumPad1:
                        var Ruta1 = new Vector3(Convert.ToSingle(-111.6915), Convert.ToSingle(-891.34), Convert.ToSingle(28.88107));
                        if (Jugador.Position.DistanceTo(Ruta1) < 10)
                        {
                            GTA.UI.Screen.ShowSubtitle("Está en la ubicación", 3000);

                        }
                        else
                        {

                            Jugador.Position = Ruta1;
                            Vehicle auto2 = Function.Call<Vehicle>(Hash.CREATE_VEHICLE, VehicleHash.Blista, -113.9271, -896.3893, 29.32886, Jugador.Heading, false, true);
                            Function.Call(Hash.SET_VEHICLE_ON_GROUND_PROPERLY, auto2);
                            GTA.UI.Screen.ShowSubtitle(auto2.ToString() + VehicleHash.Blista);
                            Function.Call(GTA.Native.Hash.CLEAR_GPS_MULTI_ROUTE);
                            Function.Call(GTA.Native.Hash.START_GPS_MULTI_ROUTE, 7, true, true);
                            Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -111.6915, -891.34, 28.88107);
                            Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -630.1589, -626.7308, 32.20092);
                            Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -1100.076, -292.8331, 37.46314);
                            Function.Call(GTA.Native.Hash.SET_GPS_MULTI_ROUTE_RENDER, true);
                        }
                        break;
                    case Keys.NumPad2:
                        var Ruta2 = new Vector3(Convert.ToSingle(26.75446), Convert.ToSingle(6349.708), Convert.ToSingle(31.23984));
                        if (Jugador.Position.DistanceTo(Ruta2) < 10)
                        {
                            GTA.UI.Screen.ShowSubtitle("Está en la ubicación", 3000);

                        }
                        else
                        {

                            Jugador.Position = Ruta2;
                            Vehicle auto2 = Function.Call<Vehicle>(Hash.CREATE_VEHICLE, VehicleHash.Blista, 30.67473, 6351.935, 31.06434, Jugador.Heading, false, true);
                            Function.Call(Hash.SET_VEHICLE_ON_GROUND_PROPERLY, auto2);
                            GTA.UI.Screen.ShowSubtitle(auto2.ToString() + VehicleHash.Blista);
                            Function.Call(GTA.Native.Hash.CLEAR_GPS_MULTI_ROUTE);
                            Function.Call(GTA.Native.Hash.START_GPS_MULTI_ROUTE, 7, true, true);
                            Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -35.48402, -1014.748, 28.82768);
                            Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 85.81989, -1023.425, 29.19493);
                            Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -103.8164, -681.3545, 34.97224);
                            Function.Call(GTA.Native.Hash.SET_GPS_MULTI_ROUTE_RENDER, true);
                        }
                        break;
                    case Keys.NumPad3:

                        var Ruta3 = new Vector3(Convert.ToSingle(-22.70162), Convert.ToSingle(-1444.728), Convert.ToSingle(30.77637));
                        if (Jugador.Position.DistanceTo(Ruta3) < 10)
                        {
                            GTA.UI.Screen.ShowSubtitle("Está en la ubicación", 3000);

                        }
                        else
                        {
                            Jugador.Position = Ruta3;
                            Vehicle auto2 = Function.Call<Vehicle>(Hash.CREATE_VEHICLE, VehicleHash.Blista, -25.56438, -1445.296, 30.47844, Jugador.Heading, false, true);
                            Function.Call(Hash.SET_VEHICLE_ON_GROUND_PROPERLY, auto2);
                            GTA.UI.Screen.ShowSubtitle(auto2.ToString() + VehicleHash.Blista);
                            Function.Call(GTA.Native.Hash.CLEAR_GPS_MULTI_ROUTE);
                            Function.Call(GTA.Native.Hash.START_GPS_MULTI_ROUTE, 7, true, true);
                            Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -26.21502, -1459.268, 30.62076);
                            Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 341.3826, -1538.669, 29.09086);
                            Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -45.02396, -1825.508, 26.23182);
                            Function.Call(GTA.Native.Hash.SET_GPS_MULTI_ROUTE_RENDER, true);
                        }
                        break;
                    case Keys.NumPad4:
                        Function.Call(GTA.Native.Hash.CLEAR_GPS_MULTI_ROUTE);
                        break;
                    case Keys.NumPad5:
                        obtenerCoordenadas();
                        break;
                    case Keys.NumPad6:
                        almacenarPuntos();
                        break;
                }
            }
            catch (Exception Err)
            {
                GTA.UI.Screen.ShowSubtitle(Err.Message);
            }

        }
    }
}
