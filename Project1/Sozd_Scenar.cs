using System;
using System.Windows.Forms;

namespace Project1
{
    public partial class Sozd_Scenar : Form
    {
        private GTS m_parent;

        public Sozd_Scenar(GTS frm1, string ScriptNameXml)
        {
            InitializeComponent();
            m_parent = frm1;
        }
        
        private void Sozd_Scenar_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        object varvar;

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Klassi();
        }

        // Добавление пункта в сценарий
        private void button1_Click(object sender, EventArgs e)
        {
            if (m_parent.v_nachalo == true && m_parent.posle == false)
            {
                if (m_parent.ScriptList.Count == 0)
                {
                    m_parent.ScriptList.Add((ICommand)varvar);
                }
                else
                {
                    m_parent.ScriptList.Insert(0, (ICommand)varvar);
                }
            }
            else if (m_parent.v_nachalo == false && m_parent.posle == true)
            {
                int jjj = m_parent.select_script_index;
                m_parent.ScriptList.Insert(jjj + 1, (ICommand)varvar);
            }

            m_parent.Refresh2();
            this.Close();
        }

        // Очистка полей
        private void button2_Click(object sender, EventArgs e)
        {
            Klassi();
        }

        // Метод. Открытия классов
        public void Klassi()
        {
            int aaa = comboBox1.SelectedIndex;
            switch (aaa)
            {
                case 0:
                    varvar = new Pause();
                    propertyGrid1.SelectedObject = varvar;
                    break;
                case 1:
                    varvar = new Assign();
                    propertyGrid1.SelectedObject = varvar;
                    break;
                case 2:
                    varvar = new Summation();
                    propertyGrid1.SelectedObject = varvar;
                    break;
                case 3:
                    varvar = new Subtraction();
                    propertyGrid1.SelectedObject = varvar;
                    break;
                case 4:
                    varvar = new Multiplication();
                    propertyGrid1.SelectedObject = varvar;
                    break;
                case 5:
                    varvar = new Division();
                    propertyGrid1.SelectedObject = varvar;
                    break;
                case 6:
                    varvar = new Square();
                    propertyGrid1.SelectedObject = varvar;
                    break;
                case 7:
                    varvar = new Sinusoid();
                    propertyGrid1.SelectedObject = varvar;
                    break;
                case 8:
                    varvar = new Triangle();
                    propertyGrid1.SelectedObject = varvar;
                    break;
                case 9:
                    varvar = new Saw();
                    propertyGrid1.SelectedObject = varvar;
                    break;
            }
        }
    }
}
