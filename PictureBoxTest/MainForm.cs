// ReSharper disable LocalizableElement

using Emgu.CV;
using Emgu.CV.Structure;
using TestCoordinator;

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

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            imageCanvas.ImageCanvasOnKeyDown(e);
            imageCanvas.Refresh();
        }

        private void OnLoad(object sender, EventArgs e)
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

        private void ButtonPathClick(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                Multiselect = false, // 允许打开多个文件
                DefaultExt = "Picture", // 打开文件时显示的可选文件类型
                Filter = "(*.jpg;*.bmp)|*.jpg;*.bmp" // 打开多个文件
            };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            imageCanvas.Image = new Bitmap(dialog.FileName);
            imageCanvas.MatchPointList.Clear();
            imageCanvas.IsAutoZoom = true;
            imageCanvas.Refresh();
        }

        private void ButtonTestClick(object sender, EventArgs e)
        {
            imageCanvas.MatchPointList.Clear();
            imageCanvas.Refresh();
            foreach (var (position, edgeLength) in ImageCanvas.RegionList)
            {
                Image<Gray, byte> imTmp = null;
                var rst = HalconHelper.TryMatch(((Bitmap)imageCanvas.Image).ToImage<Gray, byte>(),
                    position.X,
                    position.Y,
                    edgeLength,
                    ImageCanvas.MatchThreshold,
                    ImageCanvas.BinarizationThresholds,
                    ImageCanvas.MatchTemplatePath,
                    0,
                    out var pHalfWidthHeight,
                    out var pMatchedCenter,
                    ref imTmp,
                    false); //匹配1#选区//匹配1#选区

                if (rst <= 0)
                {
                    imageCanvas.MatchPointList.Add(pMatchedCenter);
                }
            }

            if (imageCanvas.MatchPointList.Count <= 0)
            {
                MessageBox.Show("匹配失败");
            }

            imageCanvas.Refresh();
        }

        /// <summary>
        /// float to string
        /// </summary>
        public static void ObjectToString(object sender, ConvertEventArgs convertEventArgs)
        {
            if (convertEventArgs.DesiredType != typeof(string)) return;

            convertEventArgs.Value = convertEventArgs.Value is null ? string.Empty : convertEventArgs.Value.ToString();
        }

        /// <summary>
        /// string to float
        /// </summary>
        public static void StringToInt(object sender, ConvertEventArgs convertEventArgs)
        {
            if (convertEventArgs.DesiredType != typeof(int)) return;

            convertEventArgs.Value = int.TryParse(convertEventArgs.Value?.ToString(), out var result) ? result : 500;
        }
    }
}