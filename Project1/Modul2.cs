using System;
using System.Windows.Forms;
using System.IO;
using OICDAC;

namespace Project1
{
    public partial class Modul2 : Form
    {
        public Modul2()
        {
            InitializeComponent();
        }

        OpenFileDialog TEST = new OpenFileDialog();
        string FileNameTxt;
        string[] opfl = new string[0];
        double[] lfpo = new double[0];
        public static DAC dagg = new DACClass();
        OIRequest rqActual;

        // Открытие файла
        private void button2_Click(object sender, EventArgs e)
        {
             TEST.Filter = "TXT (*.txt)| *.txt; | Все (*.*)|*.*;";
             if (TEST.ShowDialog() == DialogResult.OK)
             {
                 FileNameTxt = TEST.FileName;
                 textBox6.Text = FileNameTxt;
                 opfl = File.ReadAllLines(FileNameTxt);
                 Array.Resize(ref lfpo, opfl.Length);
                 for (int i = 0; i < opfl.Length; i++)
                 {
                     lfpo[i] = Convert.ToDouble(opfl[i]);
                 }                 
             }
        }
        
        // Создание константы
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                if (textBox1.Text != "")
                {
                    if (textBox3.Text != "")
                    {
                        if (textBox2.Text != "")
                        // Создание файла
                        {
                            // Задаём массив
                            Random rand = new Random();
                            int sch = Convert.ToInt32(label1.Text);
                            double sig = Convert.ToInt32(textBox1.Text);
                            int shum = Convert.ToInt32(textBox2.Text);
                            int s1 = shum * 2;

                            string[] msv = new string[sch];
                            for (int i = 0; i < sch; i++)
                            {
                                double ent = rand.Next(0, s1) - shum;
                                double bff = sig * ent / 100;
                                double chis = sig + bff;
                                msv[i] = Convert.ToString(chis);
                            }
                            File.WriteAllLines("Test_1.txt", msv);
                        }
                        else
                        {
                            MessageBox.Show("Задайте шум");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Задайте точность");
                    }
                }
                else
                {
                    MessageBox.Show("Задайте значение");
                }
            }
            else
            {
                MessageBox.Show("Сигнал должен быть константой");
            }
        }
        

