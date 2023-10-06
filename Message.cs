namespace Parser{
    public class Message
    {
        public Message(int fromNodeId, int toNodeId, MessageType type)
        {
            this.ToNodeId = toNodeId;
            this.FromNodeId = fromNodeId;
            this.Type = type;
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
        }

        public int FromNodeId { get; set; }
        public int ToNodeId { get; set; }
        public int AccusedNode { get; set; }
        public MessageType Type { get; set; }
    }
    public enum MessageType
    {
        Signature,
        Normal,
        Accusation
    }
}