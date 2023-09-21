namespace Parser
{
    public class Node
    {
        public int NodeId { get; set; }
        public double Time { get; set; }
        public Point? Position{get;set;}
        public bool Start { get; set; }
        public List<NodeDistance> Distances { get; set; }

        public static Node GetNode(string[] line, List<Node> nodes) =>
                        line switch
                        {
                            [_, _, var time, _, var id, _, var x, var y, var z, ..] => new Node { NodeId = int.Parse(id), Time = double.Parse(time), Position = new Point(double.Parse(x), double.Parse(y), double.Parse(z)), Start = false },
                            [_, var id, _, "X_", var x] => new Node { NodeId = int.Parse(id), Time = -1, Position = new Point(double.Parse(x)) },
                            [_, var id, _, "Y_", var y] => nodes.Where(n => n.NodeId == int.Parse(id) && n.Time == -1).Select(n => { n.Position.Y = double.Parse(y); return n; }).First(),
                            [_, var id, _, "Z_", var z] => nodes.Where(n => n.NodeId == int.Parse(id) && n.Time == -1).Select(n => { n.Position.Z = double.Parse(z); return n; }).First(),
                            _ => throw new NotImplementedException(),
                        };
        public void SetDistances(List<Node> nodes){
            Distances = new List<NodeDistance>();
            foreach (var node in 
                nodes.Where(n => n.Time == this.Time && n.NodeId != this.NodeId && n.Time != -1))
            {
                Distances.Add(NodeDistance.CalculateDistance(this, node));
            }
        }
    }
}

