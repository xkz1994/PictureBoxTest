// ReSharper disable LocalizableElement

using DXFLib;

namespace PictureBoxTest
{
    public partial class MainForm : Form
    {
        private DXFDocument _doc = null!;

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
            try
            {
                _doc.Load(filename);
                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show("The file could not be opened - perhaps already open in other application?\r\n" + err.ToString());
            }

            return false;
        }
    }
}