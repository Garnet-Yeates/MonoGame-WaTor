using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame_WaTor.DataStructures;
using MonoGame_WaTor.GameObjects;
using System;
using System.Diagnostics;
using static MonoGame_WaTor.GameObjects.Entity;

namespace MonoGame_WaTor
{
    public class WaTorGame : Game
    {
        public static readonly Random R = new();

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // 2D array to check if an entity exists at a specific 2D location
        public EntityGrid World { get; private set; }

        // List to keep track of all existing entities in the world, grouped by priority
        // Sharks will have priority 0 meaning they are processed first by the enumerator
        public PriorityGroupedList<Entity> Entities { get; private set; }

        public WaTorGame()
        {
            IsFixedTimeStep = true;
            graphics = new GraphicsDeviceManager(this);
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000 / 30f); // Run game at 30 FPS
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            var (numEntitiesFitX, numEntitiesFitY) = CalculateScreenSizeAndEntityCount();

            World = new EntityGrid(numEntitiesFitX, numEntitiesFitY);
            Entities = new();

            Debug.WriteLine($"Max Entities: {World.TotalEntitiesThatCanFit}");
            AddRandomEntities();

            base.Initialize();
        }

        private void AddRandomEntities()
        {
            double percFishToAdd = 0.09;
            int numFishToAdd = (int)(percFishToAdd * World.TotalEntitiesThatCanFit);
            int fishAdded = 0;

            double percSharksToAdd = 0.01;
            int numSharksToAdd = (int)(percSharksToAdd * World.TotalEntitiesThatCanFit);
            int sharksAdded = 0;

            while (fishAdded < numFishToAdd)
            {
                int x = R.Next(World.NumEntitiesFitX);
                int y = R.Next(World.NumEntitiesFitY);

                if (World[x, y] is not null)
                {
                    continue;
                }

                new Fish(this, x, y).AddToWorld(updateOnCurrentUpdate: false);
                fishAdded++;
            }

            while (sharksAdded < numSharksToAdd)
            {
                int x = R.Next(World.NumEntitiesFitX);
                int y = R.Next(World.NumEntitiesFitY);

                if (World[x, y] is not null)
                {
                    continue;
                }

                new Shark(this, x, y).AddToWorld(updateOnCurrentUpdate: false);
                sharksAdded++;
            }
        }

        private bool isRunningEntityUpdates = false;

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.P))
                isRunningEntityUpdates = !isRunningEntityUpdates;

            if (isRunningEntityUpdates)
            {
                foreach (var entity in Entities)
                {
                    entity.Update();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            foreach (var entity in Entities)
            {
                entity.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SetGameScreenSize(int w, int h)
        {
            graphics.PreferredBackBufferWidth = w;
            graphics.PreferredBackBufferHeight = h;
            graphics.ApplyChanges();
        }

        private (int, int) CalculateScreenSizeAndEntityCount()
        {
            float availableWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.9f;
            float availableHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.9f;

            // Find out how many entities can fit within available width and height, rounding down to prevent overfitting
            int numEntitiesFitX = (int)(availableWidth / EntitySize);
            int numEntitiesFitY = (int)(availableHeight / EntitySize);

            // Will always be exactly the same as availaleW/H or a bit smaller
            // Multiply the number of entities we are allowed to fit (rounded down) by their size to figure out the exact screen width and height needed
            int calculatedWidth = numEntitiesFitX * EntitySize;
            int calculatedHeight = numEntitiesFitY * EntitySize;

            SetGameScreenSize(calculatedWidth, calculatedHeight);

            return (numEntitiesFitX, numEntitiesFitY);
        }

        // Load our textures and initialize SpriteBatch
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Fish.LoadStaticContent(GraphicsDevice);
            Shark.LoadStaticContent(GraphicsDevice);
        }

        // Unload our textures
        protected override void UnloadContent()
        {
            Fish.UnloadStaticContent();
            Shark.UnloadStaticContent();
        }
    }
}
