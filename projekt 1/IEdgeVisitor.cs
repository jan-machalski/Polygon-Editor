using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt_1
{
    public interface IEdgeVisitor
    {
        public bool Visit(NoConstraintEdge currentEdge, NoConstraintEdge nextEdge, Point delta);
        public bool Visit(HorizontalEdge currentEdge, NoConstraintEdge nextEdge, Point delta);
        public bool Visit(VerticalEdge currentEdge, NoConstraintEdge nextEdge, Point delta);
        public bool Visit(FixedLengthEdge currentEdge, NoConstraintEdge nextEdge, Point delta);
        public bool Visit(NoConstraintEdge currentEdge, HorizontalEdge nextEdge, Point delta);  
        public bool Visit(VerticalEdge currentEdge, HorizontalEdge nextEdge, Point delta);
        public bool Visit(FixedLengthEdge currentEdge, HorizontalEdge nextEdge, Point delta);
        public bool Visit(NoConstraintEdge currentEdge, VerticalEdge nextEdge, Point delta);
        public bool Visit(HorizontalEdge currentEdge, VerticalEdge nextEdge, Point delta);
        public bool Visit(FixedLengthEdge currentEdge, VerticalEdge nextEdge, Point delta);
        public bool Visit(NoConstraintEdge currentEdge, FixedLengthEdge nextEdge, Point delta);
        public bool Visit(HorizontalEdge currentEdge, FixedLengthEdge nextEdge, Point delta);
        public bool Visit(VerticalEdge currentEdge, FixedLengthEdge nextEdge, Point delta);
        public bool Visit(FixedLengthEdge currentEdge, FixedLengthEdge nextEdge, Point delta);
        public bool Visit(HorizontalEdge currentEdge, HorizontalEdge nextEdge, Point delta);
        public bool Visit(VerticalEdge currentEdge, VerticalEdge nextEdge, Point delta);
    }

    public class EdgeStartChangeVisitor : IEdgeVisitor
    {
        public bool Visit(NoConstraintEdge currentEdge, NoConstraintEdge nextEdge, Point delta)
        {
            return false;
        }
        public bool Visit(HorizontalEdge currentEdge, NoConstraintEdge nextEdge, Point delta)
        {
            currentEdge.End = new Point(currentEdge.End.X, currentEdge.Start.Y);
            nextEdge.Start = currentEdge.End;
            return false;
        }
        public bool Visit(VerticalEdge currentEdge, NoConstraintEdge nextEdge, Point delta)
        {
            currentEdge.End = new Point(currentEdge.Start.X, currentEdge.End.Y);
            nextEdge.Start = currentEdge.End;   
            return false;
        }
        public bool Visit(FixedLengthEdge currentEdge, NoConstraintEdge nextEdge, Point delta)
        {
            
            currentEdge.End = new Point(currentEdge.End.X + delta.X,currentEdge.End.Y+delta.Y);
            nextEdge.Start = currentEdge.End;

            return false;
        }
        public bool Visit(NoConstraintEdge currentEdge, HorizontalEdge nextEdge, Point delta)
        {
            return false;
        }
        public bool Visit(VerticalEdge currentEdge, HorizontalEdge nextEdge, Point delta)
        {
            currentEdge.End = new Point(currentEdge.Start.X,nextEdge.End.Y);
            nextEdge.Start = currentEdge.End;
            return false;
        }
        public bool Visit(FixedLengthEdge currentEdge, HorizontalEdge nextEdge, Point delta)
        {
            double verticalDistance = Math.Abs(currentEdge.Start.Y - nextEdge.Start.Y);

            if (verticalDistance <= currentEdge.Length)
            {
                double horizontalDistance = Math.Sqrt(Math.Pow(currentEdge.Length, 2) - Math.Pow(verticalDistance, 2));

                int newEndX = currentEdge.Start.X < nextEdge.Start.X
                    ? currentEdge.Start.X + (int)horizontalDistance
                    : currentEdge.Start.X - (int)horizontalDistance;

                currentEdge.End = new Point(newEndX, nextEdge.Start.Y);
                nextEdge.Start = currentEdge.End;

                return false;
            }
            else
            {
                currentEdge.End = new Point(currentEdge.End.X + delta.X, currentEdge.End.Y + delta.Y);
                nextEdge.Start = currentEdge.End;

                return true;
            }
        }
        public bool Visit(NoConstraintEdge currentEdge, VerticalEdge nextEdge, Point delta)
        {
            return false;
        }
        public bool Visit(HorizontalEdge currentEdge, VerticalEdge nextEdge, Point delta)
        {
            currentEdge.End = new Point(nextEdge.Start.X, currentEdge.Start.Y);
            nextEdge.Start = currentEdge.End;
            return false;
        }
        public bool Visit(FixedLengthEdge currentEdge, VerticalEdge nextEdge, Point delta)
        {
            double horizontalDistance = Math.Abs(currentEdge.Start.X - nextEdge.Start.X);

            if (horizontalDistance <= currentEdge.Length)
            {
                double verticalDistance = Math.Sqrt(Math.Pow(currentEdge.Length, 2) - Math.Pow(horizontalDistance, 2));

                int newEndY = currentEdge.Start.Y < nextEdge.Start.Y
                    ? currentEdge.Start.Y + (int)verticalDistance
                    : currentEdge.Start.Y - (int)verticalDistance;

                currentEdge.End = new Point(nextEdge.Start.X, newEndY);
                nextEdge.Start = currentEdge.End;

                return false;
            }
            else
            {
                currentEdge.End = new Point(currentEdge.End.X + delta.X, currentEdge.End.Y + delta.Y);
                nextEdge.Start = currentEdge.End;
           
                return true;
            }
        }
        public bool Visit(NoConstraintEdge currentEdge, FixedLengthEdge nextEdge, Point delta)
        {
            return false;
        }
        public bool Visit(VerticalEdge currentEdge, FixedLengthEdge nextEdge, Point delta)
        {
            double horizontalDistance = Math.Abs(currentEdge.Start.X - nextEdge.End.X);

            if (horizontalDistance <= nextEdge.Length)
            {
                double verticalDistance = Math.Sqrt(Math.Pow(nextEdge.Length, 2) - Math.Pow(horizontalDistance, 2));

                int newEndY = currentEdge.End.Y < nextEdge.Start.Y
                    ? nextEdge.End.Y + (int)verticalDistance
                    : nextEdge.End.Y - (int)verticalDistance;

                currentEdge.End = new Point(currentEdge.Start.X,newEndY);
                nextEdge.Start = currentEdge.End;

                return false;
            }
            else
            {
                currentEdge.End = new Point(currentEdge.End.X + delta.X, currentEdge.End.Y + delta.Y);
                nextEdge.Start = currentEdge.End;

                return true;
            }
        }

        public bool Visit(HorizontalEdge currentEdge, FixedLengthEdge nextEdge, Point delta)
        {
            double verticalDistance = Math.Abs(currentEdge.Start.Y - nextEdge.End.Y);

            if (verticalDistance <= nextEdge.Length)
            {
                double horizontalDistance = Math.Sqrt(Math.Pow(nextEdge.Length, 2) - Math.Pow(verticalDistance, 2));

                int newEndX = currentEdge.End.X < nextEdge.End.X
                    ? nextEdge.End.X - (int)horizontalDistance
                    : nextEdge.End.X + (int)horizontalDistance;

                currentEdge.End = new Point(newEndX,currentEdge.Start.Y);

                nextEdge.Start = currentEdge.End;

                return false;
            }
            else
            {
                currentEdge.End = new Point(currentEdge.End.X + delta.X, currentEdge.End.Y + delta.Y);
                nextEdge.Start = currentEdge.End;

                return true;
            }
        }

        public bool Visit(FixedLengthEdge currentEdge, FixedLengthEdge nextEdge, Point delta)
        {
            double deltaX = nextEdge.End.X - currentEdge.Start.X + delta.X;
            double deltaY = nextEdge.End.Y - currentEdge.Start.Y + delta.Y;

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            double totalLength = currentEdge.Length + nextEdge.Length;

            if (distance < totalLength && currentEdge.Length < nextEdge.Length+distance && nextEdge.Length < currentEdge.Length+distance)
            {

                double unitX = deltaX / distance;
                double unitY = deltaY / distance;

                double ratio = currentEdge.Length / totalLength;

                double midX = currentEdge.Start.X + ratio * deltaX;
                double midY = currentEdge.Start.Y + ratio * deltaY;

                double offsetX = -unitY * (totalLength - distance) / 2;  // Na plus (dodajemy)
                double offsetY = unitX * (totalLength - distance) / 2;  // Na plus

                // Opcja 1: Przesunięcie z dodaniem offsetu
                Point option1 = new Point((int)(midX + offsetX), (int)(midY + offsetY));

                // Opcja 2: Przesunięcie z odjęciem offsetu
                Point option2 = new Point((int)(midX - offsetX), (int)(midY - offsetY));

                // Obliczamy odległość między oryginalnym currentEdge.End a każdą z opcji
                double distanceOption1 = Math.Sqrt(Math.Pow(option1.X - currentEdge.End.X, 2) + Math.Pow(option1.Y - currentEdge.End.Y, 2));
                double distanceOption2 = Math.Sqrt(Math.Pow(option2.X - currentEdge.End.X, 2) + Math.Pow(option2.Y - currentEdge.End.Y, 2));

                // Wybieramy tę opcję, która jest bliżej oryginalnej wartości currentEdge.End
                if (distanceOption1 < distanceOption2)
                {
                    currentEdge.End = option1;
                }
                else
                {
                    currentEdge.End = option2;
                }
                nextEdge.Start = currentEdge.End;

                return false;
            }
            else
            {

                currentEdge.End = new Point(currentEdge.End.X + delta.X, currentEdge.End.Y + delta.Y);
                nextEdge.Start = currentEdge.End;

                return true;
            }
        }
        public bool Visit(HorizontalEdge currentEdge, HorizontalEdge nextEdge, Point delta)
        {
            throw new NotImplementedException();
        }
        public bool Visit(VerticalEdge currentEdge, VerticalEdge nextEdge, Point delta)
        {
            throw new NotSupportedException();
        }



    }

    public class EdgeEndChangeVisitor : IEdgeVisitor
    {
        public bool Visit(NoConstraintEdge currentEdge, NoConstraintEdge prevEdge, Point delta)
        {
            return false;
        }

        public bool Visit(HorizontalEdge currentEdge, NoConstraintEdge prevEdge, Point delta)
        {
            currentEdge.Start = new Point(currentEdge.Start.X, currentEdge.End.Y);
            prevEdge.End = currentEdge.Start;
            return false;
        }

        public bool Visit(VerticalEdge currentEdge, NoConstraintEdge prevEdge, Point delta)
        {
            currentEdge.Start = new Point(currentEdge.End.X, currentEdge.Start.Y);
            prevEdge.End = currentEdge.Start;
            return false;
        }

        public bool Visit(FixedLengthEdge currentEdge, NoConstraintEdge prevEdge, Point delta)
        {
            currentEdge.Start = new Point(currentEdge.Start.X + delta.X, currentEdge.Start.Y + delta.Y);
            prevEdge.End = currentEdge.Start;

            return false;
        }

        public bool Visit(NoConstraintEdge currentEdge, HorizontalEdge prevEdge, Point delta)
        {
            return false;
        }

        public bool Visit(VerticalEdge currentEdge, HorizontalEdge prevEdge, Point delta)
        {
            currentEdge.Start = new Point(currentEdge.End.X, prevEdge.Start.Y);
            prevEdge.End = currentEdge.Start;
            return false;
        }

        public bool Visit(FixedLengthEdge currentEdge, HorizontalEdge prevEdge, Point delta)
        {
            double verticalDistance = Math.Abs(currentEdge.End.Y - prevEdge.Start.Y);

            if (verticalDistance <= currentEdge.Length)
            {
                double horizontalDistance = Math.Sqrt(Math.Pow(currentEdge.Length, 2) - Math.Pow(verticalDistance, 2));

                int newStartX = currentEdge.End.X < prevEdge.Start.X
                    ? currentEdge.End.X + (int)horizontalDistance
                    : currentEdge.End.X - (int)horizontalDistance;

                currentEdge.Start = new Point(newStartX, prevEdge.Start.Y);
                prevEdge.End = currentEdge.Start;

                return false;
            }
            else
            {
                currentEdge.Start = new Point(currentEdge.Start.X + delta.X, currentEdge.Start.Y + delta.Y);
                prevEdge.End = currentEdge.Start;

                return true;
            }
        }

        public bool Visit(NoConstraintEdge currentEdge, VerticalEdge prevEdge, Point delta)
        {
            return false;
        }

        public bool Visit(HorizontalEdge currentEdge, VerticalEdge prevEdge, Point delta)
        {
            currentEdge.Start = new Point(prevEdge.End.X, currentEdge.End.Y);
            prevEdge.End = currentEdge.Start;
            return false;
        }

        public bool Visit(FixedLengthEdge currentEdge, VerticalEdge prevEdge, Point delta)
        {
            double horizontalDistance = Math.Abs(currentEdge.End.X - prevEdge.Start.X);

            if (horizontalDistance <= currentEdge.Length)
            {
                double verticalDistance = Math.Sqrt(Math.Pow(currentEdge.Length, 2) - Math.Pow(horizontalDistance, 2));

                int newStartY = currentEdge.End.Y < prevEdge.Start.Y
                    ? currentEdge.End.Y + (int)verticalDistance
                    : currentEdge.End.Y - (int)verticalDistance;

                currentEdge.Start = new Point(prevEdge.Start.X, newStartY);
                prevEdge.End = currentEdge.Start;

                return false;
            }
            else
            {
                currentEdge.Start = new Point(currentEdge.Start.X + delta.X, currentEdge.Start.Y + delta.Y);
                prevEdge.End = currentEdge.Start;

                return true;
            }
        }

        public bool Visit(NoConstraintEdge currentEdge, FixedLengthEdge prevEdge, Point delta)
        {
            return false;
        }

        public bool Visit(VerticalEdge currentEdge, FixedLengthEdge prevEdge, Point delta)
        {
            double horizontalDistance = Math.Abs(currentEdge.End.X - prevEdge.Start.X);

            if (horizontalDistance <= prevEdge.Length)
            {
                double verticalDistance = Math.Sqrt(Math.Pow(prevEdge.Length, 2) - Math.Pow(horizontalDistance, 2));

                int newStartY = currentEdge.Start.Y < prevEdge.Start.Y
                    ? prevEdge.Start.Y - (int)verticalDistance
                    : prevEdge.Start.Y + (int)verticalDistance;

                currentEdge.Start = new Point(currentEdge.End.X,newStartY);
                prevEdge.End = currentEdge.Start;

                return false;
            }
            else
            {
                currentEdge.Start = new Point(currentEdge.Start.X + delta.X, currentEdge.Start.Y + delta.Y);
                prevEdge.End = currentEdge.Start;

                return true;
            }
        }

        public bool Visit(HorizontalEdge currentEdge, FixedLengthEdge prevEdge, Point delta)
        {
            double verticalDistance = Math.Abs(currentEdge.End.Y - prevEdge.Start.Y);

            if (verticalDistance <= prevEdge.Length)
            {
                double horizontalDistance = Math.Sqrt(Math.Pow(prevEdge.Length, 2) - Math.Pow(verticalDistance, 2));

                int newStartX = currentEdge.Start.X < prevEdge.Start.X
                    ? prevEdge.Start.X - (int)horizontalDistance
                    : prevEdge.Start.X + (int)horizontalDistance;

                currentEdge.Start = new Point(newStartX,currentEdge.End.Y);

                prevEdge.End = currentEdge.Start;

                return false;
            }
            else
            {
                currentEdge.Start = new Point(currentEdge.Start.X + delta.X, currentEdge.Start.Y + delta.Y);
                prevEdge.End = currentEdge.Start;

                return true;
            }
        }

        public bool Visit(FixedLengthEdge currentEdge, FixedLengthEdge prevEdge, Point delta)
        {
            double deltaX = prevEdge.Start.X + delta.X - currentEdge.End.X;
            double deltaY = prevEdge.Start.Y + delta.Y - currentEdge.End.Y;

            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            double totalLength = currentEdge.Length + prevEdge.Length;

            if (distance < totalLength && currentEdge.Length < prevEdge.Length + distance && prevEdge.Length < currentEdge.Length + distance)
            {
                double unitX = deltaX / distance;
                double unitY = deltaY / distance;

                double ratio = currentEdge.Length / totalLength;

                double midX = currentEdge.End.X + ratio * deltaX;
                double midY = currentEdge.End.Y + ratio * deltaY;

                double offsetX = unitY * (totalLength - distance) / 2;
                double offsetY = -unitX * (totalLength - distance) / 2;

                Point option1 = new Point((int)(midX + offsetX), (int)(midY + offsetY));

                // Opcja 2: Odejmuje offset
                Point option2 = new Point((int)(midX - offsetX), (int)(midY - offsetY));

                // Obliczamy odległość między oryginalnym currentEdge.Start a każdą z opcji
                double distanceOption1 = Math.Sqrt(Math.Pow(option1.X - currentEdge.Start.X, 2) + Math.Pow(option1.Y - currentEdge.Start.Y, 2));
                double distanceOption2 = Math.Sqrt(Math.Pow(option2.X - currentEdge.Start.X, 2) + Math.Pow(option2.Y - currentEdge.Start.Y, 2));

                // Wybieramy tę opcję, która jest bliżej oryginalnej wartości currentEdge.Start
                if (distanceOption1 < distanceOption2)
                {
                    currentEdge.Start = option1;
                }
                else
                {
                    currentEdge.Start = option2;
                }

                prevEdge.End = currentEdge.Start;

                return false;
            }
            else
            {
                currentEdge.Start = new Point(currentEdge.Start.X + delta.X, currentEdge.Start.Y + delta.Y);
                prevEdge.End = currentEdge.Start;

                return true;
            }
        }

        public bool Visit(HorizontalEdge currentEdge, HorizontalEdge prevEdge, Point delta)
        {
            throw new NotImplementedException();
        }

        public bool Visit(VerticalEdge currentEdge, VerticalEdge prevEdge, Point delta)
        {
            throw new NotSupportedException();
        }
    }
}

