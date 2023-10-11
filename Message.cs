namespace Parser{
    public class Message
    {
        public Message(int fromNodeId, int toNodeId, MessageType type)
        {
            this.ToNodeId = toNodeId;
            this.FromNodeId = fromNodeId;
            this.Type = type;
        }

        public Message(int fromNodeId, int toNodeId, MessageType type, List<int> SignedBy)
        {
            this.ToNodeId = toNodeId;
            this.FromNodeId = fromNodeId;
            this.Type = type;
            this.NodesSigned = SignedBy;
        }

        public Message(int fromNodeId, int toNodeId)
        {
            this.ToNodeId = toNodeId;
            this.FromNodeId = fromNodeId;
            this.Type = MessageType.Signature;
        }

        public Message(int fromNodeId, int toNodeId, int accusedNodeId)
        {
            this.ToNodeId = toNodeId;
            this.FromNodeId = fromNodeId;
            this.AccusedNode = accusedNodeId;
            this.Type = MessageType.Accusation;
            this.NodesSigned = new List<int>();
        }

        public Message(int fromNodeId, int toNodeId, int accusedNodeId, List<int> SignedBy)
        {
            this.ToNodeId = toNodeId;
            this.FromNodeId = fromNodeId;
            this.AccusedNode = accusedNodeId;
            this.Type = MessageType.Accusation;
            this.NodesSigned = SignedBy;
        }

        public int FromNodeId { get; set; }
        public int ToNodeId { get; set; }
        public int AccusedNode { get; set; }
        public MessageType Type { get; set; }
        public List<int> NodesSigned { get; set; }
    }
    public enum MessageType
    {
        Signature,
        Normal,
        Accusation
    }
}