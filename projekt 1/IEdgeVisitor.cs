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
        public bool Visit(NoConstraintEdge currentEdge, BezierEdge nextEdge, Point delta);
        public bool Visit(HorizontalEdge currentEdge, BezierEdge nextEdge,Point delta);
        public bool Visit(VerticalEdge currentEdge, BezierEdge nextEdge, Point delta);
        public bool Visit(FixedLengthEdge currentEdge, BezierEdge nextEdge, Point delta);
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
            double deltaX = nextEdge.End.X - currentEdge.Start.X;
            double deltaY = nextEdge.End.Y - currentEdge.Start.Y;
            double d = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            double totalLength = currentEdge.Length + nextEdge.Length;

            if (d < totalLength && currentEdge.Length < nextEdge.Length + d && nextEdge.Length < currentEdge.Length + d)
            {
                double c1 = (currentEdge.Length * currentEdge.Length - nextEdge.Length * nextEdge.Length + d * d) / (2 * d);
                double c2 = d - c1;

                double unitX = deltaX / d;
                double unitY = deltaY / d;

                double h = Math.Sqrt(currentEdge.Length * currentEdge.Length - c1 * c1);


                double midX = currentEdge.Start.X + c1 * unitX;
                double midY = currentEdge.Start.Y + c1 * unitY;

                double offsetX = -unitY * h;
                double offsetY = unitX * h;

                Point option1 = new Point((int)(midX + offsetX), (int)(midY + offsetY));

                Point option2 = new Point((int)(midX - offsetX), (int)(midY - offsetY));

                double distanceOption1 = Math.Sqrt(Math.Pow(option1.X - currentEdge.End.X, 2) + Math.Pow(option1.Y - currentEdge.End.Y, 2));
                double distanceOption2 = Math.Sqrt(Math.Pow(option2.X - currentEdge.End.X, 2) + Math.Pow(option2.Y - currentEdge.End.Y, 2));

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

        public bool Visit(NoConstraintEdge currentEdge, BezierEdge nextEdge, Point delta)
        {
            if (nextEdge.StartContinuity == BezierEdge.ContinuityType.G0)
                return false;
            else if (nextEdge.StartContinuity == BezierEdge.ContinuityType.G1)
            {
                float originalDistance = (float)Math.Sqrt(
                    Math.Pow(nextEdge.ControlPoint1.X - nextEdge.Start.X, 2) +
                    Math.Pow(nextEdge.ControlPoint1.Y - nextEdge.Start.Y, 2));

                float edgeLength = (float)Math.Sqrt(
                    Math.Pow(currentEdge.End.X - currentEdge.Start.X, 2) +
                    Math.Pow(currentEdge.End.Y - currentEdge.Start.Y, 2));

                PointF direction = new PointF(
                    (currentEdge.End.X - currentEdge.Start.X) / edgeLength,
                    (currentEdge.End.Y - currentEdge.Start.Y) / edgeLength);

                nextEdge.ControlPoint1 = new Point(
                    (int)Math.Round(nextEdge.Start.X + direction.X * originalDistance),
                    (int)Math.Round(nextEdge.Start.Y + direction.Y * originalDistance));

                return false;
            }
            else
            {
                nextEdge.ControlPoint1 = new Point(currentEdge.End.X * 2 - currentEdge.Start.X, currentEdge.End.Y * 2 - currentEdge.Start.Y);

                return false;
            }
        }
        public bool Visit(HorizontalEdge currentEdge, BezierEdge nextEdge, Point delta)
        {
            currentEdge.End = new Point(currentEdge.End.X, currentEdge.Start.Y);
            int originalDistance = (int)Math.Sqrt(
               Math.Pow(nextEdge.ControlPoint1.X - nextEdge.Start.X, 2) +
               Math.Pow(nextEdge.ControlPoint1.Y - nextEdge.Start.Y, 2));
            nextEdge.Start = currentEdge.End;
            if(nextEdge.StartContinuity == BezierEdge.ContinuityType.G0)
            {
                return false;
            }
            else if(nextEdge.StartContinuity == BezierEdge.ContinuityType.G1)
            {
                float edgeLength = (float)Math.Sqrt(
                    Math.Pow(currentEdge.End.X - currentEdge.Start.X, 2) +
                    Math.Pow(currentEdge.End.Y - currentEdge.Start.Y, 2));

                PointF direction = new PointF(
                    (currentEdge.End.X - currentEdge.Start.X) / edgeLength,
                    (currentEdge.End.Y - currentEdge.Start.Y) / edgeLength);

                nextEdge.ControlPoint1 = new Point(
                    (int)Math.Round(nextEdge.Start.X + direction.X * originalDistance),
                    (int)Math.Round(nextEdge.Start.Y + direction.Y * originalDistance));

                return false;
            }
            else
            {
                nextEdge.ControlPoint1 = new Point(currentEdge.End.X * 2 - currentEdge.Start.X, currentEdge.End.Y);
                return false;
            }
        }
        public bool Visit(VerticalEdge currentEdge, BezierEdge nextEdge, Point delta)
        {
            currentEdge.End = new Point(currentEdge.Start.X, currentEdge.End.Y);
            int originalDistance = (int)Math.Sqrt(
               Math.Pow(nextEdge.ControlPoint1.X - nextEdge.Start.X, 2) +
               Math.Pow(nextEdge.ControlPoint1.Y - nextEdge.Start.Y, 2));
            nextEdge.Start = currentEdge.End;
            if (nextEdge.StartContinuity == BezierEdge.ContinuityType.G0)
            {
                return false;
            }
            else if (nextEdge.StartContinuity == BezierEdge.ContinuityType.G1)
            {
                float edgeLength = (float)Math.Sqrt(
                   Math.Pow(currentEdge.End.X - currentEdge.Start.X, 2) +
                   Math.Pow(currentEdge.End.Y - currentEdge.Start.Y, 2));

                PointF direction = new PointF(
                    (currentEdge.End.X - currentEdge.Start.X) / edgeLength,
                    (currentEdge.End.Y - currentEdge.Start.Y) / edgeLength);

                nextEdge.ControlPoint1 = new Point(
                    (int)Math.Round(nextEdge.Start.X + direction.X * originalDistance),
                    (int)Math.Round(nextEdge.Start.Y + direction.Y * originalDistance));

                return false;
            }
            else
            {
                nextEdge.ControlPoint1 = new Point(currentEdge.End.X , currentEdge.End.Y * 2 - currentEdge.Start.Y);
                return false;
            }
        }
        public bool Visit(FixedLengthEdge currentEdge, BezierEdge nextEdge, Point delta)
        {
            currentEdge.End = new Point(currentEdge.End.X + delta.X, currentEdge.End.Y + delta.Y);
            float originalDistance = (float)Math.Sqrt(
               Math.Pow(nextEdge.ControlPoint1.X - nextEdge.Start.X, 2) +
               Math.Pow(nextEdge.ControlPoint1.Y - nextEdge.Start.Y, 2));
            nextEdge.Start = currentEdge.End;
            if (nextEdge.StartContinuity == BezierEdge.ContinuityType.G0)
            {
                return false;
            }
            else if (nextEdge.StartContinuity == BezierEdge.ContinuityType.G1)
            {
                float edgeLength = (float)Math.Sqrt(
                     Math.Pow(currentEdge.End.X - currentEdge.Start.X, 2) +
                     Math.Pow(currentEdge.End.Y - currentEdge.Start.Y, 2));

                PointF direction = new PointF(
                    (currentEdge.End.X - currentEdge.Start.X) / edgeLength,
                    (currentEdge.End.Y - currentEdge.Start.Y) / edgeLength);

                nextEdge.ControlPoint1 = new Point(
                    (int)Math.Round(nextEdge.Start.X + direction.X * originalDistance),
                    (int)Math.Round(nextEdge.Start.Y + direction.Y * originalDistance));
                return false;
            }
            else
            {
                float edgeLength = (float)Math.Sqrt(
                   Math.Pow(currentEdge.End.X - currentEdge.Start.X, 2) +
                   Math.Pow(currentEdge.End.Y - currentEdge.Start.Y, 2));

                PointF direction = new PointF(
                    (currentEdge.End.X - currentEdge.Start.X) / edgeLength,
                    (currentEdge.End.Y - currentEdge.Start.Y) / edgeLength);

                nextEdge.ControlPoint1 = new Point(
                    (int)(nextEdge.Start.X + direction.X * edgeLength),
                    (int)(nextEdge.Start.Y + direction.Y * edgeLength));
                return false;
            }
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

                int newStartX = currentEdge.End.X < prevEdge.End.X
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

                int newStartY = currentEdge.End.Y < prevEdge.End.Y
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
            double d = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            double totalLength = currentEdge.Length + prevEdge.Length;

            if (d < totalLength && currentEdge.Length < prevEdge.Length + d && prevEdge.Length < currentEdge.Length + d)
            {
                double c1 = (currentEdge.Length * currentEdge.Length - prevEdge.Length * prevEdge.Length + d * d) / (2 * d);
                double c2 = d - c1;

                double h = Math.Sqrt(currentEdge.Length * currentEdge.Length - c1 * c1);

                double unitX = deltaX / d;
                double unitY = deltaY / d;

                double midX = currentEdge.End.X + c1 * unitX; 
                double midY = currentEdge.End.Y + c1 * unitY;

                double offsetX = -unitY * h;
                double offsetY = unitX * h;

                Point option1 = new Point((int)(midX + offsetX), (int)(midY + offsetY));

                Point option2 = new Point((int)(midX - offsetX), (int)(midY - offsetY));

                double distanceOption1 = Math.Sqrt(Math.Pow(option1.X - currentEdge.Start.X, 2) + Math.Pow(option1.Y - currentEdge.Start.Y, 2));
                double distanceOption2 = Math.Sqrt(Math.Pow(option2.X - currentEdge.Start.X, 2) + Math.Pow(option2.Y - currentEdge.Start.Y, 2));

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

        public bool Visit(NoConstraintEdge currentEdge, BezierEdge prevEdge, Point delta)
        {
            if (prevEdge.EndContinuity == BezierEdge.ContinuityType.G0)
                return false;
            else if(prevEdge.EndContinuity == BezierEdge.ContinuityType.G1)
            {
                float originalDistance = (float)Math.Sqrt(
                    Math.Pow(prevEdge.ControlPoint2.X - prevEdge.End.X, 2) +
                    Math.Pow(prevEdge.ControlPoint2.Y - prevEdge.End.Y, 2));

                float edgeLength = (float)Math.Sqrt(
                    Math.Pow(currentEdge.End.X - currentEdge.Start.X, 2) +
                    Math.Pow(currentEdge.End.Y - currentEdge.Start.Y, 2));

                PointF direction = new PointF(
                    (currentEdge.Start.X - currentEdge.End.X) / edgeLength,
                    (currentEdge.Start.Y - currentEdge.End.Y) / edgeLength);

                prevEdge.ControlPoint2 = new Point(
                    (int)Math.Round(prevEdge.End.X + direction.X * originalDistance),
                    (int)Math.Round(prevEdge.End.Y + direction.Y * originalDistance));

                return false;

            }
            else
            {
                prevEdge.ControlPoint2 = new Point(2*currentEdge.Start.X - currentEdge.End.X, 2 * currentEdge.Start.Y - currentEdge.End.Y);
                return false;
            }
        }
        public bool Visit(HorizontalEdge currentEdge, BezierEdge prevEdge, Point delta)
        {
            currentEdge.Start = new Point(currentEdge.Start.X, currentEdge.End.Y);
            int originalDistance = prevEdge.ControlPoint2.X - prevEdge.End.X;
            prevEdge.End = currentEdge.Start;
            if (prevEdge.EndContinuity == BezierEdge.ContinuityType.G0)
            {
                return false;
            }
            else if (prevEdge.EndContinuity == BezierEdge.ContinuityType.G1)
            {
                prevEdge.ControlPoint2 = new Point(currentEdge.Start.X + originalDistance, currentEdge.Start.Y);
                return false;
            }
            else
            {
                prevEdge.ControlPoint2 = new Point(currentEdge.Start.X * 2 - currentEdge.End.X, currentEdge.Start.Y);
                return false;
            }
        }
        public bool Visit(VerticalEdge currentEdge, BezierEdge prevEdge, Point delta)
        {
            currentEdge.Start = new Point(currentEdge.End.X, currentEdge.Start.Y);
            int originalDistance = prevEdge.ControlPoint2.Y - prevEdge.End.Y;
            prevEdge.End = currentEdge.Start;
            if (prevEdge.EndContinuity == BezierEdge.ContinuityType.G0)
            {
                return false;
            }
            else if (prevEdge.EndContinuity == BezierEdge.ContinuityType.G1)
            {
                prevEdge.ControlPoint2 = new Point(currentEdge.Start.X , currentEdge.Start.Y + originalDistance);
                return false;
            }
            else
            {
                prevEdge.ControlPoint2 = new Point(currentEdge.Start.X , currentEdge.Start.Y * 2 - currentEdge.End.Y);
                return false;
            }
        }
        public bool Visit(FixedLengthEdge currentEdge, BezierEdge prevEdge, Point delta)
        {
            currentEdge.Start = new Point(currentEdge.Start.X + delta.X, currentEdge.Start.Y + delta.Y);
            float originalDistance = (float)Math.Sqrt(
               Math.Pow(prevEdge.ControlPoint2.X - prevEdge.End.X, 2) +
               Math.Pow(prevEdge.ControlPoint2.Y - prevEdge.End.Y, 2));
            prevEdge.End = currentEdge.Start;
            if (prevEdge.EndContinuity == BezierEdge.ContinuityType.G0)
            {
                return false;
            }
            else if (prevEdge.EndContinuity == BezierEdge.ContinuityType.G1)
            {
                float edgeLength = (float)Math.Sqrt(
                    Math.Pow(currentEdge.End.X - currentEdge.Start.X, 2) +
                    Math.Pow(currentEdge.End.Y - currentEdge.Start.Y, 2));

                PointF direction = new PointF(
                    (currentEdge.Start.X - currentEdge.End.X) / edgeLength,
                    (currentEdge.Start.Y - currentEdge.End.Y) / edgeLength);

                prevEdge.ControlPoint2 = new Point(
                    (int)Math.Round(prevEdge.End.X + direction.X * originalDistance),
                    (int)Math.Round(prevEdge.End.Y + direction.Y * originalDistance));
                return false;
            }
            else
            {
                float edgeLength = (float)Math.Sqrt(
                   Math.Pow(currentEdge.Start.X - currentEdge.End.X, 2) +
                   Math.Pow(currentEdge.Start.Y - currentEdge.End.Y, 2));

                PointF direction = new PointF(
                    (currentEdge.Start.X - currentEdge.End.X) / edgeLength,
                    (currentEdge.Start.Y - currentEdge.End.Y) / edgeLength);

                prevEdge.ControlPoint2 = new Point(
                    (int)(prevEdge.End.X + direction.X * edgeLength),
                    (int)(prevEdge.End.Y + direction.Y * edgeLength));
                return false;
            }
        }
    }
}

