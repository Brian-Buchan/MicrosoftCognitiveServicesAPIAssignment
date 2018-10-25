using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// 3rd party NuGet packages
using JsonFormatterPlus;
using CommandLine;

namespace MCSAPI
{
    public partial class Form1 : Form
    {
        string LUIS_base_Querey = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/cd59a1de-7d6e-4a3a-9b17-a4c6ffd36056?subscription-key=533fe2634b0f42f68c996c0af991c9f8&timezoneOffset=-360&q=";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                Do_LUIS_Querey(textBox1.Text);
            }
        }

        private void Do_LUIS_Querey(string querey)
        {
            textBox1.Text = LUIS_base_Querey += querey;
        }

        private void add_Control(Control control)
        {
            Controls.Add(control);
        }

        private void delete_Contol(Control control)
        {
            Controls.Remove(control);
        }

        private void focus_Control(Control control)
        {
            control.Focus();
        }
    }
}
