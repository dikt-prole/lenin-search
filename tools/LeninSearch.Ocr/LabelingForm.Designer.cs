
namespace LeninSearch.Ocr
{
    partial class LabelingForm
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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.csvFile_tb = new System.Windows.Forms.TextBox();
            this.load_btn = new System.Windows.Forms.Button();
            this.saveLabeled_btn = new System.Windows.Forms.Button();
            this.saveAll_btn = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ocrBlock_lb = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.garbage_rb = new System.Windows.Forms.RadioButton();
            this.comment_rb = new System.Windows.Forms.RadioButton();
            this.title_rb = new System.Windows.Forms.RadioButton();
            this.paragraph_rb = new System.Windows.Forms.RadioButton();
            this.none_rb = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(649, 585);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.Controls.Add(this.csvFile_tb, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.load_btn, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.saveLabeled_btn, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.saveAll_btn, 3, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(649, 28);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // csvFile_tb
            // 
            this.csvFile_tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.csvFile_tb.Location = new System.Drawing.Point(3, 3);
            this.csvFile_tb.Name = "csvFile_tb";
            this.csvFile_tb.ReadOnly = true;
            this.csvFile_tb.Size = new System.Drawing.Size(343, 23);
            this.csvFile_tb.TabIndex = 0;
            // 
            // load_btn
            // 
            this.load_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.load_btn.Location = new System.Drawing.Point(350, 1);
            this.load_btn.Margin = new System.Windows.Forms.Padding(1);
            this.load_btn.Name = "load_btn";
            this.load_btn.Size = new System.Drawing.Size(98, 26);
            this.load_btn.TabIndex = 1;
            this.load_btn.Text = "Load";
            this.load_btn.UseVisualStyleBackColor = true;
            // 
            // saveLabeled_btn
            // 
            this.saveLabeled_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.saveLabeled_btn.Location = new System.Drawing.Point(450, 1);
            this.saveLabeled_btn.Margin = new System.Windows.Forms.Padding(1);
            this.saveLabeled_btn.Name = "saveLabeled_btn";
            this.saveLabeled_btn.Size = new System.Drawing.Size(98, 26);
            this.saveLabeled_btn.TabIndex = 2;
            this.saveLabeled_btn.Text = "Save Labeled";
            this.saveLabeled_btn.UseVisualStyleBackColor = true;
            // 
            // saveAll_btn
            // 
            this.saveAll_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.saveAll_btn.Location = new System.Drawing.Point(550, 1);
            this.saveAll_btn.Margin = new System.Windows.Forms.Padding(1);
            this.saveAll_btn.Name = "saveAll_btn";
            this.saveAll_btn.Size = new System.Drawing.Size(98, 26);
            this.saveAll_btn.TabIndex = 3;
            this.saveAll_btn.Text = "Save All";
            this.saveAll_btn.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.pictureBox1, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.ocrBlock_lb, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel6, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 28);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(649, 557);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(283, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(363, 551);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // ocrBlock_lb
            // 
            this.ocrBlock_lb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ocrBlock_lb.FormattingEnabled = true;
            this.ocrBlock_lb.ItemHeight = 15;
            this.ocrBlock_lb.Location = new System.Drawing.Point(3, 3);
            this.ocrBlock_lb.Name = "ocrBlock_lb";
            this.ocrBlock_lb.Size = new System.Drawing.Size(174, 551);
            this.ocrBlock_lb.TabIndex = 1;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.Controls.Add(this.garbage_rb, 0, 4);
            this.tableLayoutPanel6.Controls.Add(this.comment_rb, 0, 3);
            this.tableLayoutPanel6.Controls.Add(this.title_rb, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.paragraph_rb, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.none_rb, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(180, 0);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 6;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(100, 557);
            this.tableLayoutPanel6.TabIndex = 2;
            // 
            // garbage_rb
            // 
            this.garbage_rb.AutoSize = true;
            this.garbage_rb.Location = new System.Drawing.Point(6, 115);
            this.garbage_rb.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.garbage_rb.Name = "garbage_rb";
            this.garbage_rb.Size = new System.Drawing.Size(69, 19);
            this.garbage_rb.TabIndex = 4;
            this.garbage_rb.TabStop = true;
            this.garbage_rb.Text = "Garbage";
            this.garbage_rb.UseVisualStyleBackColor = true;
            // 
            // comment_rb
            // 
            this.comment_rb.AutoSize = true;
            this.comment_rb.Location = new System.Drawing.Point(6, 87);
            this.comment_rb.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.comment_rb.Name = "comment_rb";
            this.comment_rb.Size = new System.Drawing.Size(79, 19);
            this.comment_rb.TabIndex = 3;
            this.comment_rb.TabStop = true;
            this.comment_rb.Text = "Comment";
            this.comment_rb.UseVisualStyleBackColor = true;
            // 
            // title_rb
            // 
            this.title_rb.AutoSize = true;
            this.title_rb.Location = new System.Drawing.Point(6, 59);
            this.title_rb.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.title_rb.Name = "title_rb";
            this.title_rb.Size = new System.Drawing.Size(47, 19);
            this.title_rb.TabIndex = 2;
            this.title_rb.TabStop = true;
            this.title_rb.Text = "Title";
            this.title_rb.UseVisualStyleBackColor = true;
            // 
            // paragraph_rb
            // 
            this.paragraph_rb.AutoSize = true;
            this.paragraph_rb.Location = new System.Drawing.Point(6, 31);
            this.paragraph_rb.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.paragraph_rb.Name = "paragraph_rb";
            this.paragraph_rb.Size = new System.Drawing.Size(79, 19);
            this.paragraph_rb.TabIndex = 1;
            this.paragraph_rb.TabStop = true;
            this.paragraph_rb.Text = "Paragraph";
            this.paragraph_rb.UseVisualStyleBackColor = true;
            // 
            // none_rb
            // 
            this.none_rb.AutoSize = true;
            this.none_rb.Location = new System.Drawing.Point(6, 3);
            this.none_rb.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.none_rb.Name = "none_rb";
            this.none_rb.Size = new System.Drawing.Size(54, 19);
            this.none_rb.TabIndex = 0;
            this.none_rb.TabStop = true;
            this.none_rb.Text = "None";
            this.none_rb.UseVisualStyleBackColor = true;
            // 
            // LabelingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(649, 585);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LabelingForm";
            this.Text = "LabelingForm";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox csvFile_tb;
        private System.Windows.Forms.Button load_btn;
        private System.Windows.Forms.Button saveLabeled_btn;
        private System.Windows.Forms.ListBox ocrBlock_lb;
        private System.Windows.Forms.RadioButton none_rb;
        private System.Windows.Forms.RadioButton paragraph_rb;
        private System.Windows.Forms.RadioButton title_rb;
        private System.Windows.Forms.RadioButton comment_rb;
        private System.Windows.Forms.RadioButton garbage_rb;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Button saveAll_btn;
    }
}