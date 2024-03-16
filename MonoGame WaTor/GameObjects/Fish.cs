using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_WaTor.DataStructures;

namespace MonoGame_WaTor.GameObjects
{
    public class Fish : Entity
    {
        public static byte FishGroupIndex => 1;
        public static Texture2D FishTexture { get; private set; }
        public static Color FishColor => Color.Lime;

        public override byte GroupIndex => FishGroupIndex;
        public override Color Color => FishColor;
        public override Texture2D Texture => FishTexture;

        public Fish(EntityGrid world, PriorityGroupedList<Entity> entities, short x, short y, bool addToWorld = true) : base(world, entities, x, y, addToWorld) { }

        public static void LoadStaticContent(GraphicsDevice graphics)
        {
            FishTexture = new Texture2D(graphics, width: 1, height: 1);
            FishTexture.SetData(new[] { FishColor });
        }

        internal static void UnloadStaticContent()
        {
            FishTexture.Dispose();
        }

        public override void Update()
        {
        }
    }
}
