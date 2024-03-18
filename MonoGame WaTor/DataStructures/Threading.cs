namespace MonoGame_WaTor.DataStructures
{
    public class WorkInterval
    {
        public int Start { get; }
        public int End { get; }

        public WorkInterval(int start, int end)
        {
            Start = start;
            End = end;
        }
    }

    public class Threading
    {
        public static WorkInterval[] DivideWorkIntoIntervals(int totalWork, int numIntervals)
        {
            WorkInterval[] intervals = new WorkInterval[numIntervals];

            // Calculate the number of intervals needed
            int intervalSizeRoundedDown = totalWork / numIntervals;
            int remainder = totalWork % numIntervals;

            int[] workPerInterval = new int[numIntervals];

            for (int i = 0; i < numIntervals; i++) workPerInterval[i] = intervalSizeRoundedDown;
            for (int i = 0; i < remainder; i++) workPerInterval[i]++;

            // Create intervals
            int currStart = 0;
            for (int i = 0; i < numIntervals; i++)
            {
                int start = currStart;
                int end = currStart + workPerInterval[i] - 1;  // - 1 reminder: if workPerInterval[0] is 2 then it would have to be indeces 0 to 1, not indeces 0 to 2
                intervals[i] = (new WorkInterval(start, end));
                currStart = end + 1;
            }

            return intervals;
        }
    }
}
