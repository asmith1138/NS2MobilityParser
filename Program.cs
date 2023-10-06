namespace Parser
{
    using System;
    public class Program
    {
        public static int LidarDist = 500;
        public static int BroadcastRange = 500;
        public static double SignatureLife = 5;
        public static double AccusationLife = 5;
        public static List<NodeDistance> distances = new List<NodeDistance>();
        public static List<Node> nodes = new List<Node>();

        static void Main(string[] args)
        {
            ParseFile(args);

            CalculateDistances();

            Console.WriteLine("We did it, we loaded the whole simulation into memory!!");

            var sybilNodes = SybilSeedFactory.CreateSybilSeedData();
            double maxTime = nodes.MaxBy(n => n.Time).Time;
            double Previous = -1;
            for (double t = 0.0; t <= maxTime; t += 0.1)
            {
                List<Message> messages = new List<Message>();
                /// At each timestamp foreach node do the following:
                foreach (var node in nodes.Where(n => n.Time == t))
                {
                    /// 1. Figure out which vehicles you can see
                    node.NodesInSight = distances.Where(d => d.Time == t
                    && d.DestinationNodeId == node.NodeId
                    && d.DistanceToNode < LidarDist)
                    .Select(d => d.StartNodeId)
                    .Union(
                        distances.Where(d => d.Time == t
                        && d.StartNodeId == node.NodeId
                        && d.DistanceToNode < LidarDist)
                        .Select(d => d.DestinationNodeId).ToList()
                    ).ToList();
                    /// 2. Trade signatures with each vehicle you can physically see
                    messages.AddRange(node.NodesInSight.Select(nis => 
                        new Message(node.NodeId, nis)
                    ));

                    /// 3. If you know of any Sybil (or other malicious) nodes:
                    /// 3a. Broadcast a msg to all your neighbors about the node
                    /// TODO: Add sending messages less often
                    if (sybilNodes.Any(sn => sn.KnowingNodeId == node.NodeId))
                    {
                        messages.AddRange(node.NodesInSight.SelectMany(nis =>
                            sybilNodes.Where(sn => sn.KnowingNodeId == node.NodeId).Select(sn =>
                                new Message(node.NodeId, nis, sn.SybilNodeId)
                        )));
                    }
                    /// 5b. Broadcast a msg to all your neighbors about that node
                    if (node.Blacklisted.Any())
                    {
                        messages.AddRange(node.NodesInSight.SelectMany(nis =>
                            node.Blacklisted.Select(bl =>
                                new Message(node.NodeId, nis, bl)
                        )));
                    }
                }
                
                foreach(var msg in messages)
                {
                    if(msg.Type == MessageType.Signature)
                    {
                        /// 2b. Save signatures for 50 timestamps
                        var node = nodes.SingleOrDefault(n => n.NodeId == msg.ToNodeId && n.Time == t);
                        
                        node.Signatures.AddRange(nodes.SingleOrDefault(
                            n => n.NodeId == msg.ToNodeId && n.Time == Previous).Signatures);
                        node.Signatures.RemoveAll(s => s.Expires < t);
                        
                        if (node.Signatures.Any(s => s.NodeId == msg.FromNodeId))
                        {
                            node.Signatures.SingleOrDefault(s => s.NodeId == msg.FromNodeId).Expires = t + SignatureLife;
                        }
                        else
                        {
                            node.Signatures.Add(new Signature(msg.FromNodeId, t + SignatureLife));
                        }
                    }
                    else if(msg.Type == MessageType.Accusation)
                    {
                        /// 4. If you recieve any msgs about malicious nodes:
                        /// 4a. Verify if you can see the vehicle that sent the msg
                        /// 4b. Verify the msg is signed by a node you know
                        /// 4c. Keep the msg for 50 timestamp values
                        var node = nodes.SingleOrDefault(n => n.NodeId == msg.ToNodeId && n.Time == t);
                        node.Suspects.AddRange(nodes.SingleOrDefault(
                            n => n.NodeId == msg.ToNodeId && n.Time == Previous).Suspects);
                        node.Suspects.RemoveAll(s => s.Expires < t);
                        /// TODO: Allow msgs to be signed by nodes other than those who sent
                        if (node.NodesInSight.Any(nis => nis == msg.FromNodeId)
                            && node.Signatures.Any(s => s.NodeId == msg.FromNodeId))
                        {
                            if (node.Suspects.Any(s => s.NodeId == msg.AccusedNode && s.SuspectedByNodeId == msg.FromNodeId))
                            {
                                node.Suspects.SingleOrDefault(s => s.NodeId == msg.AccusedNode).Expires = t + AccusationLife;
                            }
                            else
                            {
                                node.Suspects.Add(new Suspected(msg.AccusedNode, t + AccusationLife, msg.FromNodeId));
                            }
                        }
                        
                    }
                }
                
                /// 5. If there is a msg verified about a node you previously heard about:
                /// 5a. Add that node to your blacklist
                foreach (var node in nodes.Where(n => n.Time == t))
                {
                    if (node.Suspects.GroupBy(s => s.NodeId).Any(g => g.Count() > 1))
                    {
                        node.Blacklisted.AddRange(
                            node.Suspects
                                .GroupBy(s => s.NodeId)
                                .Where(g => g.Count() > 1)
                                .Select(g => g.Key));
                        node.Blacklisted = node.Blacklisted.Distinct().ToList();
                    }
                }
                
                Previous = t;
            }
            Console.WriteLine("Finished!" + distances.MinBy(d => d.DistanceToNode).DistanceToNode.ToString());
            /// -----------------------------------------------------------------------
            /// -----------------------------------------------------------------------
            /// Algorithm to verify msgs
            /// 1. msg came from a plausible location
            /// 2. msg came from a location you can see and there is a vehicle there
            /// 3. msg is signed
            /// 4. msgs have been recieved from multiple sources
        }

        private static void CalculateDistances()
        {
            foreach (Node node in nodes)
            {
                foreach (var dest in
                    nodes.Where(n => n.Time == node.Time && n.NodeId != node.NodeId && n.Time != -1))
                    if (!distances.Any(d =>
                        d.Time == node.Time
                        && ((d.StartNodeId == node.NodeId && d.DestinationNodeId == dest.NodeId)
                        || (d.StartNodeId == dest.NodeId && d.DestinationNodeId == node.NodeId))))
                    {
                        {
                            if (!node.Position.TooFar(dest.Position))
                                distances.Add(NodeDistance.CalculateDistance(node, dest));
                        }
                    }
            }
        }

        private static void ParseFile(string[] args)
        {
            var lines = File.ReadLines(args[0]);
            foreach (var line in lines)
            {
                var ln = line.Replace("\"", "").Replace("(", " ").Replace(")", "").Split(' ');
                if (line.StartsWith("$node_"))
                {
                    //new node 3 lines
                    switch (line)
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
                }
                else
                {
                    //setdest node
                    nodes.Add(Node.GetNode(ln, nodes));
                }
            }
        }
    }

    
}