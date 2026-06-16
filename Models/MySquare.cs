using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GeometryApp.Models
{
    /// <summary>
    /// Клас «Квадрат»
    /// </summary>
    public class MySquare : Shape
    {
        private double _side;

        // ─── Властивість ─────────────────────────────────────────────
        public double Side
        {
            get => _side;
            set => _side = value > 0 ? value : 1;
        }

        // ─── Конструктор 1 – за замовчуванням ───────────────────────
        public MySquare() : base()
        {
            _side  = 100;
            _color = Color.ForestGreen;
        }

        // ─── Конструктор 2 – із заданою стороною ─────────────────────
        public MySquare(double side) : base()
        {
            Side   = side;
            _color = Color.ForestGreen;
        }

        // ─── Конструктор 3 – із позицією, стороною і кольором ────────
        public MySquare(double x, double y, double side, Color color)
            : base(x, y, color)
        {
            Side = side;
        }

        // ─── Обчислення ──────────────────────────────────────────────
        public override double Area()      => _side * _side;
        public override double Perimeter() => 4.0 * _side;

        // ─── Зміна розміру (перевантаження) ──────────────────────────
        /// <summary>Масштабування стороні на коефіцієнт</summary>
        public override void Resize(double factor)
        {
            if (factor > 0) _side *= factor;
        }

        /// <summary>Встановлення конкретної довжини сторони</summary>
        public void Resize(double newSide, bool absolute)
        {
            if (absolute && newSide > 0) _side = newSide;
            else Resize(newSide);
        }

        // ─── Малювання ───────────────────────────────────────────────
        public override void Draw(Graphics g)
        {
            using var pen  = new Pen(_color, 2.5f);
            using var fill = new SolidBrush(Color.FromArgb(70, _color));

            var state = g.Save();
            g.TranslateTransform((float)_x, (float)_y);
            g.RotateTransform((float)_angle);

            float h = (float)(_side / 2.0);
            g.FillRectangle(fill, -h, -h, (float)_side, (float)_side);
            g.DrawRectangle(pen, -h, -h, (float)_side, (float)_side);

            // Мітка кута повороту
            if (_angle != 0)
            {
                using var font  = new Font("Arial", 8);
                using var brush = new SolidBrush(Color.DarkRed);
                g.DrawString($"{_angle:F0}°", font, brush, h + 4, -8);
            }

            g.Restore(state);
            g.FillEllipse(Brushes.Red, (float)_x - 3, (float)_y - 3, 6, 6);
        }

        public override string ToString() =>
            $"[Квадрат]  Сторона={_side:F1}  " + base.ToString();
    }
}
