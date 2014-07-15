using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleImageEditor
{
    enum DrawMode { Pen, Line, Circle, Rectangle }

    public partial class MainForm : Form
    {
        private Bitmap _workingBitmap;
        private DrawMode _drawMode;
        private Point? _prevPenPoint;
        private Point? _shapePoint1;
        private Point? _shapePoint2;
        private Color _fillColor;
        private Color _borderColor;

        public MainForm()
        {
            InitializeComponent();
            var bitmap = new Bitmap(imageArea.Width, imageArea.Height);
            using (var g = Graphics.FromImage(bitmap)) {
                g.FillRectangle(Brushes.White, new Rectangle(0, 0, imageArea.Width, imageArea.Height));
            }
            _workingBitmap = bitmap;
            // Параметры по умолчанию
            _drawMode = DrawMode.Pen;
            _fillColor = Color.Black;
            _borderColor = Color.Gray;
            SyncPicture();
        }

        public void SyncPicture()
        {
            imageArea.Image = _workingBitmap;
            imageArea.Invalidate();
        }

        private void DrawLine(int x, int y)
        {
            if (_prevPenPoint.HasValue) {
                var newPoint = new Point(x, y);
                using (var g = Graphics.FromImage(_workingBitmap)) {
                    g.DrawLine(new Pen(Color.Black), _prevPenPoint.Value, newPoint);
                }
                SyncPicture();
            }
            _prevPenPoint = new Point(x, y);
        }

        private void DrawShape(Point p1, Point p2)
        {
            var fillBrush = new SolidBrush(_fillColor); // цвет заливки
            var borderPen = new Pen(_borderColor); // цвет рамки
            using (var g = Graphics.FromImage(_workingBitmap)) {
                switch (_drawMode) {
                    case DrawMode.Circle:
                        var dist = ImageHelpers.GetDistance(p1, p2);
                        var diameter = dist * 2;
                        var circleRect = new RectangleF(p1.X - dist, p1.Y - dist, diameter, diameter);
                        g.FillEllipse(fillBrush, circleRect);
                        g.DrawEllipse(borderPen, circleRect);
                        //g.FillEllipse(brush, ImageHelpers.RectangleFromCoords(p1, p2));
                        break;
                    case DrawMode.Rectangle:
                        var rect = ImageHelpers.RectangleFromCoords(p1, p2);
                        g.FillRectangle(fillBrush, rect);
                        g.DrawRectangle(borderPen, rect);
                        break;
                    case DrawMode.Line:
                        g.DrawLine(new Pen(Color.Black), p1, p2);
                        break;
                }
            }
            SyncPicture();
        }



        private async void invertImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var input = (Bitmap)_workingBitmap.Clone();
            var progressUpdate = new Progress<int>(p => applyEffectProgress.Value = p);
            _workingBitmap = await ImageAdjustments.Invert(input, progressUpdate);
            applyEffectProgress.Value = 0;
            SyncPicture();
        }

        #region File menu handlers

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = openImageDialog.ShowDialog();
            if (res == DialogResult.OK) {
                imageArea.Image = new Bitmap(openImageDialog.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = saveImageDialog.ShowDialog();
            if (res == DialogResult.OK) {
                imageArea.Image.Save(saveImageDialog.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        #region Draw mode menu handlers

        private void freeHandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _drawMode = DrawMode.Pen;
        }

        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDrawType(DrawMode.Circle);
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDrawType(DrawMode.Line);
        }

        private void rectangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetDrawType(DrawMode.Rectangle);
        }

        #endregion

        #region Set color menu handlers

        private void fillColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = fillColorDialog.ShowDialog();
            if (res == DialogResult.OK) {
                _fillColor = fillColorDialog.Color;
            }
        }

        private void borderColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = borderColorDialog.ShowDialog();
            if (res == DialogResult.OK) {
                _borderColor = borderColorDialog.Color;
            }
        }

        #endregion

        private void SetDrawType(DrawMode newType)
        {
            _drawMode = newType;
            _shapePoint1 = _shapePoint2 = null;
        }

        private void imageArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _drawMode == DrawMode.Pen) {
                DrawLine(e.X, e.Y);
            }
        }

        private void imageArea_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _drawMode != DrawMode.Pen) {
                _shapePoint1 = new Point(e.X, e.Y);
            }
        }

        private void imageArea_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            switch (_drawMode) {
                case DrawMode.Pen:
                    _prevPenPoint = null;
                    break;
                default:
                    if (_shapePoint1.HasValue) {
                        _shapePoint2 = new Point(e.X, e.Y);
                        DrawShape(_shapePoint1.Value, _shapePoint2.Value);
                    }
                    break;
            }
            imageArea.Invalidate();
        }
    }

}
