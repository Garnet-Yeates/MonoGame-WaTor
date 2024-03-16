using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_WaTor.DataStructures;

namespace MonoGame_WaTor.GameObjects
{
    public class Shark : Entity
    {
        public static Texture2D SharkTexture { get; private set; }

        public static readonly Color SharkColor = Color.Orange;

        public override byte GroupIndex => 0;

        public Shark(Entity[,] world, GroupedList<Entity> entities, short x, short y) : base(world, entities, x, y) { }

        public static void LoadStaticContent(GraphicsDevice graphics)
        {
            SharkTexture = new Texture2D(graphics, width: 1, height: 1);
            SharkTexture.SetData(new[] { SharkColor });
        }

        internal static void UnloadStaticContent()
        {
            SharkTexture.Dispose();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(SharkTexture, new Rectangle(X * EntitySize, Y * EntitySize, EntitySize, EntitySize), SharkColor);
        }

        public override void Update()
        {
        }
    }
}
