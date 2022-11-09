using GTA;
using GTA.Native;
using iaMetrics.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iaMetrics
{
    /// <summary>
    /// <value>Clase que almacena los métodos de revisión del giro del auto</value>
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
    public class Direccion
    {
        /// <summary>
        /// <value>Método que verifica el ángulo de giro del auto</value>
        /// </summary>
        public void VerificarAngulo(SeguroDireccionales Direccion, Vehicle Auto)
        {
            Direccion.AnguloVolante=Auto.SteeringAngle;
            var giroEntero = Math.Round(Direccion.AnguloVolante, 0); //Izq Positivo - Der Negativo

            if (Direccion.SeguroI == true && giroEntero == 0 && Direccion.DetectorI == "OnI")
            {
                Direccion.DetectorI = "PDIA";
                GTA.UI.Screen.ShowSubtitle("Seguro Izq de giro iniciado", 2000);

            }
            else if (giroEntero > 0 && giroEntero < 35 && Direccion.DetectorI == "PDIA")
            {
                Direccion.DetectorI = "GDIA";
                GTA.UI.Screen.ShowSubtitle("Inicio de giro Izq", 2000);

            }
            else if (giroEntero == 0 && Direccion.DetectorI == "GDIA")
            {
                Direccion.DetectorI = "GCDIA";
                GTA.UI.Screen.ShowSubtitle("Fin de giro Izq", 2000);
                Direccion.SeguroI = false;
                Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, Auto, 1, false);
            }

            if (Direccion.SeguroD == true && giroEntero == 0 && Direccion.DetectorD == "OnD")
            {
                Direccion.DetectorD = "PDDA";
                GTA.UI.Screen.ShowSubtitle("Seguro Der de giro iniciado", 2000);

            }
            else if (giroEntero > -35 && giroEntero < 0 && Direccion.DetectorD == "PDDA")
            {
                Direccion.DetectorD = "GDDA";
                GTA.UI.Screen.ShowSubtitle("Inicio de giro Der:" + Direccion.DetectorD, 2000);

            }
            else if (giroEntero == 0 && Direccion.DetectorD == "GDDA")
            {
                Direccion.DetectorD = "GCDDA";
                GTA.UI.Screen.ShowSubtitle("Fin de giro Der", 2000);
                Direccion.SeguroD = false;
                Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, Auto, 0, false);
            }
        }

        /// <summary>
        /// Método que permite la activación de la direccional Izquierda
        /// </summary>
        public void Izq(Ped Jugador, SeguroDireccionales Direccion, Vehicle Auto)
        {
            try
            {

                if (Jugador.IsInVehicle() && Direccion.EstadoI == false)
                {
                    Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, Auto, 1, true);
                    Direccion.EstadoI = true;
                    Direccion.SeguroI = true;
                    Direccion.DetectorI = "OnI";
                }
                else
                {
                    Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, Auto, 1, false);
                    Direccion.EstadoI = false;
                    Direccion.SeguroI = false;
                    Direccion.DetectorI = "OffI";
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
        public void Der(Ped Jugador, SeguroDireccionales Direccion, Vehicle Auto)
        {

            try
            {

                if (Jugador.IsInVehicle() && Direccion.EstadoD == false)
                {
                    Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, Auto, 0, true);
                    Direccion.EstadoD = true;
                    Direccion.SeguroD = true;
                    Direccion.DetectorD = "OnD";

                }
                else
                {
                    Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, Auto, 0, false);
                    Direccion.EstadoD = false;
                    Direccion.SeguroD = false;
                    Direccion.DetectorD = "OffD";
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
        public void Par(Ped Jugador,SeguroDireccionales Direccion, Vehicle Auto)
        {

            try
            {
                if (Jugador.IsInVehicle() && Direccion.Parqueo == false)
                {
                    Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, Auto, 0, true);
                    Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, Auto, 1, true);
                    Direccion.Parqueo = true;
                }
                else
                {
                    Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, Auto, 0, false);
                    Function.Call(Hash.SET_VEHICLE_INDICATOR_LIGHTS, Auto, 1, false);
                    Direccion.Parqueo = false;
                }

            }
            catch
            {

                Console.WriteLine("No hay vehículo");

            }

        }

    }
}
