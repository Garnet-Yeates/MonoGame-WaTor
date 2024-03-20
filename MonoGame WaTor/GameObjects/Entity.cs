using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_WaTor.DataStructures;

namespace MonoGame_WaTor.GameObjects
{
    public abstract class Entity
    {
        // Size in pixels of how big Entities are. Entities are represented as colored squares
        public const int EntitySize = 3;

        /// <summary>
        /// A reference to the Game instance. This reference is used by other properties here to get
        /// references such as Game.World and Game.EntityList
        /// </summary>
        public WaTorGame Game { get; }

        /// <summary>
        /// A reference to the EntityGrid or "world" that we use to encapsulated self-movement
        /// </summary>
        public EntityGrid World => Game.World;

        /// <summary>
        /// My X position in the EntityGrid
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// My Y position in the EntityGrid
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// What priority am I in the PriorityGroupedList? Lower numbers are evaluated first. Subclasses
        /// should implement this to determine their order of updating. For example Sharks have 0 and fish have 1
        /// which means that every frame in the update loop, the Sharks perform their updates first
        /// </summary>
        public abstract byte GroupIndex { get; }

        /// <summary>
        /// A reference to my GroupedListNode in the PriorityGroupedList. Using this node reference in tandem with 
        /// a reference to the list itself allows for efficient O(1) self-removal of this entity from the update list.
        /// </summary>
        public GroupedListNode<Entity> MyNode { get; private set; }

        /// <summary>
        /// A reference to the entity update list, which is iterated through every frame to call Update() on all entities.
        /// When this Entity is added to the world it is added to this list and gains a reference to its node. When an entity
        /// is removed from the world it is removed from this list via the same node reference.
        /// </summary>
        public PriorityGroupedList<Entity> Entities => Game.Entities;

        /// <summary>
        /// The Color of this entity. This should usually match the color used in the data array for the texture of
        /// this entity. Child classes must implement this so this parent class knows what color to draw the entity as.
        /// </summary>
        public abstract Color Color { get; }

        /// <summary>
        /// The texture for this entity. Child classes must implement this so we can draw the entity up here in the
        /// parent class. The texture should be computed statically and the property here should return its reference
        /// so there is as little overhead as possible.
        /// </summary>
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
            World[X, Y] = null;
            Entities.Remove(MyNode);
        }

        public void AddToWorld(bool updateOnCurrentUpdate = false)
        {
            World[X, Y] = this;
            MyNode = updateOnCurrentUpdate ? Entities.AddLast(this, GroupIndex) : Entities.AddFirst(this, GroupIndex);
        }

        public void Move(int newX, int newY)
        {
            World[X, Y] = null;
            X = newX;
            Y = newY;
            World[X, Y] = this;
        }

        public void Move(Point2D p)
        {
            Move(p.X, p.Y);
        }

        public abstract void Update();
    }
}
