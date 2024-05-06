using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame_WaTor.DataStructures;
using MonoGame_WaTor.GameObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using static MonoGame_WaTor.GameObjects.Entity;

namespace MonoGame_WaTor
{
    public partial class WaTorGame : Game
    {
        private static readonly Random R = new();

        private GraphicsDeviceManager graphics;

        // 2D array to check if an entity exists at a specific 2D location
        public EntityGrid World { get; private set; }

        public WaTorGame()
        {
            IsFixedTimeStep = false;
            graphics = new GraphicsDeviceManager(this);
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000 / 20f); // Run game at 30 FPS
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            var (numEntitiesFitX, numEntitiesFitY) = CalculateScreenSizeAndEntityCount();
            World = new EntityGrid(numEntitiesFitX, numEntitiesFitY);

            // Set up multithreading stuff (see WaTorGame.Threading.cs)
            SetupMultiThreading();

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

                new Fish(this, x, y).AddToWorld();
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

                new Shark(this, x, y).AddToWorld();
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
                CurrentIsUpdatedReference.Value = false;
                CurrentIsUpdatedReference = new(true);

                for (int i = 0; i < NumIntervals; i++) UpdateResetEvents[i] = new ManualResetEvent(false);
                for (int i = 0; i < NumIntervals; i++) ThreadPool.QueueUserWorkItem(new WaitCallback(DoUpdateWork), i);
                WaitHandle.WaitAll(UpdateResetEvents);
            }

            base.Update(gameTime);
        }

        public BoolHolder CurrentIsUpdatedReference { get; set; } = new(true);

        private void DoUpdateWork(object state)
        {
            int intervalIndex = (int)state;
            WorkInterval myInterval = WorkIntervals[intervalIndex];
            Random myRandom = RandomGenerators[intervalIndex];
            ManualResetEvent myResetEvent = UpdateResetEvents[intervalIndex];

            List<Point2D> pointsToProcess = myInterval.PointsToProcess;
            //   Shuffle(pointsToProcess);
            foreach (Point2D point in pointsToProcess)
            {
                int x = point.X;
                int y = point.Y;

                if (XValuesLockMap.ContainsKey(x))
                {
                    lock (XValuesLockMap[x])
                    {
                        Process(x, y);
                    }
                }
                else
                {
                    Process(x, y);
                }
            }

            void Process(int x, int y)
            {
                if (World[x, y] is Entity e && (!e.IsUpdatedBoolHolder?.Value ?? true))
                {
                    e.IsUpdatedBoolHolder = CurrentIsUpdatedReference;
                    e.Update(myRandom);
                }
            }

            // Signal that the work is finished
            myResetEvent.Set();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            for (int i = 0; i < NumIntervals; i++) DrawResetEvents[i] = new ManualResetEvent(false);
            foreach (var batch in SpriteBatches) batch.Begin();
            for (int i = 0; i < NumIntervals; i++) ThreadPool.QueueUserWorkItem(new WaitCallback(DoDrawWork), i);
            WaitHandle.WaitAll(DrawResetEvents);
            foreach (var batch in SpriteBatches) batch.End();

            base.Draw(gameTime);
        }

        private void DoDrawWork(object state)
        {
            int intervalIndex = (int)state;
            SpriteBatch myBatch = SpriteBatches[intervalIndex];
            WorkInterval myInterval = WorkIntervals[intervalIndex];

            for (int x = myInterval.StartX; x <= myInterval.EndX; x++)
            {
                for (int y = 0; y < World.NumEntitiesFitY; y++)
                {
                    if (World[x, y] is Entity e)
                    {
                        e.Draw(myBatch);
                    }
                }
            }

            // Signal that the work is finished
            ManualResetEvent resetEvent = DrawResetEvents[intervalIndex];
            resetEvent.Set();
        }

        private void SetGameScreenSize(int w, int h)
        {
            graphics.PreferredBackBufferWidth = w;
            graphics.PreferredBackBufferHeight = h;
            graphics.ApplyChanges();
        }

        private (int, int) CalculateScreenSizeAndEntityCount()
        {
            float availableWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.85f;
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

    public class BoolHolder
    {
        public bool Value { get; set; }

        public BoolHolder(bool initialValue)
        {
            Value = initialValue;
        }
    }
}
