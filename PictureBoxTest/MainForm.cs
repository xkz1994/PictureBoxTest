// ReSharper disable LocalizableElement

using Emgu.CV;
using Emgu.CV.Structure;

namespace PictureBoxTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            KeyPreview = true; // 开启键盘事件的预览: 获取或设置一个值，该值指示在将键事件传递到具有焦点的控件前，窗体是否将接收此键事件
            KeyDown += OnKeyDown;
            Load += OnLoad;

            var binding = new Binding(nameof(textBoxWidth.Text), imageCanvas.RoiElement, nameof(imageCanvas.RoiElement.Weight), true, DataSourceUpdateMode.OnPropertyChanged);
            binding.Format += ObjectToString;
            binding.Parse += StringToInt;
            textBoxWidth.DataBindings.Add(binding);

            binding = new Binding(nameof(textBoxHeight.Text), imageCanvas.RoiElement, nameof(imageCanvas.RoiElement.Height), true, DataSourceUpdateMode.OnPropertyChanged);
            binding.Format += ObjectToString;
            binding.Parse += StringToInt;
            textBoxHeight.DataBindings.Add(binding);
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            imageCanvas.ImageCanvasOnKeyDown(e);
            imageCanvas.Refresh();
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            // imageCanvas.Image = Image.ToBitmap();
            // imageCanvas.Refresh();
        }

        private void ButtonOnClick(object sender, EventArgs e)
        {
            if (imageCanvas.Image is not Bitmap bitmap) return;
            if (new Rectangle(0, 0, bitmap.Width, bitmap.Height).Contains(imageCanvas.RoiElement.Rect) == false)
            {
                MessageBox.Show("超过图片范围");
                return;
            }

            using var image = bitmap.ToImage<Bgr, byte>();
            using var sub = image.GetSubRect(imageCanvas.RoiElement.Rect);
            sub.Save($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{Guid.NewGuid()}.jpg");
            MessageBox.Show("保存成功");
        }

        /// <summary>
        /// float to string
        /// </summary>
        public static void ObjectToString(object? sender, ConvertEventArgs convertEventArgs)
        {
            if (convertEventArgs.DesiredType != typeof(string)) return;

            convertEventArgs.Value = convertEventArgs.Value is null ? string.Empty : convertEventArgs.Value.ToString();
        }

        /// <summary>
        /// string to float
        /// </summary>
        public static void StringToInt(object? sender, ConvertEventArgs convertEventArgs)
        {
            if (convertEventArgs.DesiredType != typeof(int)) return;

            convertEventArgs.Value = int.TryParse(convertEventArgs.Value?.ToString(), out var result) ? result : 500;
        }
    }
}