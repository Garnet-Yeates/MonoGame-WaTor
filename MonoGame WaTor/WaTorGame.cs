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
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // How many entities can we fit horizontally within our working area?
        public short NumEntitiesX { get; private set; }

        // How many entities can we fit vertically within our working area?
        public short NumEntitiesY { get; private set; }

        // 2D array to check if an entity exists at a specific 2D location
        public Entity[,] World { get; private set; }

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
            CalculateScreenSizeAndEntityCount();

            // Every index of world is currently null
            World = new Entity[NumEntitiesX, NumEntitiesY];
            Entities = new();

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

        // 5 fps
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            int width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Debug.WriteLine($"Updated! width={width} height={height}");

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

        private void CalculateScreenSizeAndEntityCount()
        {
            float availableWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.9f;
            float availableHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.9f;

            // Find out how many entities can fit within available width and height, rounding down to prevent overfitting
            NumEntitiesX = (short)(availableWidth / EntitySize);
            NumEntitiesY = (short)(availableHeight / EntitySize);

            // Will always be exactly the same as availaleW/H or a bit smaller
            // Multiply the number of entities we are allowed to fit (rounded down) by their size to figure out the exact screen width and height needed
            int calculatedWidth = NumEntitiesX * EntitySize;
            int calculatedHeight = NumEntitiesY * EntitySize;

            SetGameScreenSize(calculatedWidth, calculatedHeight);
        }
    }
}
