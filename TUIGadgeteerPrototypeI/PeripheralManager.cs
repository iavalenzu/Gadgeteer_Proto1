using System;
using Microsoft.SPOT;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

using Gadgeteer.Modules.GHIElectronics;


namespace GadgeteerFlexor
{
    /*
     * Maneja cada uno de los valores entregados por los sensores y los perifcericos conectados (Motor, Pantalla)
     */ 
    class PeripheralManager
    {
        private GT.Interfaces.DigitalOutput vibrationControl;
        private GT.Interfaces.AnalogInput analogPressureMeter;
        private GT.Interfaces.AnalogInput analogFlexorMeter;


        private bool vibraPelota;

        private double forcePressureValue;
        private double forceFlexorValue;

        private int current_face;

        private Graphics graphics;

        /*
        double min_preassure = Double.MaxValue;
        double max_preassure = 0;

        double min_flexor = Double.MaxValue;
        double max_flexor = 0;
        */

        public PeripheralManager(Graphics _graphics, GT.Interfaces.DigitalOutput _vibrationControl, GT.Interfaces.AnalogInput _analogPressureMeter, GT.Interfaces.AnalogInput _analogFlexorMeter)
        {

            vibrationControl = _vibrationControl;
            analogFlexorMeter = _analogFlexorMeter;
            analogPressureMeter = _analogPressureMeter;

            graphics = _graphics;

            vibraPelota = false;

            current_face = 0;

        
        }


        public void generaVibracion()
        {

            Debug.Print("Se enciende el motor vibrador!!");
            vibrationControl.Write(true);
            vibraPelota = true;
        }

        public void apagaVibracion()
        {
            Debug.Print("Se apaga el motor vibrador!!");
            vibrationControl.Write(false);
            vibraPelota = false;
        }

        public bool estaVibrando() {
            return vibraPelota;
        }

        public void measurePressure()
        {

            forcePressureValue = analogPressureMeter.ReadVoltage();
            forceFlexorValue = analogFlexorMeter.ReadVoltage();
/*
            if (forceFlexorValue < min_flexor)
                min_flexor = forceFlexorValue;
            if (forceFlexorValue > max_flexor)
                max_flexor = forceFlexorValue;


            if (forcePressureValue < min_preassure)
                min_preassure = forcePressureValue;
            if (forcePressureValue > max_preassure)
                max_preassure = forcePressureValue;
            
            Debug.Print("Analog read flexor: " + forceFlexorValue);
            Debug.Print("Analog read pressure: " + forcePressureValue);

            Debug.Print("Analog read flexor: MIN = " + min_flexor + " MAX = " + max_flexor);
            Debug.Print("Analog read pressure: MIN = " + min_preassure + " MAX = " + max_preassure);
            
            Debug.Print("=================================================");
*/            
        }

        public bool isPressed()
        {
            return forcePressureValue > 3.280 || forceFlexorValue < 0.760;
        }

        public int obtieneActualCara() 
        {
            return current_face;
        }

        public void cambiaActualCara()
        {
            switch (current_face)
            {
                case 0:
                    graphics.drawBitmap(Graphics.smile_bmp, 8, 8, GTM.Efelunte.AdafruitLedBackpack.LED_YELLOW);
                    current_face = 1;
                    break;
                case 1:
                    graphics.drawBitmap(Graphics.frown_bmp, 8, 8, GTM.Efelunte.AdafruitLedBackpack.LED_YELLOW);
                    current_face = 2;
                    break;
                case 2:
                    graphics.drawBitmap(Graphics.frown_bmp, 8, 8, GTM.Efelunte.AdafruitLedBackpack.LED_RED);
                    current_face = 3;
                    break;
                case 3:
                    graphics.drawBitmap(Graphics.smile_bmp, 8, 8, GTM.Efelunte.AdafruitLedBackpack.LED_GREEN);
                    current_face = 0;
                    break;
                default:
                    break;
            }

            Debug.Print("La cara actual cambia a: " + DataManager.faces[current_face]);

        }

    }
}
