namespace BookProject.WinForms.Controls
{
    partial class TitleListControl
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
            this.title_lb = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // title_lb
            // 
            this.title_lb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.title_lb.FormattingEnabled = true;
            this.title_lb.ItemHeight = 15;
            this.title_lb.Location = new System.Drawing.Point(0, 0);
            this.title_lb.Name = "title_lb";
            this.title_lb.Size = new System.Drawing.Size(474, 342);
            this.title_lb.TabIndex = 0;
            // 
            // TitleListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.title_lb);
            this.Name = "TitleListControl";
            this.Size = new System.Drawing.Size(474, 342);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox title_lb;
    }
}
