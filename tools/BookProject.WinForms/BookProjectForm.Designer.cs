
using BookProject.WinForms.Controls;
using BookProject.WinForms.Controls.Detect;

namespace BookProject.WinForms
{
    partial class BookProjectForm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.page_lb = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.saveBook_btn = new System.Windows.Forms.Button();
            this.openBookFolder_btn = new System.Windows.Forms.Button();
            this.bookFolder_tb = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.generateLines_btn = new System.Windows.Forms.Button();
            this.generateFb2_btn = new System.Windows.Forms.Button();
            this.blockDetails_panel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 500F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.pictureBox1, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.page_lb, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 2, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1170, 681);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Silver;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(183, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(484, 675);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // page_lb
            // 
            this.page_lb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.page_lb.FormattingEnabled = true;
            this.page_lb.ItemHeight = 15;
            this.page_lb.Location = new System.Drawing.Point(3, 3);
            this.page_lb.Name = "page_lb";
            this.page_lb.Size = new System.Drawing.Size(174, 675);
            this.page_lb.TabIndex = 1;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.progressBar1, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.tabControl1, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.blockDetails_panel, 0, 2);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(670, 0);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 4;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(500, 681);
            this.tableLayoutPanel5.TabIndex = 6;
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(3, 31);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(494, 14);
            this.progressBar1.TabIndex = 2;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 3;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel7.Controls.Add(this.saveBook_btn, 2, 0);
            this.tableLayoutPanel7.Controls.Add(this.openBookFolder_btn, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.bookFolder_tb, 0, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(500, 28);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // saveBook_btn
            // 
            this.saveBook_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.saveBook_btn.Location = new System.Drawing.Point(401, 1);
            this.saveBook_btn.Margin = new System.Windows.Forms.Padding(1);
            this.saveBook_btn.Name = "saveBook_btn";
            this.saveBook_btn.Size = new System.Drawing.Size(98, 26);
            this.saveBook_btn.TabIndex = 3;
            this.saveBook_btn.Text = "Save Book";
            this.saveBook_btn.UseVisualStyleBackColor = true;
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
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(1, 199);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(498, 481);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(490, 453);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Detect Image";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(490, 453);
            this.tabPage2.TabIndex = 2;
            this.tabPage2.Text = "Detect Title";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 24);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(490, 453);
            this.tabPage4.TabIndex = 4;
            this.tabPage4.Text = "Detect Comment Links";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(490, 453);
            this.tabPage3.TabIndex = 5;
            this.tabPage3.Text = "Detect Garbage";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.tableLayoutPanel1);
            this.tabPage6.Location = new System.Drawing.Point(4, 24);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(490, 453);
            this.tabPage6.TabIndex = 7;
            this.tabPage6.Text = "Other";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.Controls.Add(this.generateLines_btn, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.generateFb2_btn, 0, 4);
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(484, 447);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // generateLines_btn
            // 
            this.generateLines_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generateLines_btn.Location = new System.Drawing.Point(324, 39);
            this.generateLines_btn.Margin = new System.Windows.Forms.Padding(1);
            this.generateLines_btn.Name = "generateLines_btn";
            this.generateLines_btn.Size = new System.Drawing.Size(159, 26);
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
            this.generateFb2_btn.Size = new System.Drawing.Size(162, 26);
            this.generateFb2_btn.TabIndex = 15;
            this.generateFb2_btn.Text = "Generate FB2";
            this.generateFb2_btn.UseVisualStyleBackColor = true;
            this.generateFb2_btn.Click += new System.EventHandler(this.GenerateFb2Click);
            // 
            // blockDetails_panel
            // 
            this.blockDetails_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.blockDetails_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockDetails_panel.Location = new System.Drawing.Point(0, 48);
            this.blockDetails_panel.Margin = new System.Windows.Forms.Padding(0);
            this.blockDetails_panel.Name = "blockDetails_panel";
            this.blockDetails_panel.Size = new System.Drawing.Size(500, 150);
            this.blockDetails_panel.TabIndex = 8;
            // 
            // BookProjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1170, 681);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "BookProjectForm";
            this.Text = "LabelingForm";
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox bookFolder_tb;
        private System.Windows.Forms.Button openBookFolder_btn;
        private System.Windows.Forms.Button generateLines_btn;
        private System.Windows.Forms.Button saveBook_btn;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button generateFb2_btn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage3;
        private DetectImageControl detectImageControl1;
        private DetectTitleControl detectTitleControl1;
        private DetectCommentLinkNumberControl detectCommentLinkNumberControl1;
        private DetectGarbageControl detectGarbageControl1;
        private System.Windows.Forms.Panel blockDetails_panel;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.ListBox page_lb;
    }
}