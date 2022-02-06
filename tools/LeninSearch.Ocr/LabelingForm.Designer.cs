
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
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.ocr_lb = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.none_panel = new System.Windows.Forms.Panel();
            this.pstart_panel = new System.Windows.Forms.Panel();
            this.garbage_panel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.title_panel = new System.Windows.Forms.Panel();
            this.comment_panel = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.pmiddle_panel = new System.Windows.Forms.Panel();
            this.pend_panel = new System.Windows.Forms.Panel();
            this.image_panel = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.openBookFolder_btn = new System.Windows.Forms.Button();
            this.regenerateFeatures_btn = new System.Windows.Forms.Button();
            this.generateLines_btn = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bookFolder_tb = new System.Windows.Forms.TextBox();
            this.saveOcrData_btn = new System.Windows.Forms.Button();
            this.trainModel_btn = new System.Windows.Forms.Button();
            this.applyModel_btn = new System.Windows.Forms.Button();
            this.generateImageBlocks_btn = new System.Windows.Forms.Button();
            this.rowModel_flp = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.ocr_lb, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel1, 2, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1484, 445);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // ocr_lb
            // 
            this.ocr_lb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ocr_lb.FormattingEnabled = true;
            this.ocr_lb.ItemHeight = 15;
            this.ocr_lb.Location = new System.Drawing.Point(3, 3);
            this.ocr_lb.Name = "ocr_lb";
            this.ocr_lb.Size = new System.Drawing.Size(174, 439);
            this.ocr_lb.TabIndex = 1;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel6, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(180, 0);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(904, 445);
            this.tableLayoutPanel4.TabIndex = 3;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(898, 411);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 16;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel6.Controls.Add(this.label6, 11, 0);
            this.tableLayoutPanel6.Controls.Add(this.label5, 9, 0);
            this.tableLayoutPanel6.Controls.Add(this.label4, 7, 0);
            this.tableLayoutPanel6.Controls.Add(this.label3, 5, 0);
            this.tableLayoutPanel6.Controls.Add(this.label2, 3, 0);
            this.tableLayoutPanel6.Controls.Add(this.none_panel, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.pstart_panel, 2, 0);
            this.tableLayoutPanel6.Controls.Add(this.garbage_panel, 14, 0);
            this.tableLayoutPanel6.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.title_panel, 10, 0);
            this.tableLayoutPanel6.Controls.Add(this.comment_panel, 8, 0);
            this.tableLayoutPanel6.Controls.Add(this.label7, 13, 0);
            this.tableLayoutPanel6.Controls.Add(this.pmiddle_panel, 4, 0);
            this.tableLayoutPanel6.Controls.Add(this.pend_panel, 6, 0);
            this.tableLayoutPanel6.Controls.Add(this.image_panel, 12, 0);
            this.tableLayoutPanel6.Controls.Add(this.label8, 15, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 417);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(904, 28);
            this.tableLayoutPanel6.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(596, 6);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 15);
            this.label6.TabIndex = 11;
            this.label6.Text = "Title (T)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(483, 6);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 22);
            this.label5.TabIndex = 9;
            this.label5.Text = "Comment (C)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(370, 6);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "PEnd (E)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(257, 6);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "PMiddle (M)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(144, 6);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "PStart (S)";
            // 
            // none_panel
            // 
            this.none_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.none_panel.Location = new System.Drawing.Point(5, 5);
            this.none_panel.Margin = new System.Windows.Forms.Padding(5);
            this.none_panel.Name = "none_panel";
            this.none_panel.Size = new System.Drawing.Size(18, 18);
            this.none_panel.TabIndex = 0;
            // 
            // pstart_panel
            // 
            this.pstart_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pstart_panel.Location = new System.Drawing.Point(118, 5);
            this.pstart_panel.Margin = new System.Windows.Forms.Padding(5);
            this.pstart_panel.Name = "pstart_panel";
            this.pstart_panel.Size = new System.Drawing.Size(18, 18);
            this.pstart_panel.TabIndex = 1;
            // 
            // garbage_panel
            // 
            this.garbage_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.garbage_panel.Location = new System.Drawing.Point(796, 5);
            this.garbage_panel.Margin = new System.Windows.Forms.Padding(5);
            this.garbage_panel.Name = "garbage_panel";
            this.garbage_panel.Size = new System.Drawing.Size(18, 18);
            this.garbage_panel.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "None (N)";
            // 
            // title_panel
            // 
            this.title_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.title_panel.Location = new System.Drawing.Point(570, 5);
            this.title_panel.Margin = new System.Windows.Forms.Padding(5);
            this.title_panel.Name = "title_panel";
            this.title_panel.Size = new System.Drawing.Size(18, 18);
            this.title_panel.TabIndex = 3;
            // 
            // comment_panel
            // 
            this.comment_panel.Location = new System.Drawing.Point(457, 5);
            this.comment_panel.Margin = new System.Windows.Forms.Padding(5);
            this.comment_panel.Name = "comment_panel";
            this.comment_panel.Size = new System.Drawing.Size(18, 18);
            this.comment_panel.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(709, 6);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 15);
            this.label7.TabIndex = 13;
            this.label7.Text = "Image (A)";
            // 
            // pmiddle_panel
            // 
            this.pmiddle_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pmiddle_panel.Location = new System.Drawing.Point(231, 5);
            this.pmiddle_panel.Margin = new System.Windows.Forms.Padding(5);
            this.pmiddle_panel.Name = "pmiddle_panel";
            this.pmiddle_panel.Size = new System.Drawing.Size(18, 18);
            this.pmiddle_panel.TabIndex = 12;
            // 
            // pend_panel
            // 
            this.pend_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pend_panel.Location = new System.Drawing.Point(344, 5);
            this.pend_panel.Margin = new System.Windows.Forms.Padding(5);
            this.pend_panel.Name = "pend_panel";
            this.pend_panel.Size = new System.Drawing.Size(18, 18);
            this.pend_panel.TabIndex = 14;
            // 
            // image_panel
            // 
            this.image_panel.Location = new System.Drawing.Point(683, 5);
            this.image_panel.Margin = new System.Windows.Forms.Padding(5);
            this.image_panel.Name = "image_panel";
            this.image_panel.Size = new System.Drawing.Size(18, 18);
            this.image_panel.TabIndex = 10;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(822, 6);
            this.label8.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 15);
            this.label8.TabIndex = 15;
            this.label8.Text = "Garbage (G)";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.Controls.Add(this.openBookFolder_btn, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.regenerateFeatures_btn, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.generateLines_btn, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.bookFolder_tb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.saveOcrData_btn, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.trainModel_btn, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.applyModel_btn, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.generateImageBlocks_btn, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.rowModel_flp, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1087, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(394, 439);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // openBookFolder_btn
            // 
            this.openBookFolder_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.openBookFolder_btn.Location = new System.Drawing.Point(266, 3);
            this.openBookFolder_btn.Name = "openBookFolder_btn";
            this.openBookFolder_btn.Size = new System.Drawing.Size(125, 22);
            this.openBookFolder_btn.TabIndex = 1;
            this.openBookFolder_btn.Text = "Open book folder";
            this.openBookFolder_btn.UseVisualStyleBackColor = true;
            // 
            // regenerateFeatures_btn
            // 
            this.regenerateFeatures_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.regenerateFeatures_btn.Location = new System.Drawing.Point(266, 41);
            this.regenerateFeatures_btn.Name = "regenerateFeatures_btn";
            this.regenerateFeatures_btn.Size = new System.Drawing.Size(125, 22);
            this.regenerateFeatures_btn.TabIndex = 2;
            this.regenerateFeatures_btn.Text = "Regenerate Features";
            this.regenerateFeatures_btn.UseVisualStyleBackColor = true;
            // 
            // generateLines_btn
            // 
            this.generateLines_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generateLines_btn.Location = new System.Drawing.Point(3, 41);
            this.generateLines_btn.Name = "generateLines_btn";
            this.generateLines_btn.Size = new System.Drawing.Size(127, 22);
            this.generateLines_btn.TabIndex = 0;
            this.generateLines_btn.Text = "Generate Lines";
            this.generateLines_btn.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.progressBar1, 3);
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(3, 31);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(388, 4);
            this.progressBar1.TabIndex = 2;
            // 
            // bookFolder_tb
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.bookFolder_tb, 2);
            this.bookFolder_tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bookFolder_tb.Location = new System.Drawing.Point(3, 3);
            this.bookFolder_tb.Name = "bookFolder_tb";
            this.bookFolder_tb.ReadOnly = true;
            this.bookFolder_tb.Size = new System.Drawing.Size(257, 23);
            this.bookFolder_tb.TabIndex = 0;
            // 
            // saveOcrData_btn
            // 
            this.saveOcrData_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.saveOcrData_btn.Location = new System.Drawing.Point(136, 41);
            this.saveOcrData_btn.Name = "saveOcrData_btn";
            this.saveOcrData_btn.Size = new System.Drawing.Size(124, 22);
            this.saveOcrData_btn.TabIndex = 3;
            this.saveOcrData_btn.Text = "Save Ocr Data";
            this.saveOcrData_btn.UseVisualStyleBackColor = true;
            // 
            // trainModel_btn
            // 
            this.trainModel_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trainModel_btn.Location = new System.Drawing.Point(3, 69);
            this.trainModel_btn.Name = "trainModel_btn";
            this.trainModel_btn.Size = new System.Drawing.Size(127, 22);
            this.trainModel_btn.TabIndex = 5;
            this.trainModel_btn.Text = "Train Model";
            this.trainModel_btn.UseVisualStyleBackColor = true;
            // 
            // applyModel_btn
            // 
            this.applyModel_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.applyModel_btn.Location = new System.Drawing.Point(136, 69);
            this.applyModel_btn.Name = "applyModel_btn";
            this.applyModel_btn.Size = new System.Drawing.Size(124, 22);
            this.applyModel_btn.TabIndex = 7;
            this.applyModel_btn.Text = "Apply Model";
            this.applyModel_btn.UseVisualStyleBackColor = true;
            // 
            // generateImageBlocks_btn
            // 
            this.generateImageBlocks_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generateImageBlocks_btn.Location = new System.Drawing.Point(266, 69);
            this.generateImageBlocks_btn.Name = "generateImageBlocks_btn";
            this.generateImageBlocks_btn.Size = new System.Drawing.Size(125, 22);
            this.generateImageBlocks_btn.TabIndex = 6;
            this.generateImageBlocks_btn.Text = "Generate IBs";
            this.generateImageBlocks_btn.UseVisualStyleBackColor = true;
            // 
            // rowModel_flp
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.rowModel_flp, 3);
            this.rowModel_flp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rowModel_flp.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.rowModel_flp.Location = new System.Drawing.Point(3, 97);
            this.rowModel_flp.Name = "rowModel_flp";
            this.rowModel_flp.Size = new System.Drawing.Size(388, 339);
            this.rowModel_flp.TabIndex = 8;
            // 
            // LabelingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1484, 445);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "LabelingForm";
            this.Text = "LabelingForm";
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListBox ocr_lb;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Panel none_panel;
        private System.Windows.Forms.Panel pstart_panel;
        private System.Windows.Forms.Panel comment_panel;
        private System.Windows.Forms.Panel title_panel;
        private System.Windows.Forms.Panel garbage_panel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel image_panel;
        private System.Windows.Forms.TextBox bookFolder_tb;
        private System.Windows.Forms.Button openBookFolder_btn;
        private System.Windows.Forms.Button generateLines_btn;
        private System.Windows.Forms.Button regenerateFeatures_btn;
        private System.Windows.Forms.Button saveOcrData_btn;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button trainModel_btn;
        private System.Windows.Forms.Button generateImageBlocks_btn;
        private System.Windows.Forms.Button applyModel_btn;
        private System.Windows.Forms.Panel pmiddle_panel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel pend_panel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.FlowLayoutPanel rowModel_flp;
    }
}