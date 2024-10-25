using System.Windows.Forms.VisualStyles;

namespace projekt_1
{
    public partial class Form1 : Form
    {
        private List<Edge> edges = new List<Edge>();
        private bool drawingComplete = false;
        private Point? currentMousePosition = null;
        private ContextMenuStrip contextMenu = new ContextMenuStrip();
        private ContextMenuStrip contextMenuEdge = new ContextMenuStrip();
        private int clickedEdgeIndex = -1;
        private bool isDragging = false;
        private bool isDraggingVertex = false;
        private int draggedVertexIndex = -1;
        private Point dragStartPoint;
        private Point polygonOffset = Point.Empty;
        public Form1()
        {
            InitializeComponent();

            contextMenu = new ContextMenuStrip();
            contextMenuEdge = new ContextMenuStrip();
            var deleteItem = new ToolStripMenuItem("Usu� wierzcho�ek");
            contextMenuEdge.Items.Add("Dodaj wierzcho�ek", null, AddVertexMenuItem_Click);
            contextMenuEdge.Items.Add("Dodaj ograniczenie poziome", null, AddHorizontalConstraint_Click);
            contextMenuEdge.Items.Add("Dodaj ograniczenie pionowe", null, AddVerticalConstraint_Click);
            contextMenuEdge.Items.Add("Dodaj ograniczenie d�ugo�ci", null, AddFixedLengthConstraint_Click);
            contextMenuEdge.Items.Add("Usu� ograniczenie", null, DeleteConstraint_Click);
            deleteItem.Click += DeleteVertex_Click;
            contextMenu.Items.Add(deleteItem);
        }
        private void clearButton_Click(object sender, EventArgs e)
        {
            edges.Clear();
            drawingComplete = false;
            currentMousePosition = null; // Resetowanie pozycji myszy
            Canvas.Invalidate(); // Wyczyszczenie ekranu
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen edgePen = new Pen(Color.Black, 1); // Kraw�dzie o grubo�ci 1
            Pen vertexPen = new Pen(Color.Black, 2); // Wierzcho�ki b�d� grubsze
            Brush fillBrush = new SolidBrush(Color.LightBlue); // P�dzel do wype�nienia jasnoniebieskim kolorem
            EdgeDrawingVisitor drawingVisitor = new EdgeDrawingVisitor();

            // Rysowanie wszystkich kraw�dzi
            foreach (var edge in edges)
            {
                edge.AcceptDraw(drawingVisitor, g, edgePen);
            }

            // Rysowanie wierzcho�k�w jako ma�e okr�gi
            foreach (var edge in edges)
            {
                g.FillEllipse(Brushes.Black, edge.Start.X - 5, edge.Start.Y - 5, 10, 10); // Okr�g o �rednicy 6 pikseli
            }
            if (edges.Count > 0)
            {
                g.FillEllipse(Brushes.Black, edges.Last().End.X - 5, edges.Last().End.Y - 5, 10, 10); // Ostatni wierzcho�ek
            }

            // Rysowanie p�przezroczystej linii pod��aj�cej za kursorem, je�li rysowanie jeszcze trwa
            if (!drawingComplete && edges.Count > 0)
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    g.DrawLine(edgePen, edges[i].Start, edges[i].End);
                }

