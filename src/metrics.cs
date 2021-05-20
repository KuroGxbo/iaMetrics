using System;
using System.IO;
using GTA;
using GTA.Math;
using GTA.Native;
using System.Windows.Forms;


namespace iaMetrics


{
   /// <summary>
   /// Clase que contiene los métodos necesarios para poder obtener información del videojuego Grand Theft Auto V, útiles en la elaboración de simuladores de conducción que permiten la recolección de datos sobre el conductor y el vehículo para procesamiento posterior
   /// </summary>
    public class Metrics : Script
    {
        /// <summary>
        /// <value>Variable pública del tipo Ped de la clase GTA.Ped que almacena el objeto Player que hace referencia al Jugador</value>
        /// </summary>
        public Ped jugador;
        /// <summary>
        /// <value>Variable pública del tipo Vehicle de la clase GTA.Vehicle que almacena el objeto Vehicle que hace referencia al auto conducido actualmente</value>
        /// </summary>
        public Vehicle auto;
        /// <summary>
        /// <value>Variable pública del tipo Float almacena el objeto SteeringAngle de la clase GTA.Vehicle que hace referencia al Ángulo de giro del auto</value>
        /// </summary>
        public float anguloVolante;
        /// <summary>
        /// <value>Variable pública del tipo Bool que almacena el objeto Vehicle que hace referencia al valor de True o False del estado de las luces</value>
        /// </summary>
        public bool lucesGlobales;
        /// <summary>
        /// <value>Variable pública del tipo Float que almacena el valor máximo de la velocidad de un vehículo</value>
        /// </summary>
        public float limiteVelocidad;
        /// <summary>
        /// <value>Variable pública del tipo Vector3 de la clase GTA.Math que almacena las coordenadas de un objeto del Mundo</value>
        /// </summary>
        public Vector3 coordenadasMundo;
        /// <summary>
        /// <value>Variable pública del tipo Bool que almacena el estado de encendido de la direccional Izquierda</value>
        /// </summary>
        public bool estadoI;
        /// <summary>
        /// <value>Variable pública del tipo Bool que almacena el seguro de activación de encendido de la direccional Izquierda</value>
        /// </summary>
        public bool seguroI;
        /// <summary>
        /// <value>Variable pública del tipo String que almacena el estado de Encendido de la Direccional Izquierda</value>
        /// </summary>
        public string detectorI;
        /// <summary>
        /// <value>Variable pública del tipo Bool que almacena el estado de encendido de la direccional Derecha</value>
        /// </summary>
        public bool estadoD;
        /// <summary>
        /// <value>Variable pública del tipo String que almacena el estado de la luz Derecha (Siendo modificada por el método verificarAngulo()</value>
        /// </summary>
        public bool seguroD;
        /// <summary>
        /// <value>Variable pública del tipo String que almacena el estado de Encendido de la Direccional Derecha</value>
        /// </summary>
        public string detectorD;
        /// <summary>
        /// <value>Variable pública del tipo Bool que almacena si las luces direccionales fueron activadas por el botón de parqueo</value>
        /// </summary>
        public bool parqueo;
        /// <summary>
        /// <value>Variable pública que almacena la ruta en donde se almacenará el archivo de datos (Por defecto buscará el directorio de instalación del juego)</value>
        /// </summary>
        public string rutaArchivo = Directory.GetCurrentDirectory();
        /// <summary>
        /// <value>Variable pública del tipo string que indica si existe aceleración del auto</value>
        /// </summary>
        public string aceleracion;
        /// <summary>
        /// <value>Variable pública del tipo string que almacena el valor de las revoluciones del motor</value>
        /// </summary>
        public string rpm;
        /// <summary>
        /// <value>Variable pública del tipo Bool que indica si el auto ha colisionado con algo</value>
        /// </summary>
        public bool colision;
        /// <summary>
        /// <value>Variable pública del tipo float que almacena el valor de presión del pedal del embrague</value>
        /// </summary>
        public float embrague;
        /// <summary>
        /// <value>Variable pública del tipo Bool que indica si las luces han sido encendidas</value>
        /// </summary>
        public bool luces;
        /// <summary>
        /// <value>Variable pública del tipo Bool que indica si las luces intensas han sido encendidas</value>
        /// </summary>
        public bool lucesIntensas;
        /// <summary>
        /// <value>Variable pública del tipo double que almacena la velocidad del auto</value>
        /// </summary>
        public double velocidad;
        /// <summary>
        /// <value>Variable pública del tipo Bool que permite saber si la bocina fue activada</value>
        /// </summary>
        public bool bocina;
        /// <summary>
        /// <value>Variable pública del tipo int que permite conocer el índice de la estación de radio (Va del 1 al 16 si está prendida, 255 si está apagada)</value>
        /// </summary>
        public int rd;




