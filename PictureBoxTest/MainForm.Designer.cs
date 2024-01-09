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
            button = new Button();
            textBoxWidth = new TextBox();
            textBoxHeight = new TextBox();
            buttonTest = new Button();
            buttonPath = new Button();
            SuspendLayout();
            // 
            // imageCanvas
            // 
            imageCanvas.AllowDrop = true;
            imageCanvas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            imageCanvas.ImageMouseMoveButton = MouseButtons.Middle;
            imageCanvas.IsAutoZoom = true;
            imageCanvas.IsShowCross = true;
            imageCanvas.IsShowRoi = false;
            imageCanvas.Location = new Point(0, 0);
            imageCanvas.Name = "imageCanvas";
            imageCanvas.RoiMouseMoveButton = MouseButtons.Right;
            imageCanvas.Size = new Size(1101, 631);
            imageCanvas.TabIndex = 0;
            imageCanvas.TabStop = false;
            // 
            // button
            // 
            button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button.Location = new Point(1107, 12);
            button.Name = "button";
            button.Size = new Size(81, 30);
            button.TabIndex = 1;
            button.Text = "Save";
            button.UseVisualStyleBackColor = true;
            button.Click += ButtonOnClick;
            // 
            // textBoxWidth
            // 
            textBoxWidth.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBoxWidth.Location = new Point(1107, 53);
            textBoxWidth.Name = "textBoxWidth";
            textBoxWidth.Size = new Size(81, 23);
            textBoxWidth.TabIndex = 2;
            textBoxWidth.Text = "950";
            // 
            // textBoxHeight
            // 
            textBoxHeight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBoxHeight.Location = new Point(1107, 87);
            textBoxHeight.Name = "textBoxHeight";
            textBoxHeight.Size = new Size(81, 23);
            textBoxHeight.TabIndex = 3;
            textBoxHeight.Text = "950";
            // 
            // buttonTest
            // 
            buttonTest.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonTest.Location = new Point(1107, 162);
            buttonTest.Name = "buttonTest";
            buttonTest.Size = new Size(81, 30);
            buttonTest.TabIndex = 4;
            buttonTest.Text = "Test";
            buttonTest.UseVisualStyleBackColor = true;
            buttonTest.Click += ButtonTestClick;
            // 
            // buttonPath
            // 
            buttonPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonPath.Location = new Point(1107, 121);
            buttonPath.Name = "buttonPath";
            buttonPath.Size = new Size(81, 30);
            buttonPath.TabIndex = 5;
            buttonPath.Text = "Path";
            buttonPath.UseVisualStyleBackColor = true;
            buttonPath.Click += ButtonPathClick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 635);
            Controls.Add(buttonPath);
            Controls.Add(buttonTest);
            Controls.Add(textBoxHeight);
            Controls.Add(textBoxWidth);
            Controls.Add(button);
            Controls.Add(imageCanvas);
            Name = "MainForm";
            Text = "MainForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ImageCanvas imageCanvas;
        private Button button;
        private TextBox textBoxWidth;
        private TextBox textBoxHeight;
        private Button buttonTest;
        private Button buttonPath;
    }
}