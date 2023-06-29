using Emgu.CV.Structure;
using System.ComponentModel;

namespace PictureBoxTest;

[ToolboxItem(true)]
[DefaultProperty(nameof(Image))]
[Description("ROI区域与图片动态显示")]
public sealed class ImageCanvas : Control
{
    [Category("自定义")]
    [Description("图片")]
    [DefaultValue(typeof(Bitmap), null)]
    public Image? Image { get; set; }

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
        // destRect: Rectangle 结构，它指定所绘制图像的位置和大小(相对于绘制区域只显示这么大, 类似于截图)
        // srcRect: Rectangle 结构，它指定 image 对象中要绘制的部分, 因为缩放了: 所以viewer.Viewport变大/变小了: 所以图片可以显示了且缩放
        if (Image is null) return;

        e.Graphics.DrawImage(Image, new Rectangle(0, 0, Width, Height), Viewer.Viewport, GraphicsUnit.Pixel);
        foreach (var rectangle in PictureBoxToRectangle())
        {
            // 本地（图纸）矩形变换到显示矩形
            var localToShow = Viewer.LocalToShow(rectangle);

            e.Graphics.DrawRectangle(Pens.White, localToShow);
        }
        var xPos = ParasDoubleArrayByStr(
           "[3100, 3406, 3714, 3406, 3099, 3714, 3714, 20, 3098, 3099, 329, 3714, 637, 2483, 3098, 3714, 2791, 329, 945, 3406, 2791, 2791, 21, 329, 2176, 1253, 2791, 3406, 3714, 637, 2483, 329, 1561, 1560, 3406, 329, 1868, 2175, 2176, 20, 945, 637, 637, 3406, 2175, 2175, 1868, 2791, 1868, 21, 329, 21, 1560, 3714, 1868, 2791, 637, 3714, 1560, 2483, 329, 1560, 1253, 2483, 2175, 3407, 2176, 945, 637, 1253, 945, 1561, 3714, 1868, 1560, 1252, 1868, 1868, 3406, 1253, 2483, 3099, 3099, 2483, 1868, 1253, 945, 3098, 329, 3099, 1560, 2791, 2176, 637, 2483, 945, 21, 2175, 21, 944, 1253, 1560, 2791, 329, 2483, 637, 1868, 2791, 637, 20, 945, 945, 2483, 1252, 21, 3406, 1253]");
        var yPos = ParasDoubleArrayByStr(
            "[995, 378, 70, 70, 70, 378, 686, 2537, 686, 378, 2536, 994, 2536, 70, 1303, 1302, 378, 1920, 2536, 1303, 70, 1303, 2229, 2228, 70, 2536, 687, 1611, 2535, 1920, 378, 1612, 1611, 2536, 1919, 687, 70, 687, 378, 1920, 2228, 2228, 1303, 994, 1919, 1611, 2535, 2535, 1303, 70, 1304, 1612, 70, 1918, 1920, 1919, 378, 2227, 378, 995, 378, 1920, 2228, 1919, 1303, 2227, 2227, 1919, 1611, 1919, 1612, 2228, 1611, 1611, 1303, 70, 379, 2228, 2535, 1611, 1611, 2227, 1919, 686, 686, 687, 1303, 1611, 70, 2535, 687, 1611, 2536, 687, 2535, 687, 1304, 994, 687, 70, 1303, 995, 995, 995, 2227, 70, 995, 2227, 995, 378, 378, 995, 1303, 995, 995, 686, 378]");
        for (var i = 0; i < xPos.Length; i++)
        {
            var line1 = new LineSegment2D(new Point(Viewer.ToShowX((int)xPos[i]), Viewer.ToShowY((int)yPos[i] - 30)), new Point(Viewer.ToShowX((int)xPos[i]), Viewer.ToShowY((int)yPos[i] + 30)));
            var line2 = new LineSegment2D(new Point(Viewer.ToShowX((int)xPos[i] - 30) , Viewer.ToShowY((int)yPos[i])), new Point(Viewer.ToShowX((int)xPos[i] + 30), Viewer.ToShowY((int)yPos[i])));

            e.Graphics.DrawLine(Pens.Green, line1.P1, line1.P2);
            e.Graphics.DrawLine(Pens.Green, line2.P1, line2.P2);
        }
    }

    private static double[] ParasDoubleArrayByStr(string str)
    {
        double[] result = new double[0];
        if (!string.IsNullOrEmpty(str) && str.Length > 2)
        {
            str = str.Replace("[", "");
            str = str.Replace("]", "");
            string[] arrStr = str.Split(',');
            if (arrStr.Length > 0)
            {
                int count = arrStr.Length;
                result = new double[count];
                for (int i = 0; i < count; i++)
                {
                    if (double.TryParse(arrStr[i], out double tempValue))
                    {
                        result[i] = tempValue;
                    }
                }
            }
        }

        return result;
    }

    public Rectangle[,] PictureBoxToRectangle()
    {
        var imageSize = new Size(Image!.Width, Image.Height);
        var rectangles = new Rectangle[9, 13];

        // pictureBox Zoom模式 宽度填满 所以以宽度为准
        var width = 308;
        var height = 308;

        // 网格线两边流白一样宽度: 留白宽度 = (图片宽度 - (列数 * 每列宽度 + 2)) / 2
        int x = (int)Math.Round((imageSize.Width - (13 * width + 2)) / 2.0d),
            y = (int)Math.Round((imageSize.Height - (9 * height + 2)) / 2.0d);

        for (var rowIndex = 0; rowIndex < 9; rowIndex++)
        {
            for (var colIndex = 0; colIndex < 13; colIndex++)
            {
                rectangles[rowIndex, colIndex] = new Rectangle(x + colIndex * width, y + rowIndex * height, width, height);
            }
        }

        return rectangles;
    }
}