using System;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Project1
{
    public partial class SettingsMy : Form
    {
        public SettingsMy()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists("Settings GTS.xml"))
            {
                System.IO.File.Delete("Settings GTS.xml");
                XDocument hDoc = new XDocument(new XElement("settings",
                        new XElement("start_about",
                        new XAttribute("value", checkBox1.Checked))));

                hDoc.Save("Settings GTS.xml");
            }
            else
            {
                XDocument hDoc = new XDocument(new XElement("settings",
                        new XElement("start_about", 
                        new XAttribute("value", checkBox1.Checked))));

                hDoc.Save("Settings GTS.xml");
            }
            this.Close();
        }

        private void SettingsMy_Load(object sender, EventArgs e)
        {
            bool st_ab_val = false;
            XDocument settings_doc = XDocument.Load("Settings GTS.xml");
            XElement root = settings_doc.Element("settings");
            
            foreach (XElement xe in root.Elements("start_about"))
            {                
                XAttribute start_ab = xe.Attribute("value");
                st_ab_val = Convert.ToBoolean(start_ab.Value);                
            }

            checkBox1.Checked = st_ab_val;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
