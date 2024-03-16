using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGame_WaTor.GameObjects
{
    public class Fish : Entity
    {
        public static Texture2D FishTexture { get; private set; }

        public static readonly Color FishColor = Color.Lime;

        public Fish(Entity[,] world, short x, short y) : base(world, x, y)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(FishTexture, new Rectangle(X * EntitySize, Y * EntitySize, EntitySize, EntitySize), FishColor);
        }

        public static void LoadStaticContent(GraphicsDevice graphics)
        {
            FishTexture = new Texture2D(graphics, width: 1, height: 1);
            FishTexture.SetData(new[] { FishColor });
        }

        internal static void UnloadStaticContent()
        {
            FishTexture.Dispose();
        }
    }
}
