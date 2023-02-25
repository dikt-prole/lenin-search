namespace BookProject.WinForms.Controls
{
    partial class BlockListControl
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
            this.pages_chb = new System.Windows.Forms.CheckBox();
            this.titles_chb = new System.Windows.Forms.CheckBox();
            this.comments_chb = new System.Windows.Forms.CheckBox();
            this.block_lb = new System.Windows.Forms.ListBox();
            this.image_chb = new System.Windows.Forms.CheckBox();
            this.line_chb = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.pages_chb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.titles_chb, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.comments_chb, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.block_lb, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.image_chb, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.line_chb, 4, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(362, 384);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // pages_chb
            // 
            this.pages_chb.AutoSize = true;
            this.pages_chb.Location = new System.Drawing.Point(5, 3);
            this.pages_chb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.pages_chb.Name = "pages_chb";
            this.pages_chb.Size = new System.Drawing.Size(52, 18);
            this.pages_chb.TabIndex = 0;
            this.pages_chb.Text = "Pages";
            this.pages_chb.UseVisualStyleBackColor = true;
            // 
            // titles_chb
            // 
            this.titles_chb.AutoSize = true;
            this.titles_chb.Location = new System.Drawing.Point(65, 3);
            this.titles_chb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.titles_chb.Name = "titles_chb";
            this.titles_chb.Size = new System.Drawing.Size(52, 18);
            this.titles_chb.TabIndex = 1;
            this.titles_chb.Text = "Titles";
            this.titles_chb.UseVisualStyleBackColor = true;
            // 
            // comments_chb
            // 
            this.comments_chb.AutoSize = true;
            this.comments_chb.Location = new System.Drawing.Point(125, 3);
            this.comments_chb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.comments_chb.Name = "comments_chb";
            this.comments_chb.Size = new System.Drawing.Size(80, 18);
            this.comments_chb.TabIndex = 2;
            this.comments_chb.Text = "Comment";
            this.comments_chb.UseVisualStyleBackColor = true;
            // 
            // block_lb
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.block_lb, 5);
            this.block_lb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.block_lb.FormattingEnabled = true;
            this.block_lb.ItemHeight = 15;
            this.block_lb.Location = new System.Drawing.Point(3, 27);
            this.block_lb.Name = "block_lb";
            this.block_lb.Size = new System.Drawing.Size(356, 354);
            this.block_lb.TabIndex = 3;
            // 
            // image_chb
            // 
            this.image_chb.AutoSize = true;
            this.image_chb.Location = new System.Drawing.Point(215, 3);
            this.image_chb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.image_chb.Name = "image_chb";
            this.image_chb.Size = new System.Drawing.Size(59, 18);
            this.image_chb.TabIndex = 4;
            this.image_chb.Text = "Image";
            this.image_chb.UseVisualStyleBackColor = true;
            // 
            // line_chb
            // 
            this.line_chb.AutoSize = true;
            this.line_chb.Location = new System.Drawing.Point(285, 3);
            this.line_chb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.line_chb.Name = "line_chb";
            this.line_chb.Size = new System.Drawing.Size(48, 18);
            this.line_chb.TabIndex = 5;
            this.line_chb.Text = "Line";
            this.line_chb.UseVisualStyleBackColor = true;
            // 
            // BlockListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "BlockListControl";
            this.Size = new System.Drawing.Size(362, 384);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox pages_chb;
        private System.Windows.Forms.CheckBox titles_chb;
        private System.Windows.Forms.CheckBox comments_chb;
        private System.Windows.Forms.ListBox block_lb;
        private System.Windows.Forms.CheckBox image_chb;
        private System.Windows.Forms.CheckBox line_chb;
    }
}
