namespace MonoGame_WaTor.DataStructures
{

    // Used to be readonly struct but I feel like structs create unneccessary overhead compared to readonly
    // reference types
    public class Point2D
    {
        public readonly int X;
        public readonly int Y;

        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
