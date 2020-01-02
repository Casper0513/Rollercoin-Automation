using Rollercoin.API.Core;
using Rollercoin.API.Mining;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.WinForms
{
    static class Program
    {
        /// <summary>
        /// 
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Console.WriteLine("Rollercoin Automation, Client.WinForms, sample 2");
            if(Directory.Exists("Cache"))
            {
                Console.WriteLine("Clearing chromium cache from the last session...");
                //Directory.Delete("Cache", true);
            }
            if(Directory.Exists("GPUCache"))
            {
                Console.WriteLine("Clearing chromium GPU cache from the last session...");
                Directory.Delete("GPUCache", true);
            }
            API_Instance API = new API_Instance();
            Console.WriteLine($"API.Login => {API.Login(new CredentialModel("0x27492temp@gmail.com", null, "Asusamd74"))}");
            Console.WriteLine("Initializing the GameWindow...");
            GameWindow game = new GameWindow(API);
            game.Show();

            Application.Run(game);
        }
    }
}
