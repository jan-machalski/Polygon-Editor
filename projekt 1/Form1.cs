using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Windows.Forms.VisualStyles;

namespace projekt_1
{
    public partial class Form1 : Form
    {
        private List<Edge> edges = new List<Edge>();
        private bool drawingComplete = true;
        private Point? currentMousePosition = null;
        private ContextMenuStrip contextMenu = new ContextMenuStrip();
        private ContextMenuStrip contextMenuEdge = new ContextMenuStrip();
        private ContextMenuStrip contextMenuBezier = new ContextMenuStrip();
        private int clickedEdgeIndex = -1;
        private bool isDragging = false;
        private bool isDraggingVertex = false;
        private int draggedVertexIndex = -1;
        private int draggedCP1 = -1;
        private int draggedCP2 = -1;
        private Point dragStartPoint;
        private Point polygonOffset = Point.Empty;
        public bool wuClicked = false;
        public Form1()
        {
            InitializeComponent();

            contextMenu = new ContextMenuStrip();
            contextMenuEdge = new ContextMenuStrip();
            var deleteItem = new ToolStripMenuItem("Usuñ wierzcho³ek");
            contextMenuEdge.Items.Add("Dodaj wierzcho³ek", null, AddVertexMenuItem_Click);
            contextMenuEdge.Items.Add("Dodaj ograniczenie poziome", null, AddHorizontalConstraint_Click);
            contextMenuEdge.Items.Add("Dodaj ograniczenie pionowe", null, AddVerticalConstraint_Click);
            contextMenuEdge.Items.Add("Dodaj ograniczenie d³ugoœci", null, AddFixedLengthConstraint_Click);
            contextMenuEdge.Items.Add("Ustaw krzyw¹ Beziera", null, AddBezierEdgeConstraint_Click);
            contextMenuEdge.Items.Add("Usuñ ograniczenie", null, DeleteConstraint_Click);
            contextMenuBezier.Items.Add("Ustaw ci¹g³oœæ G0", null, SetG0_Click);
            contextMenuBezier.Items.Add("Ustaw ci¹g³oœæ G1", null, SetG1_Click);
            contextMenuBezier.Items.Add("Ustaw ci¹g³oœæ C1", null, SetC1_Click);
            contextMenuBezier.Items.Add("Usuñ wierzcho³ek", null, DeleteVertex_Click);
            deleteItem.Click += DeleteVertex_Click;
            contextMenu.Items.Add(deleteItem);

            InitEdges();
            Canvas.Invalidate();
        }
        private void clearButton_Click(object sender, EventArgs e)
        {
            edges.Clear();
            drawingComplete = false;
            currentMousePosition = null;
            Canvas.Invalidate(); 
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen edgePen = new Pen(Color.Black, 1); 
            Pen vertexPen = new Pen(Color.Black, 2); 
            Brush fillBrush = new SolidBrush(Color.LightBlue); 
            EdgeDrawingVisitor drawingVisitor = new EdgeDrawingVisitor();
            bool bresenham = bresenhamButton.Checked;

            foreach (var edge in edges)
            {
                edge.AcceptDraw(drawingVisitor, g, edgePen, bresenham, wuClicked);
            }

            for (int i = 0; i < edges.Count; i++)
            {
                if (edges[i] is BezierEdge be)
                {
                    string continuityInfo = be.StartContinuity.ToString();
                    PointF textPosition = new PointF(be.Start.X + 5, be.Start.Y - 15);

                    g.DrawString(continuityInfo, new Font("Arial", 8), Brushes.Black, textPosition);
                    if (edges[(i + 1) % edges.Count] is not BezierEdge)
                    {
                        continuityInfo = be.EndContinuity.ToString();
                        textPosition = new PointF(be.End.X + 5, be.End.Y - 15);

                        g.DrawString(continuityInfo, new Font("Arial", 8), Brushes.Black, textPosition);
                    }
                }
            }
            Pen blackPen = new Pen(Color.Black);
            foreach (var edge in edges)
            {
                g.DrawEllipse(blackPen, edge.Start.X - 5, edge.Start.Y - 5, 10, 10);
            }
            if (edges.Count > 0)
            {
                g.DrawEllipse(blackPen, edges.Last().End.X - 5, edges.Last().End.Y - 5, 10, 10); 
            }

            if (!drawingComplete && edges.Count > 0)
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    g.DrawLine(edgePen, edges[i].Start, edges[i].End);
                }

                if (currentMousePosition != null)
                {
                    Pen semiTransparentPen = new Pen(Color.FromArgb(128, Color.Black), 1); 
                    g.DrawLine(semiTransparentPen, edges.Last().End, currentMousePosition.Value);
                }
            }
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (drawingComplete)
            {
                if (e.Button == MouseButtons.Right)
                {
                    Point clickLocation = new Point(e.X, e.Y);
                    clickedEdgeIndex = FindNearestVertex(clickLocation); 

                    if (clickedEdgeIndex != -1)
                    {
                        if (edges[clickedEdgeIndex] is BezierEdge || edges[(clickedEdgeIndex + 1) % edges.Count] is BezierEdge)
                            contextMenuBezier.Show(Canvas, e.Location);
                        else
                            contextMenu.Show(Canvas, e.Location);
                    }
                    else
                    {
                        clickedEdgeIndex = FindNearestEdge(clickLocation);
                        if (clickedEdgeIndex != -1)
                        {
                            contextMenuEdge.Show(Canvas, e.Location);
                        }
                    }
                }
                return;
            }

            Point clickLocationLeft = new Point(e.X, e.Y);

