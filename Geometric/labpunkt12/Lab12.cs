using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    [Serializable]
    public struct Point
    {
        public double x;
        public double y;

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Point p1, Point p2) { return p1.x == p2.x && p1.y == p2.y; }

        public static bool operator !=(Point p1, Point p2) { return !(p1 == p2); }

        public override string ToString()
        {
            return string.Format("({0},{1})", x, y);
        }
        public static double Distance(Point p1, Point p2)
        {
            double dx, dy;
            dx = p1.x - p2.x;
            dy = p1.y - p2.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

    }

    public class Lab12 : MarshalByRefObject
    {

        private class ByY : IComparer<Point>
        {
            public int Compare(Point p1, Point p2)
            {
                int res = p1.y.CompareTo(p2.y);
                return res == 0 ? p1.x.CompareTo(p2.x) : res;
            }
        }


        /// <summary>
        /// Metoda zwraca dwa najbliższe punkty w dwuwymiarowej przestrzeni Euklidesowej
        /// </summary>
        /// <param name="points">Chmura punktów</param>
        /// <param name="minDistance">Odległość pomiędzy najbliższymi punktami</param>
        /// <returns>Para najbliższych punktów. Kolejność nie ma znaczenia</returns>
        /// <remarks>
        /// 1) Algorytm powinien mieć złożoność O(n^2), gdzie n to liczba punktów w chmurze
        /// </remarks>
        public Tuple<Point, Point> FindClosestPointsBrute(List<Point> points, out double minDistance)
        {
            return FindClosestPoints(points, out minDistance);
        }

        /// <summary>
        /// Metoda zwraca dwa najbliższe punkty w dwuwymiarowej przestrzeni Euklidesowej
        /// </summary>
        /// <param name="points">Chmura punktów</param>
        /// <param name="minDistance">Odległość pomiędzy najbliższymi punktami</param>
        /// <returns>Para najbliższych punktów. Kolejność nie ma znaczenia</returns>
        /// <remarks>
        /// 1) Algorytm powinien mieć złożoność n*logn, gdzie n to liczba punktów w chmurze
        /// </remarks>
        public Tuple<Point, Point> FindClosestPoints(List<Point> points, out double minDistance)
        {
            var sortedX = points.OrderBy(p => p.x).ToList();
    
            Point p1 = sortedX[0];
            Point p2 = sortedX[1];
            minDistance = Point.Distance(p1, p2);
            var closestPair = new Tuple<Point, Point>(p1, p2);
             
            var stripPoints = new SortedSet<Point>(new ByY());
            stripPoints.Add(sortedX[0]);

            int removed = 0;
            
            for (int i = 1; i < sortedX.Count; i++)
            {
                Point current = sortedX[i];
                for(int j = removed; j < i; j++)
                {
                    if (current.x - sortedX[j].x > minDistance)
                    {
                        stripPoints.Remove(sortedX[j]);
                        removed++;
                    }
                    else
                    {
                        break;
                    }
                }
                
                
                
                var lowerBound = new Point(0, current.y - minDistance);
                var upperBound = new Point(0, current.y + minDistance);
                var candidates = stripPoints.GetViewBetween(lowerBound, upperBound);
        
                foreach (var candidate in candidates)
                {
                    double distance = Point.Distance(current, candidate);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPair = new Tuple<Point, Point>(current, candidate);
                    }
                }
                stripPoints.Add(current);
            }
    
            return closestPair;
        }

    }

}
