using CefSharp;
using CefSharp.Web;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rollercoin.API
{
    public class ChromiumWebBrowserMouseControls : ChromiumWebBrowser
    {
        public ChromiumWebBrowserMouseControls() : base() { }
        public ChromiumWebBrowserMouseControls(HtmlString html, IRequestContext requestContext = null) : base(html, requestContext) { }
        public ChromiumWebBrowserMouseControls(string address, IRequestContext requestContext = null) : base(address, requestContext) { }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Console.WriteLine($"[ChromiumWebBrowserMouseControls] Mouse click at: {e.X}, {e.Y}");
            base.OnMouseClick(e);
        }


        protected override void OnClick(EventArgs e)
        {
            Console.WriteLine($"[ChromiumWebBrowserMouseControls] Click");
            base.OnClick(e);
        }

        public void ClickMouse(Point location)
        {
            MouseEventArgs args = new MouseEventArgs(MouseButtons.Left, 1, location.X, location.Y, 0);
            OnMouseClick(args);
        }
    }
}
