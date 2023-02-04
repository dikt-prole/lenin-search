namespace BookProject.WinForms.Controls.BlockDetails
{
    partial class TitleBlockDetailsControl
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
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.text_tb = new System.Windows.Forms.TextBox();
            this.recognizeText_btn = new System.Windows.Forms.Button();
            this.toUpper_btn = new System.Windows.Forms.Button();
            this.toLower_btn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 7;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Controls.Add(this.numericUpDown1, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.text_tb, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.recognizeText_btn, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.toUpper_btn, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.toLower_btn, 3, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(699, 251);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDown1.Location = new System.Drawing.Point(652, 3);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(44, 23);
            this.numericUpDown1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Right;
            this.label2.Location = new System.Drawing.Point(609, 5);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "Level:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Text:";
            // 
            // text_tb
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.text_tb, 7);
            this.text_tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.text_tb.Location = new System.Drawing.Point(3, 31);
            this.text_tb.Multiline = true;
            this.text_tb.Name = "text_tb";
            this.text_tb.Size = new System.Drawing.Size(693, 189);
            this.text_tb.TabIndex = 1;
            // 
            // recognizeText_btn
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.recognizeText_btn, 2);
            this.recognizeText_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recognizeText_btn.Location = new System.Drawing.Point(1, 224);
            this.recognizeText_btn.Margin = new System.Windows.Forms.Padding(1);
            this.recognizeText_btn.Name = "recognizeText_btn";
            this.recognizeText_btn.Size = new System.Drawing.Size(98, 26);
            this.recognizeText_btn.TabIndex = 4;
            this.recognizeText_btn.Text = "Recognize Text";
            this.recognizeText_btn.UseVisualStyleBackColor = true;
            // 
            // toUpper_btn
            // 
            this.toUpper_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toUpper_btn.Location = new System.Drawing.Point(101, 224);
            this.toUpper_btn.Margin = new System.Windows.Forms.Padding(1);
            this.toUpper_btn.Name = "toUpper_btn";
            this.toUpper_btn.Size = new System.Drawing.Size(98, 26);
            this.toUpper_btn.TabIndex = 5;
            this.toUpper_btn.Text = "To Upper";
            this.toUpper_btn.UseVisualStyleBackColor = true;
            // 
            // toLower_btn
            // 
            this.toLower_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toLower_btn.Location = new System.Drawing.Point(201, 224);
            this.toLower_btn.Margin = new System.Windows.Forms.Padding(1);
            this.toLower_btn.Name = "toLower_btn";
            this.toLower_btn.Size = new System.Drawing.Size(98, 26);
            this.toLower_btn.TabIndex = 6;
            this.toLower_btn.Text = "To Lower";
            this.toLower_btn.UseVisualStyleBackColor = true;
            // 
            // TitleBlockDetailsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TitleBlockDetailsControl";
            this.Size = new System.Drawing.Size(699, 251);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox text_tb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button recognizeText_btn;
        private System.Windows.Forms.Button toUpper_btn;
        private System.Windows.Forms.Button toLower_btn;
    }
}
