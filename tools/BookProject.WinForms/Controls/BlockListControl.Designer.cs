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
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.pages_chb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.titles_chb, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.comments_chb, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.block_lb, 0, 1);
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
            this.pages_chb.Size = new System.Drawing.Size(57, 18);
            this.pages_chb.TabIndex = 0;
            this.pages_chb.Text = "Pages";
            this.pages_chb.UseVisualStyleBackColor = true;
            // 
            // titles_chb
            // 
            this.titles_chb.AutoSize = true;
            this.titles_chb.Location = new System.Drawing.Point(85, 3);
            this.titles_chb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.titles_chb.Name = "titles_chb";
            this.titles_chb.Size = new System.Drawing.Size(53, 18);
            this.titles_chb.TabIndex = 1;
            this.titles_chb.Text = "Titles";
            this.titles_chb.UseVisualStyleBackColor = true;
            // 
            // comments_chb
            // 
            this.comments_chb.AutoSize = true;
            this.comments_chb.Location = new System.Drawing.Point(165, 3);
            this.comments_chb.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.comments_chb.Name = "comments_chb";
            this.comments_chb.Size = new System.Drawing.Size(85, 18);
            this.comments_chb.TabIndex = 2;
            this.comments_chb.Text = "Comments";
            this.comments_chb.UseVisualStyleBackColor = true;
            // 
            // block_lb
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.block_lb, 3);
            this.block_lb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.block_lb.FormattingEnabled = true;
            this.block_lb.ItemHeight = 15;
            this.block_lb.Location = new System.Drawing.Point(3, 27);
            this.block_lb.Name = "block_lb";
            this.block_lb.Size = new System.Drawing.Size(356, 354);
            this.block_lb.TabIndex = 3;
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
    }
}
