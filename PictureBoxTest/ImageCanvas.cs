using System.ComponentModel;

namespace PictureBoxTest
{
    [ToolboxItem(true)]
    [DefaultProperty(nameof(Image))]
    [Description("ROI区域与图片动态显示")]
    public sealed class ImageCanvas : System.Windows.Forms.Control
    {
        public static readonly List<(Point Position, int EdgeLength)> RegionList = new List<(Point, int)>
        {
            (new Point(1590, 624), 300),
            (new Point(1590, 996), 300),
            (new Point(1590, 1356), 300),
            (new Point(1260, 1362), 300),
            (new Point(930, 1356), 300),
            (new Point(588, 264), 300),
            (new Point(936, 258), 300),
            (new Point(1236, 264), 300),
            (new Point(1596, 264), 300),
            (new Point(1920, 1362), 300)
        };

        public static readonly int BinarizationThresholds = 17;
        public static readonly float MatchThreshold = 0.7f;
        public static readonly string MatchTemplatePath = "D:\\mb\\40z.png";

        private Image _image;

        [Category("自定义")]
        [Description("图片")]
        [DefaultValue(typeof(Bitmap), null)]
        public Image Image
        {
            get => _image;
            set
            {
                _image?.Dispose();
                _image = value;
                Refresh();
            }
        }

        [Category("CatAppearance")]
        [Description("ControlCursorDescr")]
        [AmbientValue(null)]
        public override Cursor Cursor
        {
            get => base.Cursor;
            set => base.Cursor = value;
        }

        [Category("自定义")]
        [Description("是否显示ROI")]
        public bool IsShowRoi { get; set; }

        [Category("自定义")]
        [Description("是否显示十字架")]
        public bool IsShowCross { get; set; }

        [Category("自定义")]
        [Description("第一次是否缩放")]
        public bool IsFirstZoom { get; set; }

        [Category("自定义")]
        [Description("移动图片按键类型")]
        public MouseButtons ImageMouseMoveButton
        {
            get => Viewer.MouseMoveButton;
            set => Viewer.MouseMoveButton = value;
        }

        [Category("自定义")]
        [Description("移动Roi按键类型")]
        public MouseButtons RoiMouseMoveButton
        {
            get => RoiElement.MouseMoveButton;
            set => RoiElement.MouseMoveButton = value;
        }

        public Viewer Viewer { get; private set; }

        public RoiElement RoiElement { get; private set; }

        public readonly List<Point> MatchPointList = new List<Point>();

        public ImageCanvas()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            AllowDrop = true;

            IsShowRoi = true;
            IsShowCross = false;
            IsFirstZoom = true;
            Viewer = new Viewer(this, new Point(0, 0));
            RoiElement = new RoiElement(this)
            {
                Rect = new Rectangle(0, 0, 950, 950)
            };

            SizeChanged += ImageCanvasOnSizeChanged;
            Paint += ImageCanvasOnPaint;
            MouseMove += ImageCanvasOnMouseMove;
            MouseDown += ImageCanvasOnMouseDown;
            MouseUp += ImageCanvasOnMouseUp;
            MouseWheel += ImageCanvasOnMouseWheel;
        }

        private void ImageCanvasOnSizeChanged(object sender, EventArgs e)
        {
            Viewer.UpdateViewport();
            // 不能用创建新对象，否则binding失效
            Refresh();
        }

        private void ImageCanvasOnMouseWheel(object sender, MouseEventArgs e)
        {
            Viewer.MouseWheel(e);
            if (IsShowRoi)
                RoiElement.MouseWheel(e);
            Refresh();
        }

        private void ImageCanvasOnMouseUp(object sender, MouseEventArgs e)
        {
            Viewer.MouseUp(e);
            if (IsShowRoi)
                RoiElement.MouseUp(e);
            Refresh();
        }

        private void ImageCanvasOnMouseDown(object sender, MouseEventArgs e)
        {
            Viewer.MouseDown(e);
            if (IsShowRoi)
                RoiElement.MouseDown(e);
            Refresh();
        }

        private void ImageCanvasOnMouseMove(object sender, MouseEventArgs e)
        {
            Viewer.MouseMove(e);
            if (IsShowRoi)
                RoiElement.MouseMove(e);
            if (e.Button != MouseButtons.None)
            {
                // 增加了刷新条件，尝试减少刷新来优化系统性
                Refresh();
            }
        }

