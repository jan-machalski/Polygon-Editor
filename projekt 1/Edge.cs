﻿using System;
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

        public virtual void Draw(Graphics g, Pen pen)
        {
            g.DrawLine(pen, Start, End);
        }

        protected Point GetEdgeCenter()
        {
            // Oblicz środek krawędzi
            return new Point((Start.X + End.X) / 2, (Start.Y + End.Y) / 2);
        }
        public abstract Edge Clone();
        public abstract bool Accept(IEdgeVisitor visitor, Edge nextEdge, Point delta);
        public abstract bool AcceptNoConstraint(IEdgeVisitor visitor, NoConstraintEdge edge, Point delta);
        public abstract bool AcceptVertical(IEdgeVisitor visitor, VerticalEdge edge, Point delta);
        public abstract bool AcceptHorizontal(IEdgeVisitor visitor, HorizontalEdge edge, Point delta);
        public abstract bool AcceptFixedLength(IEdgeVisitor visitor, FixedLengthEdge edge, Point delta);
    }

    public class NoConstraintEdge : Edge
    {
        public NoConstraintEdge(Point start, Point end) : base(start, end) { }

        public override void Draw(Graphics g, Pen pen)
        {
            base.Draw(g, pen);
            // Brak dodatkowej ikonki, bo nie ma ograniczenia
        }
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
    }

    public class HorizontalEdge : Edge
    {
        public HorizontalEdge(Point start, Point end) : base(start, end) { }

        public override void Draw(Graphics g, Pen pen)
        {
            base.Draw(g, pen);

            // Rysuj małą poziomą linię jako ikonę, przesuniętą w dół od środka krawędzi
            Point center = GetEdgeCenter();
            Pen iconPen = new Pen(Color.Blue, 2); // Używamy grubszej linii dla ikonki
            int offsetY = 10; // Przesunięcie w dół o 10 pikseli
            g.DrawLine(iconPen, center.X - 5, center.Y + offsetY, center.X + 5, center.Y + offsetY); // Pozioma linia o długości 10 px
        }
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
    }

    public class VerticalEdge : Edge
    {
        public VerticalEdge(Point start, Point end) : base(start, end) { }

        public override void Draw(Graphics g, Pen pen)
        {
            base.Draw(g, pen);

            // Rysuj małą pionową linię jako ikonę, przesuniętą w prawo od środka krawędzi
            Point center = GetEdgeCenter();
            Pen iconPen = new Pen(Color.Green, 2); // Używamy grubszej linii dla ikonki
            int offsetX = 10; // Przesunięcie w prawo o 10 pikseli
            g.DrawLine(iconPen, center.X + offsetX, center.Y - 5, center.X + offsetX, center.Y + 5); // Pionowa linia o długości 10 px
        }
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
    }

    public class FixedLengthEdge : Edge
    {
        public double Length { get; private set; }

        public FixedLengthEdge(Point start, Point end, double length) : base(start, end)
        {
            Length = length;
        }

        public override void Draw(Graphics g, Pen pen)
        {
            base.Draw(g, pen);

            // Rysuj ikonę dla krawędzi o ustalonej długości, przesuniętą w dół
            Point center = GetEdgeCenter();
            Pen iconPen = new Pen(Color.Red, 2);
            int offsetY = 10; // Przesunięcie w dół o 10 pikseli
            g.DrawEllipse(iconPen, center.X - 5, center.Y + offsetY - 5, 10, 10); // Okrąg o średnicy 10 pikseli

            // Wyświetl długość obok krawędzi, również z przesunięciem w dół
            g.DrawString(Length.ToString("F1"), new Font("Arial", 8), Brushes.Black, center.X + 10, center.Y + offsetY - 10);
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
    }


}
