using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
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
            // Параметры по умолчанию
            _drawMode = DrawMode.Pen;
            _fillColor = Color.Black;
        }

        private void DrawLine(Point p1, Point p2, Image image)
        {
            using (var g = Graphics.FromImage(image)) {
                g.DrawLine(new Pen(Color.Black), p1, p2);
            }
        }

        private void DrawPoint(int x, int y, Image image)
        {
            using (var g = Graphics.FromImage(image)) {
                g.FillRectangle(Brushes.Black, x, y, 1, 1);
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
                        var dist = ImageHelpers.GetDistance(p1, p2);
                        g.FillEllipse(brush, p1.X - dist, p1.Y - dist, dist * 2, dist * 2);
                        //g.FillEllipse(brush, ImageHelpers.RectangleFromCoords(p1, p2));
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



        private void invertImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (! imageBackgroundWorker.IsBusy) {
                imageBackgroundWorker.RunWorkerAsync();
                //var newImage = ImageHelpers.InvertBitmap((Bitmap)imageArea.Image);
                //imageArea.Image = newImage;
                imageArea.Invalidate();
            }
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

        private void imageArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _drawMode == DrawMode.Pen) {
                DrawLine(e.X, e.Y);
            }
        }

        private void imageArea_MouseDown(object sender, MouseEventArgs e)
        {
        //    if (e.Button == MouseButtons.Left && _drawMode == DrawMode.Pen) {
        //        DrawPoint(e.X, e.Y, imageArea.Image);
        //    }
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

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            applyEffectProgress.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var invertedImage = (Bitmap)e.Result;
            imageArea.Image = invertedImage;
            applyEffectProgress.Value = 0;
            MessageBox.Show(@"Изображение было инвертировано");
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var bw = (BackgroundWorker)sender;
            var cl = (Bitmap)imageArea.Image.Clone();
            var input = new Bitmap(cl);
            var newImage = new Bitmap(input.Width, input.Height);
            var pixels = input.Height * input.Width;
            var height = input.Height;
            for (int i = 0; i < input.Width; i++) {
                for (int j = 0; j < input.Height; j++) {
                    var current = input.GetPixel(i, j);
                    newImage.SetPixel(i, j, ImageHelpers.InvertPixel(current));
                    var progress = (i * height + j) / (float)pixels * 100;
                    bw.ReportProgress((int)progress);
                }
            }
            e.Result = newImage;
        }
    }
}