        private void ImageCanvasOnPaint(object sender, PaintEventArgs e)
        {
            // destRect: Rectangle 结构，它指定所绘制图像的位置和大小(相对于绘制区域只显示这么大, 类似于截图)
            // srcRect: Rectangle 结构，它指定 image 对象中要绘制的部分, 因为缩放了: 所以viewer.Viewport变大/变小了: 所以图片可以显示了且缩放
            if (Image is null) return;
            if (IsFirstZoom)
            {
                var zoomWidth = (float)Width / Image.Width;
                var zoomHeight = (float)Height / Image.Height;
                Viewer.Zero = new Point(0, 0);
                if (zoomWidth < zoomHeight) // 宽度合适
                    Viewer.Zero.Offset(0, (int)((Height - Image.Height * zoomWidth) / 2));
                else // 高度合适
                    Viewer.Zero.Offset((int)((Width - Image.Width * zoomHeight) / 2), 0);
                Viewer.SetZoom(Math.Min(zoomWidth, zoomHeight));

                IsFirstZoom = false;
            }

            e.Graphics.DrawImage(Image, new Rectangle(0, 0, Width, Height), Viewer.Viewport, GraphicsUnit.Pixel);

            if (IsShowRoi)
                RoiElement.Drawing(e.Graphics);
            if (IsShowCross == false) return;

            // 画十字架
            var horizontalP1 = new Point(Image.Width / 2, 0);
            var horizontalP2 = new Point(Image.Width / 2, Image.Height);
            e.Graphics.DrawLine(Pens.Red, Viewer.LocalToShow(horizontalP1), Viewer.LocalToShow(horizontalP2));

            var verticalP1 = new Point(0, Image.Height / 2);
            var verticalP2 = new Point(Image.Width, Image.Height / 2);
            e.Graphics.DrawLine(Pens.Red, Viewer.LocalToShow(verticalP1), Viewer.LocalToShow(verticalP2));

            foreach (var (position, edgeLength) in RegionList)
            {
                e.Graphics.DrawRectangle(Pens.Green, new Rectangle(Viewer.LocalToShow(position), Viewer.LocalToShow(new Size(edgeLength, edgeLength))));
            }

            foreach (var point in MatchPointList)
            {
                // 画十字架
                horizontalP1 = new Point(point.X, point.Y - 100);
                horizontalP2 = new Point(point.X, point.Y + 100);
                e.Graphics.DrawLine(Pens.Blue, Viewer.LocalToShow(horizontalP1), Viewer.LocalToShow(horizontalP2));
                verticalP1 = new Point(point.X - 100, point.Y);
                verticalP2 = new Point(point.X + 100, point.Y);
                e.Graphics.DrawLine(Pens.Blue, Viewer.LocalToShow(verticalP1), Viewer.LocalToShow(verticalP2));
            }

            /*//画校准点
            var middleX = Image.Width / 2;
            var middleY = Image.Height / 2;
            var centerHorizontalP1 = new Point(middleX - (int)(Image.Width * 0.01 / Viewer.Zoom), middleY);
            var centerHorizontalP2 = new Point(middleX + (int)(Image.Width * 0.01 / Viewer.Zoom), middleY);
            e.Graphics.DrawLine(Pens.Green, Viewer.LocalToShow(centerHorizontalP1), Viewer.LocalToShow(centerHorizontalP2));

            var centerVerticalP1 = new Point(middleX, middleY - (int)(Image.Width * 0.01 / Viewer.Zoom));
            var centerVerticalP2 = new Point(middleX, middleY + (int)(Image.Width * 0.01 / Viewer.Zoom));
            e.Graphics.DrawLine(Pens.Green, Viewer.LocalToShow(centerVerticalP1), Viewer.LocalToShow(centerVerticalP2));*/
        }

        public void ImageCanvasOnKeyDown(KeyEventArgs e)
        {
            if (IsShowRoi)
                RoiElement.KeyDown(e);
            Refresh();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Image?.Dispose();
            // 在窗口句柄销毁后执行的操作
            base.OnHandleDestroyed(e);
        }
    }
}