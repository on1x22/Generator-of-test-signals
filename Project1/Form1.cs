using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Threading;
using OICDAC;
using System.Diagnostics;

namespace Project1
{

    public partial class GTS : Form
    {
        // Инициализация компонентов
        public GTS()
        {
            InitializeComponent();
                        
            button3.Enabled = false;
            button4.Enabled = false;
            groupBox1.Visible = false;
            label2.Text = "Не выбран сервер ОИК";
        }

        // Вывод текущего времени
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            label12.Text = now.ToString("HH:mm:ss");
            label11.Text = now.ToString("dd.MM.yy");
        }
        
        // Общие переменные
        OpenFileDialog OPN = new OpenFileDialog();
        SaveFileDialog SVE = new SaveFileDialog();
        OpenFileDialog SCR_OPN = new OpenFileDialog();
        SaveFileDialog SCR_SVE = new SaveFileDialog();
        XDocument xdoc;
        XDocument datadoc;
        XDocument scriptdoc;
        int secondes = 0;
        int emiT = 0;
        int stroka = 0;
        string Comment;
        string FileNameXml;
        string ScriptNameXml;                
        public int select_script_index;
        public static string[,] Masper = new string[0, 0];
        public static string[,] MSChecking1 = new string[0, 0];
        public static string[] MSChecking2 = new string[0];
        public bool ParamChanged = false;
        public bool ScriptChanged = false;
        bool comment_window = false;        
        bool NewScript = false;        
        public bool v_nachalo = false;
        public bool posle = false;        
        public static DAC dak = new DACClass();
        public static int sravnen;
        int dla = 0;       

        public List<Signal> SignalList = new List<Signal>();
        public List<ICommand> ScriptList = new List<ICommand>();
        
        // Меню. Открывает файл *.xml с ПАРАМЕТРАМИ // Открытие xml-файла. Из меню сверху
        private void параметрыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ParamChanged == true)
            {
                if (FileNameXml != null)
                {
                    DialogResult dr1 = MessageBox.Show("В файл с данными внесены изменения. Хотите переписать данные?", "Изменение параметров", MessageBoxButtons.YesNo);
                    if (dr1 == DialogResult.Yes)
                    {
                        xdoc = XDocument.Load(FileNameXml);

                        XElement root = xdoc.Element("parametrs");
                        foreach (XElement xe in root.Elements("signal").ToList())
                        {
                            xe.Remove();
                        }

                        xdoc.Save(FileNameXml);

                        foreach (Signal sig in SignalList)
                        {
                            XNode xNewnod2 = new XElement("signal",
                               new XAttribute("name", sig.name),
                               new XAttribute("type", sig.Type),
                               new XAttribute("id", sig.id),                               
                               new XAttribute("value", sig.start_value),
                               new XAttribute("code", sig.code),
                               new XElement("peredacha",
                               new XAttribute("kind", sig.kind),
                               new XAttribute("prop", sig.prop)));
                            xdoc.Root.Add(xNewnod2);
                        }

                        xdoc.Save(FileNameXml);

                        otkritie();
                    }                   
                }
                else
                {
                    DialogResult dr1 = MessageBox.Show("Хотите создать новый файл с данными?", "Сохранение параметров", MessageBoxButtons.YesNo);
                    if (dr1 == DialogResult.Yes)
                    {
                        string put;
                        XDocument hDoc = new XDocument(new XElement("parametrs"));
                        SVE.Filter = "XML (*.xml)| *.xml; | Все (*.*)|*.*;";
                        if (SVE.ShowDialog() == DialogResult.OK)
                        {
                            put = SVE.FileName;
                            FileNameXml = put;

                            hDoc.Save(FileNameXml);

                            hDoc = XDocument.Load(FileNameXml);
                            foreach (Signal sig in SignalList)
                            {
                                // Добавление новой информации
                                XNode xNewnod2 = new XElement("signal",
                                   new XAttribute("name", sig.name),
                                   new XAttribute("type", sig.Type),
                                   new XAttribute("id", sig.id),
                                   new XAttribute("value", sig.start_value),
                                   new XAttribute("code", sig.code),
                                   new XElement("peredacha",
                                   new XAttribute("kind", sig.kind),
                                   new XAttribute("prop", sig.prop)));
                                hDoc.Root.Add(xNewnod2);
                            }
                            hDoc.Save(FileNameXml);

                            Param_imia(FileNameXml);
                            otkritie();
                        }
                    }
                    // ТОЖЕ ЧТО-ТО НЕПОНЯТНОЕ (наверно, должно быть пусто)
                    else
                    {
                        SignalList.RemoveRange(0, SignalList.Count);
                        ParamChanged = false;
                        otkritie();
                    }
                }
            }
            else
            {
                SignalList.RemoveRange(0, SignalList.Count);
                ParamChanged = false;
                otkritie();
            }
        }

        public static DAC dagg = new DACClass();
        OIRequest rqActual;

        public void otkritie()
        {
            try
            {
                // Открытие файла
                OPN.Filter = "XML (*.xml)| *.xml; | Все (*.*)|*.*;";
                if (OPN.ShowDialog() == DialogResult.OK)
                {
                    SignalList.RemoveRange(0, SignalList.Count);
                    ParamChanged = false;

                    FileNameXml = OPN.FileName;
                    listBox1.Items.Clear();

                    // Занесение данных в список
                    datadoc = XDocument.Load(FileNameXml);
                    XElement root = datadoc.Element("parametrs");
                    foreach (XElement xe in root.Elements("signal"))
                    {
                        var sig = new Signal();
                        XAttribute nameg = xe.Attribute("name");
                        sig.name = nameg.Value;

                        XAttribute typeg = xe.Attribute("type");
                        sig.Type = typeg.Value;

                        XAttribute idg = xe.Attribute("id");
                        sig.id = Convert.ToInt32(idg.Value);

                        XAttribute valueg = xe.Attribute("value");
                        sig.value = Convert.ToDouble(valueg.Value);
                        sig.start_value = sig.value;
                        sig.last_value = sig.value;

                        XAttribute codeg = xe.Attribute("code");
                        sig.code = codeg.Value;
                        sig.last_code = codeg.Value;
                        sig.start_code = codeg.Value;

                        XElement kindg = xe.Element("peredacha");

                        XAttribute kik = kindg.Attribute("kind");
                        sig.kind = kik.Value;

                        XAttribute propk = kindg.Attribute("prop");
                        sig.prop = Convert.ToDouble(propk.Value);

                        sig.last_command = "Assign";

                        SignalList.Add(sig);
                        SignalList.Sort();
                    }
                    SignalList.Sort();

                    Refresh1();                    
                    Param_imia(FileNameXml);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Структура файла не совпадает со структурой файла с параметрами");
            }
        }

        // Менюха сверху. Закрытие программы
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        // Меню. Создание новых пустых xml-файлов ПАРАМЕТРОВ. Меню сверху
        private void параметрыToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            // Проверка
            if (ParamChanged == true)
            {
                if (FileNameXml != null)
                {
                    DialogResult dr1 = MessageBox.Show("В файл с данными внесены изменения. Хотите переписать данные?", "Изменение параметров", MessageBoxButtons.YesNo);
                    if (dr1 == DialogResult.Yes)
                    {
                        xdoc = XDocument.Load(FileNameXml);

                        XElement root = xdoc.Element("parametrs");
                        foreach (XElement xe in root.Elements("signal").ToList())
                        {
                            xe.Remove();
                        }

                        xdoc.Save(FileNameXml);

                        foreach (Signal sig in SignalList)
                        {
                            XNode xNewnod2 = new XElement("signal",
                               new XAttribute("name", sig.name),
                               new XAttribute("type", sig.Type),
                               new XAttribute("id", sig.id),
                               new XAttribute("value", sig.start_value),
                               new XAttribute("code", sig.code),
                               new XElement("peredacha",
                               new XAttribute("kind", sig.kind),
                               new XAttribute("prop", sig.prop)));
                            xdoc.Root.Add(xNewnod2);
                        }

                        xdoc.Save(FileNameXml);

                        SignalList.RemoveRange(0, SignalList.Count);
                        ParamChanged = false;
                        FileNameXml = null;
                    }
                    else
                    {
                        SignalList.RemoveRange(0, SignalList.Count);
                        ParamChanged = false;
                        FileNameXml = null;
                    }
                }
                else
                {
                    DialogResult dr1 = MessageBox.Show("Хотите создать новый файл с данными?", "Сохранение параметров", MessageBoxButtons.YesNo);
                    if (dr1 == DialogResult.Yes)
                    {
                        string put;
                        XDocument hDoc = new XDocument(new XElement("parametrs"));
                        SVE.Filter = "XML (*.xml)| *.xml; | Все (*.*)|*.*;";
                        if (SVE.ShowDialog() == DialogResult.OK)
                        {
                            put = SVE.FileName;
                            FileNameXml = put;

                            hDoc.Save(FileNameXml);

                            xdoc = XDocument.Load(FileNameXml);
                            foreach (Signal sig in SignalList)
                            {
                                XNode xNewnod2 = new XElement("signal",
                                   new XAttribute("name", sig.name),
                                   new XAttribute("type", sig.Type),
                                   new XAttribute("id", sig.id),
                                   new XAttribute("value", sig.start_value),
                                   new XAttribute("code", sig.code),
                                   new XElement("peredacha",
                                   new XAttribute("kind", sig.kind),
                                   new XAttribute("prop", sig.prop)));
                                xdoc.Root.Add(xNewnod2);
                            }

                            xdoc.Save(FileNameXml);

                            Param_imia(FileNameXml);
                            SignalList.RemoveRange(0, SignalList.Count);
                            ParamChanged = false;
                            FileNameXml = null;
                        }
                        else
                        {
                            SignalList.RemoveRange(0, SignalList.Count);
                            ParamChanged = false;
                            FileNameXml = null;
                        }
                    }
                    else
                    {
                        SignalList.RemoveRange(0, SignalList.Count);
                        ParamChanged = false;
                        FileNameXml = null;
                    }
                }
            }
            else
            {
                SignalList.RemoveRange(0, SignalList.Count);
                ParamChanged = false;
                FileNameXml = null;
            }

            Refresh1();

            tabControl1.TabPages[0].Text = "Параметры: " + "``@ " + "parametrs" + " @``";
        }

        // Меню. Сохранение Параметров сигналов поверх существующего файла
        private void параметрыToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            File.Delete(FileNameXml);

            XDocument hDoc = new XDocument(new XElement("parametrs"));
            hDoc.Save(FileNameXml);

            hDoc = XDocument.Load(FileNameXml);
            foreach (Signal sig in SignalList)
            {
                XNode xNewnod2 = new XElement("signal",
                   new XAttribute("name", sig.name),
                   new XAttribute("type", sig.Type),
                   new XAttribute("id", sig.id),
                   new XAttribute("value", sig.start_value),
                   new XAttribute("code", sig.code),
                   new XElement("peredacha",
                   new XAttribute("kind", sig.kind),
                   new XAttribute("prop", sig.prop)));
                hDoc.Root.Add(xNewnod2);
            }

            hDoc.Save(FileNameXml);
        }

        // Меню. Сохранение файла c параметрами сигналов KAK. Меню сверху
        private void параметрыToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            string put;

            XDocument hDoc = new XDocument(new XElement("parametrs"));
            SVE.Filter = "XML (*.xml)| *.xml; | Все (*.*)|*.*;";
            if (SVE.ShowDialog() == DialogResult.OK)
            {
                put = SVE.FileName;
                FileNameXml = put;

                hDoc.Save(FileNameXml);

                hDoc = XDocument.Load(FileNameXml);
                foreach (Signal sig in SignalList)
                {
                    XNode xNewnod2 = new XElement("signal",
                       new XAttribute("name", sig.name),
                       new XAttribute("type", sig.Type),
                       new XAttribute("id", sig.id),
                       new XAttribute("value", sig.start_value),
                       new XAttribute("code", sig.code),
                       new XElement("peredacha",
                       new XAttribute("kind", sig.kind),
                       new XAttribute("prop", sig.prop)));
                    hDoc.Root.Add(xNewnod2);
                }

                hDoc.Save(FileNameXml);
            }




        }

        // Меню. Настройка подключения к КДД
        private void кДДToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KDD_Option opsh = new KDD_Option(this);
            opsh.ShowDialog();
        }
               
        // Меню. Открытие дополнения
        private void дополнительноToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        // Закрытие программы
        private void GTS_FormClosing(object sender, FormClosingEventArgs e)
        {           
        }

        // Удаление сигнала. Из верхнего меню
        private void удалллитьСигнToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr1 = MessageBox.Show("Вы уверены, что хотите удалить сигнал?", "Удаление", MessageBoxButtons.YesNo);
            if (dr1 == DialogResult.Yes)
            {
                int delit = listBox1.SelectedIndex;

                // Удаление информации из выбраной строки массива
                for (int i = 0; i < SignalList.Count; i++)
                {
                    if (i == delit)
                    {
                        SignalList.RemoveAt(i);
                    }
                }
                ParamChanged = true;
                Refresh1();
            }
        }

        // Удаление всех сигналов. Из верхнего меню
        private void удалитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SignalList.RemoveRange(0, SignalList.Count);
            ParamChanged = true;
            Refresh1();
        }

        // Добавление нового сигнала
        private void добаввитьСигToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sozd_signala sozdsig = new Sozd_signala(this, ParamChanged);
            sozdsig.Show();
        }
        
        // Метод. Обновление списка сигналов
        public void Refresh1()
        {
            listBox1.Items.Clear();

            foreach (Signal sig in SignalList)
            {
                listBox1.Items.Add(sig.name);
            }
        }

        // Метод. Обновление списка СЦЕНАРИЯ
        public void Refresh2()
        {
            listBox2.Items.Clear();

            foreach (ICommand scr in ScriptList)
            {
                string itiem = "";
                string ggg = scr.Com_name;
                itiem = scr.Title;

                listBox2.Items.Add(itiem);
                itiem = "";
            }
        }
        
        // Метод, показывающий имя открытого файла с СИГНАЛАМИ
        void Param_imia(string FileNameXml)
        {
            tabControl1.TabPages[0].Text = "Параметры: " + Path.GetFileName(FileNameXml);
        }

        // Метод, показывающий имя открытого файла со СЦЕНАРИЕМ
        void Script_imia(string ScriptNameXml)
        {
            tabControl1.TabPages[1].Text = "Сценарий: " + Path.GetFileName(ScriptNameXml);
        }
        
        // Операции при выборе регламента
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        // Метод. Сброс сравнения
        public void Sbros_sravnenya()
        {
        }

        // Открытие справки с кодами сигналов
        private void button4_Click(object sender, EventArgs e)
        {
            Kod_dostovernosti kodd = new Kod_dostovernosti();
            kodd.Show();
        }

        // При открытии программы поля с параметрами будут недоступны
        public Thread Spravka;
        public void zApusk_spravki()
        {
            bool st_ab_val = false;
            if (System.IO.File.Exists("Settings GTS.xml"))
            {
                XDocument settings_doc = XDocument.Load("Settings GTS.xml");
                XElement root = settings_doc.Element("settings");

                foreach (XElement xe in root.Elements("start_about"))
                {
                    XAttribute start_ab = xe.Attribute("value");
                    st_ab_val = Convert.ToBoolean(start_ab.Value);
                }

                if (st_ab_val == true)
                {
                    About abou = new About();
                    abou.ShowDialog();
                }
            }
            else
            {
                XDocument hDoc = new XDocument(new XElement("settings",
                        new XElement("start_about",
                        new XAttribute("value", true))));

                hDoc.Save("Settings GTS.xml");

                About abou = new About();
                abou.ShowDialog();
            }
        }

        private void GTS_Load(object sender, EventArgs e)
        {
            Spravka = new Thread(zApusk_spravki);
            Spravka.Start();
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();

            for (int i = 0; i < Masper.GetLength(0); i++)
            {
                listBox2.Items.Add(Masper[i, 0]);
            }
        }
        
        // Выбор Сигнала из списка
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int fff = listBox1.SelectedIndex;
            int num = 0;
            foreach (Signal sig in SignalList)
            {
                if (num == fff)
                {
                    propertyGrid2.SelectedObject = sig;
                }
                num++;
            }
            podkluchenie_OIK();
        }

        // Изменение параметров СИГНАЛОВ
        private void propertyGrid2_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            SignalList.Sort();
            Refresh1();
            propertyGrid2.Refresh();
            ParamChanged = true;
            for (int i = 0; i < SignalList.Count; i++)
            {
                if (SignalList[i].code != SignalList[i].start_code)
                {
                    SignalList[i].start_code = SignalList[i].code;
                }
            }
            try
                {
                    if (CheckScript.IsAlive == true)
                    {
                        CheckScript.Abort();
                        timer2.Enabled = false;
                        this.Invoke((MethodInvoker)delegate()
                        {
                            richTextBox1.Select(stroka, stroka + 31); //выделяем текст                
                            richTextBox1.SelectionColor = Color.Red; //для выделенного текста устанавливаем цвет
                            richTextBox1.AppendText("Завершение выполнения сценария" + Environment.NewLine);
                            stroka = stroka + 31;
                            richTextBox1.ScrollToCaret();
                        });
                    
                        // Сброс поля Прошлая команда
                        foreach (Signal sig in SignalList)
                        {
                            sig.last_command = "Assign";
                        }
                    }
                }
                catch (NullReferenceException)
                { }
                       
            podkluchenie_OIK();
            secondes = 0;
        }
        
        // Открытие файла СЦЕНАРИЯ
        private void сценарийToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                // Открытие файла
                SCR_OPN.Filter = "XML (*.xml)| *.xml; | Все (*.*)|*.*;";
                if (SCR_OPN.ShowDialog() == DialogResult.OK)
                {
                    ScriptList.RemoveRange(0, ScriptList.Count);

                    ScriptNameXml = SCR_OPN.FileName;

                    // Занесение данных в список
                    scriptdoc = XDocument.Load(ScriptNameXml);
                    XElement root = scriptdoc.Element("Scenariy");
                    foreach (XElement xe in root.Elements("Command"))
                    {
                        XAttribute com_nameg = xe.Attribute("Com_name");
                        string eman = com_nameg.Value;
                        if (eman == "Assign")
                        {
                            var scen = new Assign();

                            XElement scr_nameg = xe.Element("name");
                            scen.name = scr_nameg.Value;

                            XElement scr_typeg = xe.Element("type");
                            scen.type = scr_typeg.Value;

                            XElement scr_valueg = xe.Element("value");
                            scen.value = Convert.ToDouble(scr_valueg.Value);

                            ScriptList.Add(scen);
                        }
                        else if (eman == "Pause")
                        {
                            var paus = new Pause();

                            XElement scr_valueg = xe.Element("time");
                            paus.time = Convert.ToDouble(scr_valueg.Value);

                            ScriptList.Add(paus);
                        }
                        else if (eman == "Summation")
                        {
                            var summ = new Summation();

                            XElement scr_nameg = xe.Element("name");
                            summ.name = scr_nameg.Value;

                            XElement scr_typeg = xe.Element("type");
                            summ.type = scr_typeg.Value;

                            XElement scr_valueg_1 = xe.Element("value_1");
                            summ.value_1 = scr_valueg_1.Value;

                            XElement scr_valueg_2 = xe.Element("value_2");
                            summ.value_2 = scr_valueg_2.Value;

                            ScriptList.Add(summ);
                        }
                        else if (eman == "Subtraction")
                        {
                            var subtr = new Subtraction();

                            XElement scr_nameg = xe.Element("name");
                            subtr.name = scr_nameg.Value;

                            XElement scr_typeg = xe.Element("type");
                            subtr.type = scr_typeg.Value;

                            XElement scr_valueg_1 = xe.Element("value_1");
                            subtr.value_1 = scr_valueg_1.Value;

                            XElement scr_valueg_2 = xe.Element("value_2");
                            subtr.value_2 = scr_valueg_2.Value;

                            ScriptList.Add(subtr);
                        }
                        else if (eman == "Multiplication")
                        {
                            var mult = new Multiplication();

                            XElement scr_nameg = xe.Element("name");
                            mult.name = scr_nameg.Value;

                            XElement scr_typeg = xe.Element("type");
                            mult.type = scr_typeg.Value;

                            XElement scr_valueg_1 = xe.Element("value_1");
                            mult.value_1 = scr_valueg_1.Value;

                            XElement scr_valueg_2 = xe.Element("value_2");
                            mult.value_2 = scr_valueg_2.Value;

                            ScriptList.Add(mult);

                        }
                        else if (eman == "Division")
                        {
                            var div = new Division();

                            XElement scr_nameg = xe.Element("name");
                            div.name = scr_nameg.Value;

                            XElement scr_typeg = xe.Element("type");
                            div.type = scr_typeg.Value;

                            XElement scr_valueg_1 = xe.Element("value_1");
                            div.value_1 = scr_valueg_1.Value;

                            XElement scr_valueg_2 = xe.Element("value_2");
                            div.value_2 = scr_valueg_2.Value;

                            ScriptList.Add(div);
                        }
                        else if (eman == "Square")
                        {
                            var squ = new Square();

                            XElement scr_nameg = xe.Element("name");
                            squ.name = scr_nameg.Value;

                            XElement scr_typeg = xe.Element("type");
                            squ.type = scr_typeg.Value;

                            XElement scr_value_ming = xe.Element("value_min");
                            squ.value_min = Convert.ToDouble(scr_value_ming.Value);

                            XElement scr_value_maxg = xe.Element("value_max");
                            squ.value_max = Convert.ToDouble(scr_value_maxg.Value);

                            XElement scr_periodg = xe.Element("period");
                            squ.period = Convert.ToDouble(scr_periodg.Value);

                            squ.tek_time = 0;

                            squ.tek_value = 0;

                            ScriptList.Add(squ);
                        }
                        else if (eman == "Sinusoid")
                        {
                            var sin = new Sinusoid();

                            XElement scr_nameg = xe.Element("name");
                            sin.name = scr_nameg.Value;

                            XElement scr_typeg = xe.Element("type");
                            sin.type = scr_typeg.Value;

                            XElement scr_value_stertg = xe.Element("value_start");
                            sin.value_start = Convert.ToDouble(scr_value_stertg.Value);

                            XElement scr_value_maxg = xe.Element("value_max");
                            sin.value_max = Convert.ToDouble(scr_value_maxg.Value);

                            XElement scr_periodg = xe.Element("period");
                            sin.period = Convert.ToDouble(scr_periodg.Value);

                            XElement scr_middleg = xe.Element("middle");
                            sin.middle = Convert.ToDouble(scr_middleg.Value);

                            ScriptList.Add(sin);
                        }

                        else if (eman == "Triangle")
                        {
                            var tri = new Triangle();

                            XElement scr_nameg = xe.Element("name");
                            tri.name = scr_nameg.Value;

                            XElement scr_typeg = xe.Element("type");
                            tri.type = scr_typeg.Value;

                            XElement scr_value_ming = xe.Element("value_min");
                            tri.value_min = Convert.ToDouble(scr_value_ming.Value);

                            XElement scr_value_maxg = xe.Element("value_max");
                            tri.value_max = Convert.ToDouble(scr_value_maxg.Value);

                            XElement scr_periodg = xe.Element("period");
                            tri.period = Convert.ToDouble(scr_periodg.Value);

                            ScriptList.Add(tri);
                        }

                        else if (eman == "Saw")
                        {
                            var sav = new Saw();

                            XElement scr_nameg = xe.Element("name");
                            sav.name = scr_nameg.Value;

                            XElement scr_typeg = xe.Element("type");
                            sav.type = scr_typeg.Value;

                            XElement scr_value_ming = xe.Element("value_min");
                            sav.value_min = Convert.ToDouble(scr_value_ming.Value);

                            XElement scr_value_maxg = xe.Element("value_max");
                            sav.value_max = Convert.ToDouble(scr_value_maxg.Value);

                            XElement scr_periodg = xe.Element("period");
                            sav.period = Convert.ToDouble(scr_periodg.Value);

                            ScriptList.Add(sav);
                        }

                        else if (eman == "Code")
                        {
                            var cod = new Code();

                            XElement scr_nameg = xe.Element("name");
                            cod.name = scr_nameg.Value;

                            XElement scr_typeg = xe.Element("type");
                            cod.type = scr_typeg.Value;

                            XElement scr_codeg = xe.Element("code");
                            cod.code = scr_codeg.Value;

                            ScriptList.Add(cod);
                        }
                    }
                    foreach (XElement xe in root.Elements("Comment"))
                    {
                        XElement scr_commentg = xe.Element("text");
                        Comment = scr_commentg.Value;
                        textBox2.Text = Comment;
                    }

                    Refresh2();
                    Script_imia(ScriptNameXml);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Структура файла не совпадает со структурой сценария");
            }
        }

        // Выбор пункта в СЦЕНАРИИ
        public void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int fff = listBox2.SelectedIndex;
            select_script_index = fff;
            int num = 0;
            foreach (ICommand scr in ScriptList)
            {
                if (num == fff)
                {
                    propertyGrid1.SelectedObject = scr;
                }
                num++;
            }
        }
        
        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            Refresh2();
            propertyGrid1.Refresh();
            ParamChanged = true;

            try
            {
                if (CheckScript.IsAlive == true)
                {
                    CheckScript.Abort();
                    timer2.Enabled = false;
                    this.Invoke((MethodInvoker)delegate()
                    {
                        richTextBox1.Select(stroka, stroka + 31); //выделяем текст                
                        richTextBox1.SelectionColor = Color.Red; //для выделенного текста устанавливаем цвет
                        richTextBox1.AppendText("Завершение выполнения сценария" + Environment.NewLine);
                        stroka = stroka + 31;
                        richTextBox1.ScrollToCaret();
                    });
                
                    // Сброс поля Прошлая команда
                    foreach (Signal sig in SignalList)
                    {
                        sig.last_command = "Assign";
                    }
                }                
            }
            catch (NullReferenceException)
            { }
            
            button1.Enabled = true;
            button3.Enabled = false;
            button4.Enabled = false;
            secondes = 0;
        }

        // Сохранение сценария поверх существующего файла
        private void сценарийToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            File.Delete(ScriptNameXml);

            XDocument hDoc = new XDocument(new XElement("Scenariy"));
            hDoc.Save(ScriptNameXml);

            hDoc = XDocument.Load(ScriptNameXml);
            foreach (ICommand scr in ScriptList)
            {
                string comand = scr.Com_name;
                // Добавление новой информации
                switch (comand)
                {
                    // Пауза
                    case "Pause":
                        XNode xNewnod1 = new XElement("Command",
                   new XAttribute("Com_name", scr.Com_name),
                   new XElement("time", ((Pause)scr).time));
                        hDoc.Root.Add(xNewnod1);
                        break;

                    // Присвоение
                    case "Assign":
                        XNode xNewnod2 = new XElement("Command",
                   new XAttribute("Com_name", scr.Com_name),
                   new XElement("type", ((Assign)scr).type),
                   new XElement("name", ((Assign)scr).name),
                   new XElement("value", ((Assign)scr).value));
                        hDoc.Root.Add(xNewnod2);
                        break;

                    // Сумма
                    case "Summation":
                        XNode xNewnod3 = new XElement("Command",
                   new XAttribute("Com_name", scr.Com_name),
                   new XElement("type", ((Summation)scr).type),
                   new XElement("name", ((Summation)scr).name),
                   new XElement("value_1", ((Summation)scr).value_1),
                   new XElement("value_2", ((Summation)scr).value_2));
                        hDoc.Root.Add(xNewnod3);
                        break;

                    // Разность
                    case "Subtraction":
                        XNode xNewnod4 = new XElement("Command",
                   new XAttribute("Com_name", scr.Com_name),
                   new XElement("type", ((Subtraction)scr).type),
                   new XElement("name", ((Subtraction)scr).name),
                   new XElement("value_1", ((Subtraction)scr).value_1),
                   new XElement("value_2", ((Subtraction)scr).value_2));
                        hDoc.Root.Add(xNewnod4);
                        break;

                    // Произведение
                    case "Multiplication":
                        XNode xNewnod5 = new XElement("Command",
                   new XAttribute("Com_name", scr.Com_name),
                   new XElement("type", ((Multiplication)scr).type),
                   new XElement("name", ((Multiplication)scr).name),
                   new XElement("value_1", ((Multiplication)scr).value_1),
                   new XElement("value_2", ((Multiplication)scr).value_2));
                        hDoc.Root.Add(xNewnod5);
                        break;

                    // Деление
                    case "Division":
                        XNode xNewnod6 = new XElement("Command",
                   new XAttribute("Com_name", scr.Com_name),
                   new XElement("type", ((Division)scr).type),
                   new XElement("name", ((Division)scr).name),
                   new XElement("value_1", ((Division)scr).value_1),
                   new XElement("value_2", ((Division)scr).value_2));
                        hDoc.Root.Add(xNewnod6);
                        break;

                    // Меандр
                    case "Square":
                        XNode xNewnod7 = new XElement("Command",
                            new XAttribute("Com_name", scr.Com_name),
                            new XElement("type", ((Square)scr).type),
                            new XElement("name", ((Square)scr).name),
                            new XElement("value_min", ((Square)scr).value_min),
                            new XElement("value_max", ((Square)scr).value_max),
                            new XElement("period", ((Square)scr).period));
                        hDoc.Root.Add(xNewnod7);
                        break;

                    // Синусоида
                    case "Sinusoid":
                        XNode xNewnod8 = new XElement("Command",
                            new XAttribute("Com_name", scr.Com_name),
                            new XElement("type", ((Sinusoid)scr).type),
                            new XElement("name", ((Sinusoid)scr).name),
                            new XElement("value_start", ((Sinusoid)scr).value_start),
                            new XElement("value_max", ((Sinusoid)scr).value_max),
                            new XElement("period", ((Sinusoid)scr).period),
                            new XElement("middle", ((Sinusoid)scr).middle));
                        hDoc.Root.Add(xNewnod8);
                        break;

                    // Треугольник
                    case "Triangle":
                        XNode xNewnod9 = new XElement("Command",
                            new XAttribute("Com_name", scr.Com_name),
                            new XElement("type", ((Triangle)scr).type),
                            new XElement("name", ((Triangle)scr).name),
                            new XElement("value_min", ((Triangle)scr).value_min),
                            new XElement("value_max", ((Triangle)scr).value_max),
                            new XElement("period", ((Triangle)scr).period));
                        hDoc.Root.Add(xNewnod9);
                        break;

                    // Пила
                    case "Saw":
                        XNode xNewnod10 = new XElement("Command",
                            new XAttribute("Com_name", scr.Com_name),
                            new XElement("type", ((Saw)scr).type),
                            new XElement("name", ((Saw)scr).name),
                            new XElement("value_min", ((Saw)scr).value_min),
                            new XElement("value_max", ((Saw)scr).value_max),
                            new XElement("period", ((Saw)scr).period));
                        hDoc.Root.Add(xNewnod10);
                        break;
                }
            }
            XNode xNewnod11 = new XElement("Comment",
                               new XElement("text", textBox2.Text));
            hDoc.Root.Add(xNewnod11);

            hDoc.Save(ScriptNameXml);
        }

        // Сохранение Сценария КАК
        private void сценарийToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            string put;
            XDocument hDoc = new XDocument(new XElement("Scenariy"));
            SCR_SVE.Filter = "XML (*.xml)| *.xml; | Все (*.*)|*.*;";
            if (SCR_SVE.ShowDialog() == DialogResult.OK)
            {
                put = SCR_SVE.FileName;
                ScriptNameXml = put;

                hDoc.Save(ScriptNameXml);

                hDoc = XDocument.Load(ScriptNameXml);
                foreach (ICommand scr in ScriptList)
                {
                    string comand = scr.Com_name;
                    // Добавление новой информации
                    switch (comand)
                    {
                        // Пауза
                        case "Pause":
                            XNode xNewnod1 = new XElement("Command",
                       new XAttribute("Com_name", scr.Com_name),
                       new XElement("time", ((Pause)scr).time));
                            hDoc.Root.Add(xNewnod1);
                            break;

                        // Присвоение
                        case "Assign":
                            XNode xNewnod2 = new XElement("Command",
                       new XAttribute("Com_name", scr.Com_name),
                       new XElement("type", ((Assign)scr).type),
                       new XElement("name", ((Assign)scr).name),
                       new XElement("value", ((Assign)scr).value));
                            hDoc.Root.Add(xNewnod2);
                            break;

                        // Сумма
                        case "Summation":
                            XNode xNewnod3 = new XElement("Command",
                       new XAttribute("Com_name", scr.Com_name),
                       new XElement("type", ((Summation)scr).type),
                       new XElement("name", ((Summation)scr).name),
                       new XElement("value_1", ((Summation)scr).value_1),
                       new XElement("value_2", ((Summation)scr).value_2));
                            hDoc.Root.Add(xNewnod3);
                            break;

                        // Разность
                        case "Subtraction":
                            XNode xNewnod4 = new XElement("Command",
                       new XAttribute("Com_name", scr.Com_name),
                       new XElement("type", ((Subtraction)scr).type),
                       new XElement("name", ((Subtraction)scr).name),
                       new XElement("value_1", ((Subtraction)scr).value_1),
                       new XElement("value_2", ((Subtraction)scr).value_2));
                            hDoc.Root.Add(xNewnod4);
                            break;

                        // Произведение
                        case "Multiplication":
                            XNode xNewnod5 = new XElement("Command",
                       new XAttribute("Com_name", scr.Com_name),
                       new XElement("type", ((Multiplication)scr).type),
                       new XElement("name", ((Multiplication)scr).name),
                       new XElement("value_1", ((Multiplication)scr).value_1),
                       new XElement("value_2", ((Multiplication)scr).value_2));
                            hDoc.Root.Add(xNewnod5);
                            break;

                        // Деление
                        case "Division":
                            XNode xNewnod6 = new XElement("Command",
                       new XAttribute("Com_name", scr.Com_name),
                       new XElement("type", ((Division)scr).type),
                       new XElement("name", ((Division)scr).name),
                       new XElement("value_1", ((Division)scr).value_1),
                       new XElement("value_2", ((Division)scr).value_2));
                            hDoc.Root.Add(xNewnod6);
                            break;

                        // Меандр
                        case "Square":
                            XNode xNewnod7 = new XElement("Command",
                                new XAttribute("Com_name", scr.Com_name),
                                new XElement("type", ((Square)scr).type),
                                new XElement("name", ((Square)scr).name),
                                new XElement("value_min", ((Square)scr).value_min),
                                new XElement("value_max", ((Square)scr).value_max),
                                new XElement("period", ((Square)scr).period));
                            hDoc.Root.Add(xNewnod7);
                            break;

                        // Синусоида
                        case "Sinusoid":
                            XNode xNewnod8 = new XElement("Command",
                                new XAttribute("Com_name", scr.Com_name),
                                new XElement("type", ((Sinusoid)scr).type),
                                new XElement("name", ((Sinusoid)scr).name),
                                new XElement("value_start", ((Sinusoid)scr).value_start),
                                new XElement("value_max", ((Sinusoid)scr).value_max),
                                new XElement("period", ((Sinusoid)scr).period),
                                new XElement("middle", ((Sinusoid)scr).middle));
                            hDoc.Root.Add(xNewnod8);
                            break;

                        // Треугольник
                        case "Triangle":
                            XNode xNewnod9 = new XElement("Command",
                                new XAttribute("Com_name", scr.Com_name),
                                new XElement("type", ((Triangle)scr).type),
                                new XElement("name", ((Triangle)scr).name),
                                new XElement("value_min", ((Triangle)scr).value_min),
                                new XElement("value_max", ((Triangle)scr).value_max),
                                new XElement("period", ((Triangle)scr).period));
                            hDoc.Root.Add(xNewnod9);
                            break;

                        // Пила
                        case "Saw":
                            XNode xNewnod10 = new XElement("Command",
                                new XAttribute("Com_name", scr.Com_name),
                                new XElement("type", ((Saw)scr).type),
                                new XElement("name", ((Saw)scr).name),
                                new XElement("value_min", ((Saw)scr).value_min),
                                new XElement("value_max", ((Saw)scr).value_max),
                                new XElement("period", ((Saw)scr).period));
                            hDoc.Root.Add(xNewnod10);
                            break;
                    }
                }

                XNode xNewnod11 = new XElement("Comment",
                               new XElement("text", textBox2.Text));
                hDoc.Root.Add(xNewnod11);

                hDoc.Save(ScriptNameXml);
            }
        }

        // Добавление пункта сценария в начало
        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            v_nachalo = true;
            posle = false;
            if (ScriptNameXml != null || NewScript == true)
            {
                Sozd_Scenar fomr = new Sozd_Scenar(this, ScriptNameXml);

                fomr.ShowDialog();
            }
            else
                MessageBox.Show("Не выбран файл со сценарием");
        }

        // Создание нового файла со сценарием
        private void сценарийToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            // Проверка
            if (ScriptChanged == true)
            {
                if (ScriptNameXml != null)
                {
                    DialogResult dr1 = MessageBox.Show("В файл с данными внесены изменения. Хотите переписать данные?", "Изменение параметров", MessageBoxButtons.YesNo);
                    if (dr1 == DialogResult.Yes)
                    {
                        XDocument hDoc = XDocument.Load(ScriptNameXml);

                        XElement root = hDoc.Element("Scenariy");
                        foreach (XElement xe in root.Elements("Command").ToList())
                        {
                            xe.Remove();
                        }

                        xdoc.Save(ScriptNameXml);

                        foreach (ICommand scr in ScriptList)
                        {
                            string comand = scr.Com_name;
                            // Добавление новой информации
                            switch (comand)
                            {
                                // Пауза
                                case "Pause":
                                    XNode xNewnod1 = new XElement("Command",
                               new XAttribute("Com_name", scr.Com_name),
                               new XElement("time", ((Pause)scr).time));
                                    hDoc.Root.Add(xNewnod1);
                                    break;

                                // Присвоение
                                case "Assign":
                                    XNode xNewnod2 = new XElement("Command",
                               new XAttribute("Com_name", scr.Com_name),
                               new XElement("type", ((Assign)scr).type),
                               new XElement("name", ((Assign)scr).name),
                               new XElement("value", ((Assign)scr).value));
                                    hDoc.Root.Add(xNewnod2);
                                    break;

                                // Сумма
                                case "Summation":
                                    XNode xNewnod3 = new XElement("Command",
                               new XAttribute("Com_name", scr.Com_name),
                               new XElement("type", ((Summation)scr).type),
                               new XElement("name", ((Summation)scr).name),
                               new XElement("value_1", ((Summation)scr).value_1),
                               new XElement("value_2", ((Summation)scr).value_2));
                                    hDoc.Root.Add(xNewnod3);
                                    break;

                                // Разность
                                case "Subtraction":
                                    XNode xNewnod4 = new XElement("Command",
                               new XAttribute("Com_name", scr.Com_name),
                               new XElement("type", ((Subtraction)scr).type),
                               new XElement("name", ((Subtraction)scr).name),
                               new XElement("value_1", ((Subtraction)scr).value_1),
                               new XElement("value_2", ((Subtraction)scr).value_2));
                                    hDoc.Root.Add(xNewnod4);
                                    break;

                                // Произведение
                                case "Multiplication":
                                    XNode xNewnod5 = new XElement("Command",
                               new XAttribute("Com_name", scr.Com_name),
                               new XElement("type", ((Multiplication)scr).type),
                               new XElement("name", ((Multiplication)scr).name),
                               new XElement("value_1", ((Multiplication)scr).value_1),
                               new XElement("value_2", ((Multiplication)scr).value_2));
                                    hDoc.Root.Add(xNewnod5);
                                    break;

                                // Деление
                                case "Division":
                                    XNode xNewnod6 = new XElement("Command",
                               new XAttribute("Com_name", scr.Com_name),
                               new XElement("type", ((Division)scr).type),
                               new XElement("name", ((Division)scr).name),
                               new XElement("value_1", ((Division)scr).value_1),
                               new XElement("value_2", ((Division)scr).value_2));
                                    hDoc.Root.Add(xNewnod6);
                                    break;
                            }
                        }

                        hDoc.Save(FileNameXml);

                        ScriptList.RemoveRange(0, ScriptList.Count);
                        ScriptChanged = false;
                        ScriptNameXml = null;
                        NewScript = true;
                    }
                    else
                    {
                        ScriptList.RemoveRange(0, ScriptList.Count);
                        ScriptChanged = false;
                        ScriptNameXml = null;
                        NewScript = true;
                    }
                }
                else
                {
                    DialogResult dr1 = MessageBox.Show("Хотите создать новый файл с данными?", "Сохранение параметров", MessageBoxButtons.YesNo);
                    if (dr1 == DialogResult.Yes)
                    {
                        string put;
                        XDocument hDoc = new XDocument(new XElement("Scenariy"));
                        SCR_SVE.Filter = "XML (*.xml)| *.xml; | Все (*.*)|*.*;";
                        if (SCR_SVE.ShowDialog() == DialogResult.OK)
                        {
                            put = SCR_SVE.FileName;
                            ScriptNameXml = put;

                            hDoc.Save(ScriptNameXml);

                            hDoc = XDocument.Load(FileNameXml);
                            foreach (ICommand scr in ScriptList)
                            {
                                string comand = scr.Com_name;
                                // Добавление новой информации
                                switch (comand)
                                {
                                    // Пауза
                                    case "Pause":
                                        XNode xNewnod1 = new XElement("Command",
                                   new XAttribute("Com_name", scr.Com_name),
                                   new XElement("time", ((Pause)scr).time));
                                        hDoc.Root.Add(xNewnod1);
                                        break;

                                    // Присвоение
                                    case "Assign":
                                        XNode xNewnod2 = new XElement("Command",
                                   new XAttribute("Com_name", scr.Com_name),
                                   new XElement("type", ((Assign)scr).type),
                                   new XElement("name", ((Assign)scr).name),
                                   new XElement("value", ((Assign)scr).value));
                                        hDoc.Root.Add(xNewnod2);
                                        break;

                                    // Сумма
                                    case "Summation":
                                        XNode xNewnod3 = new XElement("Command",
                                   new XAttribute("Com_name", scr.Com_name),
                                   new XElement("type", ((Summation)scr).type),
                                   new XElement("name", ((Summation)scr).name),
                                   new XElement("value_1", ((Summation)scr).value_1),
                                   new XElement("value_2", ((Summation)scr).value_2));
                                        hDoc.Root.Add(xNewnod3);
                                        break;

                                    // Разность
                                    case "Subtraction":
                                        XNode xNewnod4 = new XElement("Command",
                                   new XAttribute("Com_name", scr.Com_name),
                                   new XElement("type", ((Subtraction)scr).type),
                                   new XElement("name", ((Subtraction)scr).name),
                                   new XElement("value_1", ((Subtraction)scr).value_1),
                                   new XElement("value_2", ((Subtraction)scr).value_2));
                                        hDoc.Root.Add(xNewnod4);
                                        break;

                                    // Произведение
                                    case "Multiplication":
                                        XNode xNewnod5 = new XElement("Command",
                                   new XAttribute("Com_name", scr.Com_name),
                                   new XElement("type", ((Multiplication)scr).type),
                                   new XElement("name", ((Multiplication)scr).name),
                                   new XElement("value_1", ((Multiplication)scr).value_1),
                                   new XElement("value_2", ((Multiplication)scr).value_2));
                                        hDoc.Root.Add(xNewnod5);
                                        break;

                                    // Деление
                                    case "Division":
                                        XNode xNewnod6 = new XElement("Command",
                                   new XAttribute("Com_name", scr.Com_name),
                                   new XElement("type", ((Division)scr).type),
                                   new XElement("name", ((Division)scr).name),
                                   new XElement("value_1", ((Division)scr).value_1),
                                   new XElement("value_2", ((Division)scr).value_2));
                                        hDoc.Root.Add(xNewnod6);
                                        break;
                                }
                            }

                            hDoc.Save(ScriptNameXml);

                            Script_imia(ScriptNameXml);
                            ScriptList.RemoveRange(0, ScriptList.Count);
                            ScriptChanged = false;
                            ScriptNameXml = null;
                            NewScript = true;
                        }
                        else
                        {
                            ScriptList.RemoveRange(0, ScriptList.Count);
                            ScriptChanged = false;
                            ScriptNameXml = null;
                            NewScript = true;
                        }
                    }
                    else
                    {
                        ScriptList.RemoveRange(0, ScriptList.Count);
                        ScriptChanged = false;
                        ScriptNameXml = null;
                        NewScript = true;
                    }
                }
            }
            else
            {
                ScriptList.RemoveRange(0, ScriptList.Count);
                ScriptChanged = false;
                ScriptNameXml = null;
                NewScript = true;
            }

            Refresh2();

            tabControl1.TabPages[1].Text = "Сценарий: " + "``@ " + "script" + " @``";
        }

        // Добавление пункта сценария после выбранного пункта
        private void добавитьПослеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            v_nachalo = false;
            posle = true;
            if (ScriptNameXml != null || NewScript == true)
            {
                Sozd_Scenar fomr = new Sozd_Scenar(this, ScriptNameXml);

                fomr.ShowDialog();
            }
            else
                MessageBox.Show("Не выбран файл со сценарием");
        }

        // Удаление одного пункта в сценарии
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptChanged = true;
            int punkt = listBox2.SelectedIndex;
            if (punkt == -1)
            {
                MessageBox.Show("Не выбран пункт для удаления");
            }
            else
            {

                ScriptList.RemoveAt(punkt);
            }
            Refresh2();
        }

        // Удаление ВСЕГО сценария
        private void удалитьВсеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ScriptChanged = true;
            ScriptList.Clear();
            Refresh2();
        }
        
        public Thread CheckParametrs;
        public Thread CheckScript;
        
        // Выполение сценария
        private void timer2_Tick(object sender, EventArgs e)
        {
            secondes++;
            int min = secondes / 60;
            int sec = secondes % 60;
            string mins = String.Format("{0:00}", min);
            string secc = String.Format("{0:00}", sec);
            label1.Text = Convert.ToString(mins + ":" + secc);            
        }

        public void pAramets()
        {
            for (int i = 0; i < SignalList.Count; i++)
            {
                if (SignalList[i].kind == "Циклическая" && emiT % SignalList[i].prop == 0)
                {     
                }
                else if (SignalList[i].kind == "По изменению" && SignalList[i].value != SignalList[i].last_value)
                {                    
                    SignalList[i].last_value = SignalList[i].value;
                }
            }
        }
        
        public void sCrip()
        {
            for (int i = 0; i < ScriptList.Count; i++)
            {
                if (ScriptList[i].Com_name == "Pause")
                {
                    Thread.Sleep(Convert.ToInt32(((Pause)ScriptList[i]).time) * 1000);
                }
                else if (ScriptList[i].Com_name == "Assign")
                {
                    for (int k = 0; k < SignalList.Count; k++)
                    {
                        if (((Assign)ScriptList[i]).name == SignalList[k].name &&
                            ((Assign)ScriptList[i]).type == SignalList[k].Type)
                        {
                            SignalList[k].value = ((Assign)ScriptList[i]).value;
                        }
                    }
                }
            }
        }

        public Thread Ostanov;

        Stopwatch watch = new Stopwatch();
        private void button3_Click(object sender, EventArgs e)
        {
            watch.Start();
            button1.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = true;
            timer2.Enabled = true;
            CheckScript = new Thread(Zapusk_scenariya);
            CheckScript.Start();            
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            CheckScript.Abort();
            timer2.Enabled = false;
            this.Invoke((MethodInvoker)delegate()
            {
                richTextBox1.Select(stroka, stroka + 31); //выделяем текст                
                richTextBox1.SelectionColor = Color.Red; //для выделенного текста устанавливаем цвет
                richTextBox1.AppendText("Завершение выполнения сценария" + Environment.NewLine);
                stroka = stroka + 31;
                richTextBox1.ScrollToCaret();
            });

            // Сброс поля Прошлая команда
            foreach (Signal sig in SignalList)
            {
                sig.last_command = "Assign";
            }

            button1.Enabled = true;
            button3.Enabled = false;
            button4.Enabled = false;
            secondes = 0;
        }

        

        public void Zapusk_scenariya()
        {
            // запрос актуальных значений
            if (!dak.Connection.Connected)
            {
            }

            if (dak.OIRequests.Count > 0)
            {
                // останавливаем и удаляем старый запрос
                dak.OIRequests.Item(0).Stop();
                dak.OIRequests.Item(0).Delete();
            }

            // добавляем новый запрос            
            rqActual = dak.OIRequests.Add();
            // получаем данные с миллисекундами
            rqActual.UseMilliseconds = true;
                 
            rqActual.Stop();
            rqActual.OIRequestItems.Clear();
            
            timer2.Enabled = true;
            int vr = Environment.TickCount;
            //stop_prog = false;
            this.Invoke((MethodInvoker)delegate()
            {
                richTextBox1.Select(stroka, stroka + 57); //выделяем текст
                richTextBox1.SelectionColor = Color.Green; //для выделенного текста устанавливаем цвет
                richTextBox1.AppendText("Запуск выполнения сценария" + Environment.NewLine);
                stroka = stroka + 57;
                richTextBox1.ScrollToCaret();
            });

            for (int i = 0; i < SignalList.Count; i++)
            {
                if (SignalList[i].start_value != SignalList[i].value)
                {
                    SignalList[i].value = SignalList[i].start_value;
                    SignalList[i].last_value = SignalList[i].start_value;
                }
                if (SignalList[i].start_code != SignalList[i].code)
                {
                    SignalList[i].code = SignalList[i].start_code;
                    SignalList[i].last_code = SignalList[i].start_code;
                }
            }
            dla = 0;
            emiT = 0;
            
            int iee = 0;
            int cikl = 0;
            int nach_sch = 0;
            vr = Environment.TickCount - vr;
            for (;;)
            {
                int vr2 = Environment.TickCount;
                int min = emiT / 60;
                int sec = emiT % 60;
                string mins = String.Format("{0:00}", min);
                string secc = String.Format("{0:00}", sec);
                this.Invoke((MethodInvoker)delegate()
                {
                    richTextBox1.AppendText(mins + ":" + secc + "  |   " + " Прошло секунд: " +
                    emiT + Environment.NewLine);
                    stroka = stroka + 24 + mins.Count() + secc.Count() + Convert.ToString(emiT).Count();
                    richTextBox1.ScrollToCaret();
                });                

                // Проход по сценарию
                if (nach_sch < ScriptList.Count)
                {
                    for (int i = 0; i < ScriptList.Count; i++)
                    {
                        if (nach_sch == i)
                        {
                            // ПАУЗА
                            if (ScriptList[i].Com_name == "Pause")
                            {
                                cikl++;
                                if (emiT == dla && cikl != ((Pause)ScriptList[i]).time)
                                {
                                    dla = dla + Convert.ToInt32(((Pause)ScriptList[i]).time);

                                    break;
                                }
                                else if (cikl == ((Pause)ScriptList[i]).time)
                                {
                                    Proverka_Cikla();
                                    nach_sch++;
                                    cikl = 0;
                                    break;
                                }
                                else
                                {
                                    Proverka_Cikla();
                                    break;
                                }
                            }
                            // ПРИСВОЕНИЕ
                            else if (ScriptList[i].Com_name == "Assign")
                            {
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Assign)ScriptList[i]).name == SignalList[k].name &&
                                        ((Assign)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        SignalList[k].value = ((Assign)ScriptList[i]).value;
                                        SignalList[k].last_command = "Assign";
                                    }
                                }
                                nach_sch++;
                            }
                            // СУММА
                            else if (ScriptList[i].Com_name == "Summation")
                            {
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Summation)ScriptList[i]).name == SignalList[k].name &&
                                        ((Summation)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        try
                                        {
                                            // Если первое значение число, то
                                            Convert.ToDouble(((Summation)ScriptList[i]).value_1);
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Summation)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Summation)ScriptList[i]).value_1) +
                                                    Convert.ToDouble(((Summation)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое значение число, а второе - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Summation)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Summation)ScriptList[i]).value_1) +
                                                    SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // Если первое значение TM, то
                                            double perv_znach = 0;
                                            for (int ii = 0; ii < SignalList.Count; ii++)
                                            {
                                                if (((Summation)ScriptList[i]).value_1 == SignalList[ii].name &&
                                                    SignalList[k].Type == SignalList[ii].Type)
                                                {
                                                    perv_znach = SignalList[ii].value;
                                                }
                                            }
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Summation)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(perv_znach +
                                                    Convert.ToDouble(((Summation)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое и второе значения - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Summation)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(perv_znach + SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        SignalList[k].last_command = "Summation";
                                    }
                                }
                                nach_sch++;
                            }

                            // РАЗНОСТЬ
                            else if (ScriptList[i].Com_name == "Subtraction")
                            {
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Subtraction)ScriptList[i]).name == SignalList[k].name &&
                                        ((Subtraction)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        try
                                        {
                                            // Если первое значение число, то
                                            Convert.ToDouble(((Subtraction)ScriptList[i]).value_1);
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Subtraction)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Subtraction)ScriptList[i]).value_1) -
                                                    Convert.ToDouble(((Subtraction)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое значение число, а второе - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Subtraction)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Subtraction)ScriptList[i]).value_1) -
                                                    SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // Если первое значение TM, то
                                            double perv_znach = 0;
                                            for (int ii = 0; ii < SignalList.Count; ii++)
                                            {
                                                if (((Subtraction)ScriptList[i]).value_1 == SignalList[ii].name &&
                                                    SignalList[k].Type == SignalList[ii].Type)
                                                {
                                                    perv_znach = SignalList[ii].value;
                                                }
                                            }
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Subtraction)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(perv_znach -
                                                    Convert.ToDouble(((Subtraction)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое и второе значения - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Subtraction)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(perv_znach - SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        SignalList[k].last_command = "Subtraction";
                                    }
                                }
                                nach_sch++;
                            }


                            // ПРОИЗВЕДЕНИЕ
                            else if (ScriptList[i].Com_name == "Multiplication")
                            {
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Multiplication)ScriptList[i]).name == SignalList[k].name &&
                                        ((Multiplication)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        try
                                        {
                                            // Если первое значение число, то
                                            Convert.ToDouble(((Multiplication)ScriptList[i]).value_1);
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Multiplication)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Multiplication)ScriptList[i]).value_1) *
                                                    Convert.ToDouble(((Multiplication)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое значение число, а второе - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Multiplication)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Multiplication)ScriptList[i]).value_1) *
                                                    SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // Если первое значение TM, то
                                            double perv_znach = 0;
                                            for (int ii = 0; ii < SignalList.Count; ii++)
                                            {
                                                if (((Multiplication)ScriptList[i]).value_1 == SignalList[ii].name &&
                                                    SignalList[k].Type == SignalList[ii].Type)
                                                {
                                                    perv_znach = SignalList[ii].value;
                                                }
                                            }
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Multiplication)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(perv_znach *
                                                    Convert.ToDouble(((Multiplication)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое и второе значения - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Multiplication)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(perv_znach * SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        SignalList[k].last_command = "Multiplication";
                                    }
                                }
                                nach_sch++;
                            }

                            // ДЕЛЕНИЕ
                            else if (ScriptList[i].Com_name == "Division")
                            {
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Division)ScriptList[i]).name == SignalList[k].name &&
                                        ((Division)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        try
                                        {
                                            // Если первое значение число, то
                                            Convert.ToDouble(((Division)ScriptList[i]).value_1);
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Division)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Division)ScriptList[i]).value_1) /
                                                    Convert.ToDouble(((Division)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое значение число, а второе - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Division)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Division)ScriptList[i]).value_1) /
                                                    SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // Если первое значение TM, то
                                            double perv_znach = 0;
                                            for (int ii = 0; ii < SignalList.Count; ii++)
                                            {
                                                if (((Division)ScriptList[i]).value_1 == SignalList[ii].name &&
                                                    SignalList[k].Type == SignalList[ii].Type)
                                                {
                                                    perv_znach = SignalList[ii].value;
                                                }
                                            }
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Division)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(perv_znach /
                                                    Convert.ToDouble(((Division)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое и второе значения - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Division)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(perv_znach / SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        SignalList[k].last_command = "Division";
                                    }
                                }
                                nach_sch++;
                            }

                            // МЕАНДР
                            else if (ScriptList[i].Com_name == "Square")
                            {
                                int fifi = 0;
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Square)ScriptList[i]).name == SignalList[k].name &&
                                        ((Square)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        SignalList[k].value_min = ((Square)ScriptList[i]).value_min;
                                        SignalList[k].value_max = ((Square)ScriptList[i]).value_max;
                                        SignalList[k].period = ((Square)ScriptList[i]).period;
                                        SignalList[k].tek_time = ((Square)ScriptList[i]).tek_time;
                                        SignalList[k].middle = ((Square)ScriptList[i]).middle;
                                        SignalList[k].tek_value = ((Square)ScriptList[i]).tek_value;

                                        fifi = k;
                                    }
                                }
                                if (i != ScriptList.Count - 1)
                                {
                                    if (SignalList[fifi].tek_time >= SignalList[fifi].middle)
                                    {
                                        SignalList[fifi].value = SignalList[fifi].value_max;
                                    }
                                    else
                                    {
                                        SignalList[fifi].value = SignalList[fifi].value_min;
                                    }

                                    if (SignalList[fifi].tek_time == SignalList[fifi].period - 1)
                                    { SignalList[fifi].tek_time = 0; }
                                    else
                                    {
                                        SignalList[fifi].tek_time = SignalList[fifi].tek_time + 1;
                                    }
                                }

                                SignalList[fifi].last_command = "Square";

                                nach_sch++;
                            }

                            // Синусоида
                            else if (ScriptList[i].Com_name == "Sinusoid")
                            {
                                int fifi = 0;
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Sinusoid)ScriptList[i]).name == SignalList[k].name &&
                                        ((Sinusoid)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        SignalList[k].value_min = ((Sinusoid)ScriptList[i]).value_start;
                                        SignalList[k].value_max = ((Sinusoid)ScriptList[i]).value_max;
                                        SignalList[k].period = ((Sinusoid)ScriptList[i]).period;
                                        SignalList[k].middle = ((Sinusoid)ScriptList[i]).middle;
                                        SignalList[k].tek_time = 0;

                                        SignalList[k].value = SignalList[k].value_min + SignalList[k].value_max *
                                            Math.Sin((2 * Math.PI * SignalList[k].tek_time / SignalList[k].period) +
                                            SignalList[k].middle);

                                        fifi = k;
                                    }
                                }
                                SignalList[fifi].tek_time = SignalList[fifi].tek_time + 1;
                                SignalList[fifi].last_command = "Sinusoid";

                                nach_sch++;
                            }

                            // Треугольник
                            else if (ScriptList[i].Com_name == "Triangle")
                            {
                                int fifi = 0;
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Triangle)ScriptList[i]).name == SignalList[k].name &&
                                        ((Triangle)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        SignalList[k].value_min = ((Triangle)ScriptList[i]).value_min;
                                        SignalList[k].value_max = ((Triangle)ScriptList[i]).value_max;
                                        SignalList[k].period = ((Triangle)ScriptList[i]).period;
                                        SignalList[k].middle = ((Triangle)ScriptList[i]).middle;
                                        SignalList[k].tek_time = 0;
                                        SignalList[k].tek_value = SignalList[k].value_min;

                                        fifi = k;

                                        SignalList[fifi].value = SignalList[fifi].value_min;                                        
                                    }


                                }
                                SignalList[fifi].last_command = "Triangle";

                                nach_sch++;
                            }

                            // ПИЛА
                            else if (ScriptList[i].Com_name == "Saw")
                            {
                                int fifi = 0;
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Saw)ScriptList[i]).name == SignalList[k].name &&
                                        ((Saw)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        SignalList[k].value_min = ((Saw)ScriptList[i]).value_min;
                                        SignalList[k].value_max = ((Saw)ScriptList[i]).value_max;
                                        SignalList[k].period = ((Saw)ScriptList[i]).period;
                                        SignalList[k].middle = ((Saw)ScriptList[i]).step;
                                        SignalList[k].tek_time = 0;
                                        SignalList[k].tek_value = SignalList[k].value_min;

                                        fifi = k;

                                        SignalList[k].value = SignalList[k].value_min;
                                    }
                                }
                                SignalList[fifi].last_command = "Saw";

                                nach_sch++;
                            }

                            // КОД
                            else if (ScriptList[i].Com_name == "Code")
                            {
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Code)ScriptList[i]).name == SignalList[k].name &&
                                        ((Code)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        SignalList[k].code = ((Code)ScriptList[i]).code;
                                        SignalList[k].last_command = "Code";
                                    }
                                }
                                nach_sch++;
                            }

                            else
                            {
                                Proverka_Cikla();
                                nach_sch++;
                            } // ДОРАБОТАТЬ

                            if (i == ScriptList.Count - 1)
                            { Proverka_Cikla(); }
                        }
                    }
                    //break;
                }
                else
                {
                    Proverka_Cikla();
                }


                // Переменные для ОИКа
                string tip = "";
                int nomer;
                string kod = "";
                double znachenie;
                int ggg = 0;
                bool chtoto = false;
                // Проход по параметрам сигналов
                for (int i = 0; i < SignalList.Count; i++)
                {
                    if (emiT == 0)
                    {
                        if (chtoto == false)
                        {
                            this.Invoke((MethodInvoker)delegate()
                            {
                                richTextBox1.AppendText("           |    " + "Инициализация начальных значений" + Environment.NewLine);
                                stroka = stroka + 49;
                                richTextBox1.ScrollToCaret();
                            });

                            this.Invoke((MethodInvoker)delegate()
                            {
                                richTextBox1.AppendText("           |    " + SignalList[i].name + ".Значение = " +
                                    SignalList[i].value + Environment.NewLine);
                                stroka = stroka + 29 + SignalList[i].name.Count() + Convert.ToString(SignalList[i].value).Count();
                                richTextBox1.ScrollToCaret();
                            });
                            
                            chtoto = true;
                        }
                        else
                        {
                            this.Invoke((MethodInvoker)delegate()
                            {
                                richTextBox1.AppendText("           |    " + SignalList[i].name + ".Значение = " +
                                    SignalList[i].value + Environment.NewLine);
                                stroka = stroka + 29 + SignalList[i].name.Count() + Convert.ToString(SignalList[i].value).Count();
                                richTextBox1.ScrollToCaret();
                            });
                            
                            chtoto = true;
                        }

                        tip = SignalList[i].Type;
                        nomer = SignalList[i].id;
                        kod = SignalList[i].code;
                        znachenie = SignalList[i].value;

                        ggg = Otpravka_v_OIK(tip, nomer, kod, znachenie);
                    }
                    else if (SignalList[i].kind == "Циклическая" && emiT % SignalList[i].prop == 0)
                    {
                        if (SignalList[i].code != SignalList[i].last_code)
                        {
                            this.Invoke((MethodInvoker)delegate()
                            {
                                richTextBox1.AppendText("           |    " + SignalList[i].name + ".Код = " +
                                    SignalList[i].code + Environment.NewLine);
                                stroka = stroka + 24 + SignalList[i].name.Count() + SignalList[i].code.Count();
                                richTextBox1.ScrollToCaret();
                            });
                            
                            SignalList[i].last_code = SignalList[i].code;
                        }

                        this.Invoke((MethodInvoker)delegate()
                        {
                            richTextBox1.AppendText("           |    " + SignalList[i].name + ".Значение = " +
                                SignalList[i].value + Environment.NewLine);
                            stroka = stroka + 29 + SignalList[i].name.Count() + Convert.ToString(SignalList[i].value).Count();
                            richTextBox1.ScrollToCaret();
                        });

                        tip = SignalList[i].Type;
                        nomer = SignalList[i].id;
                        kod = SignalList[i].code;
                        znachenie = SignalList[i].value;

                        ggg = Otpravka_v_OIK(tip, nomer, kod, znachenie);
                    }
                    else if (SignalList[i].kind == "По изменению" && (SignalList[i].value != SignalList[i].last_value || 
                        SignalList[i].code != SignalList[i].last_code))
                    {
                        if (SignalList[i].code != SignalList[i].last_code)
                        {
                            this.Invoke((MethodInvoker)delegate()
                            {
                                richTextBox1.AppendText("           |    " + SignalList[i].name + ".Код = " +
                                    SignalList[i].code + Environment.NewLine);
                                stroka = stroka + 24 + SignalList[i].name.Count() + SignalList[i].code.Count();
                                richTextBox1.ScrollToCaret();
                            });
                            
                            SignalList[i].last_code = SignalList[i].code;
                        }

                        this.Invoke((MethodInvoker)delegate()
                        {
                            richTextBox1.AppendText("           |    " + SignalList[i].name + ".Значение = " +
                                SignalList[i].value + Environment.NewLine);
                            stroka = stroka + 29 + SignalList[i].name.Count() + Convert.ToString(SignalList[i].value).Count();
                            richTextBox1.ScrollToCaret();
                        });
                                                
                        tip = SignalList[i].Type;
                        nomer = SignalList[i].id;
                        kod = SignalList[i].code;
                        znachenie = SignalList[i].value;

                        ggg = Otpravka_v_OIK(tip, nomer, kod, znachenie);

                        SignalList[i].last_value = SignalList[i].value;
                    }                    
                }

                emiT++;
                iee++;
                
                rqActual.Start();
                vr2 = Environment.TickCount - vr2;                
                Thread.Sleep(1000 - vr - vr2);
                watch.Stop();
                long trt = watch.ElapsedMilliseconds;
                vr = 0;
            }
        }

        // Запись действий в ОИК 
        public int Otpravka_v_OIK(string tip, int nomer, string kod, double znachenie)
        {


            // добавляем элемент запроса
            var rqi = rqActual.AddOIRequestItem();
            rqi.IsLocalTime = true;
            rqi.KindRefresh = KindRefreshEnum.kr_WriteData;

            // Составление названия сигнала для ОИКа
            string nazvanie = "";
            string bukva = "";
            if (tip == "ТИ")
            {
                bukva = "I";
            }
            else bukva = "S";

            nazvanie = bukva + nomer;

            kod = Perevod_koda(kod);          

            rqi.DataSource = nazvanie;
            rqi.DataValue = znachenie;
            rqi.Sign = Convert.ToInt32(kod);            

            return (1);
        }
        

        // Проверка наличия циклических функций
        public void Proverka_Cikla()
        {
            for (int k = 0; k < SignalList.Count; k++)
            {
                if (SignalList[k].last_command == "Square")
                {
                    if (SignalList[k].tek_time >= SignalList[k].middle)
                    {
                        SignalList[k].value = SignalList[k].value_max;
                    }
                    else
                    {
                        SignalList[k].value = SignalList[k].value_min;
                    }

                    if (SignalList[k].tek_time == SignalList[k].period - 1)
                    { SignalList[k].tek_time = 0; }
                    else
                    { SignalList[k].tek_time = SignalList[k].tek_time + 1; }

                    SignalList[k].last_command = "Square";
                }
                else if (SignalList[k].last_command == "Sinusoid")
                {
                    SignalList[k].value = SignalList[k].value_min + SignalList[k].value_max *
                                            Math.Sin((2 * Math.PI * SignalList[k].tek_time /
                                            SignalList[k].period) + SignalList[k].middle);

                    SignalList[k].tek_time = SignalList[k].tek_time + 1;
                    SignalList[k].last_command = "Sinusoid";
                }
                else if (SignalList[k].last_command == "Triangle")
                {
                    double gir = SignalList[k].period % 2;
                    // Если период нечётный - треугольник без вершины
                    if (gir != 0)
                    {
                        if (SignalList[k].tek_time < SignalList[k].period / 2)
                        {
                            if (SignalList[k].tek_time == 0)
                            {
                                SignalList[k].value = SignalList[k].value_min;
                                SignalList[k].tek_value = SignalList[k].value;
                                SignalList[k].tek_time = SignalList[k].tek_time + 1;
                            }
                            else if ((SignalList[k].period / 2) - SignalList[k].tek_time < 1)
                            {
                                SignalList[k].value = SignalList[k].tek_value + SignalList[k].middle;
                                SignalList[k].tek_value = SignalList[k].value_max - SignalList[k].value;
                                SignalList[k].tek_time = SignalList[k].tek_time + 1;
                            }
                            else
                            {
                                SignalList[k].value = SignalList[k].tek_value + SignalList[k].middle *
                                SignalList[k].tek_time;
                                SignalList[k].tek_value = SignalList[k].value;
                                SignalList[k].tek_time = SignalList[k].tek_time + 1;
                            }
                        }
                        else if (SignalList[k].tek_time >= SignalList[k].period / 2)
                        {
                            if (SignalList[k].tek_time - (SignalList[k].period / 2) < 1)
                            {
                                SignalList[k].value = SignalList[k].value_max - SignalList[k].tek_value;
                                SignalList[k].tek_value = SignalList[k].value;
                                SignalList[k].tek_time = SignalList[k].tek_time + 1;
                            }
                            else if (SignalList[k].tek_time == SignalList[k].period - 1)
                            {
                                SignalList[k].value = SignalList[k].tek_value - SignalList[k].middle;
                                SignalList[k].tek_value = SignalList[k].value;
                                SignalList[k].tek_time = 0;
                            }
                            else
                            {
                                SignalList[k].value = SignalList[k].tek_value - SignalList[k].middle;
                                SignalList[k].tek_value = SignalList[k].value;
                                SignalList[k].tek_time = SignalList[k].tek_time + 1;
                            }
                        }
                    }
                    // Если период чётный - треугольник с вершиной
                    else
                    {
                        if (SignalList[k].tek_time < SignalList[k].period / 2)
                        {
                            if (SignalList[k].tek_time == 0)
                            {
                                SignalList[k].value = SignalList[k].value_min;
                                SignalList[k].tek_value = SignalList[k].value;
                                SignalList[k].tek_time = SignalList[k].tek_time + 1;
                            }
                            else
                            {
                                SignalList[k].value = SignalList[k].tek_value + SignalList[k].middle;
                                SignalList[k].tek_value = SignalList[k].value;
                                SignalList[k].tek_time = SignalList[k].tek_time + 1;
                            }
                        }
                        else if (SignalList[k].tek_time == SignalList[k].period / 2)
                        {
                            SignalList[k].value = SignalList[k].value_max;
                            SignalList[k].tek_value = SignalList[k].value;
                            SignalList[k].tek_time = SignalList[k].tek_time + 1;
                        }
                        else if (SignalList[k].tek_time > SignalList[k].period / 2)
                        {
                            if (SignalList[k].tek_time == SignalList[k].period - 1)
                            {
                                SignalList[k].value = SignalList[k].tek_value - SignalList[k].middle;
                                SignalList[k].tek_value = SignalList[k].value_min;
                                SignalList[k].tek_time = 0;
                            }
                            else
                            {
                                SignalList[k].value = SignalList[k].tek_value - SignalList[k].middle;
                                SignalList[k].tek_value = SignalList[k].value;
                                SignalList[k].tek_time = SignalList[k].tek_time + 1;
                            }
                        }
                    }

                    SignalList[k].last_command = "Triangle";
                }

                else if (SignalList[k].last_command == "Saw")
                {
                    // Возрастающая пила                    
                    if (SignalList[k].tek_time == 0)
                    {
                        SignalList[k].value = SignalList[k].value_min;
                        SignalList[k].tek_value = SignalList[k].value;
                        SignalList[k].last_value = SignalList[k].value - 1;
                        SignalList[k].tek_time = SignalList[k].tek_time + 1;
                    }
                    else if (SignalList[k].tek_time == SignalList[k].period - 1)
                    {
                        SignalList[k].value = SignalList[k].tek_value + SignalList[k].middle;
                        SignalList[k].tek_value = SignalList[k].value;
                        SignalList[k].tek_time = 0;
                    }
                    else
                    {
                        SignalList[k].value = SignalList[k].tek_value + SignalList[k].middle;
                        SignalList[k].tek_value = SignalList[k].value;
                        SignalList[k].tek_time = SignalList[k].tek_time + 1;
                    }
                    
                    // Обпатная пила                    
                    SignalList[k].last_command = "Saw";
                }                
            }     
        }

        public string Perevod_koda(string kod)
        {
            switch (kod)
            {
                case "0x1":
                    kod = Convert.ToString(1, 10);
                    break;
                case "0x2":
                    kod = Convert.ToString(2, 10);
                    break;
                case "0x42":
                    kod = Convert.ToString(66, 10);
                    break;
                case "0x4":
                    kod = Convert.ToString(4, 10);
                    break;
                case "0x8":
                    kod = Convert.ToString(8, 10);
                    break;
                case "0x10":
                    kod = Convert.ToString(16, 10);
                    break;
                case "0x20":
                    kod = Convert.ToString(32, 10);
                    break;
                case "0x40":
                    kod = Convert.ToString(64, 10);
                    break;
                case "0x80":
                    kod = Convert.ToString(128, 10);
                    break;
                case "0x100":
                    kod = Convert.ToString(256, 10);
                    break;
                case "0x200":
                    kod = Convert.ToString(512, 10);
                    break;
                case "0x202":
                    kod = Convert.ToString(514, 10);
                    break;
                case "0x400":
                    kod = Convert.ToString(1024, 10);
                    break;
                case "0x800":
                    kod = Convert.ToString(2048, 10);
                    break;
                case "0x1000":
                    kod = Convert.ToString(4096, 10);
                    break;
                case "0x2000":
                    kod = Convert.ToString(8192, 10);
                    break;
                case "0x4000":
                    kod = Convert.ToString(16384, 10);
                    break;
                case "0x8000":
                    kod = Convert.ToString(32768, 10);
                    break;
                case "0x8200":
                    kod = Convert.ToString(33280, 10);
                    break;
                case "0x10000":
                    kod = Convert.ToString(65536, 10);
                    break;
                case "0x20000":
                    kod = Convert.ToString(131072, 10);
                    break;
                case "0x40000":
                    kod = Convert.ToString(262144, 10);
                    break;
                case "0x80000":
                    kod = Convert.ToString(524288, 10);
                    break;
                case "0x100000":
                    kod = Convert.ToString(1048576, 10);
                    break;
                case "0x100040":
                    kod = Convert.ToString(1048640, 10);
                    break;
                case "0x200000":
                    kod = Convert.ToString(2097152, 10);
                    break;
                case "0x800000":
                    kod = Convert.ToString(8388608, 10);
                    break;
                case "0x1000000":
                    kod = Convert.ToString(16777216, 10);
                    break;
                case "0x4000000":
                    kod = Convert.ToString(67108864, 10);
                    break;
                case "0x8000000":
                    kod = Convert.ToString(134217728, 10);
                    break;
                case "0x10000000":
                    kod = Convert.ToString(268435456, 10);
                    break;
                case "0x20000000":
                    kod = Convert.ToString(536870912, 10);
                    break;
                case "0x40000000":
                    kod = Convert.ToString(1073741824, 10);
                    break;
            }
            return (kod);
        }

        // Проверка последнего действия в сценарии
        public void Proverka_Poslednego_Cikla(int i)
        {
            if (i == ScriptList.Count - 1)
            {
                Proverka_Cikla();
            }
        }

        // Открытие настроек
        private void настройкиToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SettingsMy sttngs = new SettingsMy();
            sttngs.ShowDialog();
        }

        // Открытие справки
        private void справкаToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            About abou = new About();
            abou.ShowDialog();
        }

        // Открытие списка кодов достоверности
        private void кодыДостоверностиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Kod_dostovernosti kokoko = new Kod_dostovernosti();
            kokoko.Show();
        }

        // Проверка соответствия сигналов в сценарии и списке параметров
        private void button1_Click(object sender, EventArgs e)
        {
            string proverka_imeni;
            string nesootvetstviya = "";
            for (int i = 0; i < ScriptList.Count; i++)
            {
                if (ScriptList[i].Com_name == "Assign")
                {
                    int counter = 0;
                    proverka_imeni = ((Assign)ScriptList[i]).name;
                    foreach (Signal sig in SignalList)
                    {
                        if (proverka_imeni != sig.name)
                        {
                            counter++;
                        }
                    }
                    if (counter == SignalList.Count)
                    {
                        nesootvetstviya = nesootvetstviya + " " + proverka_imeni;
                    }
                }
                else if (ScriptList[i].Com_name == "Summation")
                {
                    int counter = 0;
                    int counter1 = 0;
                    int counter2 = 0;
                    proverka_imeni = ((Summation)ScriptList[i]).name;
                    string prov_val1 = "";
                    string prov_val2 = "";
                    try { Convert.ToInt32(((Summation)ScriptList[i]).value_1); }
                    catch { prov_val1 = ((Summation)ScriptList[i]).value_1; }

                    try { Convert.ToInt32(((Summation)ScriptList[i]).value_2); }
                    catch { prov_val2 = ((Summation)ScriptList[i]).value_2; }

                    foreach (Signal sig in SignalList)
                    {
                        if (proverka_imeni != sig.name)
                        {
                            counter++;
                        }
                        if (prov_val1 != "" && prov_val1 != sig.name)
                        {
                            counter1++;
                        }
                        if (prov_val2 != "" && prov_val2 != sig.name)
                        {
                            counter2++;
                        }
                    }
                    if (counter == SignalList.Count)
                    { nesootvetstviya = nesootvetstviya + " " + proverka_imeni; }
                    else if (counter1 == SignalList.Count)
                    { nesootvetstviya = nesootvetstviya + " " + prov_val1; }
                    else if (counter2 == SignalList.Count)
                    { nesootvetstviya = nesootvetstviya + " " + prov_val2; }
                }
                else if (ScriptList[i].Com_name == "Subtraction")
                {
                    int counter = 0;
                    int counter1 = 0;
                    int counter2 = 0;
                    proverka_imeni = ((Subtraction)ScriptList[i]).name;
                    string prov_val1 = "";
                    string prov_val2 = "";
                    try { Convert.ToInt32(((Subtraction)ScriptList[i]).value_1); }
                    catch { prov_val1 = ((Subtraction)ScriptList[i]).value_1; }

                    try { Convert.ToInt32(((Subtraction)ScriptList[i]).value_2); }
                    catch { prov_val2 = ((Subtraction)ScriptList[i]).value_2; }

                    foreach (Signal sig in SignalList)
                    {
                        if (proverka_imeni != sig.name)
                        {
                            counter++;
                        }
                        if (prov_val1 != "" && prov_val1 != sig.name)
                        {
                            counter1++;
                        }
                        if (prov_val2 != "" && prov_val2 != sig.name)
                        {
                            counter2++;
                        }
                    }
                    if (counter == SignalList.Count)
                    { nesootvetstviya = nesootvetstviya + " " + proverka_imeni; }
                    else if (counter1 == SignalList.Count)
                    { nesootvetstviya = nesootvetstviya + " " + prov_val1; }
                    else if (counter2 == SignalList.Count)
                    { nesootvetstviya = nesootvetstviya + " " + prov_val2; }
                }

                else if (ScriptList[i].Com_name == "Multiplication")
                {
                    int counter = 0;
                    int counter1 = 0;
                    int counter2 = 0;
                    proverka_imeni = ((Multiplication)ScriptList[i]).name;
                    string prov_val1 = "";
                    string prov_val2 = "";
                    try { Convert.ToInt32(((Multiplication)ScriptList[i]).value_1); }
                    catch { prov_val1 = ((Multiplication)ScriptList[i]).value_1; }

                    try { Convert.ToInt32(((Multiplication)ScriptList[i]).value_2); }
                    catch { prov_val2 = ((Multiplication)ScriptList[i]).value_2; }

                    foreach (Signal sig in SignalList)
                    {
                        if (proverka_imeni != sig.name)
                        {
                            counter++;
                        }
                        if (prov_val1 != "" && prov_val1 != sig.name)
                        {
                            counter1++;
                        }
                        if (prov_val2 != "" && prov_val2 != sig.name)
                        {
                            counter2++;
                        }
                    }
                    if (counter == SignalList.Count)
                    { nesootvetstviya = nesootvetstviya + " " + proverka_imeni; }
                    else if (counter1 == SignalList.Count)
                    { nesootvetstviya = nesootvetstviya + " " + prov_val1; }
                    else if (counter2 == SignalList.Count)
                    { nesootvetstviya = nesootvetstviya + " " + prov_val2; }
                }

                else if (ScriptList[i].Com_name == "Division")
                {
                    int counter = 0;
                    int counter1 = 0;
                    int counter2 = 0;
                    proverka_imeni = ((Division)ScriptList[i]).name;
                    string prov_val1 = "";
                    string prov_val2 = "";
                    try { Convert.ToInt32(((Division)ScriptList[i]).value_1); }
                    catch { prov_val1 = ((Division)ScriptList[i]).value_1; }

                    try { Convert.ToInt32(((Division)ScriptList[i]).value_2); }
                    catch { prov_val2 = ((Division)ScriptList[i]).value_2; }

                    foreach (Signal sig in SignalList)
                    {
                        if (proverka_imeni != sig.name)
                        {
                            counter++;
                        }
                        if (prov_val1 != "" && prov_val1 != sig.name)
                        {
                            counter1++;
                        }
                        if (prov_val2 != "" && prov_val2 != sig.name)
                        {
                            counter2++;
                        }
                    }
                    if (counter == SignalList.Count)
                    { nesootvetstviya = nesootvetstviya + " " + proverka_imeni; }
                    else if (counter1 == SignalList.Count)
                    { nesootvetstviya = nesootvetstviya + " " + prov_val1; }
                    else if (counter2 == SignalList.Count)
                    { nesootvetstviya = nesootvetstviya + " " + prov_val2; }
                }

                else if (ScriptList[i].Com_name == "Square")
                {
                    int counter = 0;
                    proverka_imeni = ((Square)ScriptList[i]).name;
                    foreach (Signal sig in SignalList)
                    {
                        if (proverka_imeni != sig.name)
                        {
                            counter++;
                        }
                    }
                    if (counter == SignalList.Count)
                    {
                        nesootvetstviya = nesootvetstviya + " " + proverka_imeni;
                    }
                }
                else if (ScriptList[i].Com_name == "Sinusoid")
                {
                    int counter = 0;
                    proverka_imeni = ((Sinusoid)ScriptList[i]).name;
                    foreach (Signal sig in SignalList)
                    {
                        if (proverka_imeni != sig.name)
                        {
                            counter++;
                        }
                    }
                    if (counter == SignalList.Count)
                    {
                        nesootvetstviya = nesootvetstviya + " " + proverka_imeni;
                    }
                }
                else if (ScriptList[i].Com_name == "Triangle")
                {
                    int counter = 0;
                    proverka_imeni = ((Triangle)ScriptList[i]).name;
                    foreach (Signal sig in SignalList)
                    {
                        if (proverka_imeni != sig.name)
                        {
                            counter++;
                        }
                    }
                    if (counter == SignalList.Count)
                    {
                        nesootvetstviya = nesootvetstviya + " " + proverka_imeni;
                    }
                }
                else if (ScriptList[i].Com_name == "Saw")
                {
                    int counter = 0;
                    proverka_imeni = ((Saw)ScriptList[i]).name;
                    foreach (Signal sig in SignalList)
                    {
                        if (proverka_imeni != sig.name)
                        {
                            counter++;
                        }
                    }
                    if (counter == SignalList.Count)
                    {
                        nesootvetstviya = nesootvetstviya + " " + proverka_imeni;
                    }
                }
            }

            if (nesootvetstviya != "")
            {
                this.Invoke((MethodInvoker)delegate()
                {
                    int nesootv = nesootvetstviya.Count();
                    richTextBox1.Select(stroka, stroka + 39 + nesootvetstviya.Count()); //выделяем текст                
                    richTextBox1.SelectionColor = Color.Purple; //для выделенного текста устанавливаем цвет
                    richTextBox1.AppendText(" Сигналы" + nesootvetstviya + " отсутствуют в списке сигналов"
                        + Environment.NewLine);
                    stroka = stroka + 39 + nesootvetstviya.Count();
                    richTextBox1.ScrollToCaret();
                });                
                button3.Enabled = false;
                button4.Enabled = false;
            }
            else
            {
                this.Invoke((MethodInvoker)delegate()
                {
                    richTextBox1.Select(stroka, stroka + 37); //выделяем текст                
                    richTextBox1.SelectionColor = Color.Green; //для выделенного текста устанавливаем цвет
                    richTextBox1.AppendText("Проверка соответствия прошла успешно" + Environment.NewLine);
                    stroka = stroka + 37;
                    richTextBox1.ScrollToCaret();
                });                
                button3.Enabled = true;
            }
        }

        // Проверка подключения к ОИК
        public void podkluchenie_OIK()
        {
            if (dak.Connection.RTDBAbbrev == null)
            {
                button1.Enabled = false;
                button3.Enabled = false;
                button3.Enabled = false;
                label2.Text = "Не выбран сервер ОИК";
            }
            else 
            {
                button1.Enabled = true;
                button3.Enabled = false;
                button3.Enabled = false;
                label2.Text = string.Format("{0}\\{1}\\{2}",
                dak.Connection.Domain, dak.Connection.Group, dak.Connection.RTDBAbbrev);
            }
        }

        // Быстрая проверка.F5 и F6
        public Thread QuickCheck;
        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                timer2.Enabled = true;
                QuickCheck = new Thread(Bistraya_proverka);
                QuickCheck.Start();
            }

            if (e.KeyData == Keys.F6)
            {
                QuickCheck.Abort();
                timer2.Enabled = false;
                this.Invoke((MethodInvoker)delegate()
                {
                    richTextBox1.Select(stroka, stroka + 39);
                    richTextBox1.SelectionColor = Color.Blue;
                    richTextBox1.AppendText("! Завершение быстрой проверки сценария" + Environment.NewLine);
                    stroka = stroka + 39;
                    richTextBox1.ScrollToCaret();
                });                

                // Сброс поля Прошлая команда
                foreach (Signal sig in SignalList)
                {
                    sig.last_command = "Assign";
                }

                secondes = 0;
            }
        }

        public void Bistraya_proverka()
        {
            timer2.Enabled = true;
            int vr = Environment.TickCount;
            this.Invoke((MethodInvoker)delegate()
            {
                richTextBox1.Select(stroka, stroka + 35); //выделяем текст
                richTextBox1.SelectionColor = Color.Blue; //для выделенного текста устанавливаем цвет
                richTextBox1.AppendText("! Запуск быстрой проверки сценария" + Environment.NewLine);
                stroka = stroka + 35;
                richTextBox1.ScrollToCaret();
            });
            
            for (int i = 0; i < SignalList.Count; i++)
            {
                if (SignalList[i].start_value != SignalList[i].value)
                {
                    SignalList[i].value = SignalList[i].start_value;
                    SignalList[i].last_value = SignalList[i].start_value;
                }
                if (SignalList[i].start_code != SignalList[i].code)
                {
                    SignalList[i].code = SignalList[i].start_code;
                    SignalList[i].last_code = SignalList[i].start_code;
                }
            }
            dla = 0;
            emiT = 0;

            int iee = 0;
            int cikl = 0;
            int nach_sch = 0;
            vr = Environment.TickCount - vr;
            for (; ; )
            {
                int vr2 = Environment.TickCount;
                int min = emiT / 60;
                int sec = emiT % 60;
                string mins = String.Format("{0:00}", min);
                string secc = String.Format("{0:00}", sec);

                this.Invoke((MethodInvoker)delegate()
                {
                    richTextBox1.Select(stroka, stroka + 26 + mins.Count() + secc.Count() + Convert.ToString(emiT).Count());
                    richTextBox1.SelectionColor = Color.Blue; //для выделенного текста устанавливаем цвет
                    richTextBox1.AppendText("! " + mins + ":" + secc + "  |   " + " Прошло секунд: " +
                    emiT + Environment.NewLine);
                    stroka = stroka + 26 + mins.Count() + secc.Count() + Convert.ToString(emiT).Count();
                    richTextBox1.ScrollToCaret();
                });

                // Проход по сценарию
                if (nach_sch < ScriptList.Count)
                {
                    for (int i = 0; i < ScriptList.Count; i++)
                    {
                        if (nach_sch == i)
                        {
                            // ПАУЗА
                            if (ScriptList[i].Com_name == "Pause")
                            {
                                cikl++;
                                if (emiT == dla && cikl != ((Pause)ScriptList[i]).time)
                                {
                                    dla = dla + Convert.ToInt32(((Pause)ScriptList[i]).time);

                                    break;
                                }
                                else if (cikl == ((Pause)ScriptList[i]).time)
                                {
                                    Proverka_Cikla();
                                    nach_sch++;
                                    cikl = 0;
                                    break;
                                }
                                else
                                {
                                    Proverka_Cikla();
                                    break;
                                }
                            }
                            // ПРИСВОЕНИЕ
                            else if (ScriptList[i].Com_name == "Assign")
                            {
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Assign)ScriptList[i]).name == SignalList[k].name &&
                                        ((Assign)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        SignalList[k].value = ((Assign)ScriptList[i]).value;
                                        SignalList[k].last_command = "Assign";
                                    }
                                }
                                nach_sch++;
                            }
                            // СУММА
                            else if (ScriptList[i].Com_name == "Summation")
                            {
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Summation)ScriptList[i]).name == SignalList[k].name &&
                                        ((Summation)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        try
                                        {
                                            // Если первое значение число, то
                                            Convert.ToDouble(((Summation)ScriptList[i]).value_1);
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Summation)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Summation)ScriptList[i]).value_1) +
                                                    Convert.ToDouble(((Summation)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое значение число, а второе - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Summation)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Summation)ScriptList[i]).value_1) +
                                                    SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // Если первое значение TM, то
                                            double perv_znach = 0;
                                            for (int ii = 0; ii < SignalList.Count; ii++)
                                            {
                                                if (((Summation)ScriptList[i]).value_1 == SignalList[ii].name &&
                                                    SignalList[k].Type == SignalList[ii].Type)
                                                {
                                                    perv_znach = SignalList[ii].value;
                                                }
                                            }
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Summation)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(perv_znach +
                                                    Convert.ToDouble(((Summation)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое и второе значения - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Summation)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(perv_znach + SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        SignalList[k].last_command = "Summation";
                                    }
                                }
                                nach_sch++;
                            }

                            // РАЗНОСТЬ
                            else if (ScriptList[i].Com_name == "Subtraction")
                            {
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Subtraction)ScriptList[i]).name == SignalList[k].name &&
                                        ((Subtraction)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        try
                                        {
                                            // Если первое значение число, то
                                            Convert.ToDouble(((Subtraction)ScriptList[i]).value_1);
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Subtraction)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Subtraction)ScriptList[i]).value_1) -
                                                    Convert.ToDouble(((Subtraction)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое значение число, а второе - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Subtraction)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Subtraction)ScriptList[i]).value_1) -
                                                    SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // Если первое значение TM, то
                                            double perv_znach = 0;
                                            for (int ii = 0; ii < SignalList.Count; ii++)
                                            {
                                                if (((Subtraction)ScriptList[i]).value_1 == SignalList[ii].name &&
                                                    SignalList[k].Type == SignalList[ii].Type)
                                                {
                                                    perv_znach = SignalList[ii].value;
                                                }
                                            }
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Subtraction)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(perv_znach -
                                                    Convert.ToDouble(((Subtraction)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое и второе значения - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Subtraction)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(perv_znach - SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        SignalList[k].last_command = "Subtraction";
                                    }
                                }
                                nach_sch++;
                            }


                            // ПРОИЗВЕДЕНИЕ
                            else if (ScriptList[i].Com_name == "Multiplication")
                            {
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Multiplication)ScriptList[i]).name == SignalList[k].name &&
                                        ((Multiplication)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        try
                                        {
                                            // Если первое значение число, то
                                            Convert.ToDouble(((Multiplication)ScriptList[i]).value_1);
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Multiplication)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Multiplication)ScriptList[i]).value_1) *
                                                    Convert.ToDouble(((Multiplication)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое значение число, а второе - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Multiplication)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Multiplication)ScriptList[i]).value_1) *
                                                    SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // Если первое значение TM, то
                                            double perv_znach = 0;
                                            for (int ii = 0; ii < SignalList.Count; ii++)
                                            {
                                                if (((Multiplication)ScriptList[i]).value_1 == SignalList[ii].name &&
                                                    SignalList[k].Type == SignalList[ii].Type)
                                                {
                                                    perv_znach = SignalList[ii].value;
                                                }
                                            }
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Multiplication)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(perv_znach *
                                                    Convert.ToDouble(((Multiplication)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое и второе значения - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Multiplication)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(perv_znach * SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        SignalList[k].last_command = "Multiplication";
                                    }
                                }
                                nach_sch++;
                            }

                            // ДЕЛЕНИЕ
                            else if (ScriptList[i].Com_name == "Division")
                            {
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Division)ScriptList[i]).name == SignalList[k].name &&
                                        ((Division)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        try
                                        {
                                            // Если первое значение число, то
                                            Convert.ToDouble(((Division)ScriptList[i]).value_1);
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Division)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Division)ScriptList[i]).value_1) /
                                                    Convert.ToDouble(((Division)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое значение число, а второе - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Division)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(Convert.ToDouble(((Division)ScriptList[i]).value_1) /
                                                    SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            // Если первое значение TM, то
                                            double perv_znach = 0;
                                            for (int ii = 0; ii < SignalList.Count; ii++)
                                            {
                                                if (((Division)ScriptList[i]).value_1 == SignalList[ii].name &&
                                                    SignalList[k].Type == SignalList[ii].Type)
                                                {
                                                    perv_znach = SignalList[ii].value;
                                                }
                                            }
                                            try
                                            {
                                                // Если и второе значение число, то
                                                Convert.ToDouble(((Division)ScriptList[i]).value_2);
                                                SignalList[k].value = Convert.ToDouble(perv_znach /
                                                    Convert.ToDouble(((Division)ScriptList[i]).value_2));
                                            }
                                            catch
                                            {
                                                // Если первое и второе значения - ТМ
                                                for (int ii = 0; ii < SignalList.Count; ii++)
                                                {
                                                    if (((Division)ScriptList[i]).value_2 == SignalList[ii].name &&
                                                        SignalList[k].Type == SignalList[ii].Type)
                                                    {
                                                        SignalList[k].value = Convert.ToDouble(perv_znach / SignalList[ii].value);
                                                    }
                                                }
                                            }
                                        }
                                        SignalList[k].last_command = "Division";
                                    }
                                }
                                nach_sch++;
                            }

                            // МЕАНДР
                            else if (ScriptList[i].Com_name == "Square")
                            {
                                int fifi = 0;
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Square)ScriptList[i]).name == SignalList[k].name &&
                                        ((Square)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        SignalList[k].value_min = ((Square)ScriptList[i]).value_min;
                                        SignalList[k].value_max = ((Square)ScriptList[i]).value_max;
                                        SignalList[k].period = ((Square)ScriptList[i]).period;
                                        SignalList[k].tek_time = ((Square)ScriptList[i]).tek_time;
                                        SignalList[k].middle = ((Square)ScriptList[i]).middle;
                                        SignalList[k].tek_value = ((Square)ScriptList[i]).tek_value;

                                        fifi = k;
                                    }
                                }
                                if (i != ScriptList.Count - 1)
                                {
                                    if (SignalList[fifi].tek_time >= SignalList[fifi].middle)
                                    {
                                        SignalList[fifi].value = SignalList[fifi].value_max;
                                    }
                                    else
                                    {
                                        SignalList[fifi].value = SignalList[fifi].value_min;
                                    }

                                    if (SignalList[fifi].tek_time == SignalList[fifi].period - 1)
                                    { SignalList[fifi].tek_time = 0; }
                                    else
                                    {
                                        SignalList[fifi].tek_time = SignalList[fifi].tek_time + 1;
                                    }
                                }

                                SignalList[fifi].last_command = "Square";

                                nach_sch++;
                            }

                            // Синусоида
                            else if (ScriptList[i].Com_name == "Sinusoid")
                            {
                                int fifi = 0;
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Sinusoid)ScriptList[i]).name == SignalList[k].name &&
                                        ((Sinusoid)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        SignalList[k].value_min = ((Sinusoid)ScriptList[i]).value_start;
                                        SignalList[k].value_max = ((Sinusoid)ScriptList[i]).value_max;
                                        SignalList[k].period = ((Sinusoid)ScriptList[i]).period;
                                        SignalList[k].middle = ((Sinusoid)ScriptList[i]).middle;
                                        SignalList[k].tek_time = 0;

                                        SignalList[k].value = SignalList[k].value_min + SignalList[k].value_max *
                                            Math.Sin((2 * Math.PI * SignalList[k].tek_time / SignalList[k].period) +
                                            SignalList[k].middle);

                                        fifi = k;
                                    }
                                }
                                SignalList[fifi].tek_time = SignalList[fifi].tek_time + 1;
                                SignalList[fifi].last_command = "Sinusoid";

                                nach_sch++;
                            }

                            // Треугольник
                            else if (ScriptList[i].Com_name == "Triangle")
                            {
                                int fifi = 0;
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Triangle)ScriptList[i]).name == SignalList[k].name &&
                                        ((Triangle)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        SignalList[k].value_min = ((Triangle)ScriptList[i]).value_min;
                                        SignalList[k].value_max = ((Triangle)ScriptList[i]).value_max;
                                        SignalList[k].period = ((Triangle)ScriptList[i]).period;
                                        SignalList[k].middle = ((Triangle)ScriptList[i]).middle;
                                        SignalList[k].tek_time = 0;
                                        SignalList[k].tek_value = SignalList[k].value_min;

                                        fifi = k;
                                        
                                        SignalList[fifi].value = SignalList[fifi].value_min;   
                                    }
                                }
                                SignalList[fifi].last_command = "Triangle";

                                nach_sch++;
                            }

                            // ПИЛА
                            else if (ScriptList[i].Com_name == "Saw")
                            {
                                int fifi = 0;
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Saw)ScriptList[i]).name == SignalList[k].name &&
                                        ((Saw)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        SignalList[k].value_min = ((Saw)ScriptList[i]).value_min;
                                        SignalList[k].value_max = ((Saw)ScriptList[i]).value_max;
                                        SignalList[k].period = ((Saw)ScriptList[i]).period;
                                        SignalList[k].middle = ((Saw)ScriptList[i]).step;
                                        SignalList[k].tek_time = 0;
                                        SignalList[k].tek_value = SignalList[k].value_min;

                                        fifi = k;

                                        SignalList[k].value = SignalList[k].value_min;
                                    }
                                }
                                SignalList[fifi].last_command = "Saw";

                                nach_sch++;
                            }

                            // КОД
                            else if (ScriptList[i].Com_name == "Code")
                            {
                                for (int k = 0; k < SignalList.Count; k++)
                                {
                                    if (((Code)ScriptList[i]).name == SignalList[k].name &&
                                        ((Code)ScriptList[i]).type == SignalList[k].Type)
                                    {
                                        SignalList[k].code = ((Code)ScriptList[i]).code;
                                    }
                                }
                                nach_sch++;
                            }

                            else
                            {
                                Proverka_Cikla();
                                nach_sch++;
                            } // ДОРАБОТАТЬ

                            if (i == ScriptList.Count - 1)
                            { Proverka_Cikla(); }
                        }                        
                    }
                }
                else
                {
                    Proverka_Cikla();
                }
                // Проход по параметрам сигналов
                for (int i = 0; i < SignalList.Count; i++)
                {
                    if (SignalList[i].kind == "Циклическая" && emiT % SignalList[i].prop == 0)
                    {
                        if (SignalList[i].code != SignalList[i].last_code)
                        {
                            this.Invoke((MethodInvoker)delegate()
                            {
                                richTextBox1.Select(stroka, stroka + 26 + SignalList[i].name.Count() + SignalList[i].code.Count());
                                richTextBox1.SelectionColor = Color.Blue;
                                richTextBox1.AppendText("!            |    " + SignalList[i].name + ".Код = " +
                                    SignalList[i].code + Environment.NewLine);
                                stroka = stroka + 26 + SignalList[i].name.Count() + SignalList[i].code.Count();
                                richTextBox1.ScrollToCaret();
                            });
                            
                            SignalList[i].last_code = SignalList[i].code;
                        }

                        this.Invoke((MethodInvoker)delegate()
                        {
                            richTextBox1.Select(stroka, stroka + 31 + SignalList[i].name.Count() +
                                Convert.ToString(SignalList[i].value).Count());
                            richTextBox1.SelectionColor = Color.Blue;
                            richTextBox1.AppendText("!            |    " + SignalList[i].name + ".Значение = " +
                                SignalList[i].value + Environment.NewLine);
                            stroka = stroka + 31 + SignalList[i].name.Count() + Convert.ToString(SignalList[i].value).Count();
                            richTextBox1.ScrollToCaret();
                        });
                    }
                    else if (SignalList[i].kind == "По изменению" && (SignalList[i].value != SignalList[i].last_value ||
                        SignalList[i].code != SignalList[i].last_code))
                    {
                        if (SignalList[i].code != SignalList[i].last_code)
                        {
                            this.Invoke((MethodInvoker)delegate()
                            {
                                richTextBox1.Select(stroka, stroka + 26 + SignalList[i].name.Count() + SignalList[i].code.Count());
                                richTextBox1.SelectionColor = Color.Blue;
                                richTextBox1.AppendText("!            |    " + SignalList[i].name + ".Код = " +
                                    SignalList[i].code + Environment.NewLine);
                                stroka = stroka + 26 + SignalList[i].name.Count() + SignalList[i].code.Count();
                                richTextBox1.ScrollToCaret();
                            });
                            
                            SignalList[i].last_code = SignalList[i].code;
                        }

                        this.Invoke((MethodInvoker)delegate()
                        {
                            richTextBox1.Select(stroka, stroka + 31 + SignalList[i].name.Count() +
                                Convert.ToString(SignalList[i].value).Count());
                            richTextBox1.SelectionColor = Color.Blue;
                            richTextBox1.AppendText("!            |    " + SignalList[i].name + ".Значение = " +
                                SignalList[i].value + Environment.NewLine);
                            stroka = stroka + 31 + SignalList[i].name.Count() + Convert.ToString(SignalList[i].value).Count();
                            richTextBox1.ScrollToCaret();
                        });
                        SignalList[i].last_value = SignalList[i].value;
                    }
                }

                emiT++;
                iee++;
                
                vr2 = Environment.TickCount - vr2;
                vr = 0;
            }
        }

        // Большое окно лога
        private void richTextBox1_DoubleClick(object sender, EventArgs e)
        {
            string bof = richTextBox1.Text;
            Log log = new Log(this, bof);
            log.Show();
        }

        // Комментарий к сценарию
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (comment_window == false)
            {
                groupBox1.Visible = true;
                this.Width = 900;
                button2.Text = "<";
                comment_window = true;
            }
            else
            {
                groupBox1.Visible = false;
                this.Width = 683;
                button2.Text = ">";
                comment_window = false;
            }
        }
    }
}

