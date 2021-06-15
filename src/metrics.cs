using System;
using System.IO;
using GTA;
using GTA.Math;
using GTA.Native;
using System.Windows.Forms;
using System.Collections.Generic;


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
        /// <value>Variable pública del tipo List que almacena los puntos de coordenadas del mapa</value>>
        /// </summary>
        public List<string> coor=new List<string>();





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
            try {
                jugador = Game.Player.Character; //Obtiene al Objeto Jugador
                auto = jugador.CurrentVehicle; // Obtener objeto Vehicle del jugador
                auto.MaxSpeed = 33; //Límite de velocidad del auto
                jugador.MaxDrivingSpeed = 33; //Establecer en m/s (Límite fijado en 120 km/h)
            }
            catch (Exception err) {
                Console.WriteLine("Error de Inicio");
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
                    if (Game.IsControlJustPressed(GTA.Control.ScriptLB)||Game.IsKeyPressed(Keys.Left)) {
                        Izq();

                    }
                    if (Game.IsControlJustPressed(GTA.Control.ScriptRB)|| Game.IsKeyPressed(Keys.Right)) {
                        Der();

                    }
                    if (Game.IsControlJustPressed(GTA.Control.ScriptRDown)||Game.IsKeyPressed(Keys.Down)) {
                        Par();
                    }
                    
                    verificarAngulo();
                    StreamWriter sw = new StreamWriter(rutaArchivo + "\\Driver_Data.csv", true);
                    sw.WriteLine(aceleracion + ";" + rpm + ";" + colision + ";" + embrague + ";" + Math.Round(anguloVolante, 0) + ";" + luces + ";" + lucesIntensas + ";" + velocidad + ";" + bocina + ";" + rd + ";" + estadoI + ";" + seguroI + ";" + estadoD + ";" + seguroD);
                    sw.Close();

                }

            }
            catch (Exception err)
            {
                Console.WriteLine("Error:" + err);
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
                GTA.UI.Screen.ShowSubtitle("Seguro Izq de giro iniciado:" + detectorI, 2000);

            }
            else if (giroEntero > 0 && giroEntero < 35 && detectorI == "PDIA")
            {
                detectorI = "GDIA";
                GTA.UI.Screen.ShowSubtitle("Inicio de giro Izq:" + detectorI, 2000);

            }
            else if (giroEntero == 0 && detectorI == "GDIA")
            {
                detectorI = "GCDIA";
                GTA.UI.Screen.ShowSubtitle("Fin de giro Izq:" + detectorI, 2000);
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

            coor.Add(coordenadasMundo.ToString());
           


        }
        /// <summary>
        /// Método que guarda los puntos almacenados en un archivo de texto, una vez es activado la variable coor será vaciada.
        /// </summary>

        private void almacenarPuntos() {
            StreamWriter sw = new StreamWriter(rutaArchivo + "\\PuntosTomados.txt", true);
            foreach (string puntos in coor)
            { 
                sw.WriteLine(puntos);
               
            };
            sw.Close();
            coor.Clear();

        }
        /// <summary>
        /// Método que permite la activación de la direccional Izquierda
        /// </summary>
        private void Izq() {

            try
            {

                if (jugador.IsInVehicle() && estadoI == false)
                {
                    Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, auto, 1, true);
                    estadoI = true;
                    seguroI = true;
                    detectorI = "OnI";
                }
                else
                {
                    Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, auto, 1, false);
                    estadoI = false;
                    seguroI = false;
                    detectorI = "OffI";
                    Console.WriteLine("No hay vehículo");
                }

            }
            catch
            {

                Console.WriteLine("No hay vehículo");

            }
        }

        /// <summary>
        /// Método que permite la activación de la direccional Derecha
        /// </summary>
        private void Der()
        { 

            try
                {

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

        /// <summary>
        /// Método que permite la activación de las luces de parqueo
        /// </summary>
        private void Par() {

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
        /// <item><description>1: Genera una ruta en entorno Urbano (Centro de la Ciudad) hacia un punto prefijado mediante el GPS del juego</description></item>
        /// <item><description>2: Genera una ruta en entorno Urbano (Suburbio de la Ciudad) hacia un punto prefijado mediante el GPS del juego</description></item>
        /// <item><description>3: Genera una ruta en entorno Urbano (Autopista de la Ciudad) hacia un punto prefijado mediante el GPS del juego</description></item>
        /// <item><description>4: Elimina la ruta prefijada si esta ha sido puesta</description></item>
        /// <item><description>5: Obtiene coordenadas mediante el método obtenerCoordenadas</description></item>
        /// <item><description>6: Almacena coordenadas mediante el método almacenarPuntos</description></item>
        /// </list>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void onKeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.J) {
                GTA.UI.Screen.ShowSubtitle("Almacenado en: " + rutaArchivo);

            }
            if (e.KeyCode == Keys.Left) {
                Izq();


            }
            if (e.KeyCode == Keys.Right) {
                Der();


            }
            if (e.KeyCode == Keys.Down)
            {
                Par();

            }
            if (e.KeyCode == Keys.NumPad1) {

                var puntoInicial=new Vector3(Convert.ToSingle(-22.83574), Convert.ToSingle(-1021.892), Convert.ToSingle(28.88758));
               
                jugador.Position=puntoInicial;

                
                Vehicle auto2 = Function.Call<Vehicle>(Hash.CREATE_VEHICLE, VehicleHash.Blista, -21.9049,-1017.696,28.73725, jugador.Heading,false,true );
                Function.Call(Hash.SET_VEHICLE_ON_GROUND_PROPERLY, auto2);
                GTA.UI.Screen.ShowSubtitle(auto2.ToString()+ VehicleHash.Blista);
                
               
               
                Function.Call(GTA.Native.Hash.CLEAR_GPS_MULTI_ROUTE);
                Function.Call(GTA.Native.Hash.START_GPS_MULTI_ROUTE, 7, true, true);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -33.81958, -984.3044, 28.96712);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 28.32723, -799.6833, 31.20931);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 106.0485, -593.1719, 31.36516);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 68.72903, -553.0438, 32.7597);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -39.39139, -541.5793, 39.83604);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 33.42474, -300.1317, 47.2943);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 5.647297, -267.1422, 47.11627);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -73.58354, -231.5509, 44.6971);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -87.41713, -196.2236, 46.80207);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -72.69206, -129.5126, 57.61015);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -157.0445, -70.02403, 53.54333);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -269.5167, -78.61893, 48.61173);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -286.335, -161.6859, 40.35384);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -228.3162, -179.2843, 43.08128);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -122.6482, -222.7078, 44.546);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -89.37512, -189.9388, 47.7024);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -74.34047, -129.7199, 57.62627);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -43.38224, 9.409852, 71.68886);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 6.104521, 159.4679, 94.99844);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 29.88078, 236.4629, 109.2927);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 93.64304, 315.6416, 111.6262);
                Function.Call(GTA.Native.Hash.SET_GPS_MULTI_ROUTE_RENDER, true);

            }
            if (e.KeyCode == Keys.NumPad2)
            {

                var puntoInicial = new Vector3(Convert.ToSingle(26.75446), Convert.ToSingle(6349.708), Convert.ToSingle(31.23984));

                jugador.Position = puntoInicial;


                Vehicle auto2 = Function.Call<Vehicle>(Hash.CREATE_VEHICLE, VehicleHash.Blista, 30.67473, 6351.935, 31.06434, jugador.Heading, false, true);
                Function.Call(Hash.SET_VEHICLE_ON_GROUND_PROPERLY, auto2);
                GTA.UI.Screen.ShowSubtitle(auto2.ToString() + VehicleHash.Blista);



                Function.Call(GTA.Native.Hash.CLEAR_GPS_MULTI_ROUTE);
                Function.Call(GTA.Native.Hash.START_GPS_MULTI_ROUTE, 7, true, true);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 67.806, 6388.052, 31.06171);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 78.18993, 6379.748, 31.0476);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 82.04499, 6428.502, 31.16607);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 117.1966, 6485.928, 31.18302);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 171.0286, 6531.095, 31.57377);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 251.5667, 6560.112, 30.91903);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 338.3748, 6563.589, 28.47905);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 402.6208, 6561.248, 27.26217);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 467.3207, 6556.963, 26.80363);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 525.5704, 6546.496, 27.28797);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 648.3453, 6519.086, 28.02758);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 711.5515, 6504.904, 27.27016);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 783.3488, 6493.25, 24.35097);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 859.0434, 6486.048, 21.77704);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1163.172, 6481.658, 20.84893);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1203.413, 6482.277, 20.73394);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1250.652, 6483.536, 20.44848);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1292.767, 6483.739, 20.02492);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1330.773, 6481.685, 19.7448);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1365.566, 6477.263, 19.79068);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1406.82, 6468.064, 20.07292);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1458.607, 6451.059, 21.19958);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1527.255, 6426.073, 23.2416);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1565.703, 6410.894, 24.52149);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1635.977, 6382.745, 28.88565);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1685.877, 6363.901, 31.99587);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1730.89, 6351.189, 34.51751);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1781.093, 6342.66, 36.98964);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1838.465, 6335.366, 39.49408);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1915.181, 6294.554, 42.22581);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1947.662, 6240.594, 43.73876);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1967.663, 6189.924, 45.19021);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1987.494, 6153.422, 46.26238);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2019.895, 6115.565, 47.36792);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2056.504, 6080.292, 48.48095);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2125.624, 6017.983, 50.98586);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2182.654, 5980.317, 51.01777);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2225.262, 5943.455, 50.31858);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2260.107, 5908.712, 49.09557);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2296.986, 5866.672, 47.76222);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2344.853, 5806.438, 46.39536);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2395.544, 5733.678, 45.39938);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2426.734, 5679.317, 44.99742);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2456.488, 5614.294, 44.75779);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2475.837, 5567.724, 44.64801);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2494.879, 5519.648, 44.5303);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2528.978, 5421.278, 44.35084);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2541.344, 5372.873, 44.37463);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2556.044, 5317.083, 44.42191);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2569.291, 5268.296, 44.46967);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2589.604, 5193.323, 44.54964);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2606.786, 5131.759, 44.58877);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2637.536, 5011.888, 44.61148);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2655.785, 4945.266, 44.56358);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2672.074, 4877.678, 44.49366);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2687.598, 4779.938, 44.32686);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2703.405, 4704.894, 44.13901);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2727.717, 4622.854, 44.63117);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2736.607, 4574.178, 45.33999);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2764.508, 4456.86, 47.75081);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2780.156, 4401.276, 48.82304);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2794.206, 4353.802, 49.57033);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2823.561, 4273.039, 50.07319);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2890.437, 4080.218, 50.55399);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2900.936, 4044.308, 50.85064);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2912.575, 3991.301, 51.30249);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2921.06, 3916.591, 51.8945);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2920.459, 3855.759, 52.26721);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2916.754, 3816.792, 52.41993);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2906.052, 3764.995, 52.47167);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2893.337, 3718.892, 52.47462);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2877.479, 3657.691, 52.4725);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2836.309, 3546.142, 53.74559);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2809.435, 3464.714, 55.08091);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2791.723, 3423.187, 55.57665);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2769.266, 3368.414, 55.91871);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2749.327, 3323.19, 55.90964);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2723.024, 3268.471, 55.22802);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2699.365, 3225.457, 54.07643);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2667.349, 3176.176, 52.02808);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2621.136, 3117.535, 48.53868);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2569.428, 3061.826, 44.64283);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2526.384, 3020.465, 42.1607);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2437.268, 2943.063, 40.31776);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2393.324, 2907.125, 40.06178);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2325.765, 2852.9, 40.72071);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2250.813, 2797.017, 43.31763);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2206.752, 2761.701, 45.37449);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2157.054, 2720.271, 47.86862);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2088.868, 2660.703, 51.23092);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2042.951, 2620.381, 53.26103);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1967.029, 2545.195, 54.51853);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1920.558, 2484.024, 54.61698);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1858.914, 2381.621, 53.96209);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1831.731, 2313.483, 53.5909);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1811.657, 2235.76, 53.57031);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1803.858, 2154.563, 54.13056);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1811.876, 2084.527, 55.14666);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1821.859, 2035.55, 55.93245);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1845.291, 1950.165, 57.67844);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1865.064, 1890.083, 59.53509);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1939.489, 1703.568, 69.8337);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1963.451, 1653.429, 72.55424);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1988.591, 1594.424, 74.6351);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2022.98, 1520.583, 75.51639);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2061.442, 1449.223, 75.42547);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2131.091, 1337.476, 75.29375);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2191.344, 1255.099, 75.94241);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2251.884, 1182.191, 77.2635);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2326.808, 1091.436, 79.7991);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2389.876, 1016.63, 84.26126);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2431.058, 957.0792, 87.50827);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2461.279, 892.2188, 90.3418);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2476.912, 846.7803, 92.5833);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2490.666, 789.7489, 96.02119);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2532.407, 560.0238, 111.202);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2537.085, 489.1417, 113.5401);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2538.46, 394.1897, 112.0984);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2531.294, 291.4991, 108.5375);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2525.46, 217.6669, 105.2324);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2516.054, 145.8996, 101.7344);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2504.151, 64.73383, 97.59932);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2488.707, -15.70738, 93.74162);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2470.149, -82.77766, 91.33878);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2446.651, -149.0953, 88.99541);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2418.074, -211.7451, 86.13939);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2369.084, -289.3469, 84.70445);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2331.903, -346.3857, 85.63935);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2289.972, -405.1252, 87.62363);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2245.308, -460.8598, 90.18668);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2186.989, -520.897, 93.17514);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2129.068, -572.0533, 95.05732);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 2056.999, -632.1904, 95.06048);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1994.428, -681.4968, 92.31098);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1942.635, -722.5552, 88.02249);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1888.549, -762.7511, 82.84381);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1824.258, -805.5823, 76.88341);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1724.038, -878.1935, 69.41246);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1640.17, -934.1106, 63.91893);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1558.187, -986.5595, 59.02729);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1477.455, -1032.372, 56.07215);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1329.79, -1103.976, 51.45243);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1264.185, -1136.854, 51.38919);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1118.59, -1170.473, 55.23861);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 1067.193, -1171.812, 55.37096);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 986.7188, -1182.13, 53.52358);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 920.2069, -1181.522, 48.14085);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 844.5697, -1182.743, 45.47934);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 773.3617, -1184.945, 45.13867);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 710.4476, -1186.991, 44.29141);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 678.0302, -1188.101, 42.97629);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 638.9472, -1189.522, 41.38786);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 595.9416, -1190.021, 40.98395);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 554.9218, -1191.206, 42.01042);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 519.4516, -1190.224, 41.96394);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 437.6674, -1188.284, 40.87787);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 405.6743, -1186.195, 40.15943);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 366.5422, -1180.558, 39.03327);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 324.3036, -1177.304, 38.08194);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 268.8666, -1172.38, 37.94062);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 232.3409, -1166.147, 37.97741);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 193.7553, -1160.299, 37.99763);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 162.4276, -1160.387, 37.59147);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 119.6023, -1156.396, 33.18904);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 105.8021, -1156.947, 31.09251);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 78.23409, -1156.096, 29.04416);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 70.54093, -1150.189, 29.05904);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 71.84757, -1140.208, 29.06231);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 70.57652, -1150.709, 29.06948);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 68.16893, -1138.708, 29.10921);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 71.9869, -1113.121, 29.12519);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 82.40871, -1085.243, 29.08868);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 87.49779, -1075.391, 29.05291);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 96.72131, -1070.236, 29.10129);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 111.0589, -1069.049, 29.01331);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 114.1168, -1063.513, 29.01458);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 109.6271, -1052.713, 29.05144);


                Function.Call(GTA.Native.Hash.SET_GPS_MULTI_ROUTE_RENDER, true);

            }
            if (e.KeyCode == Keys.NumPad3)
            {

                var puntoInicial = new Vector3(Convert.ToSingle(-22.70162), Convert.ToSingle(-1444.728), Convert.ToSingle(30.77637));

                jugador.Position = puntoInicial;


                Vehicle auto2 = Function.Call<Vehicle>(Hash.CREATE_VEHICLE, VehicleHash.Blista, -25.56438, -1445.296, 30.47844, jugador.Heading, false, true);
                Function.Call(Hash.SET_VEHICLE_ON_GROUND_PROPERLY, auto2);
                GTA.UI.Screen.ShowSubtitle(auto2.ToString() + VehicleHash.Blista);



                Function.Call(GTA.Native.Hash.CLEAR_GPS_MULTI_ROUTE);
                Function.Call(GTA.Native.Hash.START_GPS_MULTI_ROUTE, 7, true, true);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -75.49474, -1473.744, 31.94736);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -87.3203, -1485.512, 32.42884);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -88.6295, -1487.008, 32.54189);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -89.9446, -1488.938, 32.68471);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -113.1728, -1516.741, 33.74138);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -121.4974, -1512.38, 33.84078);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -130.6868, -1505.651, 33.97663);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -154.5798, -1484.839, 32.89163);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -166.4437, -1474.443, 32.16587);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -179.1409, -1463.859, 31.54124);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -199.0576, -1436.357, 31.11051);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -193.9232, -1426.223, 31.12997);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -178.394, -1414.955, 31.01269);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -162.0707, -1404.275, 30.37601);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -140.2486, -1391.105, 29.55497);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -114.2467, -1377.896, 29.14686);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -89.487, -1373.812, 29.16687);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -64.99885, -1375.568, 29.12776);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -38.37018, -1375.793, 29.10429);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -15.51436, -1374.758, 29.13704);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 20.27494, -1374.786, 29.14234);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 49.71952, -1375.061, 29.09211);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 51.32442, -1375.086, 29.09107);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 64.99678, -1377.026, 29.0015);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 74.79571, -1375.583, 29.06212);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 87.75752, -1376.737, 29.03083);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 104.4445, -1378.879, 29.10195);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 137.9016, -1394.99, 28.92756);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 154.9267, -1404.989, 28.95548);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 173.1339, -1415.691, 29.11615);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 195.0644, -1428.563, 29.1139);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 219.3112, -1444.118, 29.07562);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 236.9421, -1457.097, 29.09114);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 258.2408, -1473.802, 29.12118);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 271.1171, -1484.241, 29.1282);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 293.0376, -1500.455, 28.97982);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 307.602, -1513.69, 29.01215);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 326.4629, -1530.574, 28.98969);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 351.2313, -1551.026, 29.02137);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 384.1554, -1581.572, 29.06583);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 397.1402, -1592.508, 29.07016);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 427.8094, -1622.082, 28.97831);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 429.5898, -1643.022, 29.06106);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 424.0654, -1659.356, 29.01299);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 403.6027, -1684.942, 29.0703);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 383.1016, -1710.358, 29.08876);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 364.434, -1733.574, 29.11411);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 340.2061, -1763.404, 28.93819);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 320.7768, -1785.917, 28.15428);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 297.1365, -1812.453, 27.01398);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 272.6199, -1841.304, 26.67266);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 257.5371, -1859.396, 26.66527);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 238.0506, -1883.595, 26.0442);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 221.8685, -1901.732, 24.58632);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 208.2164, -1907.866, 23.66396);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 195.1805, -1901.726, 23.71672);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 171.0524, -1890.757, 23.41171);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 151.2357, -1882.575, 23.43557);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 128.3519, -1871.969, 23.88494);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 86.49393, -1851.838, 24.47752);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 67.4183, -1837.825, 24.45532);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 48.69828, -1823.503, 24.09751);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 28.09608, -1807.106, 25.47576);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 11.3032, -1792.561, 27.47085);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -3.276113, -1775.688, 28.70278);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -19.69298, -1758.748, 28.94345);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -38.32817, -1743.636, 29.0146);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -54.20758, -1730.131, 28.97231);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -43.73807, -1721.518, 29.09446);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -23.56283, -1710.715, 29.1404);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 3.367403, -1696.714, 29.14849);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 43.04601, -1669.498, 29.10136);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 43.17487, -1655.123, 29.10103);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, 0.2040757, -1614.112, 29.09983);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -21.46001, -1596.731, 29.16109);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -57.4197, -1565.587, 30.22771);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -99.74911, -1535.238, 33.54902);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -130.2202, -1506.426, 33.96293);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -160.4344, -1480.398, 32.56425);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -207.2535, -1442.861, 31.29226);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -232.2663, -1416.38, 30.96999);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -230.7192, -1392.72, 31.07755);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -220.9572, -1391.302, 31.08018);
                Function.Call(GTA.Native.Hash.ADD_POINT_TO_GPS_MULTI_ROUTE, -215.4659, -1398.832, 31.09589);



                Function.Call(GTA.Native.Hash.SET_GPS_MULTI_ROUTE_RENDER, true);

            }
            if (e.KeyCode == Keys.NumPad4) {
                Function.Call(GTA.Native.Hash.CLEAR_GPS_MULTI_ROUTE);
            }
            if (e.KeyCode == Keys.NumPad5) {
                obtenerCoordenadas();

            }
            if (e.KeyCode == Keys.NumPad6) {
                almacenarPuntos();
            }
          
        }
        }
    }
