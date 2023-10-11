namespace Parser{
    public class Blacklisted
    {
        public Blacklisted(int nodeId, List<int> signedNodes)
        {
            this.NodeId = nodeId;
            this.SignedNodes = signedNodes;
        }

        public int NodeId { get; set; }
        public List<int> SignedNodes { get; set; }
    }
}