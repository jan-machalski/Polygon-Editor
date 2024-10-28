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
        public Edge() { }

        public Point GetEdgeCenter()
        {
            // Oblicz środek krawędzi
            return new Point((Start.X + End.X) / 2, (Start.Y + End.Y) / 2);
        }
        public abstract Edge Clone();
        public abstract void AcceptDraw(IEdgeDrawingVisitor visitor, Graphics g, Pen pen, bool bresenham);
        public abstract bool Accept(IEdgeVisitor visitor, Edge nextEdge, Point delta);
        public abstract bool AcceptNoConstraint(IEdgeVisitor visitor, NoConstraintEdge edge, Point delta);
        public abstract bool AcceptVertical(IEdgeVisitor visitor, VerticalEdge edge, Point delta);
        public abstract bool AcceptHorizontal(IEdgeVisitor visitor, HorizontalEdge edge, Point delta);
        public abstract bool AcceptFixedLength(IEdgeVisitor visitor, FixedLengthEdge edge, Point delta);
    }

    public class NoConstraintEdge : Edge
    {
        public NoConstraintEdge() { }
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
        public override void AcceptDraw(IEdgeDrawingVisitor visitor, Graphics g, Pen pen, bool bresenham)
        {
            visitor.DrawNoConstraintEdge(this, g, pen, bresenham);
        }
    }

    public class HorizontalEdge : Edge
    {
        public HorizontalEdge() { }
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
        public override void AcceptDraw(IEdgeDrawingVisitor visitor, Graphics g, Pen pen, bool bresenham)
        {
            visitor.DrawHorizontalEdge(this, g, pen, bresenham);
        }
    }

    public class VerticalEdge : Edge
    {
        public VerticalEdge() { }
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
        public override void AcceptDraw(IEdgeDrawingVisitor visitor, Graphics g, Pen pen, bool bresenham)
        {
            visitor.DrawVerticalEdge(this, g, pen, bresenham);
        }
    }

    public class FixedLengthEdge : Edge
    {
        public FixedLengthEdge() { }
        public double Length { get; set; }

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
        public override void AcceptDraw(IEdgeDrawingVisitor visitor, Graphics g, Pen pen, bool bresenham)
        {
            visitor.DrawFixedLengthEdge(this, g, pen,bresenham);
        }
    }
    public class BezierEdge : Edge
    {
        public Point ControlPoint1 { get; set; }  // Pierwszy punkt kontrolny dla Béziera
        public Point ControlPoint2 { get; set; }  // Drugi punkt kontrolny dla Béziera

        // Określa wymaganą ciągłość na początku i końcu krawędzi Béziera
        public ContinuityType StartContinuity { get; set; }
        public ContinuityType EndContinuity { get; set; }

        // Typy ciągłości
        public enum ContinuityType
        {
            G0,  // Brak ciągłości
            G1,  // Ciągłość jednostkowego wektora stycznego
            C1   // Ciągłość wektora stycznego (pełna ciągłość C1)
        }
        public BezierEdge() { }

        // Konstruktor dla automatycznego dopasowania punktów pomocniczych na podstawie sąsiednich krawędzi
        public BezierEdge(Point start, Point end, Edge adjacentStartEdge, Edge adjacentEndEdge) : base(start, end)
        {
            StartContinuity = ContinuityType.C1;
            EndContinuity = ContinuityType.C1;

            // Dopasowanie punktów kontrolnych, aby zachować ciągłość C1
            SetControlPointsForC1(adjacentStartEdge, adjacentEndEdge);
        }

        // Konstruktor, który przyjmuje wszystkie punkty włącznie z punktami pomocniczymi
        public BezierEdge(Point start, Point end, Point controlPoint1, Point controlPoint2) : base(start, end)
        {
            ControlPoint1 = controlPoint1;
            ControlPoint2 = controlPoint2;

            // Domyślna ciągłość, jeśli sąsiadujące krawędzie nie są uwzględniane
            StartContinuity = ContinuityType.G0;
            EndContinuity = ContinuityType.G0;
        }

        // Ustawia punkty kontrolne, aby zachować ciągłość C1 w punktach końcowych
        private void SetControlPointsForC1(Edge adjacentStartEdge, Edge adjacentEndEdge)
        {
            if (adjacentStartEdge is BezierEdge startBezier)
            {
                StartContinuity = ContinuityType.C1;
                ControlPoint1 = new Point(
                    2 * Start.X - startBezier.ControlPoint2.X,
                    2 * Start.Y - startBezier.ControlPoint2.Y
                );
                startBezier.EndContinuity = ContinuityType.C1;
            }
            else
            {
                ControlPoint1 = new Point(
                    (int)(Start.X + (adjacentStartEdge.End.X-adjacentStartEdge.Start.X)),
                    (int)(Start.Y + (adjacentStartEdge.End.Y-adjacentStartEdge.Start.Y))
                );
            }

            if (adjacentEndEdge is BezierEdge endBezier)
            {
                EndContinuity = ContinuityType.C1;
                ControlPoint2 = new Point(
                    2 * End.X - endBezier.ControlPoint1.X,
                    2 * End.Y - endBezier.ControlPoint1.Y
                );
                endBezier.StartContinuity = ContinuityType.C1;
            }
            else
            {
                ControlPoint2 = new Point(
                    (int)(End.X + (adjacentEndEdge.Start.X - adjacentEndEdge.End.X)),
                    (int)(End.Y + (adjacentEndEdge.Start.Y - adjacentEndEdge.End.Y))
                );
            }
        }

        private double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        public override Edge Clone()
        {
            return new BezierEdge(Start, End, ControlPoint1, ControlPoint2)
            {
                StartContinuity = this.StartContinuity,
                EndContinuity = this.EndContinuity
            };
        }
        public override void AcceptDraw(IEdgeDrawingVisitor visitor, Graphics g, Pen pen, bool bresenham)
        {
            visitor.DrawBezier(this,g,pen, bresenham);
        }
        public override bool Accept(IEdgeVisitor visitor, Edge nextEdge, Point delta)
        {
            throw new NotImplementedException();
        }
        public override bool AcceptNoConstraint(IEdgeVisitor visitor, NoConstraintEdge edge, Point delta)
        {
            return visitor.Visit(edge,this,delta);
        }
        public override bool AcceptVertical(IEdgeVisitor visitor, VerticalEdge edge, Point delta)
        {
            return visitor.Visit(edge, this, delta);
        }
        public override bool AcceptHorizontal(IEdgeVisitor visitor, HorizontalEdge edge, Point delta)
        {
            return visitor.Visit(edge,this,delta);
        }
        public override bool AcceptFixedLength(IEdgeVisitor visitor, FixedLengthEdge edge, Point delta)
        {
            return visitor.Visit(edge, this, delta);
        }


    }



}
