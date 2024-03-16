using MonoGame_WaTor.GameObjects;
using System.Collections.Generic;

namespace MonoGame_WaTor.DataStructures
{
    public class EntityGrid
    {
        public readonly Entity[,] Grid;

        public readonly int NumEntitiesFitX;

        public readonly int NumEntitiesFitY;

        public EntityGrid(int numEntitiesFitX, int numEntitiesFitY)
        {
            NumEntitiesFitX = numEntitiesFitX;
            NumEntitiesFitY = numEntitiesFitY;
            Grid = new Entity[NumEntitiesFitX, NumEntitiesFitY];
        }

        public Entity this[in Point2D p]
        {
            get
            {
                return Grid[p.X, p.Y];
            }
            set
            {
                Grid[p.X, p.Y] = value;
            }
        }

        public Entity this[int x, int y]
        {
            get
            {
                return Grid[x, y];
            }
            set
            {
                Grid[x, y] = value;
            }
        }

        public static int Modulo(int a, int b)
        {
            return ((a % b) + b) % b;
        }

        public IEnumerable<Point2D> GetAdjacentLocations(in Point2D p)
        {
            return GetAdjacentLocations(p.X, p.Y);
        }

        /// <summary>
        /// Conveniently enough, the default initialCapacity of a List is 4! So we can simply convert the result of this method to
        /// a List using new List(GetAdjacentLocations(...)) (Enumerable constructor) without worrying about the List inefficiently 
        /// re-allocating as a result of Count going past the Capacity, because we know GetAdjacentLocations returns exactly 4.
        /// </summary>
        public IEnumerable<Point2D> GetAdjacentLocations(int x, int y)
        {
            if (x < NumEntitiesFitX - 1)
            {
                yield return new Point2D(x + 1, y);
            }
            else
            {
                yield return new Point2D(0, y);
            }

            if (x > 0)
            {
                yield return new Point2D(x - 1, y);
            }
            else
            {
                yield return new Point2D(NumEntitiesFitX - 1, y);
            }

            if (y < NumEntitiesFitY - 1)
            {
                yield return new Point2D(x, y + 1);
            }
            else
            {
                yield return new Point2D(x, 0);
            }

            if (y > 0)
            {
                yield return new Point2D(x, y - 1);
            }
            else
            {
                yield return new Point2D(x, NumEntitiesFitY - 1);
            }
        }

        public IEnumerable<Point2D> GetAdjacentEmptyLocations(in Point2D p)
        {
            return GetAdjacentEmptyLocations(p.X, p.Y);
        }

        public IEnumerable<Point2D> GetAdjacentEmptyLocations(int x, int y)
        {
            foreach (var p in GetAdjacentLocations(x, y))
            {
                if (this[p] == null)
                {
                    yield return p;
                }
            }
        }
    }
}
