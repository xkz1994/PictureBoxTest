using System.ComponentModel;

namespace PictureBoxTest;

[ToolboxItem(true)]
[DefaultProperty(nameof(Image))]
[Description("ROI区域与图片动态显示")]
public sealed class ImageCanvas : Control
{
    public Viewer Viewer { get; private set; }

    public ImageCanvas()
    {
        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        DoubleBuffered = true;
        AllowDrop = true;

        Viewer = new Viewer(this, new Point(0, 0));

        SizeChanged += ImageCanvasOnSizeChanged;
        Paint += ImageCanvasOnPaint;
        MouseMove += ImageCanvasOnMouseMove;
        MouseDown += ImageCanvasOnMouseDown;
        MouseUp += ImageCanvasOnMouseUp;
        MouseWheel += ImageCanvasOnMouseWheel;
    }

    private void ImageCanvasOnSizeChanged(object? sender, EventArgs e)
    {
        Viewer.UpdateViewport();
        // 不能用创建新对象，否则binding失效
        Refresh();
    }

    private void ImageCanvasOnMouseWheel(object? sender, MouseEventArgs e)
    {
        Viewer.MouseWheel(e);
        Refresh();
    }

    private void ImageCanvasOnMouseUp(object? sender, MouseEventArgs e)
    {
        Viewer.MouseUp(e);
        Refresh();
    }

    private void ImageCanvasOnMouseDown(object? sender, MouseEventArgs e)
    {
        Viewer.MouseDown(e);
        Refresh();
    }

    private void ImageCanvasOnMouseMove(object? sender, MouseEventArgs e)
    {
        Viewer.MouseMove(e);
        if (e.Button != MouseButtons.None)
        {
            //TODO: 增加了刷新条件，尝试减少刷新来优化系统性
            Refresh();
        }
    }

    private void ImageCanvasOnPaint(object? sender, PaintEventArgs e)
    {
    }

    public void ImageCanvasOnKeyDown(KeyEventArgs e)
    {
        Refresh();
    }
}