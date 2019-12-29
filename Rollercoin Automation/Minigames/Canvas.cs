using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rollercoin.API.Minigames
{
    public enum MouseButton
    {
        Left,
        Right
    }

    public class Canvas
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public ChromiumWebBrowser CanvasOwner;
        public string CanvasElementSelector;

        public Canvas(ChromiumWebBrowser canvasOwner, string canvasElementSelector)
        {
            CanvasOwner = canvasOwner;
            CanvasElementSelector = canvasElementSelector;
        }

        public Size Size
        {
            get
            {
                Task<JavascriptResponse> jsResponse = CanvasOwner.EvaluateScriptAsync(@"
                    function getCanvasSize() {
                        var canvas = document.querySelector('#game1 > canvas')
                        return JSON.stringify({ width: canvas.width, height: canvas.height })
                    }

                    getCanvasSize();");

                jsResponse.Wait();

                var result = jsResponse.Result.Result;
                if (result == null) return CanvasOwner.Size;
                JToken result_deserialized = JToken.Parse((string)result);
                return new Size((int)result_deserialized["width"], (int)result_deserialized["height"]);
            }
        }

        public void Invoke_MouseClick_Safe(Point canvasPoint, MouseButton button)
        {
            Point currentMouseCoords = Cursor.Position;
            Point screenCanvasPoint = CanvasOwner.PointToScreen(canvasPoint);
            Cursor.Position = screenCanvasPoint;
            switch(button)
            {
                case MouseButton.Left:
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)screenCanvasPoint.X, (uint)screenCanvasPoint.Y, 0, 0);
                    break;
                case MouseButton.Right:
                    mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, (uint)screenCanvasPoint.X, (uint)screenCanvasPoint.Y, 0, 0);
                    break;
            }
            Cursor.Position = currentMouseCoords;
        }
    }
}
