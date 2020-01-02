using Rollercoin.API.Databases;
using Rollercoin.API.Minigames;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.WinForms
{
    public partial class Form1 : Form
    {
        DatabaseInterface DBInterface;
        Dictionary<int, BotGameLog> Dataset;
        public Form1()
        {
            InitializeComponent();
            DBInterface = new DatabaseInterface("192.168.5.9", "rollercoin_bot", "RpnMq0C1cp1MkbFq", "rollercoin_automation");
            Dataset = DBInterface.ReadAllRows();
        }

        public void Shutdown()
        {
            Application.Exit();
        }

        private void CloseBtn_MouseEnter(object sender, EventArgs e)
        {
            closeBtn.BackColor = Color.FromArgb(41, 41, 50);
        }

        private void CloseBtn_MouseLeave(object sender, EventArgs e)
        {
            closeBtn.BackColor = Color.FromArgb(21, 21, 30);
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            Shutdown();
        }
    }
}
