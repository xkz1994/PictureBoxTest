using Emgu.CV;
using Emgu.CV.Structure;

namespace PictureBoxTest;

public class Canvas : Control
{
    public static readonly Image<Bgr, byte> Image = new("origin.jpg");
    public static readonly Bitmap Bitmap = Image.ToBitmap();

    public Viewer Viewer;
    public RoiElement RoiElement;

    public Canvas()
    {
        Viewer = new Viewer(this, new Point(0, 0));
        RoiElement = new RoiElement(this)
        {
            Rect = new Rectangle(0, 0, 950, 950)
        };

        SizeChanged += PictureBoxOnSizeChanged;
        Paint += PictureBoxOnPaint;
        MouseMove += PictureBoxOnMouseMove;
        MouseDown += PictureBoxOnMouseDown;
        MouseUp += PictureBoxOnMouseUp;
        MouseWheel += PictureBoxOnMouseWheel;

        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
    }

    private void PictureBoxOnSizeChanged(object? sender, EventArgs e)
    {
        Viewer = new Viewer(this, new Point(0, 0));
        RoiElement = new RoiElement(this)
        {
            Rect = new Rectangle(0, 0, 950, 950)
        };
        Refresh();
    }

    private void PictureBoxOnMouseWheel(object? sender, MouseEventArgs e)
    {
        Viewer.MouseWheel(e);
        RoiElement.MouseWheel(e);
        Refresh();
    }

    private void PictureBoxOnMouseUp(object? sender, MouseEventArgs e)
    {
        Viewer.MouseUp(e);
        RoiElement.MouseUp(e);
        Refresh();
    }

    private void PictureBoxOnMouseDown(object? sender, MouseEventArgs e)
    {
        Viewer.MouseDown(e);
        RoiElement.MouseDown(e);
        Refresh();
    }

    private void PictureBoxOnMouseMove(object? sender, MouseEventArgs e)
    {
        Viewer.MouseMove(e);
        RoiElement.MouseMove(e);
        Refresh();
    }

    private void PictureBoxOnPaint(object? sender, PaintEventArgs e)
    {
        // destRect: Rectangle 结构，它指定所绘制图像的位置和大小(相对于绘制区域只显示这么大, 类似于截图)
        // srcRect: Rectangle 结构，它指定 image 对象中要绘制的部分, 因为缩放了: 所以viewer.Viewport变大/变小了: 所以图片可以显示了且缩放
        e.Graphics.DrawImage(Bitmap, new Rectangle(0, 0, Width, Height), Viewer.Viewport, GraphicsUnit.Pixel);
        RoiElement.Drawing(e.Graphics);
    }
}