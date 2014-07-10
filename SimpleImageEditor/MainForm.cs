using System.Drawing;
using System.Windows.Forms;

namespace SimpleImageEditor
{
    public partial class MainForm : Form
    {
        private Point? _prevPoint;
        public MainForm()
        {
            InitializeComponent();
            var bitmap = new Bitmap(imageArea.Width, imageArea.Height);
            using (var g = Graphics.FromImage(bitmap)) {
                g.FillRectangle(Brushes.White, new Rectangle(0, 0, imageArea.Width, imageArea.Height));
            }
            imageArea.Image = bitmap;
            _prevPoint = null;
        }

        private void imageArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) {
                if (_prevPoint.HasValue) {
                    using (var g = Graphics.FromImage(imageArea.Image)) {
                        g.DrawLine(new Pen(Color.Black), _prevPoint.Value, new Point(e.X, e.Y));
                    }
                    imageArea.Invalidate();
                }
                _prevPoint = new Point(e.X, e.Y);
            }
        }

        private void imageArea_MouseDown(object sender, MouseEventArgs e)
        {
            _prevPoint = null;
        }

        private void invertImageToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var image = (Bitmap)imageArea.Image;
            for (int i = 0; i < image.Width; i++) {
                for (int j = 0; j < image.Height; j++) {
                    var current = image.GetPixel(i, j);
                    var r = 255 - current.R;
                    var g = 255 - current.G;
                    var b = 255 - current.B;
                    image.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
            }
            imageArea.Image = image;
            imageArea.Invalidate();
        }

        private void saveToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var res = saveImageDialog.ShowDialog();
            if (res == DialogResult.OK) {
                imageArea.Image.Save(saveImageDialog.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }
    }
}
