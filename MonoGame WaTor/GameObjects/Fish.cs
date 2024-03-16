using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_WaTor.DataStructures;

namespace MonoGame_WaTor.GameObjects
{
    public class Fish : Entity
    {
        public static Texture2D FishTexture { get; private set; }

        public static readonly Color FishColor = Color.Lime;

        public override byte GroupIndex => 1;

        public Fish(Entity[,] world, GroupedList<Entity> entities, short x, short y) : base(world, entities, x, y) { }

        public static void LoadStaticContent(GraphicsDevice graphics)
        {
            FishTexture = new Texture2D(graphics, width: 1, height: 1);
            FishTexture.SetData(new[] { FishColor });
        }

        internal static void UnloadStaticContent()
        {
            FishTexture.Dispose();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(FishTexture, new Rectangle(X * EntitySize, Y * EntitySize, EntitySize, EntitySize), FishColor);
        }

        public override void Update()
        {
        }
    }
}
