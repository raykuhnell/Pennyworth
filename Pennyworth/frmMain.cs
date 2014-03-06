using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Management;
using MovablePython;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using Pennyworth.Entities;

namespace Pennyworth
{
    public partial class frmMain : Form
    {
        DefinitionCollection definitions = new DefinitionCollection();

        public frmMain()
        {
            InitializeComponent();

            RegisterHotKey();

            // Load definitions file.
            try
            {
                using (var reader = XmlReader.Create("Definitions.xml"))
                {
                    definitions = (DefinitionCollection)new XmlSerializer(typeof(DefinitionCollection)).Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Exception occurred loading definitions file: {0}", ex.ToString()));
            }

            AutoCompleteStringCollection collection = new AutoCompleteStringCollection();

            foreach (Definition d in definitions.Items)
            {
                collection.Add(d.Name);
            }

            // Add applications from Start Menu.
            string[] apps = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "*.lnk", SearchOption.AllDirectories);
            foreach (string app in apps)
            {
                string name = app.Substring(app.LastIndexOf("\\") + 1);
                name = name.Replace(".lnk", "");
                if (!definitions.Items.Any(x => x.Name == name))
                {
                    collection.Add(name);
                    definitions.Items.Add(new Definition() { Name = name, Command = app });
                }
                else
                {
                    Debug.WriteLine(String.Format("Duplicate key attempted to be added: {0}", name));
                }
            }

            txtQuery.AutoCompleteCustomSource = collection;
        }

        private void RegisterHotKey()
        {
            // Enable alt + space hotkey.
            // This should be an option in future versions.
            var hk = new Hotkey();
            hk.Alt = true;
            hk.KeyCode = Keys.Space;
            hk.Pressed += HotKey_Pressed;

            if (hk.GetCanRegister(this))
            {
                hk.Register(this);
            }
            else
            {
                Debug.WriteLine("Could not register hotkey.");
            }
        }

        private void HotKey_Pressed(object sender, EventArgs e)
        {
            this.Show();
        }

        private void txtQuery_TextChanged(object sender, EventArgs e)
        {
            /*
            lbResults.Items.Clear();
            
            string pattern = String.Format(@"(^|.*\s){0}.*", txtQuery.Text);
            foreach (Definition d in definitions.Items)
            {
                Match match = Regex.Match(d.Name, pattern);
                if (match.Success)
                {
                    lbResults.Items.Add(d.Name);
                }
            }

            if (lbResults.Items.Count > 0)
            {
                lbResults.Visible = true;
                lbResults.BringToFront();
            }
            */
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            // Process option.
            Definition d = definitions.Items.FirstOrDefault(x => x.Name == txtQuery.Text);

            if (d != null)
            {
                string arguments = d.Arguments == null ? String.Empty : String.Join(" ", d.Arguments);
                Process.Start(d.Command, arguments);
                txtQuery.Text = String.Empty;
                this.Hide();
            }
            else
            {
                MessageBox.Show("Command not found.", "Pennyworth");
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MINIMIZE = 0xF020;

            if (m.Msg == WM_SYSCOMMAND)
            {
                switch ((int)m.WParam)
                {
                    case SC_MINIMIZE:
                        this.Hide();
                        return;
                }
            }
            base.WndProc(ref m);
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void notifyIconContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Application.Exit();
        }
    }
}
