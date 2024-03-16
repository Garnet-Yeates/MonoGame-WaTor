using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_WaTor.DataStructures;
using System;

namespace MonoGame_WaTor.GameObjects
{
    public abstract class Entity
    {
        // Size in pixels of how big Entities are. Entities are represented as colored squares
        public const int EntitySize = 10;

        // Where am I in the world? Also a reference to the world itself for encapsulated movement
        public EntityGrid World { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        // Where am I in the Entity update list? Also a reference to my Node for efficient self-removal thru List reference
        public abstract byte GroupIndex { get; }
        public PriorityGroupedList<Entity> Entities { get; private set; }
        public GroupedListNode<Entity> MyNode { get; private set; }
        public bool ExistsInWorld => MyNode is not null;

        // For drawing
        public abstract Color Color { get; }
        public abstract Texture2D Texture { get; }

        // Constructor
        public Entity(EntityGrid world, PriorityGroupedList<Entity> entities, int x, int y, bool addToWorld = true)
        {
            World = world;
            Entities = entities;

            X = x;
            Y = y;
            if (addToWorld)
            {
                AddToWorld();
            }
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

            if (World[X, Y] is not null) throw new Exception("Can not add an Entity onto another entity");

            World[X, Y] = this;
            MyNode = Entities.Add(this, GroupIndex);
        }

        public void Move(int newX, int newY)
        {
            if (!ExistsInWorld) throw new Exception("Cannot add an Entity that doesn't exist in the world");

            if (World[newX, newY] is not null) throw new Exception("Can not move an Entity onto another entity");

            World[X, Y] = null;
            X = newX;
            Y = newY;
            World[X, Y] = this;
        }

        public abstract void Update();
    }
}
