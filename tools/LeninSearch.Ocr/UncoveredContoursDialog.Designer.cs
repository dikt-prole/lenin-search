
namespace LeninSearch.Ocr
{
    partial class UncoveredContoursDialog
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.contours_flp = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.pageSize_nud = new System.Windows.Forms.NumericUpDown();
            this.ok_btn = new System.Windows.Forms.Button();
            this.page_nud = new System.Windows.Forms.NumericUpDown();
            this.totalPages_lbl = new System.Windows.Forms.Label();
            this.next_btn = new System.Windows.Forms.Button();
            this.prev_btn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pageSize_nud)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.page_nud)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.contours_flp, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(855, 677);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // contours_flp
            // 
            this.contours_flp.AutoScroll = true;
            this.contours_flp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contours_flp.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.contours_flp.Location = new System.Drawing.Point(3, 3);
            this.contours_flp.Name = "contours_flp";
            this.contours_flp.Size = new System.Drawing.Size(849, 643);
            this.contours_flp.TabIndex = 1;
            this.contours_flp.WrapContents = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 9;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.ok_btn, 8, 0);
            this.tableLayoutPanel2.Controls.Add(this.pageSize_nud, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.next_btn, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.totalPages_lbl, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.page_nud, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.prev_btn, 3, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 649);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(855, 28);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // pageSize_nud
            // 
            this.pageSize_nud.Location = new System.Drawing.Point(73, 3);
            this.pageSize_nud.Name = "pageSize_nud";
            this.pageSize_nud.Size = new System.Drawing.Size(44, 23);
            this.pageSize_nud.TabIndex = 0;
            // 
            // ok_btn
            // 
            this.ok_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ok_btn.Location = new System.Drawing.Point(756, 1);
            this.ok_btn.Margin = new System.Windows.Forms.Padding(1);
            this.ok_btn.Name = "ok_btn";
            this.ok_btn.Size = new System.Drawing.Size(98, 26);
            this.ok_btn.TabIndex = 0;
            this.ok_btn.Text = "OK";
            this.ok_btn.UseVisualStyleBackColor = true;
            // 
            // page_nud
            // 
            this.page_nud.Location = new System.Drawing.Point(383, 3);
            this.page_nud.Name = "page_nud";
            this.page_nud.Size = new System.Drawing.Size(44, 23);
            this.page_nud.TabIndex = 2;
            // 
            // totalPages_lbl
            // 
            this.totalPages_lbl.AutoSize = true;
            this.totalPages_lbl.Location = new System.Drawing.Point(433, 5);
            this.totalPages_lbl.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.totalPages_lbl.Name = "totalPages_lbl";
            this.totalPages_lbl.Size = new System.Drawing.Size(33, 15);
            this.totalPages_lbl.TabIndex = 3;
            this.totalPages_lbl.Text = "of 99";
            // 
            // next_btn
            // 
            this.next_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.next_btn.Location = new System.Drawing.Point(481, 1);
            this.next_btn.Margin = new System.Windows.Forms.Padding(1);
            this.next_btn.Name = "next_btn";
            this.next_btn.Size = new System.Drawing.Size(58, 26);
            this.next_btn.TabIndex = 4;
            this.next_btn.Text = ">";
            this.next_btn.UseVisualStyleBackColor = true;
            // 
            // prev_btn
            // 
            this.prev_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prev_btn.Location = new System.Drawing.Point(321, 1);
            this.prev_btn.Margin = new System.Windows.Forms.Padding(1);
            this.prev_btn.Name = "prev_btn";
            this.prev_btn.Size = new System.Drawing.Size(58, 26);
            this.prev_btn.TabIndex = 5;
            this.prev_btn.Text = "<";
            this.prev_btn.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Page size:";
            // 
            // UncoveredContoursDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(855, 677);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "UncoveredContoursDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Uncovered Contours";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pageSize_nud)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.page_nud)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button ok_btn;
        private System.Windows.Forms.FlowLayoutPanel contours_flp;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.NumericUpDown pageSize_nud;
        private System.Windows.Forms.NumericUpDown page_nud;
        private System.Windows.Forms.Label totalPages_lbl;
        private System.Windows.Forms.Button next_btn;
        private System.Windows.Forms.Button prev_btn;
        private System.Windows.Forms.Label label1;
    }
}