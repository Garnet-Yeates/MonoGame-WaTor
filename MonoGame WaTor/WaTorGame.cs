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

        public const int NumIntervals = 4;
        public WorkInterval[] WorkIntervals { get; private set; }

        public class LockObject { }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var (numEntitiesFitX, numEntitiesFitY) = CalculateScreenSizeAndEntityCount();

            World = new EntityGrid(numEntitiesFitX, numEntitiesFitY);
            Entities = new();

            Debug.WriteLine($"Max Entities: {World.TotalEntitiesThatCanFit}");
            AddRandomEntities();

            // Set up multithreading stuff
            UnsafeXValues = new();
            WorkIntervals = Threading.DivideWorkIntoIntervals(World.NumEntitiesFitX, NumIntervals);
            spriteBatches = new SpriteBatch[NumIntervals];
            drawResetEvents = new ManualResetEvent[NumIntervals];
            updateResetEvents = new ManualResetEvent[NumIntervals];

            foreach (var workInterval in WorkIntervals)
            {
                UnsafeXValues.Add(workInterval.Start, new());
                UnsafeXValues.Add(workInterval.End, new());
            }
            for (int i = 0; i < spriteBatches.Length; i++)
            {
                spriteBatches[i] = new(GraphicsDevice);
            }

            base.Initialize();
        }

        private void AddRandomEntities()
        {
            double percFishToAdd = 0.9;
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
                if (false)
                {
                    UpdateMT();
                }
                else
                {
                    UpdateST();
                }
            }

            base.Update(gameTime);
        }

        private void UpdateST()
        {
            foreach (var entity in Entities)
            {
                entity.Update();
            }
        }

        // What x values do we need to use locked context for when doing the game update loop?
        // Keep in mind that for painting, we do not need locked context ever
        public Dictionary<int, LockObject> UnsafeXValues { get; private set; }

        private ManualResetEvent[] updateResetEvents;


        private void UpdateMT()
        {
            // Re allocate the draw reset events
            for (int i = 0; i < NumIntervals; i++)
            {
                updateResetEvents[i] = new ManualResetEvent(false);
            }

            // Call DoDrawWork from multiple threads
            for (int i = 0; i < NumIntervals; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(DoDrawWork), i);
            }

            // Wait for all work to finish
            WaitHandle.WaitAll(drawResetEvents);
        }

        private void DoUpdateWork(object state)
        {
            int intervalIndex = (int)state;
            WorkInterval myInterval = WorkIntervals[intervalIndex];

            for (int x = myInterval.Start; x <= myInterval.End; x++)
            {
                for (int y = 0; y < World.NumEntitiesFitY; y++)
                {
                    if (World[x, y] is Entity e)
                    {
                        if (UnsafeXValues.ContainsKey(x))
                        {
                            lock (UnsafeXValues[x])
                            {
                                e.Update();
                            }
                        }
                        else
                        {
                            e.Update();
                        }
                    }
                }
            }

            // Signal that the work is finished
            ManualResetEvent resetEvent = updateResetEvents[intervalIndex];
            resetEvent.Set();
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            if (gameTime.IsRunningSlowly)
            {
                Debug.WriteLine("Running slow in draw");
            }

            if (false)
            {
                DrawMT();
            }
            else
            {
                DrawST();
            }

            base.Draw(gameTime);
        }

        private void DrawST()
        {
            spriteBatch.Begin();

            foreach (var entity in Entities)
            {
                entity.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        private ManualResetEvent[] drawResetEvents;
        private SpriteBatch[] spriteBatches;

        protected void DrawMT()
        {
            // Re allocate the draw reset events
            for (int i = 0; i < NumIntervals; i++)
            {
                drawResetEvents[i] = new ManualResetEvent(false);
            }

            // Begin all batches
            foreach (var batch in spriteBatches)
            {
                batch.Begin();
            }

            // Call DoDrawWork from multiple threads
            for (int i = 0; i < NumIntervals; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(DoDrawWork), i);
            }

            // Wait for all work to finish
            WaitHandle.WaitAll(drawResetEvents);

            // End all batches
            foreach (var batch in spriteBatches)
            {
                batch.End();
            }
        }

        private void DoDrawWork(object state)
        {
            int intervalIndex = (int)state;
            SpriteBatch myBatch = spriteBatches[intervalIndex];
            WorkInterval myInterval = WorkIntervals[intervalIndex];

            for (int x = myInterval.Start; x <= myInterval.End; x++)
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
            ManualResetEvent resetEvent = drawResetEvents[intervalIndex];
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
}
