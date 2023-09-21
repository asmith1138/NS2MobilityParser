namespace Parser{
    public class NodeDistance{
        public NodeDistance(int start, int dest, double dist){
            DistanceToNode = dist;
            StartNodeId = start;
            DestinationNodeId = dest;
        }
        public double DistanceToNode{get;set;}
        public int StartNodeId{get;set;}
        public int DestinationNodeId{get;set;}

        public static NodeDistance CalculateDistance(Node start, Node destination){
            return new NodeDistance(start.NodeId,
                destination.NodeId,
                start.Position.CalculateDistance(destination.Position));
        }
    }
}