using CefSharp;
using CefSharp.WinForms;
using Rollercoin.API.Core;
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

namespace Client.WinForms
{
    public partial class GameWindow : Form
    {
        public API_Instance API;
        public IMinigame CurrentGame;
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
            Console.WriteLine("Done.");
            webBrowser.Load(@"https://rollercoin.com/game/choose_game");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(CurrentGame == null)
            {
                CurrentGame = new MemoryGame(webBrowser, "#game1 > canvas", pictureBox);
            }

            CurrentGame.SelectGame();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (CurrentGame == null) return;

            webBrowser.Focus();
            CurrentGame.StartGame();
        }

        public void SendMouseClick(Point point)
        {
            webBrowser.GetBrowserHost().SendMouseMoveEvent(point.X, point.Y, false, CefEventFlags.None);
            webBrowser.GetBrowserHost().SendMouseClickEvent(point.X, point.Y, MouseButtonType.Left, false, 4, CefEventFlags.None);
            webBrowser.GetBrowserHost().SendMouseMoveEvent(point.X + 5, point.Y + 5, false, CefEventFlags.None);
            webBrowser.GetBrowserHost().SendMouseClickEvent(point.X + 5, point.Y + 5, MouseButtonType.Left, true, 4, CefEventFlags.None);
        }

        private void Button3_Click(object sender, EventArgs e)
        {

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (CurrentGame == null) return;
            if (!CurrentGame.GameStarted) return;
            CurrentGame.Solve_Tick();
        }
    }
}
