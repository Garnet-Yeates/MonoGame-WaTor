using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_WaTor.DataStructures;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame_WaTor.GameObjects
{
    public class Fish : Entity
    {
        public static int FishBreedTime = 5;
        public int BreedTime;

        public static byte FishGroupIndex => 1;
        public override byte GroupIndex => FishGroupIndex;

        public static Texture2D FishTexture { get; private set; }
        public override Texture2D Texture => FishTexture;

        public static readonly Color FishColor = new(228, 218, 19);
        public override Color Color => FishColor;

        public Fish(WaTorGame game, int x, int y) : base(game, x, y)
        {
            BreedTime = FishBreedTime;
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

        public override void Update()
        {
            BreedTime--;
            bool reproducing = false;
            if (BreedTime == 0)
            {
                reproducing = true;
                BreedTime = FishBreedTime;
            }

            List<Point2D> nearby = World.GetAdjacentEmptyLocations(X, Y).ToList();
            if (nearby.Any())
            {
                Point2D movingTo = nearby[WaTorGame.R.Next(nearby.Count)];
                if (reproducing)
                {
                    Fish child = new Fish(Game, movingTo.X, movingTo.Y);
                    child.AddToWorld(updateOnCurrentUpdate: false);
                    child.BreedTime++; // give the child 1 extra tick than normal before it breeds to create a 'desync waterfall' of breed times
                }
                else
                {
                    Move(movingTo.X, movingTo.Y);
                }
            }
        }
    }
}
