using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GeometryApp.Models
{
    /// <summary>
    /// Клас «Прямокутник»
    /// </summary>
    public class MyRectangle : Shape
    {
        private double _width;
        private double _height;

        // ─── Властивості ─────────────────────────────────────────────
        public double Width
        {
            get => _width;
            set => _width = value > 0 ? value : 1;
        }

        public double Height
        {
            get => _height;
            set => _height = value > 0 ? value : 1;
        }

        // ─── Конструктор 1 – за замовчуванням ───────────────────────
        public MyRectangle() : base()
        {
            _width  = 140;
            _height = 80;
            _color  = Color.Crimson;
        }

        // ─── Конструктор 2 – із шириною та висотою ───────────────────
        public MyRectangle(double width, double height) : base()
        {
            Width   = width;
            Height  = height;
            _color  = Color.Crimson;
        }

        // ─── Конструктор 3 – із позицією, розмірами і кольором ───────
        public MyRectangle(double x, double y, double width, double height, Color color)
            : base(x, y, color)
        {
            Width  = width;
            Height = height;
        }

        // ─── Обчислення ──────────────────────────────────────────────
        public override double Area()      => _width * _height;
        public override double Perimeter() => 2.0 * (_width + _height);

        // ─── Зміна розміру (перевантаження) ──────────────────────────
        /// <summary>Рівномірне масштабування обох сторін</summary>
        public override void Resize(double factor)
        {
            if (factor > 0)
            {
                _width  *= factor;
                _height *= factor;
            }
        }

        /// <summary>Незалежне масштабування ширини і висоти</summary>
        public void Resize(double factorW, double factorH)
        {
            if (factorW > 0) _width  *= factorW;
            if (factorH > 0) _height *= factorH;
        }

        /// <summary>Встановлення конкретних розмірів</summary>
        public void Resize(double newWidth, double newHeight, bool absolute)
        {
            if (absolute)
            {
                if (newWidth  > 0) _width  = newWidth;
                if (newHeight > 0) _height = newHeight;
            }
            else Resize(newWidth, newHeight);
        }

        // ─── Малювання ───────────────────────────────────────────────
        public override void Draw(Graphics g)
        {
            using var pen  = new Pen(_color, 2.5f);
            using var fill = new SolidBrush(Color.FromArgb(70, _color));

            var state = g.Save();
            g.TranslateTransform((float)_x, (float)_y);
            g.RotateTransform((float)_angle);

            float hw = (float)(_width  / 2.0);
            float hh = (float)(_height / 2.0);

            g.FillRectangle(fill, -hw, -hh, (float)_width, (float)_height);
            g.DrawRectangle(pen, -hw, -hh, (float)_width, (float)_height);

            // Діагональ для відображення повороту
            if (_angle != 0)
            {
                using var dPen = new Pen(Color.FromArgb(100, _color), 1) { DashStyle = DashStyle.Dot };
                g.DrawLine(dPen, -hw, -hh, hw, hh);
            }

            g.Restore(state);
            g.FillEllipse(Brushes.Red, (float)_x - 3, (float)_y - 3, 6, 6);
        }

        public override string ToString() =>
            $"[Прямокутник]  Ш={_width:F1}  В={_height:F1}  " + base.ToString();
    }
}
