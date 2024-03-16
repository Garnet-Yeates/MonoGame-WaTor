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
        public static Random R = new();

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // 2D array to check if an entity exists at a specific 2D location
        public EntityGrid World { get; private set; }

        // List to keep track of all existing entities in the world, grouped by priority
        // Sharks will have priority 0 meaning they are processed first by the enumerator
        public PriorityGroupedList<Entity> Entities { get; private set; }

        public WaTorGame()
        {
            graphics = new GraphicsDeviceManager(this);
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000 / 2f); // Run game at 5fps (200ms intervals)
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            var (numEntitiesFitX, numEntitiesFitY) = CalculateScreenSizeAndEntityCount();

            // Every index of world is currently null
            World = new EntityGrid(numEntitiesFitX, numEntitiesFitY);
            Entities = new();

            Fish f = new(World, Entities, 15, 15);
            Shark s = new(World, Entities, 30, 15);

            base.Initialize();
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

        int updateCt = 0;
        protected override void Update(GameTime gameTime)
        {
            Debug.WriteLine($"Is update running behind? {gameTime.IsRunningSlowly} {++updateCt}");

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (var entity in Entities)
            {
                entity.Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Debug.WriteLine($"Is draw running behind? {gameTime.IsRunningSlowly}");

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
    }
}
