namespace BookProject.WinForms.Controls.Detect
{
    partial class DetectLineControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.test_btn = new System.Windows.Forms.Button();
            this.detect_btn = new System.Windows.Forms.Button();
            this.minIndent_nud = new System.Windows.Forms.NumericUpDown();
            this.save_btn = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minIndent_nud)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.test_btn, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.detect_btn, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.minIndent_nud, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.save_btn, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(467, 344);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Min Indent:";
            // 
            // test_btn
            // 
            this.test_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.test_btn.Location = new System.Drawing.Point(1, 85);
            this.test_btn.Margin = new System.Windows.Forms.Padding(1);
            this.test_btn.Name = "test_btn";
            this.test_btn.Size = new System.Drawing.Size(98, 26);
            this.test_btn.TabIndex = 6;
            this.test_btn.Text = "Test";
            this.test_btn.UseVisualStyleBackColor = true;
            // 
            // detect_btn
            // 
            this.detect_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detect_btn.Location = new System.Drawing.Point(101, 85);
            this.detect_btn.Margin = new System.Windows.Forms.Padding(1);
            this.detect_btn.Name = "detect_btn";
            this.detect_btn.Size = new System.Drawing.Size(98, 26);
            this.detect_btn.TabIndex = 7;
            this.detect_btn.Text = "Detect";
            this.detect_btn.UseVisualStyleBackColor = true;
            // 
            // minIndent_nud
            // 
            this.minIndent_nud.Dock = System.Windows.Forms.DockStyle.Fill;
            this.minIndent_nud.Location = new System.Drawing.Point(103, 3);
            this.minIndent_nud.Name = "minIndent_nud";
            this.minIndent_nud.Size = new System.Drawing.Size(94, 23);
            this.minIndent_nud.TabIndex = 8;
            // 
            // save_btn
            // 
            this.save_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.save_btn.Location = new System.Drawing.Point(201, 85);
            this.save_btn.Margin = new System.Windows.Forms.Padding(1);
            this.save_btn.Name = "save_btn";
            this.save_btn.Size = new System.Drawing.Size(98, 26);
            this.save_btn.TabIndex = 16;
            this.save_btn.Text = "Save";
            this.save_btn.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.progressBar1, 4);
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(1, 113);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(1);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(465, 3);
            this.progressBar1.TabIndex = 19;
            this.progressBar1.Value = 40;
            // 
            // DetectLineControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DetectLineControl";
            this.Size = new System.Drawing.Size(467, 344);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minIndent_nud)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button test_btn;
        private System.Windows.Forms.Button detect_btn;
        private System.Windows.Forms.NumericUpDown minIndent_nud;
        private System.Windows.Forms.Button save_btn;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}
