namespace wqNotes
{
    partial class Form4
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
           this.checkBox10 = new System.Windows.Forms.CheckBox();
           this.checkBox9 = new System.Windows.Forms.CheckBox();
           this.checkBox8 = new System.Windows.Forms.CheckBox();
           this.button1 = new System.Windows.Forms.Button();
           this.groupBox2 = new System.Windows.Forms.GroupBox();
           this.comboBox4 = new System.Windows.Forms.ComboBox();
           this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
           this.comboBox3 = new System.Windows.Forms.ComboBox();
           this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
           this.label8 = new System.Windows.Forms.Label();
           this.label7 = new System.Windows.Forms.Label();
           this.checkBox7 = new System.Windows.Forms.CheckBox();
           this.checkBox5 = new System.Windows.Forms.CheckBox();
           this.groupBox3 = new System.Windows.Forms.GroupBox();
           this.comboBox2 = new System.Windows.Forms.ComboBox();
           this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
           this.label4 = new System.Windows.Forms.Label();
           this.checkBox6 = new System.Windows.Forms.CheckBox();
           this.checkBox4 = new System.Windows.Forms.CheckBox();
           this.checkBox3 = new System.Windows.Forms.CheckBox();
           this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
           this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
           this.label5 = new System.Windows.Forms.Label();
           this.label9 = new System.Windows.Forms.Label();
           this.checkBox2 = new System.Windows.Forms.CheckBox();
           this.checkBox1 = new System.Windows.Forms.CheckBox();
           this.checkBox11 = new System.Windows.Forms.CheckBox();
           this.comboBox5 = new System.Windows.Forms.ComboBox();
           this.linkLabel1 = new System.Windows.Forms.LinkLabel();
           this.checkBox12 = new System.Windows.Forms.CheckBox();
           this.checkBox13 = new System.Windows.Forms.CheckBox();
           this.button2 = new System.Windows.Forms.Button();
           this.groupBox2.SuspendLayout();
           ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
           ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
           this.groupBox3.SuspendLayout();
           ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
           this.SuspendLayout();
           // 
           // checkBox10
           // 
           this.checkBox10.AutoSize = true;
           this.checkBox10.Checked = true;
           this.checkBox10.CheckState = System.Windows.Forms.CheckState.Checked;
           this.checkBox10.Location = new System.Drawing.Point(12, 91);
           this.checkBox10.Name = "checkBox10";
           this.checkBox10.Size = new System.Drawing.Size(118, 17);
           this.checkBox10.TabIndex = 15;
           this.checkBox10.Text = "Названия заметок";
           this.checkBox10.UseVisualStyleBackColor = true;
           // 
           // checkBox9
           // 
           this.checkBox9.AutoSize = true;
           this.checkBox9.Checked = true;
           this.checkBox9.CheckState = System.Windows.Forms.CheckState.Checked;
           this.checkBox9.Location = new System.Drawing.Point(12, 114);
           this.checkBox9.Name = "checkBox9";
           this.checkBox9.Size = new System.Drawing.Size(99, 17);
           this.checkBox9.TabIndex = 14;
           this.checkBox9.Text = "Текст заметок";
           this.checkBox9.UseVisualStyleBackColor = true;
           // 
           // checkBox8
           // 
           this.checkBox8.AutoSize = true;
           this.checkBox8.Checked = true;
           this.checkBox8.CheckState = System.Windows.Forms.CheckState.Checked;
           this.checkBox8.Location = new System.Drawing.Point(12, 70);
           this.checkBox8.Name = "checkBox8";
           this.checkBox8.Size = new System.Drawing.Size(107, 17);
           this.checkBox8.TabIndex = 13;
           this.checkBox8.Text = "Названия папок";
           this.checkBox8.UseVisualStyleBackColor = true;
           // 
           // button1
           // 
           this.button1.Location = new System.Drawing.Point(233, 12);
           this.button1.Name = "button1";
           this.button1.Size = new System.Drawing.Size(75, 23);
           this.button1.TabIndex = 11;
           this.button1.Text = "Искать";
           this.button1.UseVisualStyleBackColor = true;
           this.button1.Click += new System.EventHandler(this.button1_Click);
           // 
           // groupBox2
           // 
           this.groupBox2.Controls.Add(this.comboBox4);
           this.groupBox2.Controls.Add(this.numericUpDown3);
           this.groupBox2.Controls.Add(this.comboBox3);
           this.groupBox2.Controls.Add(this.numericUpDown2);
           this.groupBox2.Controls.Add(this.label8);
           this.groupBox2.Controls.Add(this.label7);
           this.groupBox2.Controls.Add(this.checkBox7);
           this.groupBox2.Controls.Add(this.checkBox5);
           this.groupBox2.Location = new System.Drawing.Point(12, 270);
           this.groupBox2.Name = "groupBox2";
           this.groupBox2.Size = new System.Drawing.Size(215, 78);
           this.groupBox2.TabIndex = 16;
           this.groupBox2.TabStop = false;
           this.groupBox2.Text = "Поиск по размеру";
           // 
           // comboBox4
           // 
           this.comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
           this.comboBox4.FormattingEnabled = true;
           this.comboBox4.Items.AddRange(new object[] {
            "байт",
            "Кбайт",
            "Мбайт"});
           this.comboBox4.Location = new System.Drawing.Point(118, 44);
           this.comboBox4.Name = "comboBox4";
           this.comboBox4.Size = new System.Drawing.Size(82, 21);
           this.comboBox4.TabIndex = 44;
           this.comboBox4.SelectedIndexChanged += new System.EventHandler(this.comboBox4_SelectedIndexChanged);
           // 
           // numericUpDown3
           // 
           this.numericUpDown3.Location = new System.Drawing.Point(67, 44);
           this.numericUpDown3.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
           this.numericUpDown3.Name = "numericUpDown3";
           this.numericUpDown3.Size = new System.Drawing.Size(46, 21);
           this.numericUpDown3.TabIndex = 43;
           this.numericUpDown3.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
           this.numericUpDown3.ValueChanged += new System.EventHandler(this.numericUpDown3_ValueChanged);
           // 
           // comboBox3
           // 
           this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
           this.comboBox3.FormattingEnabled = true;
           this.comboBox3.Items.AddRange(new object[] {
            "байт",
            "Кбайт",
            "Мбайт"});
           this.comboBox3.Location = new System.Drawing.Point(118, 17);
           this.comboBox3.Name = "comboBox3";
           this.comboBox3.Size = new System.Drawing.Size(82, 21);
           this.comboBox3.TabIndex = 42;
           this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
           // 
           // numericUpDown2
           // 
           this.numericUpDown2.Location = new System.Drawing.Point(67, 17);
           this.numericUpDown2.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
           this.numericUpDown2.Name = "numericUpDown2";
           this.numericUpDown2.Size = new System.Drawing.Size(46, 21);
           this.numericUpDown2.TabIndex = 41;
           this.numericUpDown2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
           this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
           // 
           // label8
           // 
           this.label8.AutoSize = true;
           this.label8.Location = new System.Drawing.Point(39, 47);
           this.label8.Name = "label8";
           this.label8.Size = new System.Drawing.Size(20, 13);
           this.label8.TabIndex = 40;
           this.label8.Text = "до";
           // 
           // label7
           // 
           this.label7.AutoSize = true;
           this.label7.Location = new System.Drawing.Point(40, 20);
           this.label7.Name = "label7";
           this.label7.Size = new System.Drawing.Size(19, 13);
           this.label7.TabIndex = 39;
           this.label7.Text = "от";
           // 
           // checkBox7
           // 
           this.checkBox7.AutoSize = true;
           this.checkBox7.Location = new System.Drawing.Point(20, 47);
           this.checkBox7.Name = "checkBox7";
           this.checkBox7.Size = new System.Drawing.Size(15, 14);
           this.checkBox7.TabIndex = 38;
           this.checkBox7.UseVisualStyleBackColor = true;
           // 
           // checkBox5
           // 
           this.checkBox5.AutoSize = true;
           this.checkBox5.Location = new System.Drawing.Point(21, 20);
           this.checkBox5.Name = "checkBox5";
           this.checkBox5.Size = new System.Drawing.Size(15, 14);
           this.checkBox5.TabIndex = 37;
           this.checkBox5.UseVisualStyleBackColor = true;
           // 
           // groupBox3
           // 
           this.groupBox3.Controls.Add(this.comboBox2);
           this.groupBox3.Controls.Add(this.numericUpDown1);
           this.groupBox3.Controls.Add(this.label4);
           this.groupBox3.Controls.Add(this.checkBox6);
           this.groupBox3.Controls.Add(this.checkBox4);
           this.groupBox3.Controls.Add(this.checkBox3);
           this.groupBox3.Controls.Add(this.dateTimePicker2);
           this.groupBox3.Controls.Add(this.dateTimePicker1);
           this.groupBox3.Controls.Add(this.label5);
           this.groupBox3.Controls.Add(this.label9);
           this.groupBox3.Location = new System.Drawing.Point(12, 137);
           this.groupBox3.Name = "groupBox3";
           this.groupBox3.Size = new System.Drawing.Size(215, 127);
           this.groupBox3.TabIndex = 0;
           this.groupBox3.TabStop = false;
           this.groupBox3.Text = "Поиск по дате";
           // 
           // comboBox2
           // 
           this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
           this.comboBox2.FormattingEnabled = true;
           this.comboBox2.Items.AddRange(new object[] {
            "часов",
            "суток",
            "недель",
            "месяцев",
            "лет"});
           this.comboBox2.Location = new System.Drawing.Point(120, 94);
           this.comboBox2.Name = "comboBox2";
           this.comboBox2.Size = new System.Drawing.Size(82, 21);
           this.comboBox2.TabIndex = 42;
           this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
           // 
           // numericUpDown1
           // 
           this.numericUpDown1.Location = new System.Drawing.Point(69, 94);
           this.numericUpDown1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
           this.numericUpDown1.Name = "numericUpDown1";
           this.numericUpDown1.Size = new System.Drawing.Size(46, 21);
           this.numericUpDown1.TabIndex = 41;
           this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
           this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
           // 
           // label4
           // 
           this.label4.AutoSize = true;
           this.label4.Location = new System.Drawing.Point(44, 78);
           this.label4.Name = "label4";
           this.label4.Size = new System.Drawing.Size(80, 13);
           this.label4.TabIndex = 40;
           this.label4.Text = "не старше чем";
           // 
           // checkBox6
           // 
           this.checkBox6.AutoSize = true;
           this.checkBox6.Location = new System.Drawing.Point(23, 77);
           this.checkBox6.Name = "checkBox6";
           this.checkBox6.Size = new System.Drawing.Size(15, 14);
           this.checkBox6.TabIndex = 39;
           this.checkBox6.UseVisualStyleBackColor = true;
           this.checkBox6.CheckedChanged += new System.EventHandler(this.checkBox6_CheckedChanged);
           // 
           // checkBox4
           // 
           this.checkBox4.AutoSize = true;
           this.checkBox4.Location = new System.Drawing.Point(23, 48);
           this.checkBox4.Name = "checkBox4";
           this.checkBox4.Size = new System.Drawing.Size(15, 14);
           this.checkBox4.TabIndex = 38;
           this.checkBox4.UseVisualStyleBackColor = true;
           this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
           // 
           // checkBox3
           // 
           this.checkBox3.AutoSize = true;
           this.checkBox3.Location = new System.Drawing.Point(23, 20);
           this.checkBox3.Name = "checkBox3";
           this.checkBox3.Size = new System.Drawing.Size(15, 14);
           this.checkBox3.TabIndex = 37;
           this.checkBox3.UseVisualStyleBackColor = true;
           this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
           // 
           // dateTimePicker2
           // 
           this.dateTimePicker2.Location = new System.Drawing.Point(69, 44);
           this.dateTimePicker2.Name = "dateTimePicker2";
           this.dateTimePicker2.Size = new System.Drawing.Size(133, 21);
           this.dateTimePicker2.TabIndex = 36;
           this.dateTimePicker2.ValueChanged += new System.EventHandler(this.dateTimePicker2_ValueChanged);
           // 
           // dateTimePicker1
           // 
           this.dateTimePicker1.Location = new System.Drawing.Point(69, 17);
           this.dateTimePicker1.Name = "dateTimePicker1";
           this.dateTimePicker1.Size = new System.Drawing.Size(133, 21);
           this.dateTimePicker1.TabIndex = 35;
           this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
           // 
           // label5
           // 
           this.label5.AutoSize = true;
           this.label5.Location = new System.Drawing.Point(43, 48);
           this.label5.Name = "label5";
           this.label5.Size = new System.Drawing.Size(20, 13);
           this.label5.TabIndex = 34;
           this.label5.Text = "до";
           // 
           // label9
           // 
           this.label9.AutoSize = true;
           this.label9.Location = new System.Drawing.Point(44, 21);
           this.label9.Name = "label9";
           this.label9.Size = new System.Drawing.Size(19, 13);
           this.label9.TabIndex = 33;
           this.label9.Text = "от";
           // 
           // checkBox2
           // 
           this.checkBox2.AutoSize = true;
           this.checkBox2.Location = new System.Drawing.Point(136, 91);
           this.checkBox2.Name = "checkBox2";
           this.checkBox2.Size = new System.Drawing.Size(66, 17);
           this.checkBox2.TabIndex = 40;
           this.checkBox2.Text = "Регистр";
           this.checkBox2.UseVisualStyleBackColor = true;
           this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
           // 
           // checkBox1
           // 
           this.checkBox1.AutoSize = true;
           this.checkBox1.Location = new System.Drawing.Point(136, 68);
           this.checkBox1.Name = "checkBox1";
           this.checkBox1.Size = new System.Drawing.Size(90, 17);
           this.checkBox1.TabIndex = 38;
           this.checkBox1.Text = "Целое слово";
           this.checkBox1.UseVisualStyleBackColor = true;
           this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
           // 
           // checkBox11
           // 
           this.checkBox11.AutoSize = true;
           this.checkBox11.Location = new System.Drawing.Point(136, 114);
           this.checkBox11.Name = "checkBox11";
           this.checkBox11.Size = new System.Drawing.Size(65, 17);
           this.checkBox11.TabIndex = 42;
           this.checkBox11.Text = "Регэксп";
           this.checkBox11.UseVisualStyleBackColor = true;
           this.checkBox11.CheckedChanged += new System.EventHandler(this.checkBox11_CheckedChanged);
           // 
           // comboBox5
           // 
           this.comboBox5.FormattingEnabled = true;
           this.comboBox5.Location = new System.Drawing.Point(12, 12);
           this.comboBox5.Name = "comboBox5";
           this.comboBox5.Size = new System.Drawing.Size(215, 21);
           this.comboBox5.TabIndex = 43;
           // 
           // linkLabel1
           // 
           this.linkLabel1.AutoSize = true;
           this.linkLabel1.Location = new System.Drawing.Point(207, 115);
           this.linkLabel1.Name = "linkLabel1";
           this.linkLabel1.Size = new System.Drawing.Size(20, 13);
           this.linkLabel1.TabIndex = 44;
           this.linkLabel1.TabStop = true;
           this.linkLabel1.Text = "(?)";
           // 
           // checkBox12
           // 
           this.checkBox12.AutoSize = true;
           this.checkBox12.Location = new System.Drawing.Point(12, 70);
           this.checkBox12.Name = "checkBox12";
           this.checkBox12.Size = new System.Drawing.Size(89, 17);
           this.checkBox12.TabIndex = 45;
           this.checkBox12.Text = "Заменить на";
           this.checkBox12.UseVisualStyleBackColor = true;
           this.checkBox12.Visible = false;
           // 
           // checkBox13
           // 
           this.checkBox13.AutoSize = true;
           this.checkBox13.Location = new System.Drawing.Point(12, 43);
           this.checkBox13.Name = "checkBox13";
           this.checkBox13.Size = new System.Drawing.Size(204, 17);
           this.checkBox13.TabIndex = 47;
           this.checkBox13.Text = "Искать НЕ содержащие этот текст";
           this.checkBox13.UseVisualStyleBackColor = true;
           this.checkBox13.CheckedChanged += new System.EventHandler(this.checkBox13_CheckedChanged);
           // 
           // button2
           // 
           this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
           this.button2.Location = new System.Drawing.Point(233, 41);
           this.button2.Name = "button2";
           this.button2.Size = new System.Drawing.Size(75, 23);
           this.button2.TabIndex = 48;
           this.button2.Text = "Отмена";
           this.button2.UseVisualStyleBackColor = true;
           this.button2.Click += new System.EventHandler(this.button2_Click);
           // 
           // Form4
           // 
           this.AcceptButton = this.button1;
           this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
           this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
           this.CancelButton = this.button2;
           this.ClientSize = new System.Drawing.Size(319, 357);
           this.Controls.Add(this.checkBox8);
           this.Controls.Add(this.checkBox1);
           this.Controls.Add(this.button2);
           this.Controls.Add(this.checkBox13);
           this.Controls.Add(this.checkBox12);
           this.Controls.Add(this.linkLabel1);
           this.Controls.Add(this.comboBox5);
           this.Controls.Add(this.checkBox11);
           this.Controls.Add(this.checkBox2);
           this.Controls.Add(this.groupBox3);
           this.Controls.Add(this.groupBox2);
           this.Controls.Add(this.checkBox10);
           this.Controls.Add(this.checkBox9);
           this.Controls.Add(this.button1);
           this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
           this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
           this.MaximizeBox = false;
           this.MinimizeBox = false;
           this.Name = "Form4";
           this.Text = "Поиск";
           this.groupBox2.ResumeLayout(false);
           this.groupBox2.PerformLayout();
           ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
           ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
           this.groupBox3.ResumeLayout(false);
           this.groupBox3.PerformLayout();
           ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
           this.ResumeLayout(false);
           this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBox7;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label5;
       private System.Windows.Forms.Label label9;
        private System.Windows.Forms.LinkLabel linkLabel1;
       private System.Windows.Forms.CheckBox checkBox12;
        private System.Windows.Forms.Button button2;
       public System.Windows.Forms.ComboBox comboBox5;
       public System.Windows.Forms.CheckBox checkBox10;
       public System.Windows.Forms.CheckBox checkBox9;
       public System.Windows.Forms.CheckBox checkBox8;
       public System.Windows.Forms.CheckBox checkBox2;
       public System.Windows.Forms.CheckBox checkBox1;
       public System.Windows.Forms.CheckBox checkBox11;
       public System.Windows.Forms.CheckBox checkBox13;
    }
}