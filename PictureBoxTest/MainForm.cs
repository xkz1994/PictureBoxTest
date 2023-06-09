using Emgu.CV;
using Emgu.CV.Structure;

// ReSharper disable LocalizableElement

namespace PictureBoxTest
{
    public partial class MainForm : Form
    {
        private static readonly Image<Bgr, byte> Image = new("origin.jpg");
        private static readonly Bitmap Bitmap = Image.ToBitmap();

        private Viewer _viewer;
        private RoiElement _roiElement;

        public MainForm()
        {
            InitializeComponent();
            _viewer = new Viewer(pictureBox, new Point(0, 0));
            _roiElement = new RoiElement(_viewer, pictureBox)
            {
                Rect = new Rectangle(0, 0, 950, 950)
            };
            SizeChanged += OnSizeChanged;
            KeyPreview = true; // 开启键盘事件的预览: 获取或设置一个值，该值指示在将键事件传递到具有焦点的控件前，窗体是否将接收此键事件
            KeyDown += OnKeyDown;

            pictureBox.Paint += PictureBoxOnPaint;
            pictureBox.MouseMove += PictureBoxOnMouseMove;
            pictureBox.MouseDown += PictureBoxOnMouseDown;
            pictureBox.MouseUp += PictureBoxOnMouseUp;
            pictureBox.MouseWheel += PictureBoxOnMouseWheel;

            textBoxWidth.KeyDown += TextBoxWidthOnKeyDown;
            textBoxHeight.KeyDown += TextBoxHeightOnKeyDown;
        }

        private void TextBoxHeightOnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            if (int.TryParse(textBoxHeight.Text, out var height) == false)
            {
                MessageBox.Show("请输入数字");
                return;
            }

            _roiElement.Rect.Height = height;
            pictureBox.Refresh();
        }

        private void TextBoxWidthOnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            if (int.TryParse(textBoxWidth.Text, out var width) == false)
            {
                MessageBox.Show("请输入数字");
                return;
            }

            _roiElement.Rect.Width = width;
            pictureBox.Refresh();
        }

        private void OnSizeChanged(object? sender, EventArgs e)
        {
            _viewer = new Viewer(pictureBox, new Point(0, 0));
            _roiElement = new RoiElement(_viewer, pictureBox)
            {
                Rect = new Rectangle(0, 0, 950, 950)
            };
            pictureBox.Refresh();
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            _roiElement.KeyDown(e);
            pictureBox.Refresh();
        }

        private void PictureBoxOnMouseWheel(object? sender, MouseEventArgs e)
        {
            _viewer.MouseWheel(e);
            _roiElement.MouseWheel(e);
            pictureBox.Refresh();
        }

        private void PictureBoxOnMouseUp(object? sender, MouseEventArgs e)
        {
            _viewer.MouseUp(e);
            _roiElement.MouseUp(e);
            pictureBox.Refresh();
        }

        private void PictureBoxOnMouseDown(object? sender, MouseEventArgs e)
        {
            _viewer.MouseDown(e);
            _roiElement.MouseDown(e);
            pictureBox.Refresh();
        }

        private void PictureBoxOnMouseMove(object? sender, MouseEventArgs e)
        {
            _viewer.MouseMove(e);
            _roiElement.MouseMove(e);
            pictureBox.Refresh();
        }

        private void PictureBoxOnPaint(object? sender, PaintEventArgs e)
        {
            // destRect: Rectangle 结构，它指定所绘制图像的位置和大小(相对于绘制区域只显示这么大, 类似于截图)
            // srcRect: Rectangle 结构，它指定 image 对象中要绘制的部分, 因为缩放了: 所以viewer.Viewport变大/变小了: 所以图片可以显示了且缩放
            e.Graphics.DrawImage(Bitmap, new Rectangle(0, 0, pictureBox.Width, pictureBox.Height), _viewer.Viewport, GraphicsUnit.Pixel);
            _roiElement.Drawing(e.Graphics);
        }

        private void ButtonOnClick(object sender, EventArgs e)
        {
            if (new Rectangle(0, 0, Bitmap.Width, Bitmap.Height).Contains(_roiElement.Rect) == false)
            {
                MessageBox.Show("超过图片范围");
                return;
            }

            using var sub = Image.GetSubRect(_roiElement.Rect);
            sub.Save($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{Guid.NewGuid()}.jpg");
        }
    }
}