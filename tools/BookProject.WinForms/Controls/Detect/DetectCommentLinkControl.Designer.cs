namespace BookProject.WinForms.Controls.Detect
{
    partial class DetectCommentLinkControl
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
            this.test_btn = new System.Windows.Forms.Button();
            this.detect_btn = new System.Windows.Forms.Button();
            this.save_btn = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label7 = new System.Windows.Forms.Label();
            this.lineGaussSigma1_nud = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.lineGaussSigma2_nud = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.topDeltaMax_nud = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.bottomDeltaMin_nud = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.addPadding_nud = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lineGaussSigma1_nud)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineGaussSigma2_nud)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.topDeltaMax_nud)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bottomDeltaMin_nud)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.addPadding_nud)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.test_btn, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.detect_btn, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.save_btn, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lineGaussSigma1_nud, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lineGaussSigma2_nud, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.topDeltaMax_nud, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.bottomDeltaMin_nud, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label12, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.addPadding_nud, 1, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(598, 416);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // test_btn
            // 
            this.test_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.test_btn.Location = new System.Drawing.Point(1, 141);
            this.test_btn.Margin = new System.Windows.Forms.Padding(1);
            this.test_btn.Name = "test_btn";
            this.test_btn.Size = new System.Drawing.Size(128, 26);
            this.test_btn.TabIndex = 6;
            this.test_btn.Text = "Test";
            this.test_btn.UseVisualStyleBackColor = true;
            // 
            // detect_btn
            // 
            this.detect_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detect_btn.Location = new System.Drawing.Point(131, 141);
            this.detect_btn.Margin = new System.Windows.Forms.Padding(1);
            this.detect_btn.Name = "detect_btn";
            this.detect_btn.Size = new System.Drawing.Size(58, 26);
            this.detect_btn.TabIndex = 7;
            this.detect_btn.Text = "Detect";
            this.detect_btn.UseVisualStyleBackColor = true;
            // 
            // save_btn
            // 
            this.save_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.save_btn.Location = new System.Drawing.Point(191, 141);
            this.save_btn.Margin = new System.Windows.Forms.Padding(1);
            this.save_btn.Name = "save_btn";
            this.save_btn.Size = new System.Drawing.Size(128, 26);
            this.save_btn.TabIndex = 16;
            this.save_btn.Text = "Save";
            this.save_btn.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.progressBar1, 4);
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(1, 169);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(1);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(596, 3);
            this.progressBar1.TabIndex = 27;
            this.progressBar1.Value = 40;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 5);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 15);
            this.label7.TabIndex = 14;
            this.label7.Text = "Line Gauss Sigma1:";
            // 
            // lineGaussSigma1_nud
            // 
            this.lineGaussSigma1_nud.Location = new System.Drawing.Point(133, 3);
            this.lineGaussSigma1_nud.Name = "lineGaussSigma1_nud";
            this.lineGaussSigma1_nud.Size = new System.Drawing.Size(54, 23);
            this.lineGaussSigma1_nud.TabIndex = 15;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 33);
            this.label8.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(108, 15);
            this.label8.TabIndex = 17;
            this.label8.Text = "Line Gauss Sigma2:";
            // 
            // lineGaussSigma2_nud
            // 
            this.lineGaussSigma2_nud.Location = new System.Drawing.Point(133, 31);
            this.lineGaussSigma2_nud.Name = "lineGaussSigma2_nud";
            this.lineGaussSigma2_nud.Size = new System.Drawing.Size(54, 23);
            this.lineGaussSigma2_nud.TabIndex = 18;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 61);
            this.label9.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 15);
            this.label9.TabIndex = 19;
            this.label9.Text = "Top Delta Max:";
            // 
            // topDeltaMax_nud
            // 
            this.topDeltaMax_nud.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.topDeltaMax_nud.Location = new System.Drawing.Point(133, 59);
            this.topDeltaMax_nud.Name = "topDeltaMax_nud";
            this.topDeltaMax_nud.Size = new System.Drawing.Size(54, 23);
            this.topDeltaMax_nud.TabIndex = 22;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 89);
            this.label11.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(104, 15);
            this.label11.TabIndex = 21;
            this.label11.Text = "Bottom Delta Min:";
            // 
            // bottomDeltaMin_nud
            // 
            this.bottomDeltaMin_nud.Location = new System.Drawing.Point(133, 87);
            this.bottomDeltaMin_nud.Name = "bottomDeltaMin_nud";
            this.bottomDeltaMin_nud.Size = new System.Drawing.Size(54, 23);
            this.bottomDeltaMin_nud.TabIndex = 28;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 117);
            this.label12.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(79, 15);
            this.label12.TabIndex = 25;
            this.label12.Text = "Add Padding:";
            // 
            // addPadding_nud
            // 
            this.addPadding_nud.Location = new System.Drawing.Point(133, 115);
            this.addPadding_nud.Name = "addPadding_nud";
            this.addPadding_nud.Size = new System.Drawing.Size(54, 23);
            this.addPadding_nud.TabIndex = 26;
            // 
            // DetectCommentLinkNumberControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DetectCommentLinkControl";
            this.Size = new System.Drawing.Size(598, 416);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lineGaussSigma1_nud)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineGaussSigma2_nud)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.topDeltaMax_nud)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bottomDeltaMin_nud)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.addPadding_nud)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button test_btn;
        private System.Windows.Forms.Button detect_btn;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown lineGaussSigma1_nud;
        private System.Windows.Forms.Button save_btn;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown lineGaussSigma2_nud;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown topDeltaMax_nud;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown addPadding_nud;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.NumericUpDown bottomDeltaMin_nud;
    }
}
