namespace Parser
{
    using System;
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines(args[0]);
            List<Node> nodes = new List<Node>();
            foreach (var line in lines){
                var ln = line.Replace("\"","").Replace("("," ").Replace(")","").Split(' ');
                if(line.StartsWith("$node_")){
                    //new node 3 lines
                    switch(line)
                    {
                        case string s when s.Contains("X_"):
                            //add node and set x
                            nodes.Add(Node.GetNode(ln, nodes));
                            break;
                        case string s when s.Contains("Y_"):
                        case string s2 when s2.Contains("Z_"):
                            //find node and set y
                            //find node and set z
                            _ = Node.GetNode(ln, nodes);

                            break;

                    }
                }else{
                    //setdest node
                    nodes.Add(Node.GetNode(ln, nodes));
                }
            }

            Console.WriteLine("We did it!!");
        }
        
    }

    
}