
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
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.saveBook_btn = new System.Windows.Forms.Button();
            this.openBookFolder_btn = new System.Windows.Forms.Button();
            this.bookFolder_tb = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.detectImageControl1 = new BookProject.WinForms.Controls.Detect.DetectImageControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.detectTitleControl1 = new BookProject.WinForms.Controls.Detect.DetectTitleControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.detectCommentLinkNumberControl1 = new BookProject.WinForms.Controls.Detect.DetectCommentLinkNumberControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.detectGarbageControl1 = new BookProject.WinForms.Controls.Detect.DetectGarbageControl();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.detectLineControl1 = new BookProject.WinForms.Controls.Detect.DetectLineControl();
            this.blockDetailsControl1 = new BookProject.WinForms.Controls.BlockDetailsControl();
            this.pageControl1 = new BookProject.WinForms.Controls.PageControl();
            this.blockListControl1 = new BookProject.WinForms.Controls.BlockListControl();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 450F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 450F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.pageControl1, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.blockListControl1, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1170, 681);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.tabControl1, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.blockDetailsControl1, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(720, 0);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 4;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(450, 681);
            this.tableLayoutPanel5.TabIndex = 6;
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
            this.tableLayoutPanel7.Size = new System.Drawing.Size(450, 28);
            this.tableLayoutPanel7.TabIndex = 0;
            // 
            // saveBook_btn
            // 
            this.saveBook_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.saveBook_btn.Location = new System.Drawing.Point(351, 1);
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
            this.openBookFolder_btn.Location = new System.Drawing.Point(251, 1);
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
            this.bookFolder_tb.Size = new System.Drawing.Size(244, 23);
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
            this.tabControl1.Location = new System.Drawing.Point(1, 349);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(448, 331);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.detectImageControl1);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(490, 275);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Images";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // detectImageControl1
            // 
            this.detectImageControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detectImageControl1.Location = new System.Drawing.Point(3, 3);
            this.detectImageControl1.Name = "detectImageControl1";
            this.detectImageControl1.Size = new System.Drawing.Size(484, 269);
            this.detectImageControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.detectTitleControl1);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(490, 275);
            this.tabPage2.TabIndex = 2;
            this.tabPage2.Text = "Titles";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // detectTitleControl1
            // 
            this.detectTitleControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detectTitleControl1.Location = new System.Drawing.Point(3, 3);
            this.detectTitleControl1.Name = "detectTitleControl1";
            this.detectTitleControl1.Size = new System.Drawing.Size(484, 269);
            this.detectTitleControl1.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.detectCommentLinkNumberControl1);
            this.tabPage4.Location = new System.Drawing.Point(4, 24);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(440, 303);
            this.tabPage4.TabIndex = 4;
            this.tabPage4.Text = "Comment Links";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // detectCommentLinkNumberControl1
            // 
            this.detectCommentLinkNumberControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detectCommentLinkNumberControl1.Location = new System.Drawing.Point(3, 3);
            this.detectCommentLinkNumberControl1.Name = "detectCommentLinkNumberControl1";
            this.detectCommentLinkNumberControl1.Size = new System.Drawing.Size(434, 297);
            this.detectCommentLinkNumberControl1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.detectGarbageControl1);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(490, 275);
            this.tabPage3.TabIndex = 5;
            this.tabPage3.Text = "Garbage";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // detectGarbageControl1
            // 
            this.detectGarbageControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detectGarbageControl1.Location = new System.Drawing.Point(3, 3);
            this.detectGarbageControl1.Name = "detectGarbageControl1";
            this.detectGarbageControl1.Size = new System.Drawing.Size(484, 269);
            this.detectGarbageControl1.TabIndex = 0;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.detectLineControl1);
            this.tabPage6.Location = new System.Drawing.Point(4, 24);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(490, 275);
            this.tabPage6.TabIndex = 7;
            this.tabPage6.Text = "Lines";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // detectLineControl1
            // 
            this.detectLineControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detectLineControl1.Location = new System.Drawing.Point(3, 3);
            this.detectLineControl1.Name = "detectLineControl1";
            this.detectLineControl1.Size = new System.Drawing.Size(484, 269);
            this.detectLineControl1.TabIndex = 0;
            // 
            // blockDetailsControl1
            // 
            this.blockDetailsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockDetailsControl1.Location = new System.Drawing.Point(3, 31);
            this.blockDetailsControl1.Name = "blockDetailsControl1";
            this.blockDetailsControl1.Size = new System.Drawing.Size(444, 294);
            this.blockDetailsControl1.TabIndex = 7;
            // 
            // pageControl1
            // 
            this.pageControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageControl1.Location = new System.Drawing.Point(453, 3);
            this.pageControl1.Name = "pageControl1";
            this.pageControl1.Size = new System.Drawing.Size(264, 675);
            this.pageControl1.TabIndex = 8;
            // 
            // blockListControl1
            // 
            this.blockListControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockListControl1.Location = new System.Drawing.Point(3, 3);
            this.blockListControl1.Name = "blockListControl1";
            this.blockListControl1.Size = new System.Drawing.Size(444, 675);
            this.blockListControl1.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 331);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 14);
            this.label1.TabIndex = 8;
            this.label1.Text = "Detect:";
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
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TextBox bookFolder_tb;
        private System.Windows.Forms.Button openBookFolder_btn;
        private System.Windows.Forms.Button saveBook_btn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage6;
        private BlockDetailsControl blockDetailsControl1;
        private PageControl pageControl1;
        private BlockListControl blockListControl1;
        private DetectImageControl detectImageControl1;
        private DetectTitleControl detectTitleControl1;
        private DetectCommentLinkNumberControl detectCommentLinkNumberControl1;
        private DetectGarbageControl detectGarbageControl1;
        private DetectLineControl detectLineControl1;
        private System.Windows.Forms.Label label1;
    }
}