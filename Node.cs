namespace Parser
{
    public class Node
    {
        public int NodeId { get; set; }
        public decimal Time { get; set; }
        public Point? Position{get;set;}
        public decimal XLocation { get; set; }
        public decimal YLocation { get; set; }
        public decimal ZLocation { get; set; }
        public bool Start { get; set; }

        public static Node GetNode(string[] line, List<Node> nodes) =>
                        line switch
                        {
                            [_, _, var time, _, var id, _, var x, var y, var z, ..] => new Node { NodeId = int.Parse(id), Time = decimal.Parse(time), XLocation = decimal.Parse(x), YLocation = decimal.Parse(y), ZLocation = decimal.Parse(z), Start = false },
                            [_, var id, _, "X_", var x] => new Node { NodeId = int.Parse(id), Time = -1, XLocation = decimal.Parse(x) },
                            [_, var id, _, "Y_", var y] => nodes.Where(n => n.NodeId == int.Parse(id) && n.Time == -1).Select(n => { n.YLocation = decimal.Parse(y); return n; }).First(),
                            [_, var id, _, "Z_", var z] => nodes.Where(n => n.NodeId == int.Parse(id) && n.Time == -1).Select(n => { n.ZLocation = decimal.Parse(z); return n; }).First(),
                            _ => throw new NotImplementedException(),
                        };
    }
}

