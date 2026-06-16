using System.Drawing;

namespace GeometryApp.Models
{
    /// <summary>
    /// Абстрактний базовий клас для всіх плоских геометричних фігур
    /// </summary>
    public abstract class Shape
    {
        // ─── Статичне поле ───────────────────────────────────────────
        private static int _totalCount = 0;

        // ─── Захищені поля ───────────────────────────────────────────
        protected double _x;          // координата центру по X
        protected double _y;          // координата центру по Y
        protected double _angle;      // кут повороту (градуси)
        protected Color  _color;      // колір фігури

        // ─── Статична властивість ────────────────────────────────────
        public static int TotalCount => _totalCount;

        // ─── Властивості ─────────────────────────────────────────────
        public double X
        {
            get => _x;
            set => _x = value;
        }

        public double Y
        {
            get => _y;
            set => _y = value;
        }

        public double Angle
        {
            get => _angle;
            set => _angle = value % 360.0;
        }

        public Color Color
        {
            get => _color;
            set => _color = value;
        }

        // ─── Конструктор 1 – за замовчуванням ───────────────────────
        protected Shape()
        {
            _x     = 150;
            _y     = 150;
            _angle = 0;
            _color = Color.RoyalBlue;
            _totalCount++;
        }

        // ─── Конструктор 2 – з координатами ─────────────────────────
        protected Shape(double x, double y) : this()
        {
            _x = x;
            _y = y;
        }

        // ─── Конструктор 3 – з координатами і кольором ──────────────
        protected Shape(double x, double y, Color color) : this(x, y)
        {
            _color = color;
        }

        // ─── Переміщення (перевантаження методу) ─────────────────────
        /// <summary>Переміщення на цілочисельне зміщення</summary>
        public void Move(int dx, int dy)
        {
            _x += dx;
            _y += dy;
        }

        /// <summary>Переміщення на дійсне зміщення</summary>
        public void Move(double dx, double dy)
        {
            _x += dx;
            _y += dy;
        }

        /// <summary>Переміщення в абсолютну позицію</summary>
        public void Move(double newX, double newY, bool absolute)
        {
            if (absolute) { _x = newX; _y = newY; }
            else Move(newX, newY);
        }

        // ─── Поворот ─────────────────────────────────────────────────
        public virtual void Rotate(double degrees)
        {
            _angle = (_angle + degrees) % 360.0;
            if (_angle < 0) _angle += 360.0;
        }

        // ─── Скидання лічильника ──────────────────────────────────────
        public static void ResetCount() => _totalCount = 0;

        // ─── Абстрактні методи ────────────────────────────────────────
        public abstract double Area();
        public abstract double Perimeter();
        public abstract void   Resize(double factor);
        public abstract void   Draw(Graphics g);

        public override string ToString() =>
            $"Центр: ({_x:F0},{_y:F0})  Кут: {_angle:F0}°  " +
            $"Площа: {Area():F2}  Периметр: {Perimeter():F2}";
    }
}
