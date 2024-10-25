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
            var deleteItem = new ToolStripMenuItem("Usuñ wierzcho³ek");
            contextMenuEdge.Items.Add("Dodaj wierzcho³ek", null, AddVertexMenuItem_Click);
            contextMenuEdge.Items.Add("Dodaj ograniczenie poziome", null, AddHorizontalConstraint_Click);
            contextMenuEdge.Items.Add("Dodaj ograniczenie pionowe", null, AddVerticalConstraint_Click);
            contextMenuEdge.Items.Add("Dodaj ograniczenie d³ugoœci", null, AddFixedLengthConstraint_Click);
            contextMenuEdge.Items.Add("Usuñ ograniczenie", null, DeleteConstraint_Click);
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
            Pen edgePen = new Pen(Color.Black, 1); // Krawêdzie o gruboœci 1
            Pen vertexPen = new Pen(Color.Black, 2); // Wierzcho³ki bêd¹ grubsze
            Brush fillBrush = new SolidBrush(Color.LightBlue); // Pêdzel do wype³nienia jasnoniebieskim kolorem
            EdgeDrawingVisitor drawingVisitor = new EdgeDrawingVisitor();

            // Rysowanie wszystkich krawêdzi
            foreach (var edge in edges)
            {
                edge.AcceptDraw(drawingVisitor, g, edgePen);
            }

            // Rysowanie wierzcho³ków jako ma³e okrêgi
            foreach (var edge in edges)
            {
                g.FillEllipse(Brushes.Black, edge.Start.X - 5, edge.Start.Y - 5, 10, 10); // Okr¹g o œrednicy 6 pikseli
            }
            if (edges.Count > 0)
            {
                g.FillEllipse(Brushes.Black, edges.Last().End.X - 5, edges.Last().End.Y - 5, 10, 10); // Ostatni wierzcho³ek
            }

            // Rysowanie pó³przezroczystej linii pod¹¿aj¹cej za kursorem, jeœli rysowanie jeszcze trwa
            if (!drawingComplete && edges.Count > 0)
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    g.DrawLine(edgePen, edges[i].Start, edges[i].End);
                }

                if (currentMousePosition != null)
                {
                    Pen semiTransparentPen = new Pen(Color.FromArgb(128, Color.Black), 1); // Pó³przezroczysta krawêdŸ
                    g.DrawLine(semiTransparentPen, edges.Last().End, currentMousePosition.Value);
                }
            }
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (drawingComplete)
            {
                // Obs³uga klikniêcia prawym przyciskiem myszy
                if (e.Button == MouseButtons.Right)
                {
                    Point clickLocation = new Point(e.X, e.Y);
                    clickedEdgeIndex = FindNearestVertex(clickLocation); // ZnajdŸ indeks najbli¿szej krawêdzi

                    if (clickedEdgeIndex != -1)
                    {
                        contextMenu.Show(Canvas, e.Location); // Wyœwietl menu kontekstowe
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

            // Obs³uguje lewy przycisk myszy
            Point clickLocationLeft = new Point(e.X, e.Y);

            if (edges.Count > 0 && IsFirstEdgeClicked(clickLocationLeft))
            {
                // Jeœli klikniêto blisko pierwszej krawêdzi, zamykamy wielok¹t
                drawingComplete = true;
                edges.Add(new NoConstraintEdge(edges.Last().End, edges[0].Start)); // Dodaj ostatni¹ krawêdŸ
                edges.RemoveAt(0);
                Canvas.Invalidate(); // Odœwie¿ PictureBox
            }
            else
            {
                // Dodaj now¹ krawêdŸ
                if (edges.Count > 0)
                {
                    edges.Add(new NoConstraintEdge(edges.Last().End, clickLocationLeft));
                }
                else
                {
                    // Dodaj pierwsz¹ krawêdŸ
                    edges.Add(new NoConstraintEdge(clickLocationLeft, clickLocationLeft));
                }

                Canvas.Invalidate(); // Odœwie¿ PictureBox
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
                currentMousePosition = new Point(e.X, e.Y); // Aktualizujemy bie¿¹c¹ pozycjê myszy
                Canvas.Invalidate(); // Odœwie¿ PictureBox, aby narysowaæ pó³przezroczyst¹ liniê
                return;
            }
            // Jeœli rysowanie jest zakoñczone, sprawdŸ, czy myszka jest wewn¹trz wielok¹ta
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

                // Przesuñ ka¿dy wierzcho³ek o to przesuniêcie
                for (int i = 0; i < edges.Count; i++)
                {
                    edges[i].Start = new Point(edges[i].Start.X + dx, edges[i].Start.Y + dy);
                    edges[i].End = new Point(edges[i].End.X + dx, edges[i].End.Y + dy);
                }

                // Zaktualizuj punkt pocz¹tkowy na aktualn¹ pozycjê
                dragStartPoint = currentMouseLocation;

                // Odœwie¿ rysunek
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
                // SprawdŸ, czy jest wystarczaj¹ca iloœæ krawêdzi do usuniêcia
                if (edges.Count > 3)
                {
                    int edgeToRemoveIndex = clickedEdgeIndex; // KrawêdŸ do usuniêcia
                    int nextEdgeIndex = (clickedEdgeIndex + 1) % edges.Count; // KrawêdŸ nastêpna do usuniêcia

                    // Przechowaj pocz¹tki i koñce krawêdzi
                    Point newStart = edges[edgeToRemoveIndex].Start; // Pocz¹tek pierwszej krawêdzi
                    Point newEnd = edges[nextEdgeIndex].End; // Koniec drugiej krawêdzi
                    // Usuñ krawêdzie
                    if (nextEdgeIndex == 0)
                    {
                        edges.RemoveAt(edgeToRemoveIndex);
                        edges.RemoveAt(nextEdgeIndex);
                        edges.Insert(0, new NoConstraintEdge(newStart, newEnd));
                    }
                    else
                    {
                        edges.RemoveAt(nextEdgeIndex); // Usuniêcie nastêpnej krawêdzi

                        // Usuniêcie krawêdzi, która by³a wybrana
                        edges.RemoveAt(edgeToRemoveIndex); // Usuniêcie krawêdzi wskazanej przez u¿ytkownika
                        edges.Insert(edgeToRemoveIndex, new NoConstraintEdge(newStart, newEnd));
                    }

                    // Wstaw now¹ krawêdŸ w miejsce usuniêtej pierwszej krawêdzi

                }
                else if (edges.Count == 3)
                {
                    // Jeœli s¹ tylko 3 krawêdzie, usuwamy ca³¹ figurê
                    edges.Clear();
                    drawingComplete = false;
                }

                Canvas.Invalidate(); // Odœwie¿ PictureBox
            }
        }
        private int FindNearestVertex(Point location)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                // SprawdŸ, czy klikniêto blisko wierzcho³ka krawêdzi
                if (Distance(edges[i].End, location) < 10)
                {
                    return i; // Zwróæ indeks krawêdzi
                }
            }
            return -1; // Brak krawêdzi w pobli¿u
        }
        private int FindNearestEdge(Point location)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                if (IsPointNearLine(edges[i].Start, edges[i].End, location, 5)) // 5 pikseli tolerancji
                {
                    // Wyœwietl menu kontekstowe
                    contextMenu.Tag = i; // Zapisz indeks krawêdzi
                    return i;
                }
            }
            return -1;
        }
        private bool IsFirstEdgeClicked(Point currentPoint)
        {
            if (edges.Count == 0)
                return false;

            // Sprawdza, czy klikniêto blisko pierwszej krawêdzi
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
            // Obliczanie odleg³oœci miêdzy dwoma punktami
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Zakoñcz przesuwanie
                isDragging = false;
                isDraggingVertex = false;
            }
        }
        private bool IsPointInPolygon(Point point, List<Edge> polygonEdges)
        {
            int crossings = 0;

            // Iterujemy przez wszystkie krawêdzie wielok¹ta
            foreach (Edge edge in polygonEdges)
            {
                Point start = edge.Start;
                Point end = edge.End;

                // Sprawdzamy, czy krawêdŸ przecina promieñ wychodz¹cy z punktu na prawo
                if (((start.Y > point.Y) != (end.Y > point.Y)) &&
                     (point.X < (end.X - start.X) * (point.Y - start.Y) / (end.Y - start.Y) + start.X))
                {
                    crossings++;
                }
            }

            // Jeœli liczba przeciêæ jest nieparzysta, punkt jest wewn¹trz wielok¹ta
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
                // SprawdŸ, czy klikniêto wewn¹trz wielok¹ta
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
                MessageBox.Show("Nie mo¿na dodaæ dwóch ograniczeñ poziomych obok siebie");
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

                // Wywo³ujemy visitor, aby przetworzyæ kolejn¹ krawêdŸ w stosunku do obecnej
                modificationSuccess = edges[currentIndex].Accept(new EdgeStartChangeVisitor(), nextEdge, delta);

                // Jeœli modyfikacja zatoczy ko³o i wracamy do pocz¹tku, oznacza to, ¿e nie mo¿na wprowadziæ ograniczenia
                if (modificationSuccess == false)
                    break;
                else if (modificationSuccess && currentIndex == clickedEdgeIndex)
                {
                    MessageBox.Show("Nie mo¿na wprowadziæ ograniczenia. Zmiana cofniêta.");
                    edges = originalEdges; // Przywracamy oryginalne krawêdzie
                    Canvas.Invalidate();   // Odœwie¿enie
                    return;
                }

                // Aktualizujemy indeks do kolejnej krawêdzi
                currentIndex = (currentIndex + 1) % edges.Count;

                // Jeœli po przejœciu przez wszystkie krawêdzie, zatoczymy ko³o
                if (currentIndex == clickedEdgeIndex)
                    break;
            }

            Canvas.Invalidate();

        }
        private void AddVerticalConstraint_Click(object sender, EventArgs e)
        {
            // Sprawdzanie, czy istnieje ograniczenie pionowe w s¹siednich krawêdziach
            if (GetPreviousEdge(edges[clickedEdgeIndex]) is VerticalEdge || GetNextEdge(edges[clickedEdgeIndex]) is VerticalEdge)
            {
                MessageBox.Show("Nie mo¿na dodaæ dwóch ograniczeñ pionowych obok siebie");
                return;
            }

            // Tworzenie g³êbokiej kopii listy krawêdzi przy u¿yciu Clone
            var originalEdges = edges.Select(edge => edge.Clone()).ToList();

            var currentEdge = edges[clickedEdgeIndex];
            var delta = new Point(currentEdge.Start.X - currentEdge.End.X, 0);

            // Zmiana bie¿¹cej krawêdzi na pionow¹
            edges[clickedEdgeIndex] = new VerticalEdge(edges[clickedEdgeIndex].Start, edges[clickedEdgeIndex].End);

            bool modificationSuccess = false;
            int currentIndex = clickedEdgeIndex;

            while (true)
            {
                var nextEdge = GetNextEdge(edges[currentIndex]);
                if (nextEdge == null) break;

                // Wywo³ujemy visitor, aby przetworzyæ kolejn¹ krawêdŸ w stosunku do obecnej
                modificationSuccess = edges[currentIndex].Accept(new EdgeStartChangeVisitor(), nextEdge, delta);

                // Jeœli modyfikacja zatoczy ko³o i wracamy do pocz¹tku, oznacza to, ¿e nie mo¿na wprowadziæ ograniczenia
                if (modificationSuccess == false)
                    break;
                else if (modificationSuccess && currentIndex == clickedEdgeIndex)
                {
                    MessageBox.Show("Nie mo¿na wprowadziæ ograniczenia. Zmiana cofniêta.");
                    edges = originalEdges; // Przywracamy oryginalne krawêdzie
                    Canvas.Invalidate();   // Odœwie¿enie
                    return;
                }

                // Aktualizujemy indeks do kolejnej krawêdzi
                currentIndex = (currentIndex + 1) % edges.Count;

                // Jeœli po przejœciu przez wszystkie krawêdzie, zatoczymy ko³o
                if (currentIndex == clickedEdgeIndex)
                    break;
            }

            Canvas.Invalidate();
        }
        private void AddFixedLengthConstraint_Click(object sender, EventArgs e)
        {

            var currentEdge = edges[clickedEdgeIndex];

            // Pobierz obecn¹ d³ugoœæ krawêdzi
            double currentLength = Math.Sqrt(Math.Pow(currentEdge.End.X - currentEdge.Start.X, 2) + Math.Pow(currentEdge.End.Y - currentEdge.Start.Y, 2));

            // Wyœwietl okno dialogowe z polem tekstowym
            Form prompt = new Form();
            prompt.Width = 300;  // Zwiêkszona szerokoœæ dla lepszego rozmieszczenia elementów
            prompt.Height = 150;
            prompt.Text = "Podaj d³ugoœæ krawêdzi";

            // Ustawienia napisu i pola tekstowego z odpowiednimi marginesami
            Label label = new Label() { Left = 10, Top = 15, Text = "D³ugoœæ:", AutoSize = true };
            TextBox inputBox = new TextBox() { Left = 80, Top = 10, Width = 180, Text = currentLength.ToString("F1") };
            Button confirmation = new Button() { Text = "OK", Left = 180, Width = 80, Top = 50 };

            prompt.Controls.Add(label);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);

            confirmation.Click += (s, ev) => { prompt.Close(); };
            inputBox.KeyDown += (s, ev) => { if (ev.KeyCode == Keys.Enter) prompt.Close(); };

            prompt.ShowDialog();

            // Sprawdzenie, czy wpisano poprawn¹ wartoœæ
            if (!double.TryParse(inputBox.Text, out double newLength) || newLength < 0)
            {
                MessageBox.Show("Wprowadzono niepoprawn¹ wartoœæ d³ugoœci.");
                return;
            }

            // Jeœli d³ugoœæ siê nie zmieni³a, zmieñ krawêdŸ na FixedLengthEdge
            if (Math.Abs(newLength - currentLength) < 0.2)
            {
                edges[clickedEdgeIndex] = new FixedLengthEdge(currentEdge.Start, currentEdge.End, newLength);
                Canvas.Invalidate();
                return;
            }

            // Zapisz pierwotny stan krawêdzi
            var originalEdges = edges.Select(edge => edge.Clone()).ToList();

            // Oblicz now¹ pozycjê punktu koñcowego
            double angle = Math.Atan2(currentEdge.End.Y - currentEdge.Start.Y, currentEdge.End.X - currentEdge.Start.X);

            var newEnd = new Point(
                currentEdge.Start.X + (int)(newLength * Math.Cos(angle)),
                currentEdge.Start.Y + (int)(newLength * Math.Sin(angle))
            );

            // Ustaw now¹ krawêdŸ FixedLengthEdge
            Point delta = new Point(newEnd.X - currentEdge.End.X, newEnd.Y - currentEdge.End.Y);
            edges[clickedEdgeIndex] = new FixedLengthEdge(currentEdge.Start, currentEdge.End, newLength);

            // Sprawdzanie dopasowania pozosta³ych krawêdzi
            bool modificationSuccess = false;

            int currentIndex = clickedEdgeIndex;

            while (true)
            {
                var nextEdge = GetNextEdge(edges[currentIndex]);
                if (nextEdge == null) break;

                modificationSuccess = edges[currentIndex].Accept(new EdgeStartChangeVisitor(), nextEdge, delta);

                // Jeœli modyfikacja zatoczy ko³o lub nie mo¿na dopasowaæ krawêdzi
                if (modificationSuccess == false)
                    break;
                else if (modificationSuccess && (currentIndex + 2) % edges.Count == clickedEdgeIndex)
                {
                    MessageBox.Show("Nie mo¿na wprowadziæ ograniczenia d³ugoœci. Zmiana cofniêta.");
                    edges = originalEdges;  // Przywróæ pierwotne krawêdzie
                    Canvas.Invalidate();    // Odœwie¿anie
                    return;
                }


                // Przejœcie do kolejnej krawêdzi
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

            // ZnajdŸ poprzedni element (z uwzglêdnieniem, ¿e lista mo¿e byæ cykliczna)
            int previousIndex = (index - 1 + edges.Count) % edges.Count;

            return edges[previousIndex]; // Zwraca referencjê do poprzedniego elementu
        }
        public Edge GetNextEdge(Edge currentEdge)
        {
            int index = edges.IndexOf(currentEdge);

            if (index == -1)
            {
                throw new ArgumentException("Edge not found in the list");
            }

            // ZnajdŸ nastêpny element (z uwzglêdnieniem cyklicznoœci listy)
            int nextIndex = (index + 1) % edges.Count;

            return edges[nextIndex]; // Zwraca referencjê do nastêpnego elementu
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
