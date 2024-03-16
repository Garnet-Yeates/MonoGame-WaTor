using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_WaTor.DataStructures;
using System;

namespace MonoGame_WaTor.GameObjects
{
    public abstract class Entity
    {
        // Size in pixels of how big Entities are. Entities are represented as colored squares
        public const int EntitySize = 25;

        // Where am I in the world? Also a reference to the world itself for encapsulated movement
        public Entity[,] World { get; private set; }
        public short X { get; private set; }
        public short Y { get; private set; }

        // Where am I in the Entity update list? Also a reference to my Node for efficient self-removal thru List reference
        public abstract byte GroupIndex { get; }
        public GroupedList<Entity> Entities { get; private set; }
        public GroupedListNode<Entity> MyNode { get; private set; }
        public bool ExistsInWorld => MyNode is not null;

        // For drawing
        public abstract Color Color { get; }

        public abstract Texture2D Texture { get; }

        // Constructor
        public Entity(Entity[,] world, GroupedList<Entity> entities, short x, short y)
        {
            Entities = entities;
            World = world;
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
            Entities.Remove(MyNode);
        }

        public void AddToWorld()
        {
            if (ExistsInWorld) throw new Exception($"This {GetType().Name} already exists in the world ");

            World[X, Y] = this;
            MyNode = Entities.Add(this, GroupIndex);
        }

        public void Move(short newX, short newY)
        {
            if (!ExistsInWorld) throw new Exception("Cannot add an entity that doesn't exist in the world");

            World[X, Y] = null;
            X = newX;
            Y = newY;
            World[X, Y] = this;
        }

        public abstract void Update();
    }
}
