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


            // Obliczanie liczby segmentów (może być większa dla większej dokładności)
            int segmentCount = (int)Math.Sqrt(Math.Pow(A3.X - A0.X, 2) + Math.Pow(A3.Y - A0.Y, 2));
            float increment = 1f / segmentCount; // Krok dla t
            float t = 0;

            PointF previousPoint = A0;

            // Generowanie punktów na krzywej
            for (int i = 1; i <= segmentCount; i++)
            {
                t += increment;

                // Oblicz współrzędne punktu na krzywej Béziera
                float oneMinusT = 1 - t;
                PointF P = new PointF(
                    oneMinusT * oneMinusT * oneMinusT * A0.X +
                    3 * oneMinusT * oneMinusT * t * A1.X +
                    3 * oneMinusT * t * t * A2.X +
                    t * t * t * A3.X,

                    oneMinusT * oneMinusT * oneMinusT * A0.Y +
                    3 * oneMinusT * oneMinusT * t * A1.Y +
                    3 * oneMinusT * t * t * A2.Y +
                    t * t * t * A3.Y
                );

                // Rysowanie linii pomiędzy poprzednim a bieżącym punktem na krzywej
                g.DrawLine(pen, previousPoint, P);
                previousPoint = P;
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

            // Cleanup
            brush.Dispose();
            dashedPen.Dispose();
        }
    }

}
