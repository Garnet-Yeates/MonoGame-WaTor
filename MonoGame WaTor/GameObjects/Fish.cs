using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_WaTor.DataStructures;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame_WaTor.GameObjects
{
    public class Fish : Entity
    {
        public static int BaseBreedTime { get; set; } = 30;
        private int breedTime;

        public override byte GroupIndex => 1;

        public static Texture2D FishTexture { get; private set; }
        public override Texture2D Texture => FishTexture;

        public static readonly Color FishColor = Color.Lime;
        public override Color Color => FishColor;

        public Fish(WaTorGame game, int x, int y) : base(game, x, y)
        {
            breedTime = BaseBreedTime;
        }

        public override void Update()
        {
            List<Point2D> nearbyEmpty = World.GetAdjacentEmptyLocations(X, Y).ToList();
            if (nearbyEmpty.Any())
            {
                Point2D movingTo = nearbyEmpty[WaTorGame.R.Next(nearbyEmpty.Count)];
                if (--breedTime == 0)
                {
                    breedTime = BaseBreedTime;
                    Fish child = new(Game, movingTo.X, movingTo.Y);
                    child.AddToWorld(updateOnCurrentUpdate: false);
                    child.breedTime++; // give the child 1 extra tick than normal before it breeds to create a 'desync waterfall' of breed times
                }
                else
                {
                    Move(movingTo.X, movingTo.Y);
                }
            }
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
