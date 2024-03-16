using MonoGame_WaTor.GameObjects;

namespace MonoGame_WaTor.DataStructures
{
    public class EntityGrid
    {
        public readonly Entity[,] Grid;

        public readonly short NumEntitiesFitX;

        public readonly short NumEntitiesFitY;

        public EntityGrid(short numEntitiesFitX, short numEntitiesFitY)
        {
            NumEntitiesFitX = numEntitiesFitX;
            NumEntitiesFitY = numEntitiesFitY;
            Grid = new Entity[NumEntitiesFitX, NumEntitiesFitY];
        }

        public Entity this[short x, short y]
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

        public void EnsureWithinBounds(ref short x, ref short y)
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

        public static short Modulo(short a, short b)
        {
            return (short)(((a % b) + b) % b);
        }
    }
}
