using System;
using System.Windows.Forms;
using OICDAC;

namespace Project1
{
    public partial class KDD_Option : Form
    {
        public KDD_Option(GTS gi)
        {
            InitializeComponent();
            m_parent = gi;
        }
        
        DAC dac;
        public GTS m_parent;
        
        // Подключение к серверу ОИК
        private void button1_Click(object sender, EventArgs e)
        {
            // Проверяется подключение к серверу
            if (dac.Connection.Connected == true)
            {
                MessageBox.Show("Соединение с сервером уже установлено");
            }
            else
            {
                // подключение к ОИК, выбранному из дерева
                dac.Connection.Connected = false;
                dac.Connection.ShowDialog();
            }
        }

        // Операции при открытии окна
        private void KDD_Option_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Не выбран сервер ОИК";
            
            // создаем объект - КДД
            dac = new DACClass();

            // основное окно приложения
            dac.AppHandle = (int)this.Handle;

            // устанавливаем обработчики событий соединения
            dac.Connection.OnRTDBConnected +=
                new IOICConnectionEvents_OnRTDBConnectedEventHandler(Connection_OnRTDBConnected);
            dac.Connection.OnRTDBConnectionClose +=
                new IOICConnectionEvents_OnRTDBConnectionCloseEventHandler(Connection_OnRTDBConnectionClose);
        }

        // Закрытие соединения с сервером
        private void button2_Click(object sender, EventArgs e)
        {
            // закрываем подключение
            this.Cursor = Cursors.Default;
            dac.Connection.Connected = false;
            this.Cursor = Cursors.Default;
            toolStripStatusLabel1.Text = "Не выбран сервер ОИК";
            textBox1.Clear();
        }

        
        void Connection_OnRTDBConnectionClose()
        {
            toolStripStatusLabel1.Text = "Закрыто соединение с ОИК" + Environment.NewLine;
            toolStripStatusLabel1.Text = "";
        }

        void Connection_OnRTDBConnected()
        {
            toolStripStatusLabel1.Text = "Открыто соединение с ОИК";
            textBox1.Text = string.Format("{0}\\{1}\\{2}",
                dac.Connection.Domain, dac.Connection.Group, dac.Connection.RTDBAbbrev);
        }

        private void KDD_Option_FormClosing(object sender, FormClosingEventArgs e)
        {            
            GTS.dak = dac;
            m_parent.podkluchenie_OIK();
           
        }            
    }
}