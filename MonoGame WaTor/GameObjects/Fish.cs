using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_WaTor.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame_WaTor.GameObjects
{
    public class Fish : Entity
    {
        public static int BaseBreedTime { get; set; } = 20;
        private int breedtime;

        public static Texture2D FishTexture { get; private set; }
        public override Texture2D Texture => FishTexture;

        public static readonly Color FishColor = Color.Lime;
        public override Color Color => FishColor;

        public Fish(WaTorGame game, int x, int y) : base(game, x, y)
        {
            breedtime = BaseBreedTime;
        }

        public override void Update(Random r)
        {
            breedtime--;
            bool reproducing = false;
            if (breedtime == 0)
            {
                reproducing = true;
                breedtime = BaseBreedTime;
            }

            List<Point2D> nearby = World.GetAdjacentEmptyLocations(X, Y).ToList();
            if (nearby.Any())
            {
                Point2D movingTo = nearby[r.Next(nearby.Count)];
                if (reproducing)
                {
                    Fish child = new(Game, movingTo.X, movingTo.Y);
                    child.AddToWorld();
                    child.breedtime++; // give the child 1 extra tick than normal before it breeds to create a 'desync waterfall' of breed times
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
