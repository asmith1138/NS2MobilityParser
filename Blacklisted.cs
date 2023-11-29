namespace Parser{
    public class Blacklisted
    {
        public Blacklisted(int nodeId, List<int> signedNodes, double time, double distance)
        {
            this.NodeId = nodeId;
            this.SignedNodes = signedNodes;
            this.TimeBlacklisted = time;
            this.DistanceAtBlacklist = distance;
        }

        public int NodeId { get; set; }
        public List<int> SignedNodes { get; set; }
        public double TimeBlacklisted { get; set; }
        public double DistanceAtBlacklist { get; set; }
    }
}