
namespace LeninSearch.YtEditor
{
    partial class ParagraphControl
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
            this.yt_btn = new System.Windows.Forms.Button();
            this.paragraph_tb = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Controls.Add(this.yt_btn, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.paragraph_tb, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 339F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(936, 339);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // yt_btn
            // 
            this.yt_btn.Location = new System.Drawing.Point(886, 0);
            this.yt_btn.Margin = new System.Windows.Forms.Padding(0);
            this.yt_btn.Name = "yt_btn";
            this.yt_btn.Size = new System.Drawing.Size(50, 23);
            this.yt_btn.TabIndex = 0;
            this.yt_btn.Text = ">";
            this.yt_btn.UseVisualStyleBackColor = true;
            this.yt_btn.Click += new System.EventHandler(this.yt_btn_Click);
            // 
            // paragraph_tb
            // 
            this.paragraph_tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paragraph_tb.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.paragraph_tb.Location = new System.Drawing.Point(0, 0);
            this.paragraph_tb.Margin = new System.Windows.Forms.Padding(0);
            this.paragraph_tb.Multiline = true;
            this.paragraph_tb.Name = "paragraph_tb";
            this.paragraph_tb.Size = new System.Drawing.Size(886, 339);
            this.paragraph_tb.TabIndex = 0;
            // 
            // ParagraphControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ParagraphControl";
            this.Size = new System.Drawing.Size(936, 339);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox paragraph_tb;
        private System.Windows.Forms.Button yt_btn;
    }
}
