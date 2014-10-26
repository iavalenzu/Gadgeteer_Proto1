using System;
using System.Collections;
using System.Threading;
using System.IO;
using System.Text;

using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;
using Microsoft.SPOT.Hardware;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GTI = Gadgeteer.Interfaces;
using Gadgeteer.Modules.GHIElectronics;

using Gadgeteer.Modules.Efelunte;

namespace GadgeteerFlexor
{
    public partial class Program
    {


        private GT.Interfaces.DigitalOutput vibrationControl;

        private GT.Interfaces.AnalogInput analogPressureMeter;
        private GT.Interfaces.AnalogInput analogFlexorMeter;
        

        private GTM.Efelunte.Adafruit_BicolorMatrix matrix;

        private GT.Timer intervalTimer;

        private GT.Socket Socket10;


        private int maxCycles = 9;
        private int maxVibrationCycles = 15;


        private bool is_being_pressed;
        private int pressed_times;
        private int cycles;
        private int vibrationCycles;

        private DataManager data_manager;
        private PeripheralManager peripheral_manager;
        private Graphics graphics;

        void ProgramStarted()
        {

            Socket10 = GT.Socket.GetSocket(10, true, null, null);

            vibrationControl = new GT.Interfaces.DigitalOutput(Socket10, GT.Socket.Pin.Four, false, null);

            analogPressureMeter = new GT.Interfaces.AnalogInput(Socket10, GT.Socket.Pin.Three, null);
            analogFlexorMeter = new GT.Interfaces.AnalogInput(Socket10, GT.Socket.Pin.Five, null);


            matrix = new Adafruit_BicolorMatrix(Socket10);

            graphics = new Graphics(matrix);

            data_manager = new DataManager(sdCard, graphics);

            peripheral_manager = new PeripheralManager(graphics, vibrationControl, analogPressureMeter, analogFlexorMeter);

            intervalTimer = new GT.Timer(100);
            intervalTimer.Tick += new GT.Timer.TickEventHandler(TimerTick);
            intervalTimer.Start();

            /*
             * Se inicializan las variables
             **/ 

            is_being_pressed = false;
            pressed_times = 0;

            cycles = 0;
            vibrationCycles = 0;

            Debug.Print("Program Started!!");

        }

        void TimerTick(GT.Timer timer)
        {

            //Se verifica que la tarjeta SD este insertada y montada

            if (data_manager.isReady())
            {
                
                //Se mide la presion sobre la pelota 
                 
                peripheral_manager.measurePressure();

                if (peripheral_manager.isPressed())
                {
                    if (is_being_pressed)
                    {
                        //La pelota sigue siendo presionada
                        Debug.Print("La pelota sigue siendo presionada!!");
                    }
                    else
                    {
                        //La pelota ha sido presionada
                        is_being_pressed = true;
                    }
                }
                else
                {
                    if (is_being_pressed)
                    {
                        //La pelota ha sido liberada
                        Debug.Print("La pelota ha sido liberada!!");

                        is_being_pressed = false;
                        cycles = 0;

                        pressed_times++;

                        Debug.Print("PressedTimes: " + pressed_times);

                    }
                    else
                    {
                        //La pelota esta en reposo
                    }
                }

                if (cycles >= maxCycles)
                {

                     // Termina el ciclo, verifico la accion a seguir

                    if (pressed_times == 3) {

                        Debug.Print("La pelota ha sido librerada tres veces consecutivas!!");
                        Debug.Print("Desmonto la tarjeta SD");

                        //Si se presiona tres veces la pelota, se desmonta la tarjeta SD
                        data_manager.unmountSDCard(); 
                    }
                    else if (pressed_times == 2)
                    {
                        Debug.Print("La pelota ha sido liberada dos veces consecutivas!!");

                        data_manager.guardaActualCara(peripheral_manager.obtieneActualCara());
                        peripheral_manager.generaVibracion();
                        vibrationCycles = 0;

                    }
                    else if (pressed_times == 1)
                    {
                        Debug.Print("La pelota ha sido liberada una vez!!");

                        peripheral_manager.cambiaActualCara();
                    }

                    pressed_times = 0;
                    cycles = 0;
                }
                else
                {
                    cycles++;
                }

                
                 // Lo siguiente controla el numero de ciclos en que la vibracion estara encendida

                if (peripheral_manager.estaVibrando())
                {
                    if (vibrationCycles >= maxVibrationCycles)
                    {
                        peripheral_manager.apagaVibracion();
                        vibrationCycles = 0;
                    }
                    else
                    {
                        vibrationCycles++;
                    }

                }

            }
        }

    }
}
