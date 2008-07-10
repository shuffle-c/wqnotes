/*
* Copyright (c) 2007-2008 wqNotes Project
* License: BSD
* Windows: SaveForm.cs, revision $Revision$
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
    public partial class SaveForm : Form
    {
        /// <summary>
        /// Declaration
        /// </summary>
        public Dictionary<Int32, String> wqInput = new Dictionary<Int32, String>();
        public List<Int32> wqResult = new List<Int32>();

        public SaveForm()
        {
            InitializeComponent();
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            Int32[] col = new Int32[wqInput.Keys.Count];
            wqInput.Keys.CopyTo(col, 0);
            foreach (Int32 u in col)
            {
                wqInput[u] = "/" + System.Text.RegularExpressions.Regex.Replace
                    (wqInput[u], @" \([0-9]+\)\/", "/");
                Int32 f = listBox1.Items.Add(wqInput[u]);
                listBox1.SetSelected(f, true);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            wqResult.Clear();
            foreach (Int32 u in wqInput.Keys)
            {
                if (!listBox1.SelectedItems.Contains(wqInput[u]))
                    wqResult.Add(u);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            wqResult.Clear();
        }
    }
}