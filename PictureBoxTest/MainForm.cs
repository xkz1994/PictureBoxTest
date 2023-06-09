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

        /// <summary>
        /// AOI��ͼ�����ƶ�����ʼ��
        /// </summary>
        private Point _aoiMoveStartPoint = Point.Empty;

        /// <summary>
        /// �������Ƶ�AOI��ͼ��������
        /// </summary>
        private Rectangle _aoiRect = new(0, 0, 950, 950);

        public MainForm()
        {
            InitializeComponent();
            _viewer = new Viewer(pictureBox, new Point(0, 0));
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
            pictureBox.Refresh();
        }

        private void PictureBoxOnMouseWheel(object? sender, MouseEventArgs e)
        {
            _viewer.MouseWheel(e);
            pictureBox.Refresh();
        }

        private void PictureBoxOnMouseUp(object? sender, MouseEventArgs e)
        {
            _viewer.MouseUp(e);
            pictureBox.Refresh();
        }

        private void PictureBoxOnMouseDown(object? sender, MouseEventArgs e)
        {
            _viewer.MouseDown(e);
            if (e.Button == MouseButtons.Right)
                _aoiMoveStartPoint = _viewer.MousePointToLocal(e.Location); // ��������任����ʾ�����

            pictureBox.Refresh();
        }

        private void PictureBoxOnMouseMove(object? sender, MouseEventArgs e)
        {
            _viewer.MouseMove(e);
            if (e.Button == MouseButtons.Right)
                if (_aoiRect.Contains(_aoiMoveStartPoint))
                {
                    var endMove = _viewer.MousePointToLocal(e.Location);
                    var xMove = endMove.X - _aoiMoveStartPoint.X;
                    var yMove = endMove.Y - _aoiMoveStartPoint.Y;
                    _aoiRect.Offset(xMove, yMove);
                    _aoiMoveStartPoint = endMove;
                }

            pictureBox.Refresh();
        }

        private void PictureBoxOnPaint(object? sender, PaintEventArgs e)
        {
            //  ���أ�ͼֽ�����α任����ʾ����
            var localToShow = _viewer.LocalToShow(_aoiRect);

            // destRect: Rectangle �ṹ����ָ��������ͼ���λ�úʹ�С(����ڻ�������ֻ��ʾ��ô��, �����ڽ�ͼ)
            // srcRect: Rectangle �ṹ����ָ�� image ������Ҫ���ƵĲ���, ��Ϊ������: ����viewer.Viewport���/��С��: ����ͼƬ������ʾ��������
            e.Graphics.DrawImage(Bitmap, new Rectangle(0, 0, pictureBox.Width, pictureBox.Height), _viewer.Viewport, GraphicsUnit.Pixel);
            e.Graphics.DrawRectangle(new Pen(Color.Red), localToShow);
            e.Graphics.DrawLine(new Pen(Color.Red), localToShow.X + localToShow.Width / 2, localToShow.Y, localToShow.X + localToShow.Width / 2, localToShow.Y + localToShow.Height);
            e.Graphics.DrawLine(new Pen(Color.Red), localToShow.X, localToShow.Y + localToShow.Height / 2, localToShow.X + localToShow.Width, localToShow.Y + localToShow.Height / 2);
        }

        private void ButtonOnClick(object sender, EventArgs e)
        {
            if (new Rectangle(0, 0, Bitmap.Width, Bitmap.Height).Contains(_aoiRect) == false)
            {
                MessageBox.Show("����ͼƬ��Χ");
                return;
            }

            using var sub = Image.GetSubRect(_aoiRect);
            sub.Save($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}{Guid.NewGuid()}.jpg");
        }
    }
}