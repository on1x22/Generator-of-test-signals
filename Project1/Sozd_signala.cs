using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace Project1
{
    public partial class Sozd_signala : Form
    {
        private GTS m_parent;
        public static string dokx;
        public static string[] Masti = new string[1];
        public static int size;
        public bool changed;
        public List<Signal> SignalList = new List<Signal>();
        object sig;
        
        public Sozd_signala(GTS frm1, bool ParamChanged)
        {
            InitializeComponent();
            m_parent = frm1;
        }

        private void Sozd_signala_Load(object sender, EventArgs e)
        {
            sig = new Signal();
            propertyGrid1.SelectedObject = sig;
        }
        
        
        private void Sozd_signala_FormClosing(object sender, FormClosingEventArgs e)
        {           
            m_parent.Refresh1();
        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            Kod_dostovernosti kott = new Kod_dostovernosti();
            kott.Show();
        }

        // Добавление сигнала
        private void button5_Click(object sender, EventArgs e)
        {
            
            m_parent.SignalList.Add((Signal)sig);
            m_parent.SignalList.Sort();
            m_parent.ParamChanged = true;
            m_parent.Refresh1();
            this.Close();
        }

        // Очистить
        private void button6_Click(object sender, EventArgs e)
        {
            sig = new Signal();
            propertyGrid1.SelectedObject = sig;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Kod_dostovernosti kott = new Kod_dostovernosti();
            kott.Show();
        }

        

















    }
}
