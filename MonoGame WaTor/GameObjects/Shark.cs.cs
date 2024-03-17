using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame_WaTor.DataStructures;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame_WaTor.GameObjects
{
    public class Shark : Entity
    {
        public static int BaseFishTillBreed { get; set; }
        private int fishTillBreed;

        public static int EnergyGainedFromEatingFish { get; set; }
        public static int BaseEnergy { get; set; }
        private int energy;

        public static byte SharkGroupIndex => 0;
        public static Texture2D SharkTexture { get; private set; }

        public static readonly Color SharkColor = new(191, 89, 219);

        public override byte GroupIndex => SharkGroupIndex;
        public override Color Color => SharkColor;
        public override Texture2D Texture => SharkTexture;

        public Shark(WaTorGame game, int x, int y) : base(game, x, y)
        {
            energy = BaseEnergy;
            fishTillBreed = BaseFishTillBreed;
        }

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
            energy--;

            List<Point2D> fishSquares = new(), emptySquares = new();
            foreach (var p in World.GetAdjacentLocations(X, Y))
            {
                if (World[p] is null)
                {
                    emptySquares.Add(p);
                }
                else if (World[p] is Fish)
                {
                    fishSquares.Add(p);
                }
            }

            if (fishSquares.Any())
            {
                Point2D randomFishSquare = fishSquares[WaTorGame.R.Next(fishSquares.Count)];
                Entity fishHere = World[randomFishSquare];
                fishHere.RemoveFromWorld();

                energy += EnergyGainedFromEatingFish;
                fishTillBreed--;

                if (fishTillBreed == 0)
                {
                    Fish child = new(Game, randomFishSquare.X, randomFishSquare.Y);
                    child.AddToWorld(updateOnCurrentUpdate: false);
                }
                else
                {
                    Move(randomFishSquare);
                }
                // Energy inc, possibly breed
            }
            else if (emptySquares.Any())
            {
                Move(emptySquares[WaTorGame.R.Next(fishSquares.Count)]);
            }

            if (energy == 0)
            {
                RemoveFromWorld();
            }
        }
    }
}
