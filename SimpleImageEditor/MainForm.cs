using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleImageEditor
{
    enum DrawMode { Pen, Line, Circle, Rectangle }

    public partial class MainForm : Form
    {
        private DrawMode _drawMode;
        private Point? _prevPoint;
        private Point? _shapePoint1;
        private Point? _shapePoint2;
        private Color _fillColor;

        public MainForm()
        {
            InitializeComponent();
            var bitmap = new Bitmap(imageArea.Width, imageArea.Height);
            using (var g = Graphics.FromImage(bitmap)) {
                g.FillRectangle(Brushes.White, new Rectangle(0, 0, imageArea.Width, imageArea.Height));
            }
            imageArea.Image = bitmap;
            _drawMode = DrawMode.Pen;
            _fillColor = Color.Black; // Цвет по умолчанию
        }

        private void DrawLine(Point p1, Point p2, Image image)
        {
            using (var g = Graphics.FromImage(image)) {
                g.DrawLine(new Pen(Color.Black), p1, p2);
            }
        }

        private void DrawLine(int x, int y)
        {
            if (_prevPoint.HasValue) {
                var newPoint = new Point(x, y);
                DrawLine(_prevPoint.Value, newPoint, imageArea.Image);
                imageArea.Invalidate();
            }
            _prevPoint = new Point(x, y);
        }

        private void DrawShape(Point p1, Point p2)
        {
            var pen = new Pen(Color.Black); // пусть пока ченой будет
            var brush = new SolidBrush(_fillColor);
            using (var g = Graphics.FromImage(imageArea.Image)) {
                switch (_drawMode) {
                    case DrawMode.Circle:
                        g.FillEllipse(brush, ImageHelpers.RectangleFromCoords(p1, p2));
                        break;
                    case DrawMode.Rectangle:
                        g.FillRectangle(brush, ImageHelpers.RectangleFromCoords(p1, p2));
                        break;
                    case DrawMode.Line:
                        g.DrawLine(pen, p1, p2);
                        break;
                }
            }
            imageArea.Invalidate();
        }

        private void imageArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (_drawMode != DrawMode.Pen) {
                return;
            }
            if (e.Button == MouseButtons.Left) {
                switch (_drawMode) {
                    case DrawMode.Pen:
                        DrawLine(e.X, e.Y);
                        break;
                }
            }
        }

        private void imageArea_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _drawMode == DrawMode.Pen) {
                using (var g = Graphics.FromImage(imageArea.Image)) {
                    g.FillRectangle(Brushes.Black, e.X, e.Y, 1, 1);
                }
            }
        }

        private void invertImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newImage = ImageHelpers.InvertBitmap((Bitmap)imageArea.Image);
            imageArea.Image = newImage;
            imageArea.Invalidate();
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

        private void SetDrawType(DrawMode newType)
        {
            _drawMode = newType;
            _shapePoint1 = _shapePoint2 = null;
        }

        private void imageArea_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            switch (_drawMode) {
                case DrawMode.Pen:
                    _prevPoint = null;
                    break;
                case DrawMode.Circle: case DrawMode.Rectangle: case DrawMode.Line:
                    if (_shapePoint1 == null) {
                        _shapePoint1 = new Point(e.X, e.Y);
                    } else if (_shapePoint2 == null) {
                        _shapePoint2 = new Point(e.X, e.Y);
                        DrawShape(_shapePoint1.Value, _shapePoint2.Value);
                        _shapePoint1 = _shapePoint2 = null;
                    }
                    break;
            }
            imageArea.Invalidate();
        }

        private void setFillColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = fillColorDialog.ShowDialog();
            if (res == DialogResult.OK) {
                _fillColor = fillColorDialog.Color;
            }
        }
    }
}
