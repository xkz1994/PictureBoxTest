// ReSharper disable LocalizableElement

using Emgu.CV;
using Emgu.CV.Structure;

namespace PictureBoxTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            KeyPreview = true; // 开启键盘事件的预览: 获取或设置一个值，该值指示在将键事件传递到具有焦点的控件前，窗体是否将接收此键事件
            KeyDown += OnKeyDown;
            Load += OnLoad;
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            imageCanvas.ImageCanvasOnKeyDown(e);
            imageCanvas.Refresh();
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            // imageCanvas.Image = Image.ToBitmap();
            // imageCanvas.Refresh();
        }

        private void ButtonOnClick(object sender, EventArgs e)
        {
        }
    }
}