        /// <summary>
        /// Constructor que inicializa los métodos de la clase ScriptHookVDotNet3 onKeyDown y onTick
        /// <list type="bullet">
        /// <item>
        /// <description>onKeyDown: Al presionar una técla específica</description>
        /// </item> 
        /// <item>
        /// <description>onTick: En cada fotograma de la sesión de juego</description>
        /// </item> 
        /// </list>
        /// </summary>
        public Metrics()
        {
            this.KeyDown += onKeyDown;
            this.Tick += onTick;

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
        /// </list>
        /// </summary>

        private void OpcionesGlobales() {
            jugador = Game.Player.Character; //Obtiene al Objeto Jugador
            auto = jugador.CurrentVehicle; // Obtener objeto Vehicle del jugador
            jugador.MaxDrivingSpeed = 33; //Establecer en m/s (Límite fijado en 120 km/h)
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

        private void Recogedor() {
            OpcionesGlobales();

                try
                {
                    aceleracion = auto.Acceleration.ToString(); //Permite saber si se está acelerando
                    rpm = auto.CurrentRPM.ToString(); // Obtiene los RPM del motor
                    colision = auto.HasCollided; //Detecta Colisiones
                    embrague = auto.Clutch; //Permite saber si el embragüe se está aplastando
                    anguloVolante = auto.SteeringAngle;//Ángulo de giro del volante
                    luces = auto.AreLightsOn; //¿Están las Luces Encendidas?
                    lucesIntensas = auto.AreHighBeamsOn; //¿Están las luces Intensas Encendidas?
                    velocidad = auto.Speed * 3.6; //Obtiene la Velocidad en Km/h
                    bocina = Game.Player.IsPressingHorn; //Permite saber si el jugador está presionando la Bocina
                    rd = Function.Call<int>(Hash.GET_PLAYER_RADIO_STATION_INDEX); //Permite conocer el índice de la estación de radio (255 si está apagada) 
              
                    if (jugador.IsInVehicle())
                    {
                    
                        verificarAngulo();
                        StreamWriter sw = new StreamWriter(rutaArchivo+"\\Driver_Data.csv", true);
                        sw.WriteLine(aceleracion + ";" + rpm + ";" + colision + ";" + embrague + ";" + Math.Round(anguloVolante, 0) + ";" + luces + ";" + lucesIntensas + ";" + velocidad + ";" + bocina + ";" +rd+";"+estadoI+";"+seguroI+";"+estadoD + ";"+seguroD);
                        sw.Close();

                    }

                }
                catch (Exception err)
                {
                    Console.WriteLine("Error:"+err);
                    GTA.UI.Notification.Show("No está conduciendo", true);

            }

        }


        /// <summary>
        /// Método que permite el apagado de las direccionales cuando el volante regresa a su posición normal, siendo esta un ángulo de 0 grados, modifica las varibles seguroI y seguroD con los valores de True o False y las variables detectorI y detectorD con los siguientes valores:
        /// <list type="bullet">
        /// <item> <description>giroEntero: Variable que convierte el valor de la variable pública anguloVolante a un valor entero entre el 35 y el -35</description> </item>
        /// <item> <description>OnI:"On Izq" Permite conocer si la direccional izq fue activada </description> </item>
        /// <item> <description>PDIA: Estado de Auto Parado con Direccional Izquierda Activa </description> </item>
        /// <item> <description>GDIA: Estado de Auto con Giro iniciado con Direccional Izquierda Activa </description> </item>
        /// <item> <description>GCDIA: Estado de Auto con Fin de Giro con Direccional Izquierda Activa </description> </item>
        /// <item> <description>OnD:"On Der" Permite conocer si la direccional der fue activada </description> </item>
        /// <item> <description>PDDA: Estado de Auto Parado con Direccional Derecha Activa </description> </item>
        /// <item> <description>GDDA: Estado de Auto con Giro iniciado con Direccional Derecha Activa </description> </item>
        /// <item> <description>GDIA: Estado de Auto con Giro iniciado con Direccional Izquierda Activa </description> </item>
        /// <item> <description>GCDDA: Estado de Auto con Fin de Giro con Direccional Derecha Activa </description> </item>
        /// </list>
        /// </summary>
        private void verificarAngulo()
        {
            var giroEntero = Math.Round(anguloVolante, 0); //Izq Positivo - Der Negativo
            //GTA.UI.Screen.ShowSubtitle("giro:" + giroEntero);

            if (seguroI == true && giroEntero == 0 && detectorI == "OnI")
            {
                detectorI = "PDIA";
                GTA.UI.Screen.ShowSubtitle("Seguro Izq de giro iniciado:" + detectorI,2000);

            }
            else if (giroEntero > 0 && giroEntero < 35 && detectorI == "PDIA")
            {
                detectorI = "GDIA";
                GTA.UI.Screen.ShowSubtitle("Inicio de giro Izq:" + detectorI,2000);

            }
            else if (giroEntero == 0 && detectorI == "GDIA")
            {
                detectorI = "GCDIA";
                GTA.UI.Screen.ShowSubtitle("Fin de giro Izq:" + detectorI,2000);
                seguroI = false;
                Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, auto, 1, false);
            }

            if (seguroD == true && giroEntero == 0 && detectorD == "OnD")
            {
                detectorD = "PDDA";
                GTA.UI.Screen.ShowSubtitle("Seguro Der de giro iniciado:" + detectorI, 2000);

            }
            else if (giroEntero > -35 && giroEntero < 0 && detectorD == "PDDA")
            {
                detectorD = "GDDA";
                GTA.UI.Screen.ShowSubtitle("Inicio de giro Der:" + detectorD, 2000);

            }
            else if (giroEntero == 0 && detectorD == "GDDA")
            {
                detectorD = "GCDDA";
                GTA.UI.Screen.ShowSubtitle("Fin de giro Der:" + detectorD, 2000);
                seguroD = false;
                Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, auto, 0, false);
            }



        }

        /// <summary>
        /// Método que permite obtener y almacenar en la varibale coordenadasMundo las coordenadas en el mundo del objeto jugador y mostrarla por pantalla mediante la clase GTA.UI, útil para extraer puntos para crear rutas personalizadas
        /// </summary>

        private void obtenerCoordenadas() {

            coordenadasMundo = jugador.Position;

            GTA.UI.Screen.ShowSubtitle(coordenadasMundo.ToString());

        }

        /// <summary>
        /// Método propio de la librería ScriptHookVDotNet que ejecuta acciones en cada frame del juego, en este caso exacto ejecuta la función Recogedor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void onTick(object sender, EventArgs e) {
            Recogedor();
            
        }

        /// <summary>
        /// Método propio de la librería ScriptHookVDotNet que ejecuta acciones cuando una tecla es presionada, en este caso las teclas son las siguientes:
        /// <list type="bullet">
        /// <item><description>J: Permite saber la ruta donde se almacena el archivo</description></item>
        /// <item><description>Flecha Izq: Encender direccional Izquierda</description></item>
        /// <item><description>Flecha Der: Encender direccional Derecha</description></item>
        /// <item><description>Flecha Abajo:  Encender luces de parqueo</description></item>
        /// <item><description>B: Genera una ruta hacia un punto prefijado mediante el GPS del juego</description></item>
        /// <item><description>N: Elimina la ruta prefijada si esta ha sido puesta</description></item>
        /// <item><description>M: Obtiene coordenadas mediante el método obtenerCoordenadas</description></item>
        /// </list>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void onKeyDown(object sender,KeyEventArgs e)
        {

            if (e.KeyCode == Keys.J) {
                GTA.UI.Screen.ShowSubtitle("Almacenado en: "+rutaArchivo);

            }
            if (e.KeyCode == Keys.Left) {
                try
                {
                    //GTA.UI.Screen.ShowSubtitle("Estado:"+estadoD);
                    if (jugador.IsInVehicle() && estadoI==false)
                    {
                        Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS,auto,1,true);
                        estadoI = true;
                        seguroI = true;
                        detectorI = "OnI";
                    }
                    else {
                        Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, auto, 1, false);
                        estadoI = false; 
                        seguroI = false;
                        detectorI = "OffI";
                        Console.WriteLine("No hay vehículo");
                    }
                    
                }
                catch {
                    
                    Console.WriteLine("No hay vehículo");

                }

                
            }
            if (e.KeyCode == Keys.Right) {
                try
                {
                    //GTA.UI.Screen.ShowSubtitle("Estado:"+estadoD);
                    if (jugador.IsInVehicle() && estadoD == false)
                    {
                        Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, auto, 0, true);
                        estadoD = true;
                        seguroD = true;
                        detectorD = "OnD";

                    }
                    else
                    {
                        Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, auto, 0, false);
                        estadoD = false;
                        seguroD = false;
                        detectorD = "OffD";
                        Console.WriteLine("No hay vehículo");
                    }

                }
                catch
                {

                    Console.WriteLine("No hay vehículo");

                }


            }
            if (e.KeyCode == Keys.Down)
            {
                try
                {
                    //GTA.UI.Screen.ShowSubtitle("Estado:"+estadoD);
                    if (jugador.IsInVehicle() && parqueo == false)
                    {
                        Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, auto, 0, true);
                        Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, auto, 1, true);
                        parqueo = true;
                    }
                    else
                    {
                        Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, auto, 0, false);
                        Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, auto, 1, false);
                        parqueo = false;
                        Console.WriteLine("No hay vehículo");
                    }

                }
                catch
                {

                    Console.WriteLine("No hay vehículo");

                }


            }
            if (e.KeyCode ==Keys.B) {
                Function.Call(GTA.Native.Hash.CLEAR_GPS_MULTI_ROUTE);
                Function.Call(GTA.Native.Hash.START_GPS_MULTI_ROUTE,7,true,true);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -572.5653, 5250.958, 70.46847);
                Function.Call(GTA.Native.Hash.SET_GPS_MULTI_ROUTE_RENDER,true);
                    
            }
            if (e.KeyCode == Keys.N) {
                Function.Call(GTA.Native.Hash.CLEAR_GPS_MULTI_ROUTE);
            }
            if (e.KeyCode == Keys.M) {
                obtenerCoordenadas();
                
            }

        }
        }
    }
