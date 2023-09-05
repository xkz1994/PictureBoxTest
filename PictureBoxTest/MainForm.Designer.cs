namespace PictureBoxTest
{
    partial class MainForm
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
            imageCanvas = new ImageCanvas();
            SuspendLayout();
            // 
            // imageCanvas
            // 
            imageCanvas.AllowDrop = true;
            imageCanvas.Dock = DockStyle.Fill;
            imageCanvas.Location = new Point(0, 0);
            imageCanvas.Name = "imageCanvas";
            imageCanvas.Size = new Size(1200, 635);
            imageCanvas.TabIndex = 0;
            imageCanvas.TabStop = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 635);
            Controls.Add(imageCanvas);
            Name = "MainForm";
            Text = "MainForm";
            Load += MainForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private ImageCanvas imageCanvas;
    }
}