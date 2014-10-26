using System;
using System.Threading;
using Microsoft.SPOT;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

using Gadgeteer.Modules.GHIElectronics;

namespace GadgeteerFlexor
{
    class Graphics
    {
        static public byte[] smile_bmp = new byte[8]{
            0x3c,
            0x42,
            0xa5,
            0x81,
            0xa5,
            0x99,
            0x42,
            0x3c
        };

        static public byte[] neutral_bmp = new byte[8]{
            0x3c,
            0x42,
            0xa5,
            0x81,
            0xbd,
            0x81,
            0x42,
            0x3c
        };

        static public byte[] frown_bmp = new byte[8]{
            0x3c,
            0x42,
            0xa5,
            0x81,
            0x99,
            0xa5,
            0x42,
            0x3c
        };

        private GTM.Efelunte.Adafruit_BicolorMatrix matrix;

        public Graphics(GTM.Efelunte.Adafruit_BicolorMatrix _matrix)
        {
            matrix = _matrix;

            matrix.begin();
        }

        public void drawPixel(short x, short y, ushort color)
        {
            matrix.drawPixel(x, y, color);
        }

        public void setRotation(ushort rotation)
        {
            matrix.setRotation(rotation);
        }

        public void drawBitmap(byte[] bitmap, short w, short h, ushort color)
        {

            matrix.clear();
            matrix.drawBitmap(0, 0, bitmap, w, h, color);
            matrix.writeDisplay();

        }


        public void clearScreen(){
        
            matrix.clear();
            matrix.writeDisplay();

        }

        public void fillScreen(short w, short h, ushort color) {

            byte[] bitmap = new byte[8]{
                0xff,
                0xff,
                0xff,
                0xff,
                0xff,
                0xff,
                0xff,
                0xff
            };

            matrix.clear();
            matrix.drawBitmap(0, 0, bitmap, w, h, color);
            matrix.writeDisplay();


        }

        public void blinkScreen(short w, short h, ushort color, int times) 
        {
            for (int i = 0; i < times; i++) 
            {
                clearScreen();
                Thread.Sleep(300);
                fillScreen(w, h, color);
                Thread.Sleep(600);
            }    


        }


    }
}
