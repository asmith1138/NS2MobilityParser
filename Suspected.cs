namespace Parser{
    public class Suspected
    {
        public Suspected(int nodeId, double expires, int suspectedByNodeId, List<int> signedNodes)
        {
            this.NodeId = nodeId;
            this.Expires = expires;
            this.SuspectedByNodeId = suspectedByNodeId;
            this.SignedNodes = signedNodes;
        }

        public int NodeId { get; set; }
        public int SuspectedByNodeId { get; set; }
        public List<int> SignedNodes { get; set; }
        public double Expires { get; set; }
    }
}