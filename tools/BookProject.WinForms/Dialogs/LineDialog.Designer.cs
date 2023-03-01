namespace BookProject.WinForms.Dialogs
{
    partial class LineDialog
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.replace_tb = new System.Windows.Forms.TextBox();
            this.original_rb = new System.Windows.Forms.RadioButton();
            this.replace_rb = new System.Windows.Forms.RadioButton();
            this.first_chb = new System.Windows.Forms.CheckBox();
            this.ok_btn = new System.Windows.Forms.Button();
            this.original_tb = new System.Windows.Forms.TextBox();
            this.replace_btn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.Controls.Add(this.replace_tb, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.original_rb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.replace_rb, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.first_chb, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.ok_btn, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.original_tb, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.replace_btn, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(434, 261);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // replace_tb
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.replace_tb, 4);
            this.replace_tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replace_tb.Location = new System.Drawing.Point(1, 145);
            this.replace_tb.Margin = new System.Windows.Forms.Padding(1);
            this.replace_tb.Multiline = true;
            this.replace_tb.Name = "replace_tb";
            this.replace_tb.Size = new System.Drawing.Size(432, 86);
            this.replace_tb.TabIndex = 5;
            // 
            // original_rb
            // 
            this.original_rb.AutoSize = true;
            this.original_rb.Checked = true;
            this.original_rb.Dock = System.Windows.Forms.DockStyle.Top;
            this.original_rb.Location = new System.Drawing.Point(5, 3);
            this.original_rb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.original_rb.Name = "original_rb";
            this.original_rb.Size = new System.Drawing.Size(67, 19);
            this.original_rb.TabIndex = 0;
            this.original_rb.TabStop = true;
            this.original_rb.Text = "Original";
            this.original_rb.UseVisualStyleBackColor = true;
            // 
            // replace_rb
            // 
            this.replace_rb.AutoSize = true;
            this.replace_rb.Dock = System.Windows.Forms.DockStyle.Top;
            this.replace_rb.Location = new System.Drawing.Point(5, 119);
            this.replace_rb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.replace_rb.Name = "replace_rb";
            this.replace_rb.Size = new System.Drawing.Size(67, 19);
            this.replace_rb.TabIndex = 1;
            this.replace_rb.Text = "Replace";
            this.replace_rb.UseVisualStyleBackColor = true;
            // 
            // first_chb
            // 
            this.first_chb.AutoSize = true;
            this.first_chb.Dock = System.Windows.Forms.DockStyle.Top;
            this.first_chb.Location = new System.Drawing.Point(364, 3);
            this.first_chb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.first_chb.Name = "first_chb";
            this.first_chb.Size = new System.Drawing.Size(67, 19);
            this.first_chb.TabIndex = 2;
            this.first_chb.Text = "First";
            this.first_chb.UseVisualStyleBackColor = true;
            // 
            // ok_btn
            // 
            this.ok_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ok_btn.Location = new System.Drawing.Point(360, 233);
            this.ok_btn.Margin = new System.Windows.Forms.Padding(1);
            this.ok_btn.Name = "ok_btn";
            this.ok_btn.Size = new System.Drawing.Size(73, 27);
            this.ok_btn.TabIndex = 3;
            this.ok_btn.Text = "OK";
            this.ok_btn.UseVisualStyleBackColor = true;
            // 
            // original_tb
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.original_tb, 4);
            this.original_tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.original_tb.Location = new System.Drawing.Point(1, 29);
            this.original_tb.Margin = new System.Windows.Forms.Padding(1);
            this.original_tb.Multiline = true;
            this.original_tb.Name = "original_tb";
            this.original_tb.ReadOnly = true;
            this.original_tb.Size = new System.Drawing.Size(432, 86);
            this.original_tb.TabIndex = 4;
            // 
            // replace_btn
            // 
            this.replace_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replace_btn.Location = new System.Drawing.Point(76, 117);
            this.replace_btn.Margin = new System.Windows.Forms.Padding(1);
            this.replace_btn.Name = "replace_btn";
            this.replace_btn.Size = new System.Drawing.Size(73, 26);
            this.replace_btn.TabIndex = 6;
            this.replace_btn.Text = "Replace";
            this.replace_btn.UseVisualStyleBackColor = true;
            // 
            // LineDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 261);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LineDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Line";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RadioButton original_rb;
        private System.Windows.Forms.RadioButton replace_rb;
        private System.Windows.Forms.CheckBox first_chb;
        private System.Windows.Forms.Button ok_btn;
        private System.Windows.Forms.TextBox original_tb;
        private System.Windows.Forms.TextBox replace_tb;
        private System.Windows.Forms.Button replace_btn;
    }
}