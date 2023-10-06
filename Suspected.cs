namespace Parser{
    public class Suspected
    {
        public Suspected(int nodeId, double expires, int suspectedByNodeId)
        {
            this.NodeId = nodeId;
            this.Expires = expires;
            this.SuspectedByNodeId = suspectedByNodeId;
        }

        public int NodeId { get; set; }
        public int SuspectedByNodeId { get; set; }
        public double Expires { get; set; }
    }
}