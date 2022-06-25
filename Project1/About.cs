using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Project1
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = "Тестирование оперативно-информационного комплекса СК-2007\n" +
                "\n" +
                "Для начала тестирования оперативно-информационного комплекса СК-2007 необходимо выполнить следующие действия:\n" +
                "1) Загрузить или создать список параметров сигналов;\n" +
                "2) Загрузить или создать сценарий изменения параметров сигналов;\n" +
                "3) Подключиться к одному из серверов ОИК СК-2007;\n" +
                "4) Запустить тестирование.\n" +
                "\n" +
                "Генератор тестовых сигналов телеизмерений и телесигнализации\n" +
                "Автор: Алехин Роман Александрович\n" +
                "Научный руководитель: Свечкарев Сергей Владимирович";

            richTextBox1.Select(0, 57); //выделяем текст
            richTextBox1.SelectionColor = Color.Red; //для выделенного текста устанавливаем цвет
            richTextBox1.SelectionFont = new Font("Times New Roman", 14, FontStyle.Bold);

            richTextBox1.Select(58, 305); //выделяем текст            
            richTextBox1.SelectionFont = new Font("Microsoft Sans Serif", 10, FontStyle.Regular);

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
    }
}
