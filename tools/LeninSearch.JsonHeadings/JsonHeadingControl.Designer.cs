
namespace LeninSearch.JsonHeadings
{
    partial class JsonHeadingControl
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
            this.enabled_chb = new System.Windows.Forms.CheckBox();
            this.level_nud = new System.Windows.Forms.NumericUpDown();
            this.text_tb = new System.Windows.Forms.TextBox();
            this.insert_btn = new System.Windows.Forms.Button();
            this.index_nud = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.level_nud)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.index_nud)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.Controls.Add(this.enabled_chb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.insert_btn, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.text_tb, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.level_nud, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.index_nud, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(761, 28);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // enabled_chb
            // 
            this.enabled_chb.AutoSize = true;
            this.enabled_chb.Location = new System.Drawing.Point(7, 7);
            this.enabled_chb.Margin = new System.Windows.Forms.Padding(7, 7, 3, 3);
            this.enabled_chb.Name = "enabled_chb";
            this.enabled_chb.Size = new System.Drawing.Size(15, 14);
            this.enabled_chb.TabIndex = 0;
            this.enabled_chb.UseVisualStyleBackColor = true;
            // 
            // level_nud
            // 
            this.level_nud.Location = new System.Drawing.Point(115, 3);
            this.level_nud.Name = "level_nud";
            this.level_nud.Size = new System.Drawing.Size(44, 23);
            this.level_nud.TabIndex = 1;
            // 
            // text_tb
            // 
            this.text_tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.text_tb.Location = new System.Drawing.Point(165, 3);
            this.text_tb.Name = "text_tb";
            this.text_tb.Size = new System.Drawing.Size(561, 23);
            this.text_tb.TabIndex = 2;
            // 
            // insert_btn
            // 
            this.insert_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.insert_btn.Location = new System.Drawing.Point(730, 2);
            this.insert_btn.Margin = new System.Windows.Forms.Padding(1, 2, 1, 1);
            this.insert_btn.Name = "insert_btn";
            this.insert_btn.Size = new System.Drawing.Size(30, 25);
            this.insert_btn.TabIndex = 3;
            this.insert_btn.Text = "+";
            this.insert_btn.UseVisualStyleBackColor = true;
            // 
            // index_nud
            // 
            this.index_nud.Location = new System.Drawing.Point(35, 3);
            this.index_nud.Name = "index_nud";
            this.index_nud.Size = new System.Drawing.Size(74, 23);
            this.index_nud.TabIndex = 4;
            // 
            // JsonHeadingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "JsonHeadingControl";
            this.Size = new System.Drawing.Size(761, 28);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.level_nud)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.index_nud)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox enabled_chb;
        private System.Windows.Forms.NumericUpDown level_nud;
        private System.Windows.Forms.TextBox text_tb;
        private System.Windows.Forms.Button insert_btn;
        private System.Windows.Forms.NumericUpDown index_nud;
    }
}
