namespace Parser{
    public class Point
    {
        public Point(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Point(double x)
        {
            this.X = x;
        }

        public double X { get; set; }
        public double Y{get;set;}
        public double Z{get;set;}

        public double CalculateDistance(Point destination){
            return Math.Sqrt(
                Math.Pow(X - destination.X, 2) + 
                Math.Pow(Y - destination.Y, 2) + 
                Math.Pow(Z - destination.Z, 2));
        }

        internal bool TooFar(Point? position)
        {
            return (Math.Abs(this.X - position.X) < Program.LidarDist)
            || (Math.Abs(this.Y - position.Y) < Program.LidarDist);
        }
    }
}