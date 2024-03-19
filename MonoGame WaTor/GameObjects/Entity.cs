using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_WaTor.DataStructures;
using System;

namespace MonoGame_WaTor.GameObjects
{
    public abstract class Entity
    {
        // Size in pixels of how big Entities are. Entities are represented as colored squares
        public const int EntitySize = 3;

        public WaTorGame Game { get; }

        // Where am I in the world? Also a reference to the world itself for encapsulated movement
        public EntityGrid World => Game.World;
        public int X { get; private set; }
        public int Y { get; private set; }

        public bool ExistsInWorld => World[X, Y] == this;

        // For drawing
        public abstract Color Color { get; }
        public abstract Texture2D Texture { get; }

        public Entity(WaTorGame game, int x, int y)
        {
            Game = game;
            X = x;
            Y = y;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle(X * EntitySize, Y * EntitySize, EntitySize, EntitySize), Color);
        }

        public void RemoveFromWorld()
        {
            if (!ExistsInWorld) throw new Exception($"This {GetType().Name} doesn't exist in the world ");

            World[X, Y] = null;
        }

        public void AddToWorld()
        {
            if (ExistsInWorld) throw new Exception($"This {GetType().Name} already exists in the world ");

            if (World[X, Y] is not null) throw new Exception("Can not add an Entity onto another entity");

            World[X, Y] = this;
        }

        public void Move(int newX, int newY)
        {
            Entity here = World[X, Y];
            if (!ExistsInWorld) throw new Exception("Cannot move an Entity that doesn't exist in the world");

            if (World[newX, newY] is not null) throw new Exception("Can not move an Entity onto another entity");

            World[X, Y] = null;
            X = newX;
            Y = newY;
            World[X, Y] = this;
        }

        public void Move(Point2D p)
        {
            Move(p.X, p.Y);
        }

        public abstract void Update(Random r);
    }
}
