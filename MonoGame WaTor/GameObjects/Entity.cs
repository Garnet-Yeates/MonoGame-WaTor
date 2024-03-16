using Microsoft.Xna.Framework.Graphics;

namespace MonoGame_WaTor.GameObjects
{
    public abstract class Entity
    {
        public const int EntitySize = 25;

        public short X { get; private set; }

        public short Y { get; private set; }

        public Entity[,] World { get; private set; }

        // Constructor
        public Entity(Entity[,] world, short x, short y)
        {
            World = world;
            X = x;
            Y = y;
            World[x, y] = this;
        }

        public void Move(short newX, short newY)
        {
            World[X, Y] = null;
            X = newX;
            Y = newY;
            World[X, Y] = this;
        }

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