            if (edges.Count > 2 && IsFirstEdgeClicked(clickLocationLeft))
            {
                drawingComplete = true;
                edges.Add(new NoConstraintEdge(edges.Last().End, edges[0].Start));
                edges.RemoveAt(0);
                Canvas.Invalidate();
            }
            else
            {
                if (edges.Count > 0)
                {
                    edges.Add(new NoConstraintEdge(edges.Last().End, clickLocationLeft));
                }
                else
                {
                    edges.Add(new NoConstraintEdge(clickLocationLeft, clickLocationLeft));
                }

                Canvas.Invalidate(); 
            }
        }
        private void UpdateEdgesWithVisitor(int draggedVertexIndex, Point delta)
        {

            EdgeStartChangeVisitor forwardVisitor = new EdgeStartChangeVisitor();
            EdgeEndChangeVisitor backwardVisitor = new EdgeEndChangeVisitor();
            List<Edge> edgeCopy = edges.Select(e => e.Clone()).ToList();
            int forwardIndex = (draggedVertexIndex + 1) % edges.Count;
            int backwardIndex = draggedVertexIndex;
            bool forwardSuccess = true;
            bool backwardSuccess = true;
            int modifiedEdgesCount = 2;
            if (edges.Count == 3)
                modifiedEdgesCount = 1;
            edgeCopy[forwardIndex].Start = edgeCopy[backwardIndex].End = new Point(edges[forwardIndex].Start.X + delta.X, edges[forwardIndex].Start.Y + delta.Y);

            if (edgeCopy[forwardIndex] is BezierEdge be2)
            {
                if (be2.StartContinuity != BezierEdge.ContinuityType.G0)
                    be2.ControlPoint1 = new Point(be2.ControlPoint1.X + delta.X, be2.ControlPoint1.Y + delta.Y);
            }
            else
            {
                for (int i = 0; i < edges.Count && forwardSuccess; i++)
                {
                    var currentEdge = edgeCopy[forwardIndex];
                    var nextEdge = edgeCopy[(forwardIndex + 1) % edgeCopy.Count];

                    forwardSuccess = currentEdge.Accept(forwardVisitor, nextEdge, delta);

                    modifiedEdgesCount++;

                    if (forwardSuccess == false)
                        break;


                    if ((forwardIndex + 1) % edges.Count == draggedVertexIndex)
                    {
                        break;
                    }

                    forwardIndex = (forwardIndex + 1) % edges.Count;
                }
            }
            if (edgeCopy[backwardIndex] is BezierEdge be3)
            {
                if (be3.EndContinuity != BezierEdge.ContinuityType.G0)
                    be3.ControlPoint2 = new Point(be3.ControlPoint2.X + delta.X, be3.ControlPoint2.Y + delta.Y);
            }
            else
            {
                for (int i = 0; i < edges.Count && backwardSuccess; i++)
                {
                    var currentEdge = edgeCopy[backwardIndex];
                    var prevEdge = edgeCopy[(backwardIndex + edges.Count - 1) % edgeCopy.Count];

                    backwardSuccess = currentEdge.Accept(backwardVisitor, prevEdge, delta);

                    modifiedEdgesCount++;

                    if (backwardSuccess == false)
                        break;

                    if ((backwardIndex + edges.Count - 1) % edges.Count == draggedVertexIndex)
                    {
                        break;
                    }

                    backwardIndex = (backwardIndex + edges.Count - 1) % edges.Count;
                }
            }

            if (modifiedEdgesCount > edges.Count)
            {
                foreach (var edge in edges)
                {
                    edge.Start = new Point(edge.Start.X + delta.X, edge.Start.Y + delta.Y);
                    edge.End = new Point(edge.End.X + delta.X, edge.End.Y + delta.Y);
                    if (edge is BezierEdge be)
                    {
                        be.ControlPoint1 = new Point(be.ControlPoint1.X + delta.X, be.ControlPoint1.Y + delta.Y);
                        be.ControlPoint2 = new Point(be.ControlPoint2.X + delta.X, be.ControlPoint2.Y + delta.Y);
                    }
                }
            }
            else
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    edges[i] = edgeCopy[i].Clone();
                }
                UpdateContinuity();
            }

        }
        private void UpdateWithCP1(int idx, Point delta)
        {
            BezierEdge be = (BezierEdge)edges[idx];
            if (edges[(idx - 1 + edges.Count) % edges.Count] is BezierEdge be2)
            {
                if (be2.EndContinuity == BezierEdge.ContinuityType.C1)
                {
                    be.ControlPoint1 = new Point(be.ControlPoint1.X + delta.X, be.ControlPoint1.Y + delta.Y);
                    be2.ControlPoint2 = new Point(be2.ControlPoint2.X - delta.X, be2.ControlPoint2.Y - delta.Y);
                }
                else if (be2.EndContinuity == BezierEdge.ContinuityType.G1)
                {
                    be.ControlPoint1 = new Point(be.ControlPoint1.X + delta.X, be.ControlPoint1.Y + delta.Y);
                    float originalDistance = (float)Math.Sqrt(Math.Pow(be2.ControlPoint2.X - be2.End.X, 2) + Math.Pow(be2.ControlPoint2.Y - be2.End.Y, 2));
                    float edgeLength = (float)Math.Sqrt(Math.Pow(be.ControlPoint1.X - be.Start.X, 2) + Math.Pow(be.ControlPoint1.Y - be.Start.Y, 2));
                    if (edgeLength != 0)
                    {
                        PointF direction = new PointF(
                            (be.ControlPoint1.X - be.Start.X) / edgeLength,
                            (be.ControlPoint1.Y - be.Start.Y) / edgeLength);

                        be2.ControlPoint2 = new Point(
                            (int)Math.Round(be2.End.X - direction.X * originalDistance),
                            (int)Math.Round(be2.End.Y - direction.Y * originalDistance));
                    }
                }
                else
                    be.ControlPoint1 = new Point(be.ControlPoint1.X + delta.X, be.ControlPoint1.Y + delta.Y);
            }
            else
            {
                be.ControlPoint1 = new Point(be.ControlPoint1.X + delta.X, be.ControlPoint1.Y + delta.Y);

                EdgeEndChangeVisitor backwardVisitor = new EdgeEndChangeVisitor();
                if (be.StartContinuity == BezierEdge.ContinuityType.G0)
                    return;

                int backwardIndex = (idx - 1 + edges.Count) % edges.Count;
                bool backwardSuccess = true;

                if (edges[backwardIndex] is HorizontalEdge || edges[backwardIndex] is VerticalEdge || (edges[backwardIndex] is FixedLengthEdge && be.StartContinuity == BezierEdge.ContinuityType.C1))
                {
                    be.Start = new Point(be.Start.X + delta.X, be.Start.Y + delta.Y);
                    edges[backwardIndex].End = new Point(edges[backwardIndex].End.X + delta.X, edges[backwardIndex].End.Y + delta.Y);
                }
                else
                {
                    if (be.StartContinuity == BezierEdge.ContinuityType.C1)
                    {
                        delta.X = -delta.X;
                        delta.Y = -delta.Y;
                    }
                    else
                    {
                        Edge backwardEdge = edges[backwardIndex];
                        float backwardEdgeLength = (float)Math.Sqrt(
                           Math.Pow(backwardEdge.End.X - backwardEdge.Start.X, 2) +
                           Math.Pow(backwardEdge.End.Y - backwardEdge.Start.Y, 2));
                        PointF direction = new PointF(
                            be.Start.X - be.ControlPoint1.X,
                            be.Start.Y - be.ControlPoint1.Y);
                        float directionLength = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
                        if (directionLength != 0)
                        {
                            direction = new PointF(
                                direction.X / directionLength,
                                direction.Y / directionLength);

                            Point previousStart = backwardEdge.Start;

                            Point newStart = new Point(
                                (int)Math.Round(backwardEdge.End.X + direction.X * backwardEdgeLength),
                                (int)Math.Round(backwardEdge.End.Y + direction.Y * backwardEdgeLength));
                            delta = new Point(newStart.X - previousStart.X, newStart.Y - previousStart.Y);
                        }
                    }
                }

                edges[backwardIndex].Start = new Point(edges[backwardIndex].Start.X + delta.X, edges[backwardIndex].Start.Y + delta.Y);
                backwardIndex = (backwardIndex + edges.Count - 1) % edges.Count;
                edges[backwardIndex].End = new Point(edges[backwardIndex].End.X + delta.X, edges[backwardIndex].End.Y + delta.Y);
                while (backwardSuccess)
                {
                    var currentEdge = edges[backwardIndex];
                    var prevEdge = edges[(backwardIndex + edges.Count - 1) % edges.Count];

                    backwardSuccess = currentEdge.Accept(backwardVisitor, prevEdge, delta);

                    backwardIndex = (backwardIndex + edges.Count - 1) % edges.Count;
                }
                UpdateContinuity();
            }
        }
        private void UpdateWithCP2(int idx, Point delta)
        {
            BezierEdge be = (BezierEdge)edges[idx];
            if (edges[(idx + 1) % edges.Count] is BezierEdge be2)
            {
                if (be2.StartContinuity == BezierEdge.ContinuityType.C1)
                {
                    be.ControlPoint2 = new Point(be.ControlPoint2.X + delta.X, be.ControlPoint2.Y + delta.Y);
                    be2.ControlPoint1 = new Point(be2.ControlPoint1.X - delta.X, be2.ControlPoint1.Y - delta.Y);
                }
                else if (be2.StartContinuity == BezierEdge.ContinuityType.G1)
                {
                    be.ControlPoint2 = new Point(be.ControlPoint2.X + delta.X, be.ControlPoint2.Y + delta.Y);
                    float originalDistance = (float)Math.Sqrt(Math.Pow(be2.ControlPoint1.X - be2.Start.X, 2) + Math.Pow(be2.ControlPoint1.Y - be2.Start.Y, 2));
                    float edgeLength = (float)Math.Sqrt(Math.Pow(be.ControlPoint2.X - be.End.X, 2) + Math.Pow(be.ControlPoint2.Y - be.End.Y, 2));
                    if (edgeLength != 0)
                    {
                        PointF direction = new PointF(
                            (be.ControlPoint2.X - be.End.X) / edgeLength,
                            (be.ControlPoint2.Y - be.End.Y) / edgeLength);

                        be2.ControlPoint1 = new Point(
                            (int)Math.Round(be2.Start.X - direction.X * originalDistance),
                            (int)Math.Round(be2.Start.Y - direction.Y * originalDistance));
                    }
                }
                else
                    be.ControlPoint2 = new Point(be.ControlPoint2.X + delta.X, be.ControlPoint2.Y + delta.Y);
            }
            else
            {
                be.ControlPoint2 = new Point(be.ControlPoint2.X + delta.X, be.ControlPoint2.Y + delta.Y);

                EdgeStartChangeVisitor backwardVisitor = new EdgeStartChangeVisitor();
                if (be.EndContinuity == BezierEdge.ContinuityType.G0)
                    return;

                int forwardIndex = (idx + 1) % edges.Count;
                bool forwardSuccess = true;

                if (edges[forwardIndex] is HorizontalEdge || edges[forwardIndex] is VerticalEdge || (edges[forwardIndex] is FixedLengthEdge && be.EndContinuity == BezierEdge.ContinuityType.C1))
                {
                    be.End = new Point(be.End.X + delta.X, be.End.Y + delta.Y);

                    edges[forwardIndex].Start = new Point(edges[forwardIndex].Start.X + delta.X, edges[forwardIndex].Start.Y + delta.Y);
                }
                else
                {
                    if (be.EndContinuity == BezierEdge.ContinuityType.C1)
                    {
                        delta.X = -delta.X;
                        delta.Y = -delta.Y;
                    }
                    else
                    {
                        Edge forwardEdge = edges[forwardIndex];
                        float forwardEdgeLength = (float)Math.Sqrt(
                           Math.Pow(forwardEdge.End.X - forwardEdge.Start.X, 2) +
                           Math.Pow(forwardEdge.End.Y - forwardEdge.Start.Y, 2));
                        PointF direction = new PointF(
                            be.End.X - be.ControlPoint2.X,
                            be.End.Y - be.ControlPoint2.Y);
                        float directionLength = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
                        if (directionLength != 0)
                        {
                            direction = new PointF(
                                direction.X / directionLength,
                                direction.Y / directionLength);

                            Point previousEnd = forwardEdge.End;

                            Point newEnd = new Point(
                                (int)Math.Round(forwardEdge.Start.X + direction.X * forwardEdgeLength),
                                (int)Math.Round(forwardEdge.Start.Y + direction.Y * forwardEdgeLength));
                            delta = new Point(
                                newEnd.X - previousEnd.X,
                                newEnd.Y - previousEnd.Y);
                        }
                    }
                }
                edges[forwardIndex].End = new Point(edges[forwardIndex].End.X + delta.X, edges[forwardIndex].End.Y + delta.Y);
                forwardIndex = (forwardIndex + 1) % edges.Count;
                edges[forwardIndex].Start = new Point(edges[forwardIndex].Start.X + delta.X, edges[forwardIndex].Start.Y + delta.Y);
                while (forwardSuccess)
                {
                    var currentEdge = edges[forwardIndex];
                    var nextEdge = edges[(forwardIndex + 1) % edges.Count];

                    forwardSuccess = currentEdge.Accept(backwardVisitor, nextEdge, delta);

                    forwardIndex = (forwardIndex + edges.Count - 1) % edges.Count;
                }
                UpdateContinuity();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouseLocation = new Point(e.X, e.Y);

            if (!(drawingComplete || edges.Count == 0))
            {
                currentMousePosition = new Point(e.X, e.Y); 
                Canvas.Invalidate(); 
                return;
            }
            if (isDraggingVertex)
            {
                Point currentMouseLocation = new Point(e.X, e.Y);
                if (draggedCP1 != -1)
                {
                    BezierEdge be = (BezierEdge)edges[draggedCP1];
                    Point d = new Point(currentMouseLocation.X - be.ControlPoint1.X,
                                currentMouseLocation.Y - be.ControlPoint1.Y);
                    UpdateWithCP1(draggedCP1, d);
                }
                else if (draggedCP2 != -1)
                {
                    BezierEdge be = (BezierEdge)edges[draggedCP2];
                    Point d = new Point(currentMouseLocation.X - be.ControlPoint2.X,
                                currentMouseLocation.Y - be.ControlPoint2.Y);
                    UpdateWithCP2(draggedCP2, d);
                }
                else
                {
                    Point delta = new Point(currentMouseLocation.X - edges[draggedVertexIndex].End.X,
                                    currentMouseLocation.Y - edges[draggedVertexIndex].End.Y);

                    UpdateEdgesWithVisitor(draggedVertexIndex, delta);
                }
                Canvas.Invalidate();

            }
            else if (isDragging)
            {
                Point currentMouseLocation = new Point(e.X, e.Y);
                int dx = currentMouseLocation.X - dragStartPoint.X;
                int dy = currentMouseLocation.Y - dragStartPoint.Y;

                for (int i = 0; i < edges.Count; i++)
                {
                    edges[i].Start = new Point(edges[i].Start.X + dx, edges[i].Start.Y + dy);
                    edges[i].End = new Point(edges[i].End.X + dx, edges[i].End.Y + dy);
                    if (edges[i] is BezierEdge bezierEdge)
                    {
                        bezierEdge.ControlPoint1 = new Point(bezierEdge.ControlPoint1.X + dx, bezierEdge.ControlPoint1.Y + dy);
                        bezierEdge.ControlPoint2 = new Point(bezierEdge.ControlPoint2.X + dx, bezierEdge.ControlPoint2.Y + dy);
                    }
                }

                dragStartPoint = currentMouseLocation;

                Canvas.Invalidate();
            }
            return;


        }
        private void AddVertexMenuItem_Click(object sender, EventArgs e)
        {
            if (clickedEdgeIndex == -1)
                return;

            Point start = edges[clickedEdgeIndex].Start;
            Point end = edges[clickedEdgeIndex].End;
            Point midpoint = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);

            edges.RemoveAt(clickedEdgeIndex);
            edges.Insert(clickedEdgeIndex, new NoConstraintEdge(midpoint, end));
            edges.Insert(clickedEdgeIndex, new NoConstraintEdge(start, midpoint));
            if (edges[(clickedEdgeIndex + 2) % edges.Count] is BezierEdge be)
            {
                be.AcceptNoConstraint(new EdgeStartChangeVisitor(), (NoConstraintEdge)edges[(clickedEdgeIndex + 1) % edges.Count], new Point());
            }
            if (edges[(clickedEdgeIndex + edges.Count - 1) % edges.Count] is BezierEdge be2)
                be2.AcceptNoConstraint(new EdgeEndChangeVisitor(), (NoConstraintEdge)edges[clickedEdgeIndex], new Point());

            Canvas.Invalidate();
        }
        private void DeleteVertex_Click(object sender, EventArgs e)
        {
            if (clickedEdgeIndex != -1)
            {
                if (edges.Count > 3)
                {
                    int edgeToRemoveIndex = clickedEdgeIndex;
                    int nextEdgeIndex = (clickedEdgeIndex + 1) % edges.Count;

                    Point newStart = edges[edgeToRemoveIndex].Start;
                    Point newEnd = edges[nextEdgeIndex].End; 
                    if (nextEdgeIndex == 0)
                    {
                        edges.RemoveAt(edgeToRemoveIndex);
                        edges.RemoveAt(nextEdgeIndex);
                        edges.Insert(0, new NoConstraintEdge(newStart, newEnd));
                        edgeToRemoveIndex = 0;
                    }
                    else
                    {
                        edges.RemoveAt(nextEdgeIndex); 

                        edges.RemoveAt(edgeToRemoveIndex); 
                        edges.Insert(edgeToRemoveIndex, new NoConstraintEdge(newStart, newEnd));
                    }
                    if (edges[(edgeToRemoveIndex + 1) % edges.Count] is BezierEdge be)
                    {
                        be.AcceptNoConstraint(new EdgeStartChangeVisitor(), (NoConstraintEdge)edges[(edgeToRemoveIndex)], new Point());
                    }
                    if (edges[(edgeToRemoveIndex + edges.Count - 1) % edges.Count] is BezierEdge be2)
                        be2.AcceptNoConstraint(new EdgeEndChangeVisitor(), (NoConstraintEdge)edges[edgeToRemoveIndex], new Point());

                }
                else if (edges.Count == 3)
                {
                    edges.Clear();
                    drawingComplete = false;
                }

                Canvas.Invalidate(); 
            }
        }
        private int FindNearestVertex(Point location)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                if (Distance(edges[i].End, location) < 10)
                {
                    return i;
                }
            }
            return -1; 
        }
        private int FindNearestEdge(Point location)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                if (edges[i] is BezierEdge bezierEdge)
                {
                    if (IsPointNearBezierCurve(bezierEdge.Start, bezierEdge.ControlPoint1, bezierEdge.ControlPoint2, bezierEdge.End, location, 5))
                    {
                        contextMenu.Tag = i;
                        return i;
                    }
                }
                else
                {
                    if (IsPointNearLine(edges[i].Start, edges[i].End, location, 5)) 
                    {
                        contextMenu.Tag = i; 
                        return i;
                    }
                }
            }
            return -1;
        }
        private bool IsFirstEdgeClicked(Point currentPoint)
        {
            if (edges.Count == 0)
                return false;

            return Distance(edges[0].Start, currentPoint) < 10 || Distance(edges[0].End, currentPoint) < 10;
        }
        private bool IsPointNearLine(Point start, Point end, Point point, int tolerance)
        {
            double distance = Math.Abs((end.Y - start.Y) * point.X - (end.X - start.X) * point.Y + end.X * start.Y - end.Y * start.X)
                              / Math.Sqrt(Math.Pow(end.Y - start.Y, 2) + Math.Pow(end.X - start.X, 2));

            bool withinXBounds = ((point.X >= (Math.Min(start.X, end.X) - tolerance)) && (point.X <= (Math.Max(start.X, end.X)) + tolerance));
            bool withinYBounds = (point.Y >= (Math.Min(start.Y, end.Y) - tolerance)) && (point.Y <= Math.Max(start.Y, end.Y) + tolerance);

            return distance <= tolerance && withinXBounds && withinYBounds;
        }
        private bool IsPointNearBezierCurve(Point start, Point control1, Point control2, Point end, Point location, double tolerance)
        {
            int segments = 50;
            for (int i = 0; i <= segments; i++)
            {
                double t = i / (double)segments;

                double x = Math.Pow(1 - t, 3) * start.X +
                           3 * Math.Pow(1 - t, 2) * t * control1.X +
                           3 * (1 - t) * Math.Pow(t, 2) * control2.X +
                           Math.Pow(t, 3) * end.X;

                double y = Math.Pow(1 - t, 3) * start.Y +
                           3 * Math.Pow(1 - t, 2) * t * control1.Y +
                           3 * (1 - t) * Math.Pow(t, 2) * control2.Y +
                           Math.Pow(t, 3) * end.Y;

                double distance = Math.Sqrt(Math.Pow(x - location.X, 2) + Math.Pow(y - location.Y, 2));

                if (distance <= tolerance)
                    return true;
            }
            return false;
        }

        private double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                isDraggingVertex = false;
            }
        }
        private bool IsPointInPolygon(Point point, List<Edge> polygonEdges)
        {
            int crossings = 0;

            foreach (Edge edge in polygonEdges)
            {
                Point start = edge.Start;
                Point end = edge.End;

                if (((start.Y > point.Y) != (end.Y > point.Y)) &&
                     (point.X < (end.X - start.X) * (point.Y - start.Y) / (end.Y - start.Y) + start.X))
                {
                    crossings++;
                }
            }

            return (crossings % 2 == 1);
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clickLocation = new Point(e.X, e.Y);



                draggedVertexIndex = FindNearestVertex(clickLocation);
                draggedCP1 = draggedCP2 = -1;
                if (draggedVertexIndex == -1)
                {
                    for (int i = 0; i < edges.Count; i++)
                    {
                        if (edges[i] is BezierEdge be)
                        {
                            if (Distance(clickLocation, be.ControlPoint1) < 10)
                            {
                                draggedCP1 = i;
                                break;
                            }
                            if (Distance(clickLocation, be.ControlPoint2) < 10)
                            {
                                draggedCP2 = i;
                                break;
                            }
                        }
                    }
                }
                if (draggedVertexIndex != -1 || draggedCP1 != -1 || draggedCP2 != -1)
                {
                    isDraggingVertex = true;
                }
                else if (IsPointInPolygon(clickLocation, edges))
                {
                    isDragging = true;
                    dragStartPoint = clickLocation;
                }
            }
        }
        private void DeleteConstraint_Click(object sender, EventArgs e)
        {
            edges[clickedEdgeIndex] = new NoConstraintEdge(edges[clickedEdgeIndex].Start, edges[clickedEdgeIndex].End);
            UpdateContinuity();
            Canvas.Invalidate();
        }
        private void AddHorizontalConstraint_Click(object sender, EventArgs e)
        {
            if (GetPreviousEdge(edges[clickedEdgeIndex]) is HorizontalEdge || GetNextEdge(edges[clickedEdgeIndex]) is HorizontalEdge)
            {
                MessageBox.Show("Nie mo¿na dodaæ dwóch ograniczeñ poziomych obok siebie");
                return;
            }

            var originalEdges = edges.Select(edge => edge.Clone()).ToList();

            var currentEdge = edges[clickedEdgeIndex];
            var delta = new Point(0, currentEdge.Start.Y - currentEdge.End.Y);

            edges[clickedEdgeIndex] = new HorizontalEdge(edges[clickedEdgeIndex].Start, edges[clickedEdgeIndex].End);
            if (edges[(clickedEdgeIndex + edges.Count - 1) % edges.Count] is BezierEdge be)
            {
                be.AcceptHorizontal(new EdgeEndChangeVisitor(), (HorizontalEdge)edges[clickedEdgeIndex], new Point());
            }
            bool modificationSuccess = false;
            int currentIndex = clickedEdgeIndex;


            while (true)
            {
                var nextEdge = GetNextEdge(edges[currentIndex]);
                if (nextEdge == null) break;

                modificationSuccess = edges[currentIndex].Accept(new EdgeStartChangeVisitor(), nextEdge, delta);

                if (modificationSuccess == false)
                    break;
                else if (modificationSuccess && currentIndex == clickedEdgeIndex)
                {
                    MessageBox.Show("Nie mo¿na wprowadziæ ograniczenia. Zmiana cofniêta.");
                    edges = originalEdges; 
                    Canvas.Invalidate();  
                    return;
                }

                currentIndex = (currentIndex + 1) % edges.Count;

                if (currentIndex == clickedEdgeIndex)
                    break;
            }
            UpdateContinuity();
            Canvas.Invalidate();

        }
        private void AddVerticalConstraint_Click(object sender, EventArgs e)
        {
            if (GetPreviousEdge(edges[clickedEdgeIndex]) is VerticalEdge || GetNextEdge(edges[clickedEdgeIndex]) is VerticalEdge)
            {
                MessageBox.Show("Nie mo¿na dodaæ dwóch ograniczeñ pionowych obok siebie");
                return;
            }

            var originalEdges = edges.Select(edge => edge.Clone()).ToList();

            var currentEdge = edges[clickedEdgeIndex];
            var delta = new Point(currentEdge.Start.X - currentEdge.End.X, 0);

            edges[clickedEdgeIndex] = new VerticalEdge(edges[clickedEdgeIndex].Start, edges[clickedEdgeIndex].End);
            if (edges[(clickedEdgeIndex + edges.Count - 1) % edges.Count] is BezierEdge be)
            {
                be.AcceptVertical(new EdgeEndChangeVisitor(), (VerticalEdge)edges[clickedEdgeIndex], new Point());
            }

            bool modificationSuccess = false;
            int currentIndex = clickedEdgeIndex;

            while (true)
            {
                var nextEdge = GetNextEdge(edges[currentIndex]);
                if (nextEdge == null) break;

                modificationSuccess = edges[currentIndex].Accept(new EdgeStartChangeVisitor(), nextEdge, delta);

  
                if (modificationSuccess == false)
                    break;
                else if (modificationSuccess && currentIndex == clickedEdgeIndex)
                {
                    MessageBox.Show("Nie mo¿na wprowadziæ ograniczenia. Zmiana cofniêta.");
                    edges = originalEdges;
                    Canvas.Invalidate(); 
                    return;
                }

                currentIndex = (currentIndex + 1) % edges.Count;

                if (currentIndex == clickedEdgeIndex)
                    break;
            }
            UpdateContinuity();
            Canvas.Invalidate();
        }
        private void AddFixedLengthConstraint_Click(object sender, EventArgs e)
        {

            var currentEdge = edges[clickedEdgeIndex];

            double currentLength = Math.Sqrt(Math.Pow(currentEdge.End.X - currentEdge.Start.X, 2) + Math.Pow(currentEdge.End.Y - currentEdge.Start.Y, 2));

            Form prompt = new Form();
            prompt.Width = 300; 
            prompt.Height = 150;
            prompt.Text = "Podaj d³ugoœæ krawêdzi";

            Label label = new Label() { Left = 10, Top = 15, Text = "D³ugoœæ:", AutoSize = true };
            TextBox inputBox = new TextBox() { Left = 100, Top = 10, Width = 140, Text = currentLength.ToString("F1") };
            Button confirmation = new Button() { Text = "OK", Left = 180, Width = 80, Height=30,Top = 50 };

            prompt.Controls.Add(label);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);

            confirmation.Click += (s, ev) =>
            {
                prompt.DialogResult = DialogResult.OK;
                prompt.Close();
            };

            inputBox.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter)
                {
                    prompt.DialogResult = DialogResult.OK;
                    prompt.Close();
                }
            };

            if (prompt.ShowDialog() != DialogResult.OK)
                return;

            if (!double.TryParse(inputBox.Text, out double newLength) || newLength < 0)
            {
                MessageBox.Show("Wprowadzono niepoprawn¹ wartoœæ d³ugoœci.");
                return;
            }

            if (Math.Abs(newLength - currentLength) < 0.2)
            {
                edges[clickedEdgeIndex] = new FixedLengthEdge(currentEdge.Start, currentEdge.End, newLength);
                UpdateContinuity();
                Canvas.Invalidate();
                return;
            }

            var originalEdges = edges.Select(edge => edge.Clone()).ToList();

            double angle = Math.Atan2(currentEdge.End.Y - currentEdge.Start.Y, currentEdge.End.X - currentEdge.Start.X);

            var newEnd = new Point(
                currentEdge.Start.X + (int)(newLength * Math.Cos(angle)),
                currentEdge.Start.Y + (int)(newLength * Math.Sin(angle))
            );

            Point delta = new Point(newEnd.X - currentEdge.End.X, newEnd.Y - currentEdge.End.Y);
            edges[clickedEdgeIndex] = new FixedLengthEdge(currentEdge.Start, currentEdge.End, newLength);
            if (edges[(clickedEdgeIndex + edges.Count - 1) % edges.Count] is BezierEdge be)
            {
                be.AcceptFixedLength(new EdgeEndChangeVisitor(), (FixedLengthEdge)edges[clickedEdgeIndex], new Point());
            }

            bool modificationSuccess = false;

            int currentIndex = clickedEdgeIndex;

            while (true)
            {
                var nextEdge = GetNextEdge(edges[currentIndex]);
                if (nextEdge == null) break;

                modificationSuccess = edges[currentIndex].Accept(new EdgeStartChangeVisitor(), nextEdge, delta);

 
                if (modificationSuccess == false)
                    break;
                else if (modificationSuccess && (currentIndex + 2) % edges.Count == clickedEdgeIndex)
                {
                    MessageBox.Show("Nie mo¿na wprowadziæ ograniczenia d³ugoœci. Zmiana cofniêta.");
                    edges = originalEdges;  
                    Canvas.Invalidate();    
                    return;
                }

                currentIndex = (currentIndex + 1) % edges.Count;

                if (currentIndex == clickedEdgeIndex)
                    break;
            }
            UpdateContinuity();

            Canvas.Invalidate();
        }
        public void UpdateContinuity()
        {
            for (int i = 0; i < edges.Count; i++)
            {
                if (edges[i] is BezierEdge be3)
                {
                    if (be3.StartContinuity == BezierEdge.ContinuityType.G1)
                        SetG1((i + edges.Count - 1) % edges.Count);
                    else if (be3.StartContinuity == BezierEdge.ContinuityType.C1)
                        SetC1((i + edges.Count - 1) % edges.Count);
                    if (be3.EndContinuity == BezierEdge.ContinuityType.G1)
                        SetG1(i);
                    else if (be3.EndContinuity == BezierEdge.ContinuityType.C1)
                        SetC1(i);
                }
            }
        }
        public void AddBezierEdgeConstraint_Click(object sender, EventArgs e)
        {
            edges[clickedEdgeIndex] = new BezierEdge(edges[clickedEdgeIndex].Start, edges[clickedEdgeIndex].End, edges[(clickedEdgeIndex + edges.Count - 1) % edges.Count], edges[(clickedEdgeIndex + 1) % edges.Count]);
            Canvas.Invalidate();
        }
        public void SetG0_Click(object sender, EventArgs e)
        {
            if (edges[clickedEdgeIndex] is BezierEdge be)
                be.EndContinuity = BezierEdge.ContinuityType.G0;
            if (edges[(clickedEdgeIndex + 1) % edges.Count] is BezierEdge be2)
                be2.StartContinuity = BezierEdge.ContinuityType.G0;
            Canvas.Invalidate();
        }
        public void SetG1_Click(object sender, EventArgs e)
        {
            SetG1(clickedEdgeIndex);
            Canvas.Invalidate();
        }
        private void SetG1(int idx)
        {
            if (edges[idx] is BezierEdge be && edges[(idx + 1) % edges.Count] is BezierEdge be2)
            {
                be.EndContinuity = be2.StartContinuity = BezierEdge.ContinuityType.G1;
                float originalDistance = (float)Math.Sqrt(
                    Math.Pow(be2.ControlPoint1.X - be2.Start.X, 2) +
                    Math.Pow(be2.ControlPoint1.Y - be2.Start.Y, 2));
                float edgeLength = (float)Math.Sqrt(
                   Math.Pow(be.End.X - be.ControlPoint2.X, 2) +
                   Math.Pow(be.End.Y - be.ControlPoint2.Y, 2));
                PointF direction = new PointF(
                   (be.End.X - be.ControlPoint2.X) / edgeLength,
                   (be.End.Y - be.ControlPoint2.Y) / edgeLength);
                be2.ControlPoint1 = new Point(
                   (int)Math.Round(be2.Start.X + direction.X * originalDistance),
                   (int)Math.Round(be2.Start.Y + direction.Y * originalDistance));
            }
            else if (edges[idx] is BezierEdge be3)
            {
                be3.EndContinuity = BezierEdge.ContinuityType.G1;
                Edge e1 = edges[(idx + 1) % edges.Count];
                float originalDistance = (float)Math.Sqrt(
                    Math.Pow(be3.ControlPoint2.X - be3.End.X, 2) +
                    Math.Pow(be3.ControlPoint2.Y - be3.End.Y, 2));
                float edgeLength = (float)Math.Sqrt(
                    Math.Pow(e1.End.X - e1.Start.X, 2) +
                    Math.Pow(e1.End.Y - e1.Start.Y, 2));
                PointF direction = new PointF(
                   (e1.End.X - e1.Start.X) / edgeLength,
                   (e1.End.Y - e1.Start.Y) / edgeLength);
                be3.ControlPoint2 = new Point(
                    (int)Math.Round(be3.End.X - direction.X * originalDistance),
                    (int)Math.Round(be3.End.Y - direction.Y * originalDistance));

            }
            else if (edges[(idx + 1) % edges.Count] is BezierEdge be4)
            {
                be4.StartContinuity = BezierEdge.ContinuityType.G1;
                Edge e1 = edges[idx];
                float originalDistance = (float)Math.Sqrt(
                    Math.Pow(be4.ControlPoint1.X - be4.Start.X, 2) +
                    Math.Pow(be4.ControlPoint1.Y - be4.Start.Y, 2));
                float edgeLength = (float)Math.Sqrt(
                    Math.Pow(e1.End.X - e1.Start.X, 2) +
                    Math.Pow(e1.End.Y - e1.Start.Y, 2));
                PointF direction = new PointF(
                   (e1.Start.X - e1.End.X) / edgeLength,
                   (e1.Start.Y - e1.End.Y) / edgeLength);
                be4.ControlPoint1 = new Point(
                    (int)Math.Round(be4.Start.X - direction.X * originalDistance),
                    (int)Math.Round(be4.Start.Y - direction.Y * originalDistance));

            }
        }
        public void SetC1_Click(object sender, EventArgs e)
        {

            SetC1(clickedEdgeIndex);
            Canvas.Invalidate();
        }
        private void SetC1(int idx)
        {
            if (edges[idx] is BezierEdge be && edges[(idx + 1) % edges.Count] is BezierEdge be2)
            {
                be.EndContinuity = be2.StartContinuity = BezierEdge.ContinuityType.C1;
                be2.ControlPoint1 = new Point(2 * be.End.X - be.ControlPoint2.X,
                    2 * be.End.Y - be.ControlPoint2.Y);
            }
            else if (edges[idx] is BezierEdge be3)
            {
                Edge e1 = edges[(idx + 1) % edges.Count];
                be3.EndContinuity = BezierEdge.ContinuityType.C1;
                be3.ControlPoint2 = new Point(e1.Start.X * 2 - e1.End.X,
                                                e1.Start.Y * 2 - e1.End.Y);
            }
            else if (edges[(idx + 1) % edges.Count] is BezierEdge be4)
            {
                Edge e1 = edges[idx];
                be4.StartContinuity = BezierEdge.ContinuityType.C1;
                be4.ControlPoint1 = new Point(2 * e1.End.X - e1.Start.X, 2 * e1.End.Y - e1.Start.Y);
            }
        }

        public Edge GetPreviousEdge(Edge currentEdge)
        {
            int index = edges.IndexOf(currentEdge);

            if (index == -1)
            {
                throw new ArgumentException("Edge not found in the list");
            }

            int previousIndex = (index - 1 + edges.Count) % edges.Count;

            return edges[previousIndex]; 
        }
        public Edge GetNextEdge(Edge currentEdge)
        {
            int index = edges.IndexOf(currentEdge);

            if (index == -1)
            {
                throw new ArgumentException("Edge not found in the list");
            }

            int nextIndex = (index + 1) % edges.Count;

            return edges[nextIndex]; 
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            edges.Clear();
            drawingComplete = false;
            currentMousePosition = null;
            Canvas.Invalidate();
        }

        private void normalDrawButton_Click(object sender, EventArgs e)
        {
            bresenhamButton.Checked = false;
            wuButton.Checked = false;
            wuClicked = false;
            Canvas.Invalidate();
        }

        private void bresenhamButton_Click(object sender, EventArgs e)
        {
            normalDrawButton.Checked = false;
            wuButton.Checked = false;
            wuClicked = false;
            Canvas.Invalidate();
        }
        private void wuButton_Click(object sender, EventArgs e)
        {
            bresenhamButton.Checked = false;
            normalDrawButton.Checked = false;
            wuClicked = true;
            Canvas.Invalidate();
        }

        private void controlsButton_Click(object sender, EventArgs e)
        {
            string controlsDescription =
                @"
Sterowanie programem:

-Zacznij rysowanie: Kliknij lewym przyciskiem na puste pole do rysowania.
-Zakoñcz rysowanie: Podczas rysowania kliknij na pierwszy narysowany wierzcho³ek.
-Przesuñ wierzcho³ek/punkt kontrolny: przytrzymaj lewy przycisk myszy i przesuñ kursor.
-Dodaj ograniczenie pionowe poziome: naciœnij prawym plawiszem myszy na krawêdŸ i kliknij w wybran¹ opcjê.
-Dodaj ograniczenie d³ugoœci naciœnij prawym przyciskiem myszy na krawêdŸ, wybierz opcjê ograniczenie d³ugoœci i w okienku które siê pojawi wybierz d³ugoœæ krawêdzi.
-Usuñ ograniczenie: naciœnij prawym plawiszem myszy na krawêdŸ i kliknij w opcjê usuñ ograniczenie
-Przesuñ ca³¹ figurê przytrzymaj lewy przycisk myszy wewn¹trz figury i przesuñ kursor.
-Dodaj wierzcho³ek: kliknij prawym przyciskiem myszy na krawêdŸ i wybierz opcjê dodaj wierzcho³ek.
-Ustaw krzyw¹ Beziera - kliknij prawym przyciskiem myszy i wybierz opcjê. Nowa krzywa bêdzie mia³¹ w punktach koñcowych ci¹g³oœæ C1
-Zmieñ ci¹g³oœæ punktu - kliknij prawym przyciskiem myszy w punkt granicz¹cy z krzyw¹ Beziera i wybierz ci¹g³oœæ.";

            MessageBox.Show(controlsDescription, "Opis sterowania", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void descriptionButton_Click(object sender, EventArgs e)
        {
            string programDescription =
                @"
Opis dzia³ania programu:

-Krawêdzie o poszczególnych ograniczeniach s¹ przechowywane na liœcie edges jako obiekty klas dziedzicz¹cych po abstrakcyjnej klasie Edge.
-Rysowanie odbywa siê na podstawie informacji zawartych w elementach listy edges i jest za nie odpowiedzialna klasa EdgeDrawingVisitor.
-Krótki opis dzia³ania algorytmu relacji:
    1. Zmieniamy pozycjê chwyconego wierzcho³ka zgodnie z pozycj¹ myszy
    2. Idziemy w pêtli w obie strony listy sprawdzaj¹c obecn¹ i nastêpn¹ krawêdŸ
    3. Do rozpatrzenia poszczególnych przypadków zosta³ wykorzystany Interfejs IEdgeDrawingVisitor i dwie klasy które po nim dziedzicz¹.
    4. Metoda aktualizuj¹ca pozycjê krawêdzi zak³ada ¿e zosta³ przesuniêty pierwszy punkt pierwszej krawêdzi. Stara siê dobraæ pozycjê punktu ³¹cz¹cego sprawdzane krawêdzie w taki sposób ¿eby nie poruszyæ drugiego punktu drugiej krawêdzi.
    5. Jeœli to siê uda zwraca informacjê o tym ¿e nie trzeba dalej przesuwaæ (w postaci zmiennej bool), jeœli nie przesuwa punkt ³¹cz¹cy o tyle samo i ile przesun¹³ siê punkt startowy i przesuwa indeks o jeden dalej oraz zwraca odpowiedni¹ informacjê.
    6. Jeœli zabraknie dla indeksów miejsca ca³a figura jest przesuwana za kursorem bez zmian w po³o¿eniu krawêdzi wzglêdem siebie.
-Dla krzywych beziera sprawdzanie dzia³a podobnie. Sprawdzana jest dodatkowo ci¹g³oœæ w punktach"
;
            MessageBox.Show(programDescription, "Opis zastosowanych rozwi¹zañ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void InitEdges()
        {
            edges = new List<Edge>
    {
        new BezierEdge
        {
            Start = new Point(289, 112),
            ControlPoint1 = new Point(410, 78),
            ControlPoint2 = new Point(361, 171),
            End = new Point(455, 155),
            StartContinuity = BezierEdge.ContinuityType.C1,
            EndContinuity = BezierEdge.ContinuityType.G1
        },

        new BezierEdge
        {
            Start = new Point(455,155),
            ControlPoint1 = new Point(649, 122),
            ControlPoint2 = new Point(439, 317),
            End = new Point(703, 372),
            StartContinuity = BezierEdge.ContinuityType.G1,
            EndContinuity = BezierEdge.ContinuityType.G0
        },

        new HorizontalEdge
        {
            Start = new Point(703, 372),
            End = new Point(407, 372)
        },

        new FixedLengthEdge
        {
            Start = new Point(407, 372),
            End = new Point(182, 352),
            Length = 225
        },

        new FixedLengthEdge
        {
            Start = new Point(182, 352),
            End = new Point(168, 244),
            Length = 110
        },

        new VerticalEdge
        {
            Start = new Point(168, 244),
            End = new Point(168, 146)
        },

        new NoConstraintEdge
        {
            Start = new Point(168, 146),
            End = new Point(289, 112)
        }
    };
        }

        private void initialFigureButton_Click(object sender, EventArgs e)
        {
            InitEdges();
            drawingComplete = true;
            Canvas.Invalidate();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            string jsonString = JsonSerializer.Serialize(edges, options);

            File.WriteAllText("edges.json", jsonString);
            MessageBox.Show("Pomyœlnie zapisano figurê");
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            string jsonString = File.ReadAllText("edges.json");
            edges.Clear();
            edges = JsonSerializer.Deserialize<List<Edge>>(jsonString);
            Canvas.Invalidate();
            drawingComplete = true;
        }
    }
}
