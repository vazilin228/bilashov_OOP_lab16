using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GeometryApp.Models;

namespace GeometryApp
{
    public class Form1 : Form
    {
        // ─── Колекція фігур ───────────────────────────────────────────
        private readonly List<Shape> _shapes = new();

        // ─── Елементи інтерфейсу ──────────────────────────────────────
        private Panel       _canvas       = null!;
        private ListBox     _listBox      = null!;
        private Label       _lblInfo      = null!;
        private Label       _lblCount     = null!;

        // Додавання
        private RadioButton _rbCircle     = null!;
        private RadioButton _rbSquare     = null!;
        private RadioButton _rbRect       = null!;
        private NumericUpDown _nudX       = null!;
        private NumericUpDown _nudY       = null!;
        private NumericUpDown _nudSize1   = null!;
        private NumericUpDown _nudSize2   = null!;
        private Label       _lblSize2     = null!;
        private Panel       _colorPreview = null!;
        private Color       _selectedColor = Color.RoyalBlue;

        // Операції
        private NumericUpDown _nudDX      = null!;
        private NumericUpDown _nudDY      = null!;
        private NumericUpDown _nudAngle   = null!;
        private NumericUpDown _nudFactor  = null!;

        // Ліва панель
        private Panel _leftPanel = null!;

        // ─────────────────────────────────────────────────────────────
        public Form1()
        {
            InitializeComponents();
            AddDefaultShapes();
            RefreshAll();
        }

        // ══════════════════════════════════════════════════════════════
        //  Побудова інтерфейсу
        // ══════════════════════════════════════════════════════════════
        private void InitializeComponents()
        {
            Text            = "Геометричні фігури — Лабораторна №16";
            Size            = new Size(1060, 700);
            MinimumSize     = new Size(900, 620);
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = Color.FromArgb(245, 245, 250);
            Font            = new Font("Segoe UI", 9f);

            // ── Ліва панель керування ───────────────────────────────
            _leftPanel = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 285,
                BackColor = Color.FromArgb(235, 235, 248),
                Padding   = new Padding(8)
            };

            // ─ Група «Додати фігуру» ─────────────────────────────────
            var gbAdd = new GroupBox
            {
                Text      = "➕  Додати фігуру",
                Left = 6, Top = 8, Width = 268, Height = 248,
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 120)
            };

            _rbCircle = new RadioButton { Text = "Коло",         Left = 8,   Top = 22, Width = 70, Checked = true };
            _rbSquare = new RadioButton { Text = "Квадрат",      Left = 80,  Top = 22, Width = 80 };
            _rbRect   = new RadioButton { Text = "Прямокутник",  Left = 160, Top = 22, Width = 100 };

            _rbCircle.CheckedChanged += (s, e) => { if (_rbCircle.Checked) _selectedColor = Color.RoyalBlue;   UpdateSize2Visibility(); };
            _rbSquare.CheckedChanged += (s, e) => { if (_rbSquare.Checked) _selectedColor = Color.ForestGreen; UpdateSize2Visibility(); };
            _rbRect.CheckedChanged   += (s, e) => { if (_rbRect.Checked)   _selectedColor = Color.Crimson;     UpdateSize2Visibility(); };

            _nudX    = MakeNUD(90, 48,  1, 900, 150);
            _nudY    = MakeNUD(90, 76,  1, 700, 150);
            _nudSize1= MakeNUD(90, 104, 5, 400, 80);
            _nudSize2= MakeNUD(90, 132, 5, 400, 60);

            _lblSize2 = new Label { Text = "Висота:", Left = 8, Top = 136, Width = 80, Height = 20,
                                    TextAlign = ContentAlignment.MiddleLeft };

            _colorPreview = new Panel
            {
                Left = 80, Top = 164, Width = 40, Height = 22,
                BackColor   = _selectedColor,
                BorderStyle = BorderStyle.FixedSingle,
                Cursor      = Cursors.Hand
            };
            _colorPreview.Click += PickColor;

            var btnPickColor = new Button { Text = "Вибрати...", Left = 128, Top = 163, Width = 80, Height = 24,
                                            FlatStyle = FlatStyle.Flat };
            btnPickColor.Click += PickColor;

            var btnAdd = new Button { Text = "Додати фігуру", Left = 8, Top = 196, Width = 250, Height = 30,
                                      FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                                      BackColor = Color.FromArgb(70, 130, 180), ForeColor = Color.White,
                                      Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };
            btnAdd.Click += BtnAdd_Click;

            gbAdd.Controls.AddRange(new Control[]
            {
                _rbCircle, _rbSquare, _rbRect,
                MakeLabel("X центру:",  8, 52, 80),
                MakeLabel("Y центру:",  8, 80, 80),
                MakeLabel("Розмір 1:", 8, 108, 80),
                _lblSize2,
                _nudX, _nudY, _nudSize1, _nudSize2,
                MakeLabel("Колір:", 8, 168, 70),
                _colorPreview, btnPickColor,
                btnAdd
            });

            // ─ Група «Операції» ──────────────────────────────────────
            var gbOp = new GroupBox
            {
                Text      = "⚙️  Операції з фігурою",
                Left = 6, Top = 264, Width = 268, Height = 218,
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 120)
            };

            _nudDX   = MakeNUD(48, 22, -800, 800, 0);
            _nudDY   = MakeNUD(48, 50, -700, 700, 0);
            var btnMove = MakeOpButton("Перемістити", 168, 22);
            btnMove.Click += BtnMove_Click;

            _nudAngle = MakeNUD(68, 84, -360, 360, 45);
            var btnRot = MakeOpButton("Повернути", 168, 80);
            btnRot.Click += BtnRotate_Click;

            _nudFactor = new NumericUpDown { Left = 68, Top = 114, Width = 70, Height = 24,
                                             Minimum = 1, Maximum = 500, Value = 120,
                                             DecimalPlaces = 0, Increment = 10 };
            var btnRes = MakeOpButton("Масштаб (%)", 168, 110);
            btnRes.Click += BtnResize_Click;

            var btnDel = new Button { Text = "🗑  Видалити обрану", Left = 8, Top = 148, Width = 250, Height = 26,
                                      FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(200, 80, 80),
                                      ForeColor = Color.White };
            btnDel.Click += BtnDelete_Click;

            var btnClear = new Button { Text = "🗑  Очистити всі фігури", Left = 8, Top = 180, Width = 250, Height = 26,
                                        FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(160, 60, 60),
                                        ForeColor = Color.White };
            btnClear.Click += BtnClear_Click;

            gbOp.Controls.AddRange(new Control[]
            {
                MakeLabel("dX:", 8, 26,  40), MakeLabel("dY:", 8, 54, 40),
                _nudDX, _nudDY, btnMove,
                MakeLabel("Кут °:", 8, 88, 58), _nudAngle, btnRot,
                MakeLabel("Коеф.%:", 8, 118, 58), _nudFactor, btnRes,
                btnDel, btnClear
            });

            // ─ Лічильник ─────────────────────────────────────────────
            _lblCount = new Label
            {
                Left = 8, Top = 490, Width = 268, Height = 36,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(70, 70, 140),
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold)
            };

            _leftPanel.Controls.AddRange(new Control[] { gbAdd, gbOp, _lblCount });

            // ── Права частина ───────────────────────────────────────
            var rightPanel = new Panel { Dock = DockStyle.Fill };

            _canvas = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.White
            };
            _canvas.Paint += Canvas_Paint;

            _listBox = new ListBox
            {
                Dock           = DockStyle.Bottom,
                Height         = 120,
                Font           = new Font("Consolas", 8.5f),
                BackColor      = Color.FromArgb(250, 250, 255),
                IntegralHeight = false
            };
            _listBox.SelectedIndexChanged += (s, e) => { _canvas.Invalidate(); UpdateInfo(); };

            var infoBar = new Panel { Dock = DockStyle.Bottom, Height = 26, BackColor = Color.FromArgb(218, 218, 240) };
            _lblInfo    = new Label  { Dock = DockStyle.Fill,  TextAlign = ContentAlignment.MiddleLeft,
                                       Padding = new Padding(6, 0, 0, 0) };
            infoBar.Controls.Add(_lblInfo);

            rightPanel.Controls.Add(_canvas);
            rightPanel.Controls.Add(infoBar);
            rightPanel.Controls.Add(_listBox);

            Controls.Add(rightPanel);
            Controls.Add(_leftPanel);

            UpdateSize2Visibility();
        }

        // ─── Фабрики ─────────────────────────────────────────────────
        private static Label MakeLabel(string t, int l, int top, int w) =>
            new Label { Text = t, Left = l, Top = top, Width = w, Height = 20,
                        TextAlign = ContentAlignment.MiddleLeft };

        private static Button MakeOpButton(string t, int l, int top) =>
            new Button { Text = t, Left = l, Top = top, Width = 90, Height = 26,
                         FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };

        private static NumericUpDown MakeNUD(int l, int top, decimal min, decimal max, decimal val) =>
            new NumericUpDown { Left = l, Top = top, Width = 70, Height = 24,
                                Minimum = min, Maximum = max, Value = val };

        // ══════════════════════════════════════════════════════════════
        //  Вибір кольору
        // ══════════════════════════════════════════════════════════════
        private void PickColor(object? sender, EventArgs e)
        {
            using var dlg = new ColorDialog { Color = _selectedColor };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _selectedColor          = dlg.Color;
                _colorPreview.BackColor = _selectedColor;
            }
        }

        private void UpdateSize2Visibility()
        {
            bool isRect = _rbRect?.Checked == true;
            if (_lblSize2 != null) _lblSize2.Visible = isRect;
            if (_nudSize2 != null) _nudSize2.Visible = isRect;
            if (_colorPreview != null) _colorPreview.BackColor = _selectedColor;
        }

        // ══════════════════════════════════════════════════════════════
        //  Початкові фігури для демонстрації
        // ══════════════════════════════════════════════════════════════
        private void AddDefaultShapes()
        {
            _shapes.Add(new Circle     (160, 160, 70,          Color.RoyalBlue));
            _shapes.Add(new MySquare   (380, 180, 100,         Color.ForestGreen));
            _shapes.Add(new MyRectangle(570, 200, 160, 90,     Color.Crimson));
        }

        // ══════════════════════════════════════════════════════════════
        //  Обробники кнопок
        // ══════════════════════════════════════════════════════════════
        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            double x = (double)_nudX.Value;
            double y = (double)_nudY.Value;
            double s = (double)_nudSize1.Value;

            Shape newShape;
            if (_rbCircle.Checked)
                newShape = new Circle(x, y, s, _selectedColor);
            else if (_rbSquare.Checked)
                newShape = new MySquare(x, y, s, _selectedColor);
            else
                newShape = new MyRectangle(x, y, s, (double)_nudSize2.Value, _selectedColor);

            _shapes.Add(newShape);
            RefreshAll();
            _listBox.SelectedIndex = _listBox.Items.Count - 1;
        }

        private void BtnMove_Click(object? sender, EventArgs e)
        {
            var shape = SelectedShape();
            if (shape == null) { Warn(); return; }
            shape.Move((double)_nudDX.Value, (double)_nudDY.Value);
            RefreshAll();
        }

        private void BtnRotate_Click(object? sender, EventArgs e)
        {
            var shape = SelectedShape();
            if (shape == null) { Warn(); return; }
            shape.Rotate((double)_nudAngle.Value);
            RefreshAll();
        }

        private void BtnResize_Click(object? sender, EventArgs e)
        {
            var shape = SelectedShape();
            if (shape == null) { Warn(); return; }
            double factor = (double)_nudFactor.Value / 100.0;
            shape.Resize(factor);
            RefreshAll();
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var shape = SelectedShape();
            if (shape == null) { Warn(); return; }
            int idx = _listBox.SelectedIndex;
            _shapes.Remove(shape);
            RefreshAll();
            if (_shapes.Count > 0)
                _listBox.SelectedIndex = Math.Min(idx, _shapes.Count - 1);
        }

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Видалити всі фігури з полотна?", "Підтвердження",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _shapes.Clear();
                Shape.ResetCount();
                RefreshAll();
            }
        }

        private static void Warn() =>
            MessageBox.Show("Оберіть фігуру у списку.", "Нічого не обрано",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

        // ══════════════════════════════════════════════════════════════
        //  Малювання
        // ══════════════════════════════════════════════════════════════
        private void Canvas_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            DrawGrid(g);

            var selected = SelectedShape();

            for (int i = 0; i < _shapes.Count; i++)
            {
                _shapes[i].Draw(g);
                if (_shapes[i] == selected)
                    DrawSelectionRing(g, _shapes[i]);

                // Порядковий номер
                using var fnt  = new Font("Arial", 8, FontStyle.Bold);
                using var brsh = new SolidBrush(Color.DarkSlateBlue);
                g.DrawString($"#{i + 1}", fnt, brsh,
                             (float)_shapes[i].X + 6, (float)_shapes[i].Y + 6);
            }
        }

        private void DrawGrid(Graphics g)
        {
            using var pen = new Pen(Color.FromArgb(225, 225, 235));
            for (int x = 0; x < _canvas.Width;  x += 40) g.DrawLine(pen, x, 0, x, _canvas.Height);
            for (int y = 0; y < _canvas.Height; y += 40) g.DrawLine(pen, 0, y, _canvas.Width, y);
        }

        private static void DrawSelectionRing(Graphics g, Shape s)
        {
            float r = s switch
            {
                Circle     c  => (float)c.Radius  + 8,
                MySquare   sq => (float)(sq.Side   * 0.80f),
                MyRectangle rc => (float)(Math.Max(rc.Width, rc.Height) * 0.68f),
                _             => 65f
            };
            using var pen = new Pen(Color.Gold, 2.5f) { DashStyle = DashStyle.Dash };
            g.DrawEllipse(pen, (float)s.X - r, (float)s.Y - r, r * 2, r * 2);
        }

        // ══════════════════════════════════════════════════════════════
        //  Оновлення стану
        // ══════════════════════════════════════════════════════════════
        private void RefreshAll()
        {
            int sel = _listBox.SelectedIndex;
            _listBox.Items.Clear();
            foreach (var s in _shapes)
                _listBox.Items.Add(s.ToString());
            if (sel >= 0 && sel < _listBox.Items.Count)
                _listBox.SelectedIndex = sel;

            _lblCount.Text =
                $"Всього фігур створено за сесію: {Shape.TotalCount}\n" +
                $"Зараз на полотні: {_shapes.Count}";

            _canvas.Invalidate();
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            var s = SelectedShape();
            _lblInfo.Text = s != null
                ? $"Обрано: {s}"
                : "Оберіть фігуру зі списку нижче для виконання операцій";
        }

        private Shape? SelectedShape()
        {
            int i = _listBox.SelectedIndex;
            return i >= 0 && i < _shapes.Count ? _shapes[i] : null;
        }
    }
}
