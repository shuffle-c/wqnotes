/*
* Copyright (c) 2007-2008 wqNotes Project
* License: BSD
* Windows: AttachForm.cs, $Revision$
* URL: $HeadURL$
* $Date$
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace wqNotes
{
    public partial class AttachForm : Form
    {
        public string wqInput;
        public bool wqOut_IsLink;
        public DateTime wqOut_Dtc;
        public DateTime wqOut_Dtm;

        public AttachForm()
        {
            InitializeComponent();
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            textBox1.Text = wqInput;
            bool param = wqInput.EndsWith(".exe");
            pictureBox1.Image = Program.GetFileIcon(wqInput, param, false);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = !checkBox1.Checked;
            groupBox2.Enabled = !checkBox1.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(wqInput);
            wqOut_IsLink = checkBox1.Checked;
            if (radioButton1.Checked) wqOut_Dtc = fi.CreationTimeUtc;
            if (radioButton2.Checked) wqOut_Dtc = DateTime.Now;
            if (radioButton3.Checked) wqOut_Dtc = dateTimePicker1.Value;

            if (radioButton6.Checked) wqOut_Dtm = fi.LastWriteTimeUtc;
            if (radioButton5.Checked) wqOut_Dtm = DateTime.Now;
            if (radioButton4.Checked) wqOut_Dtm = dateTimePicker2.Value;
        }

        private void dateTimePicker1_Enter(object sender, EventArgs e)
        {
            radioButton3.Checked = true;
        }

        private void dateTimePicker2_Enter(object sender, EventArgs e)
        {
            radioButton4.Checked = true;
        }
    }
}