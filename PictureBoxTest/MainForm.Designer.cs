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
            pictureBox = new PictureBox();
            button = new Button();
            textBoxWidth = new TextBox();
            textBoxHeight = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            SuspendLayout();
            // 
            // pictureBox
            // 
            pictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox.Location = new Point(0, 0);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(1101, 631);
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
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
            textBoxWidth.Location = new Point(1107, 60);
            textBoxWidth.Name = "textBoxWidth";
            textBoxWidth.Size = new Size(81, 23);
            textBoxWidth.TabIndex = 2;
            textBoxWidth.Text = "950";
            // 
            // textBoxHeight
            // 
            textBoxHeight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBoxHeight.Location = new Point(1107, 98);
            textBoxHeight.Name = "textBoxHeight";
            textBoxHeight.Size = new Size(81, 23);
            textBoxHeight.TabIndex = 3;
            textBoxHeight.Text = "950";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 635);
            Controls.Add(textBoxHeight);
            Controls.Add(textBoxWidth);
            Controls.Add(button);
            Controls.Add(pictureBox);
            Name = "MainForm";
            Text = "MainForm";
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox;
        private Button button;
        private TextBox textBoxWidth;
        private TextBox textBoxHeight;
    }
}