
namespace LeninSearch.Ocr
{
    partial class SingleImageOcrForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.load_btn = new System.Windows.Forms.Button();
            this.ocr_btn = new System.Windows.Forms.Button();
            this.test_btn = new System.Windows.Forms.Button();
            this.file_tb = new System.Windows.Forms.TextBox();
            this.prev_btn = new System.Windows.Forms.Button();
            this.next_btn = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.initial_pb = new System.Windows.Forms.PictureBox();
            this.processed_pb = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.initial_pb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.processed_pb)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1400, 816);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 6;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.tableLayoutPanel2.Controls.Add(this.load_btn, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.ocr_btn, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.test_btn, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.file_tb, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.prev_btn, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.next_btn, 5, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1400, 43);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // load_btn
            // 
            this.load_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.load_btn.Location = new System.Drawing.Point(3, 4);
            this.load_btn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.load_btn.Name = "load_btn";
            this.load_btn.Size = new System.Drawing.Size(51, 35);
            this.load_btn.TabIndex = 0;
            this.load_btn.Text = "Load";
            this.load_btn.UseVisualStyleBackColor = true;
            // 
            // ocr_btn
            // 
            this.ocr_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ocr_btn.Location = new System.Drawing.Point(60, 4);
            this.ocr_btn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ocr_btn.Name = "ocr_btn";
            this.ocr_btn.Size = new System.Drawing.Size(51, 35);
            this.ocr_btn.TabIndex = 1;
            this.ocr_btn.Text = "OCR";
            this.ocr_btn.UseVisualStyleBackColor = true;
            // 
            // test_btn
            // 
            this.test_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.test_btn.Location = new System.Drawing.Point(117, 4);
            this.test_btn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.test_btn.Name = "test_btn";
            this.test_btn.Size = new System.Drawing.Size(51, 35);
            this.test_btn.TabIndex = 2;
            this.test_btn.Text = "Test";
            this.test_btn.UseVisualStyleBackColor = true;
            // 
            // file_tb
            // 
            this.file_tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.file_tb.Location = new System.Drawing.Point(174, 7);
            this.file_tb.Margin = new System.Windows.Forms.Padding(3, 7, 3, 4);
            this.file_tb.Name = "file_tb";
            this.file_tb.ReadOnly = true;
            this.file_tb.Size = new System.Drawing.Size(1109, 27);
            this.file_tb.TabIndex = 3;
            // 
            // prev_btn
            // 
            this.prev_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prev_btn.Location = new System.Drawing.Point(1287, 1);
            this.prev_btn.Margin = new System.Windows.Forms.Padding(1);
            this.prev_btn.Name = "prev_btn";
            this.prev_btn.Size = new System.Drawing.Size(55, 41);
            this.prev_btn.TabIndex = 4;
            this.prev_btn.Text = "Prev";
            this.prev_btn.UseVisualStyleBackColor = true;
            // 
            // next_btn
            // 
            this.next_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.next_btn.Location = new System.Drawing.Point(1344, 1);
            this.next_btn.Margin = new System.Windows.Forms.Padding(1);
            this.next_btn.Name = "next_btn";
            this.next_btn.Size = new System.Drawing.Size(55, 41);
            this.next_btn.TabIndex = 5;
            this.next_btn.Text = "Next";
            this.next_btn.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.initial_pb, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.processed_pb, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 43);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1400, 730);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // initial_pb
            // 
            this.initial_pb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.initial_pb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.initial_pb.Location = new System.Drawing.Point(3, 4);
            this.initial_pb.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.initial_pb.Name = "initial_pb";
            this.initial_pb.Size = new System.Drawing.Size(694, 722);
            this.initial_pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.initial_pb.TabIndex = 1;
            this.initial_pb.TabStop = false;
            // 
            // processed_pb
            // 
            this.processed_pb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processed_pb.Location = new System.Drawing.Point(703, 4);
            this.processed_pb.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.processed_pb.Name = "processed_pb";
            this.processed_pb.Size = new System.Drawing.Size(694, 722);
            this.processed_pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.processed_pb.TabIndex = 2;
            this.processed_pb.TabStop = false;
            // 
            // SingleImageOcrForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 816);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "SingleImageOcrForm";
            this.Text = "Lenin Search Ocr ";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.initial_pb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.processed_pb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button load_btn;
        private System.Windows.Forms.Button ocr_btn;
        private System.Windows.Forms.PictureBox initial_pb;
        private System.Windows.Forms.Button test_btn;
        private System.Windows.Forms.TextBox file_tb;
        private System.Windows.Forms.Button prev_btn;
        private System.Windows.Forms.Button next_btn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.PictureBox processed_pb;
    }
}

