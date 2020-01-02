using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json.Linq;
using Rollercoin.API.Core;
using Rollercoin.API.Databases;
using Rollercoin.API.Minigames;
using Rollercoin.API.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.WinForms
{
    public partial class GameWindow : Form
    {
        public API_Instance API;
        public GameTickLoop GameTickLoop;
        public IMinigame CurrentGame;
        public Bot BotInstance;
        public DatabaseInterface DatabaseInterface;
        public GameWindow(API_Instance api)
        {
            InitializeComponent();
            Console.WriteLine("[GameWindow] WinForm initializing...");
            if (api == null)
            {
                Close();
                return;
            }
            API = api;
            var settings = new CefSettings()
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36",
                CachePath = "Cache"
            };
            Console.Write("[GameWindow] Initialize chromium... ");
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            webBrowser.IsBrowserInitializedChanged += WebBrowser_IsBrowserInitializedChanged;
        }



        private void WebBrowser_IsBrowserInitializedChanged(object sender, EventArgs e)
        {
            if (!webBrowser.IsBrowserInitialized) return;
            Console.WriteLine("Done.");
            webBrowser.Load(@"https://rollercoin.com/game/choose_game");
            DatabaseInterface = new DatabaseInterface("localhost", "rollercoin_bot", "RpnMq0C1cp1MkbFq", "rollercoin_automation");
            BotInstance = new Bot(API, webBrowser, DatabaseInterface, pictureBox);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            BotInstance.Run();
        }
    }
}
