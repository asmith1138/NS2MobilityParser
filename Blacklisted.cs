namespace Parser{
    public class Blacklisted
    {
        public Blacklisted(int nodeId, List<int> signedNodes, double time)
        {
            this.NodeId = nodeId;
            this.SignedNodes = signedNodes;
            this.TimeBlacklisted = time;
        }

        public int NodeId { get; set; }
        public List<int> SignedNodes { get; set; }
        public double TimeBlacklisted { get; set; }
    }
}