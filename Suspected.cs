namespace Parser{
    public class Suspected
    {
        public Suspected(int nodeId, double expires)
        {
            this.NodeId = nodeId;
            this.Expires = expires;
        }

        public int NodeId { get; set; }
        public double Expires { get; set; }
    }
}