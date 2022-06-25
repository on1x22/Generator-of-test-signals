using System.Windows.Forms;

namespace Project1
{
    public partial class Log : Form
    {
        private GTS n_parent;
        public Log(GTS frm1, string bof)
        {
            InitializeComponent();
            n_parent = frm1;
            richTextBox1.Text = bof;
        }
    }
}
