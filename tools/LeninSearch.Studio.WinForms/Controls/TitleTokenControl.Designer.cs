
namespace LeninSearch.Studio.WinForms.Controls
{
    partial class TitleTokenControl
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
            this.token_tb = new System.Windows.Forms.TextBox();
            this.up_btn = new System.Windows.Forms.Button();
            this.remove_btn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.token_tb, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.up_btn, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.remove_btn, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(324, 49);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // token_tb
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.token_tb, 3);
            this.token_tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.token_tb.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.token_tb.Location = new System.Drawing.Point(0, 20);
            this.token_tb.Margin = new System.Windows.Forms.Padding(0);
            this.token_tb.Name = "token_tb";
            this.token_tb.Size = new System.Drawing.Size(324, 29);
            this.token_tb.TabIndex = 0;
            // 
            // up_btn
            // 
            this.up_btn.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.up_btn.Location = new System.Drawing.Point(284, 0);
            this.up_btn.Margin = new System.Windows.Forms.Padding(0);
            this.up_btn.Name = "up_btn";
            this.up_btn.Size = new System.Drawing.Size(20, 20);
            this.up_btn.TabIndex = 1;
            this.up_btn.Text = "^";
            this.up_btn.UseVisualStyleBackColor = false;
            // 
            // remove_btn
            // 
            this.remove_btn.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.remove_btn.Location = new System.Drawing.Point(304, 0);
            this.remove_btn.Margin = new System.Windows.Forms.Padding(0);
            this.remove_btn.Name = "remove_btn";
            this.remove_btn.Size = new System.Drawing.Size(20, 20);
            this.remove_btn.TabIndex = 2;
            this.remove_btn.Text = "X";
            this.remove_btn.UseVisualStyleBackColor = false;
            // 
            // TitleTokenControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TitleTokenControl";
            this.Size = new System.Drawing.Size(324, 49);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox token_tb;
        private System.Windows.Forms.Button up_btn;
        private System.Windows.Forms.Button remove_btn;
    }
}
