// ReSharper disable LocalizableElement

using DXFLib;

namespace PictureBoxTest
{
    public partial class MainForm : Form
    {
        private DXFDocument _doc = null!;
        private static readonly Dictionary<string, int> layerColor = new Dictionary<string, int>();
        private static readonly Dictionary<string, string> layerLType = new Dictionary<string, string>();
        private static readonly Dictionary<string, double[]> lineTypes = new Dictionary<string, double[]>();
        private static readonly Dictionary<string, int> layerLineWeigth = new Dictionary<string, int>();
        private static readonly Dictionary<string, bool> layerPlot = new Dictionary<string, bool>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (LoadDxf("dxf\\1.dxf"))
            {
                MessageBox.Show("Ok");
            }
        }

        private bool LoadDxf(string filename)
        {
            _doc = new DXFDocument();

            _doc.Load(filename);

            foreach (var record in _doc.Tables.Layers)
            {
                layerColor.TryAdd(record.LayerName, record.Color); // 颜色: 黑白
                layerLType.TryAdd(record.LayerName, record.LineType); // 线形: Continuous
                layerLineWeigth.TryAdd(record.LayerName, record.LineWeight); // 宽度: 模型
                var plotFlag = record.PlottingFlag;

                // http://docs.autodesk.com/ACD/2011/DEU/filesDXF/WS1a9193826455f5ff18cb41610ec0a2e719-7a51.htm
                // record.Flags==0 - visible;	plotFlag(290) == 0 不打印
                layerPlot.TryAdd(record.LayerName, (record.Flags == 0) && (plotFlag == 1)); // true when plot
            }

            List<DXFLineTypeRecord> ltypes = _doc.Tables.LineTypes;
            foreach (DXFLineTypeRecord lt in ltypes)
            {
                string pattern = "";
                if ((lt.PatternLength > 0) && (lt.ElementCount > 0))
                {
                    double[] tmp = new double[lt.ElementCount];
                    for (int i = 0; i < lt.ElementCount; i++)
                    {
                        if (lt.Elements[i].Length == 0)
                            tmp[i] = 0.5;
                        else
                            tmp[i] = Math.Abs(lt.Elements[i].Length);
                        pattern += string.Format(" {0} ", lt.Elements[i].Length);
                    }
                    if (!lineTypes.ContainsKey(lt.LineTypeName))
                        lineTypes.Add(lt.LineTypeName, tmp);
                }
            }

            return false;
        }
    }
}