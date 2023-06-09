namespace PictureBoxTest;

public class RoiElement
{
    private static readonly Pen Pen = new(Color.Red);

    /// <summary>
    /// 当前对象的区域范围, 用来绘制的矩形区域
    /// 不能用属性，因为属性不能给修改内部值
    /// </summary>
    public Rectangle Rect;

    /// <summary>
    /// 视图
    /// </summary>
    public readonly Viewer Viewer;

    /// <summary>
    /// 画布控件
    /// </summary>
    public readonly PictureBox Canvas;

    /// <summary>
    /// AOI截图区域移动的起始点
    /// </summary>
    private Point _aoiMoveStartPoint = Point.Empty;

    private bool _isMove;

    public RoiElement(Viewer viewer, PictureBox canvas)
    {
        Viewer = viewer;
        Canvas = canvas;
    }

    public void MouseDown(MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right) return;
        _aoiMoveStartPoint = Viewer.MousePointToLocal(e.Location); // 鼠标坐标点变换到显示坐标点

        if (Rect.Contains(_aoiMoveStartPoint) == false) return;
        _isMove = true;
        Canvas.Cursor = Cursors.SizeAll;
        Pen.Color = Color.Green;
    }

    public void MouseMove(MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right) return;
        if (_isMove == false) return;

        var endMove = Viewer.MousePointToLocal(e.Location);
        var xMove = endMove.X - _aoiMoveStartPoint.X;
        var yMove = endMove.Y - _aoiMoveStartPoint.Y;
        Rect.Offset(xMove, yMove);
        _aoiMoveStartPoint = endMove;
    }

    public void MouseUp(MouseEventArgs e)
    {
        Canvas.Cursor = Cursors.Hand;
        Pen.Color = Color.Red;
    }

    public void MouseWheel(MouseEventArgs e)
    {
    }

    /// <summary>
    /// 正常绘图画红框十字架
    /// </summary>
    /// <param name="g"></param>
    public void Drawing(Graphics g)
    {
        // 本地（图纸）矩形变换到显示矩形
        var localToShow = Viewer.LocalToShow(Rect);

        g.DrawRectangle(Pen, localToShow);
        g.DrawLine(Pen, localToShow.X + localToShow.Width / 2, localToShow.Y, localToShow.X + localToShow.Width / 2, localToShow.Y + localToShow.Height);
        g.DrawLine(Pen, localToShow.X, localToShow.Y + localToShow.Height / 2, localToShow.X + localToShow.Width, localToShow.Y + localToShow.Height / 2);
    }
}