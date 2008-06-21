using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace wqNotes
{
    public partial class AuthForm : Form
    {
        public string wqResult;
        public AuthForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            wqResult = textBox1.Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox1.UseSystemPasswordChar = false;
                textBox2.Visible = false;
            }
            else
            {
                textBox1.UseSystemPasswordChar = true;
                textBox2.Visible = true;
            }
        }

        private void Form5_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!checkBox1.Checked && textBox2.Text != textBox1.Text)
            {
                MessageBox.Show("Повторный пароль введен неверно.");
                e.Cancel = true;
            }
        }
    }
}