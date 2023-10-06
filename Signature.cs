namespace Parser{
    public class Signature
    {
        public Signature(int nodeId, double expires)
        {
            this.NodeId = nodeId;
            this.Expires = expires;
        }

        public int NodeId { get; set; }
        public double Expires { get; set; }
    }
}