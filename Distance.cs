namespace Parser{
    public class Distance{
        public decimal DistanceToNode{get;set;}
        public int StartNodeId{get;set;}
        public int DestinationNodeId{get;set;}

        public static decimal CalculateDistance(Node start, Node destination){
            throw NotImplementedException();
            return 0;
        }
    }
}