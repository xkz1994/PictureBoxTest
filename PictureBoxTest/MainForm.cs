// ReSharper disable LocalizableElement

namespace PictureBoxTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            KeyPreview = true; // 开启键盘事件的预览: 获取或设置一个值，该值指示在将键事件传递到具有焦点的控件前，窗体是否将接收此键事件
            KeyDown += OnKeyDown;

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

            pictureBox.RoiElement.Rect.Height = height;
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

            pictureBox.RoiElement.Rect.Width = width;
            pictureBox.Refresh();
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            pictureBox.RoiElement.KeyDown(e);
            pictureBox.Refresh();
        }

        private void ButtonOnClick(object sender, EventArgs e)
        {
            if (new Rectangle(0, 0, Canvas.Bitmap.Width, Canvas.Bitmap.Height).Contains(pictureBox.RoiElement.Rect) == false)
            {
                MessageBox.Show("超过图片范围");
                return;
            }

            using var sub = Canvas.Image.GetSubRect(pictureBox.RoiElement.Rect);
            sub.Save($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{Guid.NewGuid()}.jpg");
        }
    }
}