using MonoGame_WaTor.GameObjects;

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

        public Entity this[int x, int y]
        {
            get
            {
                EnsureWithinBounds(ref x, ref y);
                return Grid[x, y];
            }
            set
            {
                EnsureWithinBounds(ref x, ref y);
                Grid[x, y] = value;
            }
        }

        public void EnsureWithinBounds(ref int x, ref int y)
        {
            if (x < 0 || x >= NumEntitiesFitX)
            {
                x = Modulo(x, NumEntitiesFitX);
            }

            if (y < 0 || y >= NumEntitiesFitY)
            {
                y = Modulo(y, NumEntitiesFitY);
            }
        }

        public static int Modulo(int a, int b)
        {
            return ((a % b) + b) % b;
        }
    }
}
