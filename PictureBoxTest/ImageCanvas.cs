﻿using System.ComponentModel;

namespace PictureBoxTest;

[ToolboxItem(true)]
[DefaultProperty(nameof(Image))]
[Description("ROI区域与图片动态显示")]
public class ImageCanvas : Control
{
    [Category("自定义")]
    [Description("图片")]
    [DefaultValue(typeof(Bitmap), null)]
    public Image? Image { get; set; }

    [Category("自定义")]
    [Description("是否显示ROI")]
    public bool IsShowRoi { get; set; }

    public Viewer Viewer { get; private set; }
    public RoiElement RoiElement { get; private set; }

    public ImageCanvas()
    {
        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        DoubleBuffered = true;
        AllowDrop = true;

        IsShowRoi = true;
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

    private void ImageCanvasOnSizeChanged(object? sender, EventArgs e)
    {
        Viewer.UpdateViewport();
        // 不能用创建新对象，否则binding失效
        Refresh();
    }

    private void ImageCanvasOnMouseWheel(object? sender, MouseEventArgs e)
    {
        Viewer.MouseWheel(e);
        if (IsShowRoi)
            RoiElement.MouseWheel(e);
        Refresh();
    }

    private void ImageCanvasOnMouseUp(object? sender, MouseEventArgs e)
    {
        Viewer.MouseUp(e);
        if (IsShowRoi)
            RoiElement.MouseUp(e);
        Refresh();
    }

    private void ImageCanvasOnMouseDown(object? sender, MouseEventArgs e)
    {
        Viewer.MouseDown(e);
        if (IsShowRoi)
            RoiElement.MouseDown(e);
        Refresh();
    }

    private void ImageCanvasOnMouseMove(object? sender, MouseEventArgs e)
    {
        Viewer.MouseMove(e);
        if (IsShowRoi)
            RoiElement.MouseMove(e);
        if (e.Button != MouseButtons.None)
        {
            //TODO: 增加了刷新条件，尝试减少刷新来优化系统性
            Refresh();
        }
    }

    private void ImageCanvasOnPaint(object? sender, PaintEventArgs e)
    {
        // destRect: Rectangle 结构，它指定所绘制图像的位置和大小(相对于绘制区域只显示这么大, 类似于截图)
        // srcRect: Rectangle 结构，它指定 image 对象中要绘制的部分, 因为缩放了: 所以viewer.Viewport变大/变小了: 所以图片可以显示了且缩放
        if (Image is null) return;

        e.Graphics.DrawImage(Image, new Rectangle(0, 0, Width, Height), Viewer.Viewport, GraphicsUnit.Pixel);

        if (IsShowRoi)
            RoiElement.Drawing(e.Graphics);
    }

    public void ImageCanvasOnKeyDown(KeyEventArgs e)
    {
        if (IsShowRoi)
            RoiElement.KeyDown(e);
        Refresh();
    }
}