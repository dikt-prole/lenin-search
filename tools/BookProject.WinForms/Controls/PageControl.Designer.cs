namespace BookProject.WinForms.Controls
{
    partial class PageControl
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
            this.page_pb = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.page_pb)).BeginInit();
            this.SuspendLayout();
            // 
            // page_pb
            // 
            this.page_pb.BackColor = System.Drawing.Color.Silver;
            this.page_pb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.page_pb.Location = new System.Drawing.Point(0, 0);
            this.page_pb.Name = "page_pb";
            this.page_pb.Size = new System.Drawing.Size(794, 462);
            this.page_pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.page_pb.TabIndex = 2;
            this.page_pb.TabStop = false;
            // 
            // PageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.page_pb);
            this.Name = "PageControl";
            this.Size = new System.Drawing.Size(794, 462);
            ((System.ComponentModel.ISupportInitialize)(this.page_pb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox page_pb;
    }
}
