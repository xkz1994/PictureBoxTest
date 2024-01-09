using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PictureBoxTest
{
    public sealed class RoiElement : INotifyPropertyChanged
    {
        public static SynchronizationContext? SynchronizationContext { get; set; }

        private static readonly Pen Pen = new Pen(Color.Red, 1);

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
        public readonly ImageCanvas ImageCanvas;

        /// <summary>
        /// 移动按键类型
        /// </summary>
        public MouseButtons MouseMoveButton = MouseButtons.Right;

        /// <summary>
        /// 是否是选中 默认false
        /// </summary>
        public bool IsSelected { get; private set; }

        public int Weight
        {
            get => Rect.Width;
            set
            {
                if (value == Rect.Width) return;
                Rect.Width = value;
                ImageCanvas.Refresh();
                OnPropertyChanged();
            }
        }

        public int Height
        {
            get => Rect.Height;
            set
            {
                if (value == Rect.Height) return;
                Rect.Height = value;
                ImageCanvas.Refresh();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 八个操纵柄尺寸
        /// </summary>
        public int JoystickSize => (Rect.Width + Rect.Height) / 50 + 1;

        /// <summary>
        /// AOI截图区域移动的起始点
        /// </summary>
        private Point _aoiMoveStartPoint = Point.Empty;

        /// <summary>
        /// 对象状态
        /// </summary>
        private EditorState _editorState = EditorState.None;

        /// <summary>
        /// 拖动柄状态
        /// </summary>
        private TransformState _transformState = TransformState.None;

        /// <summary>
        /// 老的光标样式
        /// </summary>
        private Cursor _oldCursor;

        public RoiElement(ImageCanvas imageCanvas)
        {
            Viewer = imageCanvas.Viewer;
            ImageCanvas = imageCanvas;
            SynchronizationContext = SynchronizationContext.Current;
        }

        public void MouseDown(MouseEventArgs e)
        {
            if (_oldCursor == null) _oldCursor = ImageCanvas.Cursor;
            if (e.Button != MouseMoveButton)
            {
                IsSelected = false;
                return;
            }

            _aoiMoveStartPoint = Viewer.MousePointToLocal(e.Location);
            if (Rect.Contains(_aoiMoveStartPoint) == false)
            {
                IsSelected = false;
                return;
            }

            IsSelected = true; //点击已经选择的对象

            var leftJoystickLeftBoardResult = _aoiMoveStartPoint.X > Rect.X; // 左排锚点 左边界
            var leftJoystickRightBoardResult = _aoiMoveStartPoint.X < Rect.X + JoystickSize; // 左排锚点 右边界

            var rightJoystickLeftBoardResult = _aoiMoveStartPoint.X > Rect.Right - JoystickSize; // 右排锚点 左边界
            var rightJoystickRightBoardResult = _aoiMoveStartPoint.X < Rect.Right; // 右排锚点 右边界

            var topJoystickTopBoardResult = _aoiMoveStartPoint.Y > Rect.Top; // 上排锚点 上边界
            var topJoystickBottomBoardResult = _aoiMoveStartPoint.Y < Rect.Top + JoystickSize; // 上排锚点 下边界

            var bottomJoystickTopBoardResult = _aoiMoveStartPoint.Y > Rect.Bottom - JoystickSize; // 下排锚点 上边界
            var bottomJoystickBottomBoardResult = _aoiMoveStartPoint.Y < Rect.Bottom; // 下排锚点 下边界

            var middleOfTopAndBottomJoystickTopBoardResult = _aoiMoveStartPoint.Y > Rect.Y + Rect.Height / 2 - JoystickSize / 2; // 上下排中间的锚点 上边界
            var middleOfTopAndBottomJoystickBottomBoardResult = _aoiMoveStartPoint.Y < Rect.Y + Rect.Height / 2 + JoystickSize / 2; // 上下排中间的锚点 上边界

            var middleOfLeftAndRightJoystickLeftBoardResult = _aoiMoveStartPoint.X > Rect.X + Rect.Width / 2 - JoystickSize / 2; // 左右排中间的锚点 左边界
            var middleOfLeftAndRightJoystickRightBoardResult = _aoiMoveStartPoint.X < Rect.X + Rect.Width / 2 + JoystickSize / 2; // 左右排中间的锚点 右边界

            if (leftJoystickLeftBoardResult && leftJoystickRightBoardResult && topJoystickTopBoardResult && topJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.LeftTop;
                ImageCanvas.Cursor = Cursors.SizeNWSE;
            }
            else if (middleOfLeftAndRightJoystickLeftBoardResult && middleOfLeftAndRightJoystickRightBoardResult && topJoystickTopBoardResult && topJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.Top;
                ImageCanvas.Cursor = Cursors.SizeNS;
            }
            else if (rightJoystickLeftBoardResult && rightJoystickRightBoardResult && topJoystickTopBoardResult && topJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.RightTop;
                ImageCanvas.Cursor = Cursors.SizeNESW;
            }
            else if (rightJoystickLeftBoardResult && rightJoystickRightBoardResult && middleOfTopAndBottomJoystickTopBoardResult && middleOfTopAndBottomJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.Right;
                ImageCanvas.Cursor = Cursors.SizeWE;
            }
            else if (rightJoystickLeftBoardResult && rightJoystickRightBoardResult && bottomJoystickTopBoardResult && bottomJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.RightBottom;
                ImageCanvas.Cursor = Cursors.SizeNWSE;
            }
            else if (middleOfLeftAndRightJoystickLeftBoardResult && middleOfLeftAndRightJoystickRightBoardResult && bottomJoystickTopBoardResult && bottomJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.Bottom;
                ImageCanvas.Cursor = Cursors.SizeNS;
            }
            else if (leftJoystickLeftBoardResult && leftJoystickRightBoardResult && bottomJoystickTopBoardResult && bottomJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.LeftBottom;
                ImageCanvas.Cursor = Cursors.SizeNESW;
            }
            else if (leftJoystickLeftBoardResult && leftJoystickRightBoardResult && middleOfTopAndBottomJoystickTopBoardResult && middleOfTopAndBottomJoystickBottomBoardResult)
            {
                _editorState = EditorState.Transform;
                _transformState = TransformState.Left;
                ImageCanvas.Cursor = Cursors.SizeWE;
            }
            else
            {
                _editorState = EditorState.Move;
                ImageCanvas.Cursor = Cursors.SizeAll;
            }
        }

        public void MouseMove(MouseEventArgs e)
        {
            if (_oldCursor == null) _oldCursor = ImageCanvas.Cursor;
            if (_editorState is EditorState.None) return;

            switch (_editorState)
            {
                case EditorState.Move: //移动模式，设定对象位置
                    var endMove = Viewer.MousePointToLocal(e.Location);
                    var xMove = endMove.X - _aoiMoveStartPoint.X;
                    var yMove = endMove.Y - _aoiMoveStartPoint.Y;

                    Rect.Offset(xMove, yMove);
                    _aoiMoveStartPoint = endMove;
                    break;

                case EditorState.Transform: //调整大小
                    var endTransform = Viewer.MousePointToLocal(e.Location);
                    var xTransform = endTransform.X - _aoiMoveStartPoint.X;
                    var yTransform = endTransform.Y - _aoiMoveStartPoint.Y;

                    switch (_transformState)
                    {
                        case TransformState.LeftTop: // X动 Y动
                            Rect.Width -= xTransform;
                            Rect.Height -= yTransform;
                            Rect.X += xTransform;
                            Rect.Y += yTransform;
                            break;

                        case TransformState.Top: // X不动 Y动
                            Rect.Height -= yTransform;
                            Rect.Y += yTransform;
                            break;

                        case TransformState.RightTop: // X不动 Y动
                            Rect.Width += xTransform;
                            Rect.Height -= yTransform;
                            Rect.Y += yTransform;
                            break;

                        case TransformState.Right:
                            Rect.Width += xTransform;
                            break;

                        case TransformState.RightBottom:
                            Rect.Width += xTransform;
                            Rect.Height += yTransform;
                            break;

                        case TransformState.Bottom:
                            Rect.Height += yTransform;
                            break;

                        case TransformState.LeftBottom: // X动 Y不动
                            Rect.Width -= xTransform;
                            Rect.Height += yTransform;
                            Rect.X += xTransform;
                            break;

                        case TransformState.Left: // X动 Y不动
                            Rect.Width -= xTransform;
                            Rect.X += xTransform;
                            break;
                    }

                    if (Rect.Width < 10) Rect.Width = 10;
                    if (Rect.Height < 10) Rect.Height = 10;

                    _aoiMoveStartPoint = endTransform;
                    break;
            }

            OnPropertyChanged(nameof(Rect));

            if (_editorState is EditorState.Transform == false) return;
            OnPropertyChanged(nameof(Weight));
            OnPropertyChanged(nameof(Height));
        }

        public void MouseUp(MouseEventArgs _)
        {
            if (_oldCursor == null) _oldCursor = ImageCanvas.Cursor;
            _editorState = EditorState.None;
            ImageCanvas.Cursor = _oldCursor;
        }

        public void MouseWheel(MouseEventArgs _)
        {
            if (_oldCursor == null) _oldCursor = ImageCanvas.Cursor;
        }

        public void KeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                    Rect.X++;
                    break;

                case Keys.Left:
                    Rect.X--;
                    break;

                case Keys.Up:
                    Rect.Y--;
                    break;

                case Keys.Down:
                    Rect.Y++;
                    break;
            }
        }

        /// <summary>
        /// 正常绘图画红框十字架
        /// </summary>
        /// <param name="g"></param>
        public void Drawing(Graphics g)
        {
            // 本地（图纸）矩形变换到显示矩形
            var localToShow = Viewer.LocalToShow(Rect);

            Pen.Width = IsSelected ? 2 : 1;
            g.DrawRectangle(Pen, localToShow);
            g.DrawLine(Pen, localToShow.X + localToShow.Width / 2, localToShow.Y, localToShow.X + localToShow.Width / 2, localToShow.Y + localToShow.Height);
            g.DrawLine(Pen, localToShow.X, localToShow.Y + localToShow.Height / 2, localToShow.X + localToShow.Width, localToShow.Y + localToShow.Height / 2);
            DrawingJoystick(g);
        }

        /// <summary>
        /// 绘制八个操纵柄
        /// </summary>
        /// <param name="g">画笔</param>
        public void DrawingJoystick(Graphics g)
        {
            if (IsSelected == false) return;

            Pen.Width = 2;
            var cX = Rect.Width / 2;
            var cY = Rect.Height / 2;
            var s = JoystickSize;
            // 画边框矩形
            g.DrawRectangle(Pen, Viewer.LocalToShow(Rect.X, Rect.Y, s, s));
            g.DrawRectangle(Pen, Viewer.LocalToShow(Rect.X + (cX - s / 2), Rect.Y, s, s));
            g.DrawRectangle(Pen, Viewer.LocalToShow(Rect.X + (Rect.Width - s), Rect.Y, s, s));
            g.DrawRectangle(Pen, Viewer.LocalToShow(Rect.X + (Rect.Width - s), Rect.Y + (cY - s / 2), s, s));
            g.DrawRectangle(Pen, Viewer.LocalToShow(Rect.X + (Rect.Width - s), Rect.Y + (Rect.Height - s), s, s));
            g.DrawRectangle(Pen, Viewer.LocalToShow(Rect.X + (cX - s / 2), Rect.Y + (Rect.Height - s), s, s));
            g.DrawRectangle(Pen, Viewer.LocalToShow(Rect.X, Rect.Y + (Rect.Height - s), s, s));
            g.DrawRectangle(Pen, Viewer.LocalToShow(Rect.X, Rect.Y + (cY - s / 2), s, s));
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Send同步 Post异步, 同步效果更快界面不卡
            SynchronizationContext?.Send(_ => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), null);
            // PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal enum EditorState
    {
        /// <summary>
        /// 没有任何操作
        /// </summary>
        None,

        /// <summary>
        /// 移动状态
        /// </summary>
        Move,

        /// <summary>
        /// 调整大小状态
        /// </summary>
        Transform
    }

    internal enum TransformState
    {
        /// <summary>
        /// 没有任何操作
        /// </summary>
        None,

        /// <summary>
        /// 鼠标再操纵柄左上角
        /// </summary>
        LeftTop,

        /// <summary>
        /// 鼠标再操纵柄上边
        /// </summary>
        Top,

        /// <summary>
        /// 鼠标在操纵柄右上角
        /// </summary>
        RightTop,

        /// <summary>
        /// 鼠标再操纵柄右边
        /// </summary>
        Right,

        /// <summary>
        /// 鼠标zai操纵柄右下角
        /// </summary>
        RightBottom,

        /// <summary>
        /// 鼠标再操纵柄下边
        /// </summary>
        Bottom,

        /// <summary>
        /// 鼠标再操纵柄左下角
        /// </summary>
        LeftBottom,

        /// <summary>
        /// 鼠标再操纵柄左边
        /// </summary>
        Left
    }
}