        // Константа или Синусоида
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                label15.Visible = false;
                label12.Visible = false;
                label13.Visible = false;
                textBox4.Visible = false;
                textBox5.Visible = false;
            }
            else 
            {
                label15.Visible = true;
                label12.Visible = true;
                label13.Visible = true;
                textBox4.Visible = true;
                textBox5.Visible = true;
            }
        }


        // Операции при открытии окна
        private void Modul2_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 1;
            if (dagg.Connection.Connected == true)
            {
                textBox7.Text = string.Format("{0}\\{1}\\{2}",
                    dagg.Connection.Domain, dagg.Connection.Group, dagg.Connection.RTDBAbbrev);
            }
            else
            {
                textBox7.Clear();
            }            
        }

        // Создание синусоиды
        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1)
            {
                if (textBox1.Text != "")
                {
                    if (textBox3.Text != "")
                    {
                        if (textBox2.Text != "")
                        // Создание файла
                        {
                            if (textBox4.Text != "")
                            {
                                if (textBox5.Text != "")
                                {
                                    try
                                    {
                                        Convert.ToInt32(textBox1.Text);
                                        try
                                        {
                                            Convert.ToInt32(textBox3.Text);
                                            try
                                            {
                                                Convert.ToInt32(textBox2.Text);
                                                try
                                                {
                                                    Convert.ToInt32(textBox4.Text);
                                                    try
                                                    {
                                                        Convert.ToInt32(textBox5.Text);
                                                        // Задаём массив
                                                        Random rand = new Random();
                                                        int toch = Convert.ToInt32(textBox3.Text);
                                                        int sch = Convert.ToInt32(label1.Text);
                                                        double sig = Convert.ToInt32(textBox1.Text);
                                                        int shum = Convert.ToInt32(textBox2.Text);
                                                        double amp = Convert.ToInt32(textBox4.Text);
                                                        double per = Convert.ToInt32(textBox5.Text);
                                                        double vrem = 0;
                                                        double jjj = 0;

                                                        int s1 = shum * 2;
                                                        double omega = 2 * Math.PI / per;

                                                        string[] msv1 = new string[sch];

                                                        // Вызов метода
                                                        msv1 = msv(sch, vrem, omega, amp, s1, rand, shum, sig, toch, jjj);

                                                        File.WriteAllLines("Test_2.txt", msv1);
                                                    }
                                                    catch (FormatException)
                                                    {
                                                        MessageBox.Show("Ошибка. Неверный формат периода сигнала");
                                                    }
                                                }
                                                catch (FormatException)
                                                {
                                                    MessageBox.Show("Ошибка. Неверный формат амплитуды сигнала");
                                                }
                                            }
                                            catch (FormatException)
                                            {
                                                MessageBox.Show("Ошибка. Неверный формат шума сигнала");
                                            }
                                        }
                                        catch (FormatException)
                                        {
                                            MessageBox.Show("Ошибка. Неверный формат точности сигнала");
                                        }
                                    }
                                    catch (FormatException)
                                    {
                                        MessageBox.Show("Ошибка. Неверный формат значения сигнала");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Задайте период");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Задайте амплитуду");
                            }

                        }
                        else
                        {
                            MessageBox.Show("Задайте шум");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Задайте точность");
                    }
                }
                else
                {
                    MessageBox.Show("Задайте значение");
                }
            }
            else
            {
                MessageBox.Show("Сигнал должен быть синусоидой");
            }
        }

        // Создание комбинированного сигнала
        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1)
            {
                if (textBox1.Text != "")
                {
                    if (textBox3.Text != "")
                    {
                        if (textBox2.Text != "")
                        // Создание файла
                        {
                            if (textBox4.Text != "")
                            {
                                if (textBox5.Text != "")
                                {
                                    // Задаём массив
                                    Random rand = new Random();
                                    int sch = Convert.ToInt32(label1.Text);
                                    double sig = Convert.ToInt32(textBox1.Text);
                                    int shum = Convert.ToInt32(textBox2.Text);
                                    double amp = Convert.ToInt32(textBox4.Text);
                                    double per = Convert.ToInt32(textBox5.Text);
                                    double vrem = 0;
                                    double ent = 0;
                                    double bff = 0;
                                    double post = 0;

                                    int s1 = shum * 2;
                                    double omega = 2 * Math.PI / per;

                                    string[] msv = new string[sch];
                                    for (int i = 0; i < sch; i++)
                                    {
                                        if (i < 1000)
                                        {
                                            ent = rand.Next(0, s1) - shum;
                                            bff = sig * ent / 100;
                                            post = sig + bff;
                                            msv[i] = Convert.ToString(post);
                                        }
                                        else
                                        {
                                            if (i >= 1000 && i < 2000)
                                            {
                                                // Формируем синусоиду
                                                vrem = vrem + 1;
                                                double skob = omega * vrem;
                                                double sinusoida = amp * Math.Sin(skob);

                                                // Формируем константу
                                                ent = rand.Next(0, s1) - shum;
                                                bff = sig * ent / 100;
                                                post = sig + bff;

                                                // Общий сигнал
                                                double chis = post + sinusoida;
                                                msv[i] = Convert.ToString(chis);
                                            }
                                            else
                                            {
                                                ent = rand.Next(0, s1) - shum;
                                                bff = sig * ent / 100;
                                                post = sig + bff;
                                                msv[i] = Convert.ToString(post);
                                            }
                                        }
                                    }
                                    File.WriteAllLines("Test_Comb.txt", msv);
                                }
                                else
                                {
                                    MessageBox.Show("Задайте период");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Задайте амплитуду");
                            }

                        }
                        else
                        {
                            MessageBox.Show("Задайте шум");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Задайте точность");
                    }
                }
                else
                {
                    MessageBox.Show("Задайте значение");
                }
            }
            else
            {
                MessageBox.Show("Сигнал должен быть синусоидой");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // запрос актуальных значений
            if (!dagg.Connection.Connected)
                return;

            textBox9.Visible = true;
            textBox8.Visible = true;

            if (dagg.OIRequests.Count > 0)
            {
                // останавливаем и удаляем старый запрос
                dagg.OIRequests.Item(0).Stop();
                dagg.OIRequests.Item(0).Delete();
            }

            // добавляем новый запрос            
            rqActual = dagg.OIRequests.Add();
            // получаем данные с миллисекундами
            rqActual.UseMilliseconds = true;
            // задаем обработчик события получения данных    
            rqActual.Stop();
            rqActual.OIRequestItems.Clear();

            OIRequestItem rqi;
            
            button5.Enabled = false;

            // добавляем элемент запроса
            rqi = rqActual.AddOIRequestItem();
            rqi.IsLocalTime = true;
            rqi.KindRefresh = KindRefreshEnum.kr_WriteData;
            rqi.DataSource = "I591";

            for (int i = 0; i < lfpo.Length; i++)
            {
                int s = Environment.TickCount;
                rqi.DataValue = lfpo[i];
                rqActual.Start();
                s = Environment.TickCount - s;
                System.Threading.Thread.Sleep(1000 - s);
                textBox8.Text = Convert.ToString(lfpo[i]);
                textBox9.Text = Convert.ToString(i + 1);
            }
                                  
            button5.Enabled = false;
        }

        // Метод задания матрицы
        public static string[] msv(int sch, double vrem, double omega, double amp, int s1, Random rand, int shum, double sig, int toch, double jjj)
        {
            string[] msv = new string[sch];
            for (int i = 0; i < sch; i++)
            {
                // Формируем синусоиду
                vrem = vrem + 1;
                double skob = omega * vrem;
                double sinusoida = amp * Math.Sin(skob);

                // Формируем константу
                double ent = rand.Next(0, s1) - shum;
                double bff = sig * ent / 100;
                double post = sig + bff;

                // Общий сигнал
                double chis = post + sinusoida;
                switch (toch)
                {
                    case 0:
                        jjj = Math.Round(chis, 0);
                        msv[i] = Convert.ToString(jjj);
                        break;
                    case 1:
                        jjj = Math.Round(chis, 1);
                        msv[i] = Convert.ToString(jjj);
                        break;
                    case 2:
                        jjj = Math.Round(chis, 2);
                        msv[i] = Convert.ToString(jjj);
                        break;
                    case 3:
                        jjj = Math.Round(chis, 3);
                        msv[i] = Convert.ToString(jjj);
                        break;
                    default:
                        MessageBox.Show("Точность должна быть в пределах от 0 до 3");
                        break;
                }
            }
            return msv;
        }
    }
}
