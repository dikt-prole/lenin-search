
namespace LeninSearch.Ocr
{
    partial class UncoveredContourControl
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
            this.word_tb = new System.Windows.Forms.TextBox();
            this.rectangle_pb = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rectangle_pb)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.word_tb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.rectangle_pb, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(848, 28);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // word_tb
            // 
            this.word_tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.word_tb.Location = new System.Drawing.Point(1, 1);
            this.word_tb.Margin = new System.Windows.Forms.Padding(1);
            this.word_tb.Name = "word_tb";
            this.word_tb.Size = new System.Drawing.Size(98, 23);
            this.word_tb.TabIndex = 0;
            // 
            // rectangle_pb
            // 
            this.rectangle_pb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rectangle_pb.Location = new System.Drawing.Point(101, 1);
            this.rectangle_pb.Margin = new System.Windows.Forms.Padding(1);
            this.rectangle_pb.Name = "rectangle_pb";
            this.rectangle_pb.Size = new System.Drawing.Size(746, 26);
            this.rectangle_pb.TabIndex = 1;
            this.rectangle_pb.TabStop = false;
            // 
            // UncoveredContourControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "UncoveredContourControl";
            this.Size = new System.Drawing.Size(848, 28);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rectangle_pb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox word_tb;
        private System.Windows.Forms.PictureBox rectangle_pb;
    }
}
