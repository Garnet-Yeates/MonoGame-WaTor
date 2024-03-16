using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_WaTor.DataStructures;

namespace MonoGame_WaTor.GameObjects
{
    public class Shark : Entity
    {
        public static byte SharkGroupIndex => 0;
        public static Texture2D SharkTexture { get; private set; }

        public static readonly Color SharkColor = new(191, 89, 219);

        public override byte GroupIndex => SharkGroupIndex;
        public override Color Color => SharkColor;
        public override Texture2D Texture => SharkTexture;

        public Shark(EntityGrid world, PriorityGroupedList<Entity> entities, short x, short y, bool addToWorld = true) : base(world, entities, x, y, addToWorld) { }

        public static void LoadStaticContent(GraphicsDevice graphics)
        {
            SharkTexture = new Texture2D(graphics, width: 1, height: 1);
            SharkTexture.SetData(new[] { SharkColor });
        }

        internal static void UnloadStaticContent()
        {
            SharkTexture.Dispose();
        }

        public override void Update()
        {
            Move((short)(X + 1), Y);
        }
    }
}
