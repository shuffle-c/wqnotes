﻿namespace wqNotes
{
    partial class OptionsForm
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
           this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
           this.button1 = new System.Windows.Forms.Button();
           this.button2 = new System.Windows.Forms.Button();
           this.SuspendLayout();
           // 
           // propertyGrid1
           // 
           this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                       | System.Windows.Forms.AnchorStyles.Left)
                       | System.Windows.Forms.AnchorStyles.Right)));
           this.propertyGrid1.Font = new System.Drawing.Font("Tahoma", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
           this.propertyGrid1.HelpBackColor = System.Drawing.SystemColors.Info;
           this.propertyGrid1.Location = new System.Drawing.Point(12, 12);
           this.propertyGrid1.Name = "propertyGrid1";
           this.propertyGrid1.Size = new System.Drawing.Size(458, 345);
           this.propertyGrid1.TabIndex = 1;
           this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
           // 
           // button1
           // 
           this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
           this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
           this.button1.Location = new System.Drawing.Point(395, 363);
           this.button1.Name = "button1";
           this.button1.Size = new System.Drawing.Size(75, 23);
           this.button1.TabIndex = 3;
           this.button1.Text = "Отмена";
           this.button1.UseVisualStyleBackColor = true;
           // 
           // button2
           // 
           this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
           this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
           this.button2.Location = new System.Drawing.Point(314, 363);
           this.button2.Name = "button2";
           this.button2.Size = new System.Drawing.Size(75, 23);
           this.button2.TabIndex = 2;
           this.button2.Text = "OK";
           this.button2.UseVisualStyleBackColor = true;
           this.button2.Click += new System.EventHandler(this.button2_Click);
           // 
           // OptionsForm
           // 
           this.AcceptButton = this.button2;
           this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
           this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
           this.CancelButton = this.button1;
           this.ClientSize = new System.Drawing.Size(482, 398);
           this.Controls.Add(this.button2);
           this.Controls.Add(this.button1);
           this.Controls.Add(this.propertyGrid1);
           this.MinimizeBox = false;
           this.Name = "OptionsForm";
           this.ShowIcon = false;
           this.ShowInTaskbar = false;
           this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
           this.Text = "Настройки";
           this.Load += new System.EventHandler(this.OptionsForm_Load);
           this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}