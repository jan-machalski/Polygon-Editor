using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_1
{
    public interface IEdgeDrawingVisitor
    {
        void DrawNoConstraintEdge(NoConstraintEdge edge, Graphics g, Pen pen);
        void DrawHorizontalEdge(HorizontalEdge edge, Graphics g, Pen pen);
        void DrawVerticalEdge(VerticalEdge edge, Graphics g, Pen pen);
        void DrawFixedLengthEdge(FixedLengthEdge edge, Graphics g, Pen pen);
    }
    public class EdgeDrawingVisitor : IEdgeDrawingVisitor
    {
        public void DrawNoConstraintEdge(NoConstraintEdge edge, Graphics g, Pen pen)
        {
            g.DrawLine(pen, edge.Start, edge.End);
        }

        public void DrawHorizontalEdge(HorizontalEdge edge, Graphics g, Pen pen)
        {
            g.DrawLine(pen, edge.Start, edge.End);

            // Rysowanie ikony poziomej
            Point center = edge.GetEdgeCenter();
            int offsetY = 10;
            g.DrawLine(new Pen(Color.Blue, 2), center.X - 5, center.Y + offsetY, center.X + 5, center.Y + offsetY);
        }

        public void DrawVerticalEdge(VerticalEdge edge, Graphics g, Pen pen)
        {
            g.DrawLine(pen, edge.Start, edge.End);

            // Rysowanie ikony pionowej
            Point center = edge.GetEdgeCenter();
            int offsetX = 10;
            g.DrawLine(new Pen(Color.Green, 2), center.X + offsetX, center.Y - 5, center.X + offsetX, center.Y + 5);
        }

        public void DrawFixedLengthEdge(FixedLengthEdge edge, Graphics g, Pen pen)
        {
            g.DrawLine(pen, edge.Start, edge.End);

            // Rysowanie ikony i długości
            Point center = edge.GetEdgeCenter();
            int offsetY = 10;
            g.DrawEllipse(new Pen(Color.Red, 2), center.X - 5, center.Y + offsetY - 5, 10, 10);
            g.DrawString(edge.Length.ToString("F1"), new Font("Arial", 8), Brushes.Black, center.X + 10, center.Y + offsetY - 10);
        }
    }

}