                if (currentMousePosition != null)
                {
                    Pen semiTransparentPen = new Pen(Color.FromArgb(128, Color.Black), 1); // P�przezroczysta kraw�d�
                    g.DrawLine(semiTransparentPen, edges.Last().End, currentMousePosition.Value);
                }
            }
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (drawingComplete)
            {
                // Obs�uga klikni�cia prawym przyciskiem myszy
                if (e.Button == MouseButtons.Right)
                {
                    Point clickLocation = new Point(e.X, e.Y);
                    clickedEdgeIndex = FindNearestVertex(clickLocation); // Znajd� indeks najbli�szej kraw�dzi

                    if (clickedEdgeIndex != -1)
                    {
                        contextMenu.Show(Canvas, e.Location); // Wy�wietl menu kontekstowe
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

            // Obs�uguje lewy przycisk myszy
            Point clickLocationLeft = new Point(e.X, e.Y);

            if (edges.Count > 0 && IsFirstEdgeClicked(clickLocationLeft))
            {
                // Je�li klikni�to blisko pierwszej kraw�dzi, zamykamy wielok�t
                drawingComplete = true;
                edges.Add(new NoConstraintEdge(edges.Last().End, edges[0].Start)); // Dodaj ostatni� kraw�d�
                edges.RemoveAt(0);
                Canvas.Invalidate(); // Od�wie� PictureBox
            }
            else
            {
                // Dodaj now� kraw�d�
                if (edges.Count > 0)
                {
                    edges.Add(new NoConstraintEdge(edges.Last().End, clickLocationLeft));
                }
                else
                {
                    // Dodaj pierwsz� kraw�d�
                    edges.Add(new NoConstraintEdge(clickLocationLeft, clickLocationLeft));
                }

                Canvas.Invalidate(); // Od�wie� PictureBox
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
            edgeCopy[forwardIndex].Start = edgeCopy[backwardIndex].End = new Point(edges[forwardIndex].Start.X + delta.X, edges[forwardIndex].Start.Y + delta.Y);


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

            if (modifiedEdgesCount > edges.Count)
            {
                foreach (var edge in edges)
                {
                    edge.Start = new Point(edge.Start.X + delta.X, edge.Start.Y + delta.Y);
                    edge.End = new Point(edge.End.X + delta.X, edge.End.Y + delta.Y);
                }
            }
            else
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    edges[i].Start = edgeCopy[i].Start;
                    edges[i].End = edgeCopy[i].End;
                }
            }

        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouseLocation = new Point(e.X, e.Y);

            if (!(drawingComplete || edges.Count == 0))
            {
                currentMousePosition = new Point(e.X, e.Y); // Aktualizujemy bie��c� pozycj� myszy
                Canvas.Invalidate(); // Od�wie� PictureBox, aby narysowa� p�przezroczyst� lini�
                return;
            }
            // Je�li rysowanie jest zako�czone, sprawd�, czy myszka jest wewn�trz wielok�ta
            if (isDraggingVertex)
            {
                Point currentMouseLocation = new Point(e.X, e.Y);
                Point delta = new Point(currentMouseLocation.X - edges[draggedVertexIndex].End.X,
                                currentMouseLocation.Y - edges[draggedVertexIndex].End.Y);

                UpdateEdgesWithVisitor(draggedVertexIndex, delta);
                Canvas.Invalidate();

            }
            else if (isDragging)
            {
                Point currentMouseLocation = new Point(e.X, e.Y);
                int dx = currentMouseLocation.X - dragStartPoint.X;
                int dy = currentMouseLocation.Y - dragStartPoint.Y;

                // Przesu� ka�dy wierzcho�ek o to przesuni�cie
                for (int i = 0; i < edges.Count; i++)
                {
                    edges[i].Start = new Point(edges[i].Start.X + dx, edges[i].Start.Y + dy);
                    edges[i].End = new Point(edges[i].End.X + dx, edges[i].End.Y + dy);
                }

                // Zaktualizuj punkt pocz�tkowy na aktualn� pozycj�
                dragStartPoint = currentMouseLocation;

                // Od�wie� rysunek
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

            Canvas.Invalidate();
        }
        private void DeleteVertex_Click(object sender, EventArgs e)
        {
            if (clickedEdgeIndex != -1)
            {
                // Sprawd�, czy jest wystarczaj�ca ilo�� kraw�dzi do usuni�cia
                if (edges.Count > 3)
                {
                    int edgeToRemoveIndex = clickedEdgeIndex; // Kraw�d� do usuni�cia
                    int nextEdgeIndex = (clickedEdgeIndex + 1) % edges.Count; // Kraw�d� nast�pna do usuni�cia

                    // Przechowaj pocz�tki i ko�ce kraw�dzi
                    Point newStart = edges[edgeToRemoveIndex].Start; // Pocz�tek pierwszej kraw�dzi
                    Point newEnd = edges[nextEdgeIndex].End; // Koniec drugiej kraw�dzi
                    // Usu� kraw�dzie
                    if (nextEdgeIndex == 0)
                    {
                        edges.RemoveAt(edgeToRemoveIndex);
                        edges.RemoveAt(nextEdgeIndex);
                        edges.Insert(0, new NoConstraintEdge(newStart, newEnd));
                    }
                    else
                    {
                        edges.RemoveAt(nextEdgeIndex); // Usuni�cie nast�pnej kraw�dzi

                        // Usuni�cie kraw�dzi, kt�ra by�a wybrana
                        edges.RemoveAt(edgeToRemoveIndex); // Usuni�cie kraw�dzi wskazanej przez u�ytkownika
                        edges.Insert(edgeToRemoveIndex, new NoConstraintEdge(newStart, newEnd));
                    }

                    // Wstaw now� kraw�d� w miejsce usuni�tej pierwszej kraw�dzi

                }
                else if (edges.Count == 3)
                {
                    // Je�li s� tylko 3 kraw�dzie, usuwamy ca�� figur�
                    edges.Clear();
                    drawingComplete = false;
                }

                Canvas.Invalidate(); // Od�wie� PictureBox
            }
        }
        private int FindNearestVertex(Point location)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                // Sprawd�, czy klikni�to blisko wierzcho�ka kraw�dzi
                if (Distance(edges[i].End, location) < 10)
                {
                    return i; // Zwr�� indeks kraw�dzi
                }
            }
            return -1; // Brak kraw�dzi w pobli�u
        }
        private int FindNearestEdge(Point location)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                if (IsPointNearLine(edges[i].Start, edges[i].End, location, 5)) // 5 pikseli tolerancji
                {
                    // Wy�wietl menu kontekstowe
                    contextMenu.Tag = i; // Zapisz indeks kraw�dzi
                    return i;
                }
            }
            return -1;
        }
        private bool IsFirstEdgeClicked(Point currentPoint)
        {
            if (edges.Count == 0)
                return false;

            // Sprawdza, czy klikni�to blisko pierwszej kraw�dzi
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
        private double Distance(Point p1, Point p2)
        {
            // Obliczanie odleg�o�ci mi�dzy dwoma punktami
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Zako�cz przesuwanie
                isDragging = false;
                isDraggingVertex = false;
            }
        }
        private bool IsPointInPolygon(Point point, List<Edge> polygonEdges)
        {
            int crossings = 0;

            // Iterujemy przez wszystkie kraw�dzie wielok�ta
            foreach (Edge edge in polygonEdges)
            {
                Point start = edge.Start;
                Point end = edge.End;

                // Sprawdzamy, czy kraw�d� przecina promie� wychodz�cy z punktu na prawo
                if (((start.Y > point.Y) != (end.Y > point.Y)) &&
                     (point.X < (end.X - start.X) * (point.Y - start.Y) / (end.Y - start.Y) + start.X))
                {
                    crossings++;
                }
            }

            // Je�li liczba przeci�� jest nieparzysta, punkt jest wewn�trz wielok�ta
            return (crossings % 2 == 1);
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point clickLocation = new Point(e.X, e.Y);


                draggedVertexIndex = FindNearestVertex(clickLocation);
                if (draggedVertexIndex != -1)
                {
                    isDraggingVertex = true;
                }
                // Sprawd�, czy klikni�to wewn�trz wielok�ta
                else if (IsPointInPolygon(clickLocation, edges))
                {
                    // Rozpocznij przesuwanie
                    isDragging = true;
                    dragStartPoint = clickLocation;
                }
            }
        }
        private void DeleteConstraint_Click(object sender, EventArgs e)
        {
            edges[clickedEdgeIndex] = new NoConstraintEdge(edges[clickedEdgeIndex].Start, edges[clickedEdgeIndex].End);
            Canvas.Invalidate();
        }
        private void AddHorizontalConstraint_Click(object sender, EventArgs e)
        {
            if (GetPreviousEdge(edges[clickedEdgeIndex]) is HorizontalEdge || GetNextEdge(edges[clickedEdgeIndex]) is HorizontalEdge)
            {
                MessageBox.Show("Nie mo�na doda� dw�ch ogranicze� poziomych obok siebie");
                return;
            }

            var originalEdges = edges.Select(edge => edge.Clone()).ToList();

            var currentEdge = edges[clickedEdgeIndex];
            var delta = new Point(0, currentEdge.Start.Y - currentEdge.End.Y);

            edges[clickedEdgeIndex] = new HorizontalEdge(edges[clickedEdgeIndex].Start, edges[clickedEdgeIndex].End);

            bool modificationSuccess = false;
            int currentIndex = clickedEdgeIndex;


            while (true)
            {
                var nextEdge = GetNextEdge(edges[currentIndex]);
                if (nextEdge == null) break;

                // Wywo�ujemy visitor, aby przetworzy� kolejn� kraw�d� w stosunku do obecnej
                modificationSuccess = edges[currentIndex].Accept(new EdgeStartChangeVisitor(), nextEdge, delta);

                // Je�li modyfikacja zatoczy ko�o i wracamy do pocz�tku, oznacza to, �e nie mo�na wprowadzi� ograniczenia
                if (modificationSuccess == false)
                    break;
                else if (modificationSuccess && currentIndex == clickedEdgeIndex)
                {
                    MessageBox.Show("Nie mo�na wprowadzi� ograniczenia. Zmiana cofni�ta.");
                    edges = originalEdges; // Przywracamy oryginalne kraw�dzie
                    Canvas.Invalidate();   // Od�wie�enie
                    return;
                }

                // Aktualizujemy indeks do kolejnej kraw�dzi
                currentIndex = (currentIndex + 1) % edges.Count;

                // Je�li po przej�ciu przez wszystkie kraw�dzie, zatoczymy ko�o
                if (currentIndex == clickedEdgeIndex)
                    break;
            }

            Canvas.Invalidate();

        }
        private void AddVerticalConstraint_Click(object sender, EventArgs e)
        {
            // Sprawdzanie, czy istnieje ograniczenie pionowe w s�siednich kraw�dziach
            if (GetPreviousEdge(edges[clickedEdgeIndex]) is VerticalEdge || GetNextEdge(edges[clickedEdgeIndex]) is VerticalEdge)
            {
                MessageBox.Show("Nie mo�na doda� dw�ch ogranicze� pionowych obok siebie");
                return;
            }

            // Tworzenie g��bokiej kopii listy kraw�dzi przy u�yciu Clone
            var originalEdges = edges.Select(edge => edge.Clone()).ToList();

            var currentEdge = edges[clickedEdgeIndex];
            var delta = new Point(currentEdge.Start.X - currentEdge.End.X, 0);

            // Zmiana bie��cej kraw�dzi na pionow�
            edges[clickedEdgeIndex] = new VerticalEdge(edges[clickedEdgeIndex].Start, edges[clickedEdgeIndex].End);

            bool modificationSuccess = false;
            int currentIndex = clickedEdgeIndex;

            while (true)
            {
                var nextEdge = GetNextEdge(edges[currentIndex]);
                if (nextEdge == null) break;

                // Wywo�ujemy visitor, aby przetworzy� kolejn� kraw�d� w stosunku do obecnej
                modificationSuccess = edges[currentIndex].Accept(new EdgeStartChangeVisitor(), nextEdge, delta);

                // Je�li modyfikacja zatoczy ko�o i wracamy do pocz�tku, oznacza to, �e nie mo�na wprowadzi� ograniczenia
                if (modificationSuccess == false)
                    break;
                else if (modificationSuccess && currentIndex == clickedEdgeIndex)
                {
                    MessageBox.Show("Nie mo�na wprowadzi� ograniczenia. Zmiana cofni�ta.");
                    edges = originalEdges; // Przywracamy oryginalne kraw�dzie
                    Canvas.Invalidate();   // Od�wie�enie
                    return;
                }

                // Aktualizujemy indeks do kolejnej kraw�dzi
                currentIndex = (currentIndex + 1) % edges.Count;

                // Je�li po przej�ciu przez wszystkie kraw�dzie, zatoczymy ko�o
                if (currentIndex == clickedEdgeIndex)
                    break;
            }

            Canvas.Invalidate();
        }
        private void AddFixedLengthConstraint_Click(object sender, EventArgs e)
        {

            var currentEdge = edges[clickedEdgeIndex];

            // Pobierz obecn� d�ugo�� kraw�dzi
            double currentLength = Math.Sqrt(Math.Pow(currentEdge.End.X - currentEdge.Start.X, 2) + Math.Pow(currentEdge.End.Y - currentEdge.Start.Y, 2));

            // Wy�wietl okno dialogowe z polem tekstowym
            Form prompt = new Form();
            prompt.Width = 300;  // Zwi�kszona szeroko�� dla lepszego rozmieszczenia element�w
            prompt.Height = 150;
            prompt.Text = "Podaj d�ugo�� kraw�dzi";

            // Ustawienia napisu i pola tekstowego z odpowiednimi marginesami
            Label label = new Label() { Left = 10, Top = 15, Text = "D�ugo��:", AutoSize = true };
            TextBox inputBox = new TextBox() { Left = 80, Top = 10, Width = 180, Text = currentLength.ToString("F1") };
            Button confirmation = new Button() { Text = "OK", Left = 180, Width = 80, Top = 50 };

            prompt.Controls.Add(label);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);

            confirmation.Click += (s, ev) => { prompt.Close(); };
            inputBox.KeyDown += (s, ev) => { if (ev.KeyCode == Keys.Enter) prompt.Close(); };

            prompt.ShowDialog();

            // Sprawdzenie, czy wpisano poprawn� warto��
            if (!double.TryParse(inputBox.Text, out double newLength) || newLength < 0)
            {
                MessageBox.Show("Wprowadzono niepoprawn� warto�� d�ugo�ci.");
                return;
            }

            // Je�li d�ugo�� si� nie zmieni�a, zmie� kraw�d� na FixedLengthEdge
            if (Math.Abs(newLength - currentLength) < 0.2)
            {
                edges[clickedEdgeIndex] = new FixedLengthEdge(currentEdge.Start, currentEdge.End, newLength);
                Canvas.Invalidate();
                return;
            }

            // Zapisz pierwotny stan kraw�dzi
            var originalEdges = edges.Select(edge => edge.Clone()).ToList();

            // Oblicz now� pozycj� punktu ko�cowego
            double angle = Math.Atan2(currentEdge.End.Y - currentEdge.Start.Y, currentEdge.End.X - currentEdge.Start.X);

            var newEnd = new Point(
                currentEdge.Start.X + (int)(newLength * Math.Cos(angle)),
                currentEdge.Start.Y + (int)(newLength * Math.Sin(angle))
            );

            // Ustaw now� kraw�d� FixedLengthEdge
            Point delta = new Point(newEnd.X - currentEdge.End.X, newEnd.Y - currentEdge.End.Y);
            edges[clickedEdgeIndex] = new FixedLengthEdge(currentEdge.Start, currentEdge.End, newLength);

            // Sprawdzanie dopasowania pozosta�ych kraw�dzi
            bool modificationSuccess = false;

            int currentIndex = clickedEdgeIndex;

            while (true)
            {
                var nextEdge = GetNextEdge(edges[currentIndex]);
                if (nextEdge == null) break;

                modificationSuccess = edges[currentIndex].Accept(new EdgeStartChangeVisitor(), nextEdge, delta);

                // Je�li modyfikacja zatoczy ko�o lub nie mo�na dopasowa� kraw�dzi
                if (modificationSuccess == false)
                    break;
                else if (modificationSuccess && (currentIndex + 2) % edges.Count == clickedEdgeIndex)
                {
                    MessageBox.Show("Nie mo�na wprowadzi� ograniczenia d�ugo�ci. Zmiana cofni�ta.");
                    edges = originalEdges;  // Przywr�� pierwotne kraw�dzie
                    Canvas.Invalidate();    // Od�wie�anie
                    return;
                }


                // Przej�cie do kolejnej kraw�dzi
                currentIndex = (currentIndex + 1) % edges.Count;

                if (currentIndex == clickedEdgeIndex)
                    break;
            }

            Canvas.Invalidate();
            ;
        }

        public Edge GetPreviousEdge(Edge currentEdge)
        {
            int index = edges.IndexOf(currentEdge);

            if (index == -1)
            {
                throw new ArgumentException("Edge not found in the list");
            }

            // Znajd� poprzedni element (z uwzgl�dnieniem, �e lista mo�e by� cykliczna)
            int previousIndex = (index - 1 + edges.Count) % edges.Count;

            return edges[previousIndex]; // Zwraca referencj� do poprzedniego elementu
        }
        public Edge GetNextEdge(Edge currentEdge)
        {
            int index = edges.IndexOf(currentEdge);

            if (index == -1)
            {
                throw new ArgumentException("Edge not found in the list");
            }

            // Znajd� nast�pny element (z uwzgl�dnieniem cykliczno�ci listy)
            int nextIndex = (index + 1) % edges.Count;

            return edges[nextIndex]; // Zwraca referencj� do nast�pnego elementu
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
        }

        private void bresenhamButton_Click(object sender, EventArgs e)
        {
            normalDrawButton.Checked = false;

        }
    }
}
