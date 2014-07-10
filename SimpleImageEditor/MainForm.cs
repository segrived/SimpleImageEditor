using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using System.Xml;

namespace SimpleImageEditor
{
    enum DrawType { FreeHand, Line, Circle, Rectangle }

    public partial class MainForm : Form
    {
        private Point? _prevPoint;
        private DrawType _drawType;

        public MainForm()
        {
            InitializeComponent();
            var bitmap = new Bitmap(imageArea.Width, imageArea.Height);
            using (var g = Graphics.FromImage(bitmap)) {
                g.FillRectangle(Brushes.White, new Rectangle(0, 0, imageArea.Width, imageArea.Height));
            }
            imageArea.Image = bitmap;
            _prevPoint = null;
            _drawType = DrawType.FreeHand;
        }

        private void DrawPoint(int x, int y)
        {
            if (_prevPoint.HasValue) {
                using (var g = Graphics.FromImage(imageArea.Image)) {
                    var newPoint = new Point(x, y);
                    g.DrawLine(new Pen(Color.Black), _prevPoint.Value, newPoint);
                }
                imageArea.Invalidate();
            }
            _prevPoint = new Point(x, y);
        }

        private void imageArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) {
                switch (_drawType) {
                    case DrawType.FreeHand:
                        DrawPoint(e.X, e.Y);
                        break;
                }
            }
        }

        private void imageArea_MouseDown(object sender, MouseEventArgs e)
        {
            _prevPoint = null;
        }

        private void invertImageToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var newImage = ImageHelpers.InvertBitmap((Bitmap)imageArea.Image);
            imageArea.Image = newImage;
            imageArea.Invalidate();
        }

        #region File menu handlers
        private void openToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var res = openImageDialog.ShowDialog();
            if (res == DialogResult.OK) {
                imageArea.Image = new Bitmap(openImageDialog.FileName);
            }
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
        #endregion

        #region Draw type menu handlers
        private void freeHandToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            _drawType = DrawType.FreeHand;
        }

        private void circleToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            _drawType = DrawType.Circle;
        }

        private void rectangeToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            _drawType = DrawType.Rectangle;
        }
        #endregion
    }
}
