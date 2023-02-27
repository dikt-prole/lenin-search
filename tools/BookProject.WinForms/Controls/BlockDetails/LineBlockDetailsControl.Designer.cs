namespace BookProject.WinForms.Controls.BlockDetails
{
    partial class LineBlockDetailsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.first_chb = new System.Windows.Forms.CheckBox();
            this.original_tb = new System.Windows.Forms.TextBox();
            this.original_rb = new System.Windows.Forms.RadioButton();
            this.replace_rb = new System.Windows.Forms.RadioButton();
            this.replace_tb = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.first_chb, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.original_tb, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.original_rb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.replace_rb, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.replace_tb, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(557, 150);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // first_chb
            // 
            this.first_chb.AutoSize = true;
            this.first_chb.Dock = System.Windows.Forms.DockStyle.Right;
            this.first_chb.Location = new System.Drawing.Point(506, 3);
            this.first_chb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.first_chb.Name = "first_chb";
            this.first_chb.Size = new System.Drawing.Size(48, 22);
            this.first_chb.TabIndex = 1;
            this.first_chb.Text = "First";
            this.first_chb.UseVisualStyleBackColor = true;
            // 
            // original_tb
            // 
            this.original_tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel1.SetColumnSpan(this.original_tb, 3);
            this.original_tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.original_tb.Location = new System.Drawing.Point(0, 28);
            this.original_tb.Margin = new System.Windows.Forms.Padding(0);
            this.original_tb.Multiline = true;
            this.original_tb.Name = "original_tb";
            this.original_tb.ReadOnly = true;
            this.original_tb.Size = new System.Drawing.Size(557, 47);
            this.original_tb.TabIndex = 2;
            // 
            // original_rb
            // 
            this.original_rb.AutoSize = true;
            this.original_rb.Checked = true;
            this.original_rb.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.original_rb.Location = new System.Drawing.Point(5, 6);
            this.original_rb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.original_rb.Name = "original_rb";
            this.original_rb.Size = new System.Drawing.Size(92, 19);
            this.original_rb.TabIndex = 3;
            this.original_rb.TabStop = true;
            this.original_rb.Text = "Original:";
            this.original_rb.UseVisualStyleBackColor = true;
            // 
            // replace_rb
            // 
            this.replace_rb.AutoSize = true;
            this.replace_rb.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.replace_rb.Location = new System.Drawing.Point(5, 81);
            this.replace_rb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.replace_rb.Name = "replace_rb";
            this.replace_rb.Size = new System.Drawing.Size(92, 19);
            this.replace_rb.TabIndex = 4;
            this.replace_rb.Text = "Replace:";
            this.replace_rb.UseVisualStyleBackColor = true;
            // 
            // replace_tb
            // 
            this.replace_tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel1.SetColumnSpan(this.replace_tb, 3);
            this.replace_tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.replace_tb.Location = new System.Drawing.Point(1, 104);
            this.replace_tb.Margin = new System.Windows.Forms.Padding(1);
            this.replace_tb.Multiline = true;
            this.replace_tb.Name = "replace_tb";
            this.replace_tb.Size = new System.Drawing.Size(555, 45);
            this.replace_tb.TabIndex = 5;
            // 
            // LineBlockDetailsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LineBlockDetailsControl";
            this.Size = new System.Drawing.Size(557, 150);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox first_chb;
        private System.Windows.Forms.TextBox original_tb;
        private System.Windows.Forms.RadioButton original_rb;
        private System.Windows.Forms.RadioButton replace_rb;
        private System.Windows.Forms.TextBox replace_tb;
    }
}
