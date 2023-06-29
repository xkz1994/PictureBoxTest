// ReSharper disable LocalizableElement

namespace PictureBoxTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            KeyPreview = true; // 开启键盘事件的预览: 获取或设置一个值，该值指示在将键事件传递到具有焦点的控件前，窗体是否将接收此键事件
        }

        private void ButtonOnClick(object sender, EventArgs e)
        {
            if (imageCanvas.Image is null) return;

            var zoomWidth = (float)imageCanvas.Width / imageCanvas.Image.Width;
            var zoomHeight = (float)imageCanvas.Height / imageCanvas.Image.Height;
            imageCanvas.Viewer.SetZoom(Math.Min(zoomWidth, zoomHeight));
        }
    }
}