using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GeometryApp.Models
{
    /// <summary>
    /// Клас «Коло»
    /// </summary>
    public class Circle : Shape
    {
        private double _radius;

        // ─── Властивість ─────────────────────────────────────────────
        public double Radius
        {
            get => _radius;
            set => _radius = value > 0 ? value : 1;
        }

        // ─── Конструктор 1 – за замовчуванням ───────────────────────
        public Circle() : base()
        {
            _radius = 60;
        }

        // ─── Конструктор 2 – із заданим радіусом ────────────────────
        public Circle(double radius) : base()
        {
            Radius = radius;
        }

        // ─── Конструктор 3 – із позицією, радіусом і кольором ───────
        public Circle(double x, double y, double radius, Color color)
            : base(x, y, color)
        {
            Radius = radius;
        }

        // ─── Обчислення ──────────────────────────────────────────────
        public override double Area()      => Math.PI * _radius * _radius;
        public override double Perimeter() => 2.0 * Math.PI * _radius;

        // ─── Зміна розміру (перевантаження) ──────────────────────────
        /// <summary>Рівномірне масштабування на коефіцієнт</summary>
        public override void Resize(double factor)
        {
            if (factor > 0) _radius *= factor;
        }

        /// <summary>Встановлення конкретного радіуса</summary>
        public void Resize(double newRadius, bool absolute)
        {
            if (absolute && newRadius > 0) _radius = newRadius;
            else Resize(newRadius);
        }

        // ─── Поворот кола не має сенсу візуально, але підтримується ──
        public override void Rotate(double degrees)
        {
            // Коло симетричне, поворот не змінює форму
            base.Rotate(degrees);
        }

        // ─── Малювання ───────────────────────────────────────────────
        public override void Draw(Graphics g)
        {
            using var pen   = new Pen(_color, 2.5f);
            using var fill  = new SolidBrush(Color.FromArgb(70, _color));

            float r = (float)_radius;
            float x = (float)_x - r;
            float y = (float)_y - r;

            g.FillEllipse(fill, x, y, r * 2, r * 2);
            g.DrawEllipse(pen, x, y, r * 2, r * 2);

            // Мітка центру
            g.FillEllipse(Brushes.Red, (float)_x - 3, (float)_y - 3, 6, 6);
        }

        public override string ToString() =>
            $"[Коло]  R={_radius:F1}  " + base.ToString();
    }
}
