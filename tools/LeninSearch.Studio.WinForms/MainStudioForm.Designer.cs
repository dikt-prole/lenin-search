
using LeninSearch.Studio.WinForms.Controls;

namespace LeninSearch.Studio.WinForms
{
    partial class MainStudioForm
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
            this.image_panel = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label9 = new System.Windows.Forms.Label();
            this.labeling_rb = new System.Windows.Forms.RadioButton();
            this.editing_rb = new System.Windows.Forms.RadioButton();
            this.blocks_rb = new System.Windows.Forms.RadioButton();
            this.all_rb = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.openBookFolder_btn = new System.Windows.Forms.Button();
            this.bookFolder_tb = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.detectImageControl1 = new LeninSearch.Studio.WinForms.Controls.DetectImageControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.detectTitleControl1 = new LeninSearch.Studio.WinForms.Controls.DetectTitleControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.saveOcrData_btn = new System.Windows.Forms.Button();
            this.regeneratePage_btn = new System.Windows.Forms.Button();
            this.uncoveredLinks_btn = new System.Windows.Forms.Button();
            this.breakByDistant_btn = new System.Windows.Forms.Button();
            this.uncoveredStarts_btn = new System.Windows.Forms.Button();
            this.removeLinks_btn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.generateLines_btn = new System.Windows.Forms.Button();
            this.generateFb2_btn = new System.Windows.Forms.Button();
            this.autoDetectImages_btn = new System.Windows.Forms.Button();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.tableLayoutPanel3.Controls.Add(this.ocr_lb, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 3, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1170, 557);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // ocr_lb
            // 
            this.ocr_lb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ocr_lb.FormattingEnabled = true;
            this.ocr_lb.ItemHeight = 15;
            this.ocr_lb.Location = new System.Drawing.Point(3, 3);
            this.ocr_lb.Name = "ocr_lb";
            this.ocr_lb.Size = new System.Drawing.Size(174, 551);
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
            this.tableLayoutPanel4.Location = new System.Drawing.Point(330, 0);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(440, 557);
            this.tableLayoutPanel4.TabIndex = 3;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(434, 523);
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
            this.tableLayoutPanel6.Controls.Add(this.image_panel, 12, 0);
            this.tableLayoutPanel6.Controls.Add(this.label8, 15, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 529);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(440, 28);
            this.tableLayoutPanel6.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(306, 6);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 22);
            this.label6.TabIndex = 11;
            this.label6.Text = "Title (T)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(251, 6);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 22);
            this.label5.TabIndex = 9;
            this.label5.Text = "Comment (C)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(141, 6);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 22);
            this.label3.TabIndex = 7;
            this.label3.Text = "PMiddle (M)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(86, 6);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 22);
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
            this.pstart_panel.Location = new System.Drawing.Point(60, 5);
            this.pstart_panel.Margin = new System.Windows.Forms.Padding(5);
            this.pstart_panel.Name = "pstart_panel";
            this.pstart_panel.Size = new System.Drawing.Size(18, 18);
            this.pstart_panel.TabIndex = 1;
            // 
            // garbage_panel
            // 
            this.garbage_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.garbage_panel.Location = new System.Drawing.Point(390, 5);
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
            this.label1.Size = new System.Drawing.Size(21, 22);
            this.label1.TabIndex = 5;
            this.label1.Text = "None (N)";
            // 
            // title_panel
            // 
            this.title_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.title_panel.Location = new System.Drawing.Point(280, 5);
            this.title_panel.Margin = new System.Windows.Forms.Padding(5);
            this.title_panel.Name = "title_panel";
            this.title_panel.Size = new System.Drawing.Size(18, 18);
            this.title_panel.TabIndex = 3;
            // 
            // comment_panel
            // 
            this.comment_panel.Location = new System.Drawing.Point(225, 5);
            this.comment_panel.Margin = new System.Windows.Forms.Padding(5);
            this.comment_panel.Name = "comment_panel";
            this.comment_panel.Size = new System.Drawing.Size(18, 17);
            this.comment_panel.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(361, 6);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 22);
            this.label7.TabIndex = 13;
            this.label7.Text = "Image (A)";
            // 
            // pmiddle_panel
            // 
            this.pmiddle_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pmiddle_panel.Location = new System.Drawing.Point(115, 5);
            this.pmiddle_panel.Margin = new System.Windows.Forms.Padding(5);
            this.pmiddle_panel.Name = "pmiddle_panel";
            this.pmiddle_panel.Size = new System.Drawing.Size(18, 18);
            this.pmiddle_panel.TabIndex = 12;
            // 
            // image_panel
            // 
            this.image_panel.Location = new System.Drawing.Point(335, 5);
            this.image_panel.Margin = new System.Windows.Forms.Padding(5);
            this.image_panel.Name = "image_panel";
            this.image_panel.Size = new System.Drawing.Size(18, 17);
            this.image_panel.TabIndex = 10;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(416, 6);
            this.label8.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(21, 22);
            this.label8.TabIndex = 15;
            this.label8.Text = "Garbage (G)";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label9, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.labeling_rb, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.editing_rb, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.blocks_rb, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.all_rb, 0, 4);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(183, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 6;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(144, 551);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(97, 15);
            this.label9.TabIndex = 0;
            this.label9.Text = "Page Menu Type:";
            // 
            // labeling_rb
            // 
            this.labeling_rb.AutoSize = true;
            this.labeling_rb.Location = new System.Drawing.Point(5, 23);
            this.labeling_rb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.labeling_rb.Name = "labeling_rb";
            this.labeling_rb.Size = new System.Drawing.Size(70, 19);
            this.labeling_rb.TabIndex = 1;
            this.labeling_rb.TabStop = true;
            this.labeling_rb.Text = "Labeling";
            this.labeling_rb.UseVisualStyleBackColor = true;
            // 
            // editing_rb
            // 
            this.editing_rb.AutoSize = true;
            this.editing_rb.Location = new System.Drawing.Point(5, 51);
            this.editing_rb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.editing_rb.Name = "editing_rb";
            this.editing_rb.Size = new System.Drawing.Size(62, 19);
            this.editing_rb.TabIndex = 2;
            this.editing_rb.TabStop = true;
            this.editing_rb.Text = "Editing";
            this.editing_rb.UseVisualStyleBackColor = true;
            // 
            // blocks_rb
            // 
            this.blocks_rb.AutoSize = true;
            this.blocks_rb.Location = new System.Drawing.Point(5, 79);
            this.blocks_rb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.blocks_rb.Name = "blocks_rb";
            this.blocks_rb.Size = new System.Drawing.Size(130, 19);
            this.blocks_rb.TabIndex = 3;
            this.blocks_rb.TabStop = true;
            this.blocks_rb.Text = "Blocks (Image/Title)";
            this.blocks_rb.UseVisualStyleBackColor = true;
            // 
            // all_rb
            // 
            this.all_rb.AutoSize = true;
            this.all_rb.Location = new System.Drawing.Point(5, 107);
            this.all_rb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.all_rb.Name = "all_rb";
            this.all_rb.Size = new System.Drawing.Size(39, 19);
            this.all_rb.TabIndex = 4;
            this.all_rb.TabStop = true;
            this.all_rb.Text = "All";
            this.all_rb.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.progressBar1, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.tabControl1, 0, 2);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(770, 0);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 3;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(400, 557);
            this.tableLayoutPanel5.TabIndex = 6;
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(3, 31);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(394, 14);
            this.progressBar1.TabIndex = 2;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel7.Controls.Add(this.openBookFolder_btn, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.bookFolder_tb, 0, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(400, 28);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // openBookFolder_btn
            // 
            this.openBookFolder_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.openBookFolder_btn.Location = new System.Drawing.Point(301, 1);
            this.openBookFolder_btn.Margin = new System.Windows.Forms.Padding(1);
            this.openBookFolder_btn.Name = "openBookFolder_btn";
            this.openBookFolder_btn.Size = new System.Drawing.Size(98, 26);
            this.openBookFolder_btn.TabIndex = 1;
            this.openBookFolder_btn.Text = "Open book folder";
            this.openBookFolder_btn.UseVisualStyleBackColor = true;
            // 
            // bookFolder_tb
            // 
            this.bookFolder_tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bookFolder_tb.Location = new System.Drawing.Point(3, 3);
            this.bookFolder_tb.Name = "bookFolder_tb";
            this.bookFolder_tb.ReadOnly = true;
            this.bookFolder_tb.Size = new System.Drawing.Size(294, 23);
            this.bookFolder_tb.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(1, 49);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(398, 507);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.detectImageControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(390, 479);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Detect Image";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // detectImageControl1
            // 
            this.detectImageControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detectImageControl1.Location = new System.Drawing.Point(3, 3);
            this.detectImageControl1.Name = "detectImageControl1";
            this.detectImageControl1.Size = new System.Drawing.Size(384, 473);
            this.detectImageControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.detectTitleControl1);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(390, 479);
            this.tabPage2.TabIndex = 2;
            this.tabPage2.Text = "Detect Title";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // detectTitleControl1
            // 
            this.detectTitleControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detectTitleControl1.Location = new System.Drawing.Point(3, 3);
            this.detectTitleControl1.Name = "detectTitleControl1";
            this.detectTitleControl1.Size = new System.Drawing.Size(384, 473);
            this.detectTitleControl1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tableLayoutPanel1);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(390, 479);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "Other";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.Controls.Add(this.saveOcrData_btn, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.regeneratePage_btn, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.uncoveredLinks_btn, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.breakByDistant_btn, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.uncoveredStarts_btn, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.removeLinks_btn, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.generateLines_btn, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.generateFb2_btn, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.autoDetectImages_btn, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(384, 473);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // saveOcrData_btn
            // 
            this.saveOcrData_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.saveOcrData_btn.Location = new System.Drawing.Point(131, 39);
            this.saveOcrData_btn.Margin = new System.Windows.Forms.Padding(1);
            this.saveOcrData_btn.Name = "saveOcrData_btn";
            this.saveOcrData_btn.Size = new System.Drawing.Size(124, 26);
            this.saveOcrData_btn.TabIndex = 3;
            this.saveOcrData_btn.Text = "Save Ocr Data";
            this.saveOcrData_btn.UseVisualStyleBackColor = true;
            // 
            // regeneratePage_btn
            // 
            this.regeneratePage_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.regeneratePage_btn.Location = new System.Drawing.Point(131, 95);
            this.regeneratePage_btn.Margin = new System.Windows.Forms.Padding(1);
            this.regeneratePage_btn.Name = "regeneratePage_btn";
            this.regeneratePage_btn.Size = new System.Drawing.Size(124, 26);
            this.regeneratePage_btn.TabIndex = 9;
            this.regeneratePage_btn.Text = "Regenerate Page";
            this.regeneratePage_btn.UseVisualStyleBackColor = true;
            this.regeneratePage_btn.Click += new System.EventHandler(this.RegeneratePageClick);
            // 
            // uncoveredLinks_btn
            // 
            this.uncoveredLinks_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uncoveredLinks_btn.Location = new System.Drawing.Point(257, 95);
            this.uncoveredLinks_btn.Margin = new System.Windows.Forms.Padding(1);
            this.uncoveredLinks_btn.Name = "uncoveredLinks_btn";
            this.uncoveredLinks_btn.Size = new System.Drawing.Size(126, 26);
            this.uncoveredLinks_btn.TabIndex = 10;
            this.uncoveredLinks_btn.Text = "Uncovered Links";
            this.uncoveredLinks_btn.UseVisualStyleBackColor = true;
            this.uncoveredLinks_btn.Click += new System.EventHandler(this.UncoveredLinksClick);
            // 
            // breakByDistant_btn
            // 
            this.breakByDistant_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.breakByDistant_btn.Location = new System.Drawing.Point(1, 123);
            this.breakByDistant_btn.Margin = new System.Windows.Forms.Padding(1);
            this.breakByDistant_btn.Name = "breakByDistant_btn";
            this.breakByDistant_btn.Size = new System.Drawing.Size(128, 26);
            this.breakByDistant_btn.TabIndex = 11;
            this.breakByDistant_btn.Text = "Break By Distant";
            this.breakByDistant_btn.UseVisualStyleBackColor = true;
            this.breakByDistant_btn.Click += new System.EventHandler(this.BreakByDistantClick);
            // 
            // uncoveredStarts_btn
            // 
            this.uncoveredStarts_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uncoveredStarts_btn.Location = new System.Drawing.Point(257, 123);
            this.uncoveredStarts_btn.Margin = new System.Windows.Forms.Padding(1);
            this.uncoveredStarts_btn.Name = "uncoveredStarts_btn";
            this.uncoveredStarts_btn.Size = new System.Drawing.Size(126, 26);
            this.uncoveredStarts_btn.TabIndex = 12;
            this.uncoveredStarts_btn.Text = "Uncovered Starts";
            this.uncoveredStarts_btn.UseVisualStyleBackColor = true;
            this.uncoveredStarts_btn.Click += new System.EventHandler(this.UncoveredStartsClick);
            // 
            // removeLinks_btn
            // 
            this.removeLinks_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.removeLinks_btn.Location = new System.Drawing.Point(131, 123);
            this.removeLinks_btn.Margin = new System.Windows.Forms.Padding(1);
            this.removeLinks_btn.Name = "removeLinks_btn";
            this.removeLinks_btn.Size = new System.Drawing.Size(124, 26);
            this.removeLinks_btn.TabIndex = 13;
            this.removeLinks_btn.Text = "Remove Links";
            this.removeLinks_btn.UseVisualStyleBackColor = true;
            this.removeLinks_btn.Click += new System.EventHandler(this.RemoveLinksClick);
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(1, 39);
            this.button1.Margin = new System.Windows.Forms.Padding(1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 26);
            this.button1.TabIndex = 14;
            this.button1.Text = "Train - Apply Model";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.TrainApplyModelClick);
            // 
            // generateLines_btn
            // 
            this.generateLines_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generateLines_btn.Location = new System.Drawing.Point(257, 39);
            this.generateLines_btn.Margin = new System.Windows.Forms.Padding(1);
            this.generateLines_btn.Name = "generateLines_btn";
            this.generateLines_btn.Size = new System.Drawing.Size(126, 26);
            this.generateLines_btn.TabIndex = 0;
            this.generateLines_btn.Text = "Generate Lines";
            this.generateLines_btn.UseVisualStyleBackColor = true;
            // 
            // generateFb2_btn
            // 
            this.generateFb2_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generateFb2_btn.Location = new System.Drawing.Point(1, 95);
            this.generateFb2_btn.Margin = new System.Windows.Forms.Padding(1);
            this.generateFb2_btn.Name = "generateFb2_btn";
            this.generateFb2_btn.Size = new System.Drawing.Size(128, 26);
            this.generateFb2_btn.TabIndex = 15;
            this.generateFb2_btn.Text = "Generate FB2";
            this.generateFb2_btn.UseVisualStyleBackColor = true;
            this.generateFb2_btn.Click += new System.EventHandler(this.GenerateFb2Click);
            // 
            // autoDetectImages_btn
            // 
            this.autoDetectImages_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.autoDetectImages_btn.Location = new System.Drawing.Point(1, 67);
            this.autoDetectImages_btn.Margin = new System.Windows.Forms.Padding(1);
            this.autoDetectImages_btn.Name = "autoDetectImages_btn";
            this.autoDetectImages_btn.Size = new System.Drawing.Size(128, 26);
            this.autoDetectImages_btn.TabIndex = 16;
            this.autoDetectImages_btn.Text = "Auto Detect Images";
            this.autoDetectImages_btn.UseVisualStyleBackColor = true;
            // 
            // MainStudioForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1170, 557);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "MainStudioForm";
            this.Text = "LabelingForm";
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel image_panel;
        private System.Windows.Forms.TextBox bookFolder_tb;
        private System.Windows.Forms.Button openBookFolder_btn;
        private System.Windows.Forms.Button generateLines_btn;
        private System.Windows.Forms.Button saveOcrData_btn;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel pmiddle_panel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton labeling_rb;
        private System.Windows.Forms.RadioButton editing_rb;
        private System.Windows.Forms.RadioButton blocks_rb;
        private System.Windows.Forms.RadioButton all_rb;
        private System.Windows.Forms.Button regeneratePage_btn;
        private System.Windows.Forms.Button uncoveredLinks_btn;
        private System.Windows.Forms.Button breakByDistant_btn;
        private System.Windows.Forms.Button uncoveredStarts_btn;
        private System.Windows.Forms.Button removeLinks_btn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button generateFb2_btn;
        private System.Windows.Forms.Button autoDetectImages_btn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private DetectTitleControl detectTitleControl1;
        private DetectImageControl detectImageControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
    }
}