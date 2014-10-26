using System;
using System.Collections;
using System.Threading;
using System.IO;
using System.Text;



using Microsoft.SPOT;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

using Gadgeteer.Modules.GHIElectronics;


namespace GadgeteerFlexor
{
    /*
     * Clase encargada de manejar la comunicacion con la tarjeta SD
     */ 

    class DataManager
    {
        public static string[] faces = new string[4] { "SMILE_GREEN", "SMILE_YELLOW", "FROWN_YELLOW", "FROW_RED" };

        private String config_file = @"\config.cfg";

        private bool sd_mounted;

        private Gadgeteer.Modules.GHIElectronics.SDCard sdCard;
        private Graphics graphics;

        public DataManager(Gadgeteer.Modules.GHIElectronics.SDCard _sdCard,  Graphics _graphics)
        {

            sdCard = _sdCard;
            graphics = _graphics;
            sd_mounted = false;

            /*
             * Se añaden los eventos asociados a la tarjeta SD
             */

            sdCard.SDCardMounted += new SDCard.SDCardMountedEventHandler(sdCard_SDCardMounted);
            sdCard.SDCardUnmounted += new SDCard.SDCardUnmountedEventHandler(sdCard_SDCardUnmounted);

            //Verifica el estado de la SD

            checkSDStatus();

        }

        void checkSDStatus() 
        {
            /*
             * Si la tarjeta esta insertada pero no montada, intentamos montarla
             */

            if (!sdCard.IsCardInserted)
            {
                //Se pinta un cuadrado amarillo indicando que la tarjeta SD se ha desmontado
                graphics.blinkScreen(8, 8, GTM.Efelunte.AdafruitLedBackpack.LED_YELLOW, 10);

            }
            else
            {

                if (sdCard.IsCardMounted)
                {
                    //Se pinta un cuadrado verde indicando que la tarjeta SD se ha cargado
                    graphics.fillScreen(8, 8, GTM.Efelunte.AdafruitLedBackpack.LED_GREEN);
                }
                else
                {
                    //Se pinta un cuadrado amarillo indicando que la tarjeta SD se ha desmontado
                    graphics.blinkScreen(8, 8, GTM.Efelunte.AdafruitLedBackpack.LED_YELLOW, 10);

                    sdCard.MountSDCard();
                }

            }

        }


        void sdCard_SDCardUnmounted(SDCard sender)
        {
            sd_mounted = false;

            //Se pinta un cuadrado amarillo indicando que la tarjeta SD se ha desmontado
            //graphics.fillScreen(8, 8, GTM.Efelunte.AdafruitLedBackpack.LED_YELLOW);
            graphics.blinkScreen(8, 8, GTM.Efelunte.AdafruitLedBackpack.LED_YELLOW, 10);

            Debug.Print("The SD card has been unmounted");
            Debug.Print("DO NOT try to access it without mounting it again first");
        }

        void sdCard_SDCardMounted(SDCard sender, GT.StorageDevice SDCard)
        {
            sd_mounted = true;

            //Se pinta un cuadrado verde indicando que la tarjeta SD se ha cargado
            graphics.fillScreen(8, 8, GTM.Efelunte.AdafruitLedBackpack.LED_GREEN);

            Debug.Print("SD card has been successfully mounted. You can now read/write/create/delete files");
            Debug.Print("Unmount before removing");
        }

        public String readConfigFile(String _root_dir, GT.StorageDevice _storage) 
        {

            string path = "";
            string new_faces_file = "";

            new_faces_file = @"\data\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-f") + ".dat";

            if (File.Exists(_root_dir + config_file))
            {
                path = readFromFile(_root_dir, config_file);

                Debug.Print("Leo del archivo de config: '" + path + "'");
            }

            if (path == null || path.CompareTo("") == 0)
            {

                /*
                * Guardamos en el archivo de configuracion, el nombre del nuevo archivo de caras
                */

                writeToFile(_root_dir, config_file, new_faces_file, false);

                Debug.Print("Escribo en " + _root_dir + config_file + " el nombre de archivo: " + new_faces_file);

                path = new_faces_file;

            }

            /*
            * Si no existe, creamos el archivo
            */

            if (!Directory.Exists(_root_dir + @"\data"))
            {
                _storage.CreateDirectory(@"\data");
                Debug.Print("Creo el directorio: " + @"\data");
            }

            return path;
        
        }


        public void guardaActualCara(int currentFace)
        {

            GT.StorageDevice storage;
            String root_dir;
            String faces_file_path;
            String new_line;

            /*
             * Se obtiene el nombre del archivo donde se van a guardar las caras
             */

            storage = sdCard.GetStorageDevice();

            if (storage == null)
            {
                Debug.Print("La referencia al storage es nula!!!");
                return;
            }

            root_dir = storage.RootDirectory;

            if (root_dir == null || root_dir == String.Empty)
            {
                Debug.Print("El directorio raiz es nulo o vacio!!");
                return;
            }

            faces_file_path = readConfigFile(root_dir, storage);

            Debug.Print("El archivo de caras es: " + faces_file_path);

            /*
             * Guardar la hora y la cara guardada
             */

            if (faces_file_path == null || faces_file_path.Trim() == String.Empty) 
            {
                Debug.Print("La ruta del archivo que guarda las caras es nulo o es vacio!!");
                return;
            }

            new_line = faces[currentFace] + "," + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + "\n";

            writeToFile(root_dir, faces_file_path, new_line, true);

            Debug.Print("Se guarda en la SD la cara: " + faces[currentFace]);

        }


        public bool isReady()
        {
            if (!sdCard.IsCardInserted)
            {
                return false;
            }

            if (!sd_mounted)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public void unmountSDCard() 
        {
            sdCard.UnmountSDCard();
        }

        public String readFromFile(String root_dir, String path)
        {

            /*
             * El path debe incluir el root directory
             **/

            StreamReader sr;
            String content;
            
            content = "";
            sr = new StreamReader(root_dir + path);

            if (sr == null) 
            {
                Debug.Print("El streamreader es NULO!!!");
                return "";
            }

            content = sr.ReadToEnd();

            sr.Close();

            if (content == null) 
            {
                return "";
            }

            return content;

        }


        public void writeToFile(String root_dir, String path, String data, bool append)
        {

            StreamWriter sw;

            sw = new StreamWriter(root_dir + path, append);

            if (sw == null) 
            {
                Debug.Print("El streamwriter es NULO!!");
                return;
            }

            sw.Write(data);

            sw.Close();

        }


    }
}
