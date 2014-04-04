using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StephenCleary;

namespace CodeFormatter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBoxInput_Leave(object sender, EventArgs e)
        {
            textBoxOutput.Text = CSharpFormatter.CSharp(textBoxInput.Text.Replace('\xA0', ' ')).ToString();
        }
    }
}
