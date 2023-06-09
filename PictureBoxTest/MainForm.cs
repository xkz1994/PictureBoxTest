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

            pictureBox.Paint += PictureBoxOnPaint;
            pictureBox.MouseMove += PictureBoxOnMouseMove;
            pictureBox.MouseDown += PictureBoxOnMouseDown;
            pictureBox.MouseUp += PictureBoxOnMouseUp;
            pictureBox.MouseWheel += PictureBoxOnMouseWheel;
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
            // destRect: Rectangle �ṹ����ָ��������ͼ���λ�úʹ�С(����ڻ�������ֻ��ʾ��ô��, �����ڽ�ͼ)
            // srcRect: Rectangle �ṹ����ָ�� image ������Ҫ���ƵĲ���, ��Ϊ������: ����viewer.Viewport���/��С��: ����ͼƬ������ʾ��������
            e.Graphics.DrawImage(Bitmap, new Rectangle(0, 0, pictureBox.Width, pictureBox.Height), _viewer.Viewport, GraphicsUnit.Pixel);
            _roiElement.Drawing(e.Graphics);
        }

        private void ButtonOnClick(object sender, EventArgs e)
        {
            if (new Rectangle(0, 0, Bitmap.Width, Bitmap.Height).Contains(_roiElement.Rect) == false)
            {
                MessageBox.Show("����ͼƬ��Χ");
                return;
            }

            using var sub = Image.GetSubRect(_roiElement.Rect);
            sub.Save($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}{Guid.NewGuid()}.jpg");
        }
    }
}