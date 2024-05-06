using Microsoft.Xna.Framework.Graphics;
using MonoGame_WaTor.DataStructures;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MonoGame_WaTor
{
    public partial class WaTorGame
    {
        public class WorkInterval
        {
            public readonly int XSize;
            public readonly int StartX;
            public readonly int EndX;

            public readonly int YSize;
            public readonly int StartY;
            public readonly int EndY;

            public readonly int TotalSize;

            public List<Point2D> PointsToProcess { get; }

            public WorkInterval(int start, int end, EntityGrid world)
            {
                StartX = start;
                EndX = end;
                XSize = end - start + 1;

                StartY = 0;
                EndY = world.NumEntitiesFitY - 1;
                YSize = world.NumEntitiesFitY;
                TotalSize = XSize * YSize;

                PointsToProcess = new(TotalSize);
                PointsToProcess.AddRange(OrderedLocationsIterator());
                Shuffle(PointsToProcess);
            }

            public IEnumerable<Point2D> OrderedLocationsIterator()
            {
                for (int x = StartX; x <= EndX; x++)
                {
                    for (int y = 0; y <= EndY; y++)
                    {
                        yield return new Point2D(x, y);
                    }
                }
            }

            public override string ToString()
            {
                return $"Interval [{StartX}-{EndX}]";
            }
        }

        public const int NumIntervals = 20;
        public WorkInterval[] WorkIntervals { get; private set; }
        public Dictionary<int, LockObject> XValuesLockMap { get; private set; }
        public SpriteBatch[] SpriteBatches { get; private set; }
        public ManualResetEvent[] UpdateResetEvents { get; private set; }
        public ManualResetEvent[] DrawResetEvents { get; private set; }

        public Random[] RandomGenerators { get; set; }

        public class LockObject
        {
            public readonly int LockId;
            private static int lockIdAssign;

            public LockObject()
            {
                LockId = lockIdAssign++;
            }

            public override string ToString()
            {
                return $"LockObject Id={LockId}";
            }
        }

        public void SetupMultiThreading()
        {
            WorkIntervals = new WorkInterval[NumIntervals];

            // Calculate the number of intervals needed
            int intervalSizeRoundedDown = World.NumEntitiesFitX / NumIntervals;
            int remainder = World.NumEntitiesFitX % NumIntervals;

            int[] workPerInterval = new int[NumIntervals];

            if (intervalSizeRoundedDown < 10)
            {
                throw new Exception("No");
            }

            for (int i = 0; i < NumIntervals; i++) workPerInterval[i] = intervalSizeRoundedDown;
            for (int i = 0; i < remainder; i++) workPerInterval[i]++;

            // Create intervals
            int currStart = 0;
            for (int i = 0; i < NumIntervals; i++)
            {
                int start = currStart;
                int end = currStart + workPerInterval[i] - 1;  // - 1 reminder: if workPerInterval[0] is 2 then it would have to be indeces 0 to 1, not indeces 0 to 2
                WorkIntervals[i] = new WorkInterval(start, end, World);
                currStart = end + 1;
            }

            XValuesLockMap = new();
            // Loop thru each interval, and merge the end of interval i with the start of interval i+1 on the same lock obj
            for (int i = 0; i < NumIntervals; i++)
            {
                WorkInterval iInterval = WorkIntervals[i];
                WorkInterval jInterval = i == NumIntervals - 1 ? WorkIntervals[0] : WorkIntervals[i + 1];
                LockObject xLockRef = new();
                XValuesLockMap[iInterval.EndX - 1] = xLockRef;
                XValuesLockMap[iInterval.EndX] = xLockRef;
                XValuesLockMap[jInterval.StartX] = xLockRef;
                XValuesLockMap[jInterval.StartX + 1] = xLockRef;
            }

            RandomGenerators = new Random[NumIntervals];
            for (int i = 0; i < NumIntervals; i++) RandomGenerators[i] = new();

            SpriteBatches = new SpriteBatch[NumIntervals];
            for (int i = 0; i < NumIntervals; i++) SpriteBatches[i] = new(GraphicsDevice);

            DrawResetEvents = new ManualResetEvent[NumIntervals];
            UpdateResetEvents = new ManualResetEvent[NumIntervals];
        }

        static void Shuffle<T>(List<T> list)
        {
            Random rng = new();

            // Start from the last element and iterate backward
            for (int i = list.Count - 1; i > 0; i--)
            {
                // Generate a random index j between 0 and i (inclusive)
                int j = rng.Next(i + 1);

                // Swap elements at indices i and j
                (list[j], list[i]) = (list[i], list[j]);
            }
        }
    }
}
