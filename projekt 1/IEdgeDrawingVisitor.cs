using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_1
{
    public interface IEdgeDrawingVisitor
    {
        void DrawNoConstraintEdge(NoConstraintEdge edge, Graphics g, Pen pen,bool bresenham);
        void DrawHorizontalEdge(HorizontalEdge edge, Graphics g, Pen pen,bool bresenham);
        void DrawVerticalEdge(VerticalEdge edge, Graphics g, Pen pen, bool bresenham);
        void DrawFixedLengthEdge(FixedLengthEdge edge, Graphics g, Pen pen, bool bresenham);
        public void DrawBezier(BezierEdge edge, Graphics g, Pen pen, bool bresenham);
    }
    public class EdgeDrawingVisitor : IEdgeDrawingVisitor
    {
        public void DrawLineBresenham(int x0, int y0, int x1, int y1,Graphics g)
        {
            int dx = Math.Abs(x0 - x1);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y0 - y1);
            int sy = y0 < y1 ? 1 : -1;
            int error = dx + dy;
            Brush brush = new SolidBrush(Color.Black);

            while(true)
            {
                g.FillRectangle(brush, x0, y0, 1, 1);
                if (x0 == x1 && y0 == y1)
                    break;
                int e2 = 2 * error;
                if(e2 >= dy)
                {
                    error = error + dy;
                    x0 = x0 + sx;
                }
                if(e2 <= dx)
                {
                    error = error + dx;
                    y0 = y0 + sy;
                }
            }
        }
        public void DrawNoConstraintEdge(NoConstraintEdge edge, Graphics g, Pen pen, bool bresenham)
        {
            if (bresenham)
                DrawLineBresenham(edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y, g);
            else
                g.DrawLine(pen, edge.Start, edge.End);
        }

        public void DrawHorizontalEdge(HorizontalEdge edge, Graphics g, Pen pen, bool bresenham)
        {
            if (bresenham)
                DrawLineBresenham(edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y, g);
            else
                g.DrawLine(pen, edge.Start, edge.End);

            // Rysowanie ikony poziomej
            Point center = edge.GetEdgeCenter();
            int offsetY = 10;
            g.DrawLine(new Pen(Color.Blue, 2), center.X - 5, center.Y + offsetY, center.X + 5, center.Y + offsetY);
        }

        public void DrawVerticalEdge(VerticalEdge edge, Graphics g, Pen pen, bool bresenham)
        {
            if (bresenham)
                DrawLineBresenham(edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y, g);
            else
                g.DrawLine(pen, edge.Start, edge.End);

            // Rysowanie ikony pionowej
            Point center = edge.GetEdgeCenter();
            int offsetX = 10;
            g.DrawLine(new Pen(Color.Green, 2), center.X + offsetX, center.Y - 5, center.X + offsetX, center.Y + 5);
        }

        public void DrawFixedLengthEdge(FixedLengthEdge edge, Graphics g, Pen pen, bool bresenham)
        {
            if (bresenham)
                DrawLineBresenham(edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y, g);
            else
                g.DrawLine(pen, edge.Start, edge.End );

            // Rysowanie ikony i długości
            Point center = edge.GetEdgeCenter();
            int offsetY = 10;
            g.DrawEllipse(new Pen(Color.Red, 2), center.X - 5, center.Y + offsetY - 5, 10, 10);
            g.DrawString(edge.Length.ToString("F1"), new Font("Arial", 8), Brushes.Black, center.X + 10, center.Y + offsetY - 10);
        }
        public void DrawBezier(BezierEdge edge, Graphics g, Pen pen, bool bresenham)
        {
           PointF A0 = edge.Start;
            PointF A1 = edge.ControlPoint1;
            PointF A2 = edge.ControlPoint2;
            PointF A3 = edge.End;

            int segmentCount = (int)Math.Sqrt(Math.Pow(A3.X - A0.X, 2) + Math.Pow(A3.Y - A0.Y, 2));

            // Ustal wartość d, będącą odwrotnością liczby segmentów
            float d = 1f / segmentCount;

            // Przyrosty deltaP, delta2P i delta3P
            PointF deltaP = new PointF(
                3 * (A1.X - A0.X) * d + 3 * (A2.X - 2 * A1.X + A0.X) * d * d + (A3.X - 3 * A2.X + 3 * A1.X - A0.X) * d * d * d,
                3 * (A1.Y - A0.Y) * d + 3 * (A2.Y - 2 * A1.Y + A0.Y) * d * d + (A3.Y - 3 * A2.Y + 3 * A1.Y - A0.Y) * d * d * d
            );

            PointF delta2P = new PointF(
                6 * (A2.X - 2 * A1.X + A0.X) * d * d + 6 * (A3.X - 3 * A2.X + 3 * A1.X - A0.X) * d * d * d,
                6 * (A2.Y - 2 * A1.Y + A0.Y) * d * d + 6 * (A3.Y - 3 * A2.Y + 3 * A1.Y - A0.Y) * d * d * d
            );

            PointF delta3P = new PointF(
                6 * (A3.X - 3 * A2.X + 3 * A1.X - A0.X) * d * d * d,
                6 * (A3.Y - 3 * A2.Y + 3 * A1.Y - A0.Y) * d * d * d
            );

            // Inicjalizuj punkt początkowy
            PointF currentPoint = A0;
            PointF previousPoint = currentPoint;

            // Rysowanie kolejnych segmentów
            for (int i = 0; i < segmentCount; i++)
            {
                // Oblicz kolejny punkt na krzywej
                currentPoint = new PointF(currentPoint.X + deltaP.X, currentPoint.Y + deltaP.Y);
                deltaP = new PointF(deltaP.X + delta2P.X, deltaP.Y + delta2P.Y);
                delta2P = new PointF(delta2P.X + delta3P.X, delta2P.Y + delta3P.Y);

                // Rysuj odcinek między poprzednim a aktualnym punktem
                g.DrawLine(pen, previousPoint, currentPoint);

                // Aktualizuj poprzedni punkt
                previousPoint = currentPoint;
            }

            // Rysowanie punktów kontrolnych jako brązowe kropki
            SolidBrush brush = new SolidBrush(Color.Brown);
            Pen dashedPen = new Pen(Color.FromArgb(128, Color.Brown), 1)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
            };

            // Kropki dla punktów kontrolnych
            g.FillEllipse(brush, A1.X - 3, A1.Y - 3, 6, 6);
            g.FillEllipse(brush, A2.X - 3, A2.Y - 3, 6, 6);

            // Przerywane linie od punktów kontrolnych do wierzchołków start i end
            g.DrawLine(dashedPen, A0, A1); // Linia start -> pierwszy punkt kontrolny
            g.DrawLine(dashedPen, A1, A2); // Linia pierwszy -> drugi punkt kontrolny
            g.DrawLine(dashedPen, A2, A3); // Linia drugi punkt kontrolny -> end
            g.DrawLine(dashedPen, A3, A0);

            // Cleanup
            brush.Dispose();
            dashedPen.Dispose();
        }
    }

}
