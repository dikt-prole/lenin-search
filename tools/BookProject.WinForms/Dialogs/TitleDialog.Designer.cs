namespace BookProject.WinForms.Dialogs
{
    partial class TitleDialog
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
            this.ocr_btn = new System.Windows.Forms.Button();
            this.upper_btn = new System.Windows.Forms.Button();
            this.lower_btn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.level_nud = new System.Windows.Forms.NumericUpDown();
            this.ok_btn = new System.Windows.Forms.Button();
            this.text_tb = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.level_nud)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.Controls.Add(this.ocr_btn, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.upper_btn, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lower_btn, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.level_nud, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.ok_btn, 5, 2);
            this.tableLayoutPanel1.Controls.Add(this.text_tb, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(434, 261);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // ocr_btn
            // 
            this.ocr_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ocr_btn.Location = new System.Drawing.Point(1, 1);
            this.ocr_btn.Margin = new System.Windows.Forms.Padding(1);
            this.ocr_btn.Name = "ocr_btn";
            this.ocr_btn.Size = new System.Drawing.Size(73, 26);
            this.ocr_btn.TabIndex = 0;
            this.ocr_btn.Text = "Ocr";
            this.ocr_btn.UseVisualStyleBackColor = true;
            // 
            // upper_btn
            // 
            this.upper_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.upper_btn.Location = new System.Drawing.Point(76, 1);
            this.upper_btn.Margin = new System.Windows.Forms.Padding(1);
            this.upper_btn.Name = "upper_btn";
            this.upper_btn.Size = new System.Drawing.Size(73, 26);
            this.upper_btn.TabIndex = 1;
            this.upper_btn.Text = "Upper";
            this.upper_btn.UseVisualStyleBackColor = true;
            // 
            // lower_btn
            // 
            this.lower_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lower_btn.Location = new System.Drawing.Point(151, 1);
            this.lower_btn.Margin = new System.Windows.Forms.Padding(1);
            this.lower_btn.Name = "lower_btn";
            this.lower_btn.Size = new System.Drawing.Size(73, 26);
            this.lower_btn.TabIndex = 2;
            this.lower_btn.Text = "Lower";
            this.lower_btn.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(228, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Level:";
            // 
            // level_nud
            // 
            this.level_nud.Dock = System.Windows.Forms.DockStyle.Left;
            this.level_nud.Location = new System.Drawing.Point(273, 3);
            this.level_nud.Name = "level_nud";
            this.level_nud.Size = new System.Drawing.Size(50, 23);
            this.level_nud.TabIndex = 4;
            // 
            // ok_btn
            // 
            this.ok_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ok_btn.Location = new System.Drawing.Point(360, 234);
            this.ok_btn.Margin = new System.Windows.Forms.Padding(1);
            this.ok_btn.Name = "ok_btn";
            this.ok_btn.Size = new System.Drawing.Size(73, 26);
            this.ok_btn.TabIndex = 5;
            this.ok_btn.Text = "OK";
            this.ok_btn.UseVisualStyleBackColor = true;
            // 
            // text_tb
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.text_tb, 6);
            this.text_tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.text_tb.Location = new System.Drawing.Point(1, 29);
            this.text_tb.Margin = new System.Windows.Forms.Padding(1);
            this.text_tb.Multiline = true;
            this.text_tb.Name = "text_tb";
            this.text_tb.Size = new System.Drawing.Size(432, 203);
            this.text_tb.TabIndex = 6;
            // 
            // TitleDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 261);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TitleDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Title";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.level_nud)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button ocr_btn;
        private System.Windows.Forms.Button upper_btn;
        private System.Windows.Forms.Button lower_btn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown level_nud;
        private System.Windows.Forms.Button ok_btn;
        private System.Windows.Forms.TextBox text_tb;
    }
}