using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_1
{
    public interface IEdgeDrawingVisitor
    {
        void DrawNoConstraintEdge(NoConstraintEdge edge, Graphics g, Pen pen,bool bresenham,bool wu);
        void DrawHorizontalEdge(HorizontalEdge edge, Graphics g, Pen pen,bool bresenham, bool wu);
        void DrawVerticalEdge(VerticalEdge edge, Graphics g, Pen pen, bool bresenham, bool wu);
        void DrawFixedLengthEdge(FixedLengthEdge edge, Graphics g, Pen pen, bool bresenham, bool wu);
        public void DrawBezier(BezierEdge edge, Graphics g, Pen pen, bool bresenham, bool wu);
    }
    public class EdgeDrawingVisitor : IEdgeDrawingVisitor
    {
        public void DrawLineBresenham(int x0, int y0, int x1, int y1, Graphics g)
        {
            if (Math.Abs(y1 - y0) < Math.Abs(x1 - x0))
            {
                if (x0 > x1)
                    plotLineLow(x1, y1, x0, y0, g);
                else
                    plotLineLow(x0, y0, x1, y1, g);
            }
            else
            {
                if (y0 > y1)
                    plotLineHigh(x1, y1, x0, y0, g);
                else
                    plotLineHigh(x0, y0, x1, y1, g);
            }
        }

        private void plotLineLow(int x0, int y0, int x1, int y1, Graphics g)
        {
            int dx = x1 - x0;
            int dy = y1 - y0;
            int yi = 1;

            if (dy < 0)
            {
                yi = -1;
                dy = -dy;
            }

            int D = (2 * dy) - dx;
            int y = y0;
            Brush brush = new SolidBrush(Color.Black);

            for (int x = x0; x <= x1; x++)
            {
                g.FillRectangle(brush, x, y, 1, 1);
                if (D > 0)
                {
                    y = y + yi;
                    D = D + (2 * (dy - dx));
                }
                else
                {
                    D = D + 2 * dy;
                }
            }
            brush.Dispose();
        }

        private void plotLineHigh(int x0, int y0, int x1, int y1, Graphics g)
        {
            int dx = x1 - x0;
            int dy = y1 - y0;
            int xi = 1;

            if (dx < 0)
            {
                xi = -1;
                dx = -dx;
            }

            int D = (2 * dx) - dy;
            int x = x0;
            Brush brush = new SolidBrush(Color.Black);

            for (int y = y0; y <= y1; y++)
            {
                g.FillRectangle(brush, x, y, 1, 1);
                if (D > 0)
                {
                    x = x + xi;
                    D = D + (2 * (dx - dy));
                }
                else
                {
                    D = D + 2 * dx;
                }
            }
            brush.Dispose();
        }
        private float FracPart(float x) => x - (float)Math.Floor(x);
        private float RFracPart(float x) => 1 - FracPart(x);
        private int IPart(float x) => (int)Math.Floor(x);

        private void Plot(Graphics g, int x, int y, float brightness, Color color)
        {
            Color blendedColor = Color.FromArgb((int)(brightness * 255), color);
            using (SolidBrush brush = new SolidBrush(blendedColor))
            {
                g.FillRectangle(brush, x, y, 1, 1);
            }
        }

        public void DrawLineXiaolinWu(int x0, int y0, int x1, int y1, Graphics g)
        {
            Color color = Color.Black;
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

            if (steep)
            {
                (x0, y0) = (y0, x0);
                (x1, y1) = (y1, x1);
            }

            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            float dx = x1 - x0;
            float dy = y1 - y0;
            float gradient = (dx == 0) ? 1 : dy / dx;

            int xEnd = (int)x0;
            float yEnd = y0 + gradient * (xEnd - x0);
            float xGap = RFracPart(x0 + 0.5f);
            int xPxl1 = xEnd;
            int yPxl1 = IPart(yEnd);

            if (steep)
            {
                Plot(g, yPxl1, xPxl1, RFracPart(yEnd) * xGap, color);
                Plot(g, yPxl1 + 1, xPxl1, FracPart(yEnd) * xGap, color);
            }
            else
            {
                Plot(g, xPxl1, yPxl1, RFracPart(yEnd) * xGap, color);
                Plot(g, xPxl1, yPxl1 + 1, FracPart(yEnd) * xGap, color);
            }

            float intery = yEnd + gradient;

            xEnd = (int)(x1);
            yEnd = y1 + gradient * (xEnd - x1);
            xGap = FracPart(x1 + 0.5f);
            int xPxl2 = xEnd;
            int yPxl2 = IPart(yEnd);

            if (steep)
            {
                Plot(g, yPxl2, xPxl2, RFracPart(yEnd) * xGap, color);
                Plot(g, yPxl2 + 1, xPxl2, FracPart(yEnd) * xGap, color);
            }
            else
            {
                Plot(g, xPxl2, yPxl2, RFracPart(yEnd) * xGap, color);
                Plot(g, xPxl2, yPxl2 + 1, FracPart(yEnd) * xGap, color);
            }

            if (steep)
            {
                for (int x = xPxl1 + 1; x < xPxl2; x++)
                {
                    Plot(g, IPart(intery), x, RFracPart(intery), color);
                    Plot(g, IPart(intery) + 1, x, FracPart(intery), color);
                    intery += gradient;
                }
            }
            else
            {
                for (int x = xPxl1 + 1; x < xPxl2; x++)
                {
                    Plot(g, x, IPart(intery), RFracPart(intery), color);
                    Plot(g, x, IPart(intery) + 1, FracPart(intery), color);
                    intery += gradient;
                }
            }
        }
        public void DrawNoConstraintEdge(NoConstraintEdge edge, Graphics g, Pen pen, bool bresenham, bool wu)
        {
            if (wu)
                DrawLineXiaolinWu(edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y, g);
            else if (bresenham)
                DrawLineBresenham(edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y, g);
            else
                g.DrawLine(pen, edge.Start, edge.End);
        }

        public void DrawHorizontalEdge(HorizontalEdge edge, Graphics g, Pen pen, bool bresenham, bool wu)
        {
            if (wu)
                DrawLineXiaolinWu(edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y, g);
            else if (bresenham)
                DrawLineBresenham(edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y, g);
            else
                g.DrawLine(pen, edge.Start, edge.End);

            Point center = edge.GetEdgeCenter();
            int offsetY = 10;
            g.DrawLine(new Pen(Color.Blue, 2), center.X - 5, center.Y + offsetY, center.X + 5, center.Y + offsetY);
        }

        public void DrawVerticalEdge(VerticalEdge edge, Graphics g, Pen pen, bool bresenham, bool wu)
        {
            if (wu)
                DrawLineXiaolinWu(edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y, g);
            else if (bresenham)
                DrawLineBresenham(edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y, g);
            else
                g.DrawLine(pen, edge.Start, edge.End);

            Point center = edge.GetEdgeCenter();
            int offsetX = 10;
            g.DrawLine(new Pen(Color.Green, 2), center.X + offsetX, center.Y - 5, center.X + offsetX, center.Y + 5);
        }

        public void DrawFixedLengthEdge(FixedLengthEdge edge, Graphics g, Pen pen, bool bresenham, bool wu)
        {
            if (wu)
                DrawLineXiaolinWu(edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y, g);
            else if (bresenham)
                DrawLineBresenham(edge.Start.X, edge.Start.Y, edge.End.X, edge.End.Y, g);
            else
                g.DrawLine(pen, edge.Start, edge.End );

            Point center = edge.GetEdgeCenter();
            int offsetY = 10;
            g.DrawEllipse(new Pen(Color.Red, 2), center.X - 5, center.Y + offsetY - 5, 10, 10);
            g.DrawString(edge.Length.ToString("F1"), new Font("Arial", 8), Brushes.Black, center.X + 10, center.Y + offsetY - 10);
        }
        public void DrawBezier(BezierEdge edge, Graphics g, Pen pen, bool bresenham, bool wu)
        {
           PointF A0 = edge.Start;
            PointF A1 = edge.ControlPoint1;
            PointF A2 = edge.ControlPoint2;
            PointF A3 = edge.End;

            int segmentCount = (int)(Math.Sqrt(Math.Pow(A3.X - A0.X, 2) + Math.Pow(A3.Y - A0.Y, 2))/10);

            float d = 1f / segmentCount;

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

            PointF currentPoint = A0;
            PointF previousPoint = currentPoint;

            for (int i = 0; i < segmentCount; i++)
            {
                currentPoint = new PointF(currentPoint.X + deltaP.X, currentPoint.Y + deltaP.Y);
                deltaP = new PointF(deltaP.X + delta2P.X, deltaP.Y + delta2P.Y);
                delta2P = new PointF(delta2P.X + delta3P.X, delta2P.Y + delta3P.Y);

                if (wu)
                    DrawLineXiaolinWu((int)Math.Round(previousPoint.X), (int)Math.Round(previousPoint.Y), (int)Math.Round(currentPoint.X), (int)Math.Round(currentPoint.Y),g);
                else
                    g.DrawLine(pen, previousPoint, currentPoint);

                previousPoint = currentPoint;
            }

            SolidBrush brush = new SolidBrush(Color.Brown);
            Pen dashedPen = new Pen(Color.FromArgb(128, Color.Brown), 1)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
            };

            g.FillEllipse(brush, A1.X - 3, A1.Y - 3, 6, 6);
            g.FillEllipse(brush, A2.X - 3, A2.Y - 3, 6, 6);

            g.DrawLine(dashedPen, A0, A1); 
            g.DrawLine(dashedPen, A1, A2); 
            g.DrawLine(dashedPen, A2, A3); 
            g.DrawLine(dashedPen, A3, A0);

            
            brush.Dispose();
            dashedPen.Dispose();
        }
    }

}
