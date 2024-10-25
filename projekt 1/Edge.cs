using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_1
{
    public abstract class Edge
    {
        public Point Start { get; set; }
        public Point End { get; set; }

        public Edge(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        public Point GetEdgeCenter()
        {
            // Oblicz środek krawędzi
            return new Point((Start.X + End.X) / 2, (Start.Y + End.Y) / 2);
        }
        public abstract Edge Clone();
        public abstract void AcceptDraw(IEdgeDrawingVisitor visitor, Graphics g, Pen pen);
        public abstract bool Accept(IEdgeVisitor visitor, Edge nextEdge, Point delta);
        public abstract bool AcceptNoConstraint(IEdgeVisitor visitor, NoConstraintEdge edge, Point delta);
        public abstract bool AcceptVertical(IEdgeVisitor visitor, VerticalEdge edge, Point delta);
        public abstract bool AcceptHorizontal(IEdgeVisitor visitor, HorizontalEdge edge, Point delta);
        public abstract bool AcceptFixedLength(IEdgeVisitor visitor, FixedLengthEdge edge, Point delta);
    }

    public class NoConstraintEdge : Edge
    {
        public NoConstraintEdge(Point start, Point end) : base(start, end) { }

        public override Edge Clone()
        {
            return new NoConstraintEdge(Start, End);
        }
        public override bool Accept(IEdgeVisitor visitor, Edge nextEdge, Point delta)
        {
            return nextEdge.AcceptNoConstraint(visitor, this, delta);
        }

        // Metoda dla podwójnej dyspozycji: NoConstraintEdge jako następna krawędź
        public override bool AcceptNoConstraint(IEdgeVisitor visitor, NoConstraintEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }

        public override bool AcceptHorizontal(IEdgeVisitor visitor, HorizontalEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }

        public override bool AcceptVertical(IEdgeVisitor visitor, VerticalEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }

        public override bool AcceptFixedLength(IEdgeVisitor visitor, FixedLengthEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }
        public override void AcceptDraw(IEdgeDrawingVisitor visitor, Graphics g, Pen pen)
        {
            visitor.DrawNoConstraintEdge(this, g, pen);
        }
    }

    public class HorizontalEdge : Edge
    {
        public HorizontalEdge(Point start, Point end) : base(start, end) { }

        public override Edge Clone()
        {
            return new HorizontalEdge(Start, End);
        }
        public override bool Accept(IEdgeVisitor visitor, Edge nextEdge, Point delta)
        {
            return nextEdge.AcceptHorizontal(visitor, this, delta);
        }

        // Metoda dla podwójnej dyspozycji: HorizontalEdge jako następna krawędź
        public override bool AcceptNoConstraint(IEdgeVisitor visitor, NoConstraintEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }

        public override bool AcceptHorizontal(IEdgeVisitor visitor, HorizontalEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }

        public override bool AcceptVertical(IEdgeVisitor visitor, VerticalEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }

        public override bool AcceptFixedLength(IEdgeVisitor visitor, FixedLengthEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }
        public override void AcceptDraw(IEdgeDrawingVisitor visitor, Graphics g, Pen pen)
        {
            visitor.DrawHorizontalEdge(this, g, pen);
        }
    }

    public class VerticalEdge : Edge
    {
        public VerticalEdge(Point start, Point end) : base(start, end) { }

        public override Edge Clone()
        {
            return new VerticalEdge(Start, End);
        }
        public void ChangeNextEdge(ref NoConstraintEdge edge)
        {
            this.End = new Point(edge.Start.X,this.Start.Y);
            edge.Start = new Point(edge.Start.X, this.Start.Y);
        }
        public override bool Accept(IEdgeVisitor visitor, Edge nextEdge, Point delta)
        {
            return nextEdge.AcceptVertical(visitor, this, delta);
        }

        // Metoda dla podwójnej dyspozycji: VerticalEdge jako następna krawędź
        public override bool AcceptNoConstraint(IEdgeVisitor visitor, NoConstraintEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }

        public override bool AcceptHorizontal(IEdgeVisitor visitor, HorizontalEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }

        public override bool AcceptVertical(IEdgeVisitor visitor, VerticalEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }

        public override bool AcceptFixedLength(IEdgeVisitor visitor, FixedLengthEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }
        public override void AcceptDraw(IEdgeDrawingVisitor visitor, Graphics g, Pen pen)
        {
            visitor.DrawVerticalEdge(this, g, pen);
        }
    }

    public class FixedLengthEdge : Edge
    {
        public double Length { get; private set; }

        public FixedLengthEdge(Point start, Point end, double length) : base(start, end)
        {
            Length = length;
        }
        public override Edge Clone()
        {
            return new FixedLengthEdge(Start, End, Length);
        }
        public override bool Accept(IEdgeVisitor visitor, Edge nextEdge, Point delta)
        {
            return nextEdge.AcceptFixedLength(visitor, this, delta);
        }

        // Metoda dla podwójnej dyspozycji: FixedLengthEdge jako następna krawędź
        public override bool AcceptNoConstraint(IEdgeVisitor visitor, NoConstraintEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }

        public override bool AcceptHorizontal(IEdgeVisitor visitor, HorizontalEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }

        public override bool AcceptVertical(IEdgeVisitor visitor, VerticalEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }

        public override bool AcceptFixedLength(IEdgeVisitor visitor, FixedLengthEdge currentEdge, Point delta)
        {
            return visitor.Visit(currentEdge, this, delta);
        }
        public override void AcceptDraw(IEdgeDrawingVisitor visitor, Graphics g, Pen pen)
        {
            visitor.DrawFixedLengthEdge(this, g, pen);
        }
    }


}
