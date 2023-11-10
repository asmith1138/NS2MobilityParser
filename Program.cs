namespace Parser
{
    using System;
    using System.Diagnostics;
    using System.Net.NetworkInformation;

    public class Program
    {
        public static int LidarDist = 200;
        public static int BroadcastRange = 500;
        public static double SignatureLife = 5;
        public static int SignatureMod = 1;
        public static int PrintMod = 10;
        public static int BlacklistMod = 1;
        public static int BlackListLimit = 30;
        public static double AccusationLife = 5;
        public static List<NodeDistance> distances = new List<NodeDistance>();
        public static List<Node> nodes = new List<Node>();

        static void Main(string[] args)
        {
            string arg = args[0];
            ReportSetup(arg);

            Trace.WriteLine("Parsing trace file...");
            ParseFile(arg);
            Trace.WriteLine("Parsing finished.");

            Trace.WriteLine("Calculating distances...");
            CalculateDistances(arg);
            Trace.WriteLine("Calculating finished.");

            Trace.WriteLine("Writing distances to file...");
            PrintDistances(arg);
            Trace.WriteLine("Distances written.");

            Trace.WriteLine("Loaded the whole simulation into memory!");
            
            Trace.WriteLine("Seeding sybil node information...");
            SybilSeeding();
            Trace.WriteLine("Seeding finished.");
            
            Trace.WriteLine("Simulating...");
            (double maxTime, int totalMsg, int maxMsg, int maxMsgPerSecond) = Simulation();
            Trace.WriteLine("Simulating finished.");

            Trace.WriteLine("Final report:");
            FinalReporting(maxTime, totalMsg, maxMsg, maxMsgPerSecond);
            Trace.WriteLine("End.");
        }

        private static void ReportSetup(string arg)
        {
            Trace.Listeners.Clear();

            TextWriterTraceListener traceListener = new TextWriterTraceListener(arg + ".report.txt",AppDomain.CurrentDomain.FriendlyName);
            traceListener.Name = "TextReporter";
            traceListener.TraceOutputOptions = TraceOptions.None;

            ConsoleTraceListener consoleTraceListener = new ConsoleTraceListener(false);
            consoleTraceListener.TraceOutputOptions = TraceOptions.None;

            Trace.Listeners.Add(traceListener);
            Trace.Listeners.Add(consoleTraceListener);
            Trace.AutoFlush = true;
        }

        private static (double, int, int, int) Simulation()
        {
            double maxTime = nodes.MaxBy(n => n.Time).Time;
            int totalMsg = 0;
            int maxMsg = 0;
            int maxMsgPerSecond = 0;
            int msgPerSecond = 0;
            double Previous = -1;
            for (double t = 0.0; t <= maxTime; t += 0.1)
            {
                t = Math.Round(t, 1, MidpointRounding.AwayFromZero);
                if (t % PrintMod == 0)
                {
                    Trace.WriteLine("Starting time: " + t);
                    Trace.WriteLine($"Total messages so far: {totalMsg}");
                }
                if (t % 1 == 0)
                {
                    msgPerSecond = 0;
                }
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
                    
                    /*
                    if(node.NodesInSight.Count() > 1)
                    {
                        Trace.WriteLine("Node " + node.NodeId + " sees nodes: "
                        + string.Join(",", node.NodesInSight) + " at time " + t);
                    }
                    */

                    /// 1a. And which are in range
                    node.NodesInRange = distances.Where(d => d.Time == t
                    && d.DestinationNodeId == node.NodeId
                    && d.DistanceToNode < BroadcastRange)
                    .Select(d => d.StartNodeId)
                    .Union(
                        distances.Where(d => d.Time == t
                        && d.StartNodeId == node.NodeId
                        && d.DistanceToNode < BroadcastRange)
                        .Select(d => d.DestinationNodeId).ToList()
                    ).ToList();

                    /// 2. Trade signatures with each vehicle you can physically see
                    /// only do so sometimes
                    if ((t % SignatureMod) == 0)
                    {
                        messages.AddRange(node.NodesInRange.Select(nis =>
                                                new Message(node.NodeId, nis)
                                            ));
                    }


                    /// 3. If you know of any Sybil (or other malicious) nodes:
                    /// 3a. Broadcast a msg to all your neighbors about the node
                    /// TODO: Add sending messages less often
                    /*if (sybilNodes.Any(sn => sn.KnowingNodeId == node.NodeId))
                    {
                        messages.AddRange(node.NodesInSight.SelectMany(nis =>
                            sybilNodes.Where(sn => sn.KnowingNodeId == node.NodeId).Select(sn =>
                                new Message(node.NodeId, nis, sn.SybilNodeId)
                        )));
                    }*/
                    /// 5b. Broadcast a msg to all your neighbors about that node                        
                    var oldNode = nodes.SingleOrDefault(
                        n => n.NodeId == node.NodeId && n.Time == Previous);
                    node.Blacklisted.AddRange(nodes.SingleOrDefault(
                        n => n.NodeId == node.NodeId && n.Time == Previous)?.Blacklisted ?? new List<Blacklisted>());

                    if (node.Blacklisted.Any(b => (b.TimeBlacklisted + BlackListLimit) <= t) 
                        && (t % BlacklistMod) == 0)
                    {
                        messages.AddRange(node.NodesInRange.SelectMany(nir =>
                            node.Blacklisted.Where(b => (b.TimeBlacklisted + BlackListLimit) <= t)
                            .Select(bl =>
                                new Message(node.NodeId, nir, bl.NodeId, bl.SignedNodes)
                        )));
                    }

                    /// Grab relevant lists from previous
                    if (node.Signatures != null && node.Signatures.Count() == 0)
                    {
                        node.Signatures.AddRange(nodes.SingleOrDefault(
                            n => n.NodeId == node.NodeId && n.Time == Previous)?.Signatures ?? new List<Signature>());
                        node.Signatures.RemoveAll(s => s.Expires < t);
                    }

                    if (node.Suspects != null && node.Suspects.Count() == 0)
                    {
                        node.Suspects.AddRange(nodes.SingleOrDefault(
                                                    n => n.NodeId == node.NodeId && n.Time == Previous)?.Suspects ?? new List<Suspected>());
                        node.Suspects.RemoveAll(s => s.Expires < t);
                    }
                    //remove suspects that are blacklisted
                    node.Suspects.RemoveAll(s => node.Blacklisted.Any(bl => bl.NodeId == s.NodeId));
                }

                foreach (var msg in messages)
                {
                    if (msg.Type == MessageType.Signature)
                    {
                        /// 2b. Save signatures for 50 timestamps
                        var node = nodes.SingleOrDefault(n => n.NodeId == msg.ToNodeId && n.Time == t);

                        /// Removed code here

                        if (node.NodesInSight.Any(nis => nis == msg.FromNodeId))
                        {
                            if (node.Signatures.Any(s => s.NodeId == msg.FromNodeId))
                            {
                                node.Signatures.SingleOrDefault(s => s.NodeId == msg.FromNodeId).Expires = t + SignatureLife;
                            }
                            else
                            {
                                node.Signatures.Add(new Signature(msg.FromNodeId, t + SignatureLife));
                            }
                        }

                    }
                    else if (msg.Type == MessageType.Accusation)
                    {
                        /// 4. If you recieve any msgs about malicious nodes:
                        /// 4a. Verify if you can see the vehicle that sent the msg
                        /// 4b. Verify the msg is signed by a node you know
                        /// 4c. Keep the msg for 50 timestamp values
                        var node = nodes.SingleOrDefault(n => n.NodeId == msg.ToNodeId && n.Time == t);
                        
                        //Removed code here

                        /// Allow msgs to be signed by nodes other than those who sent
                        if (node.NodesInSight.Any(nis => nis == msg.FromNodeId)
                            && node.Signatures.Any(s => s.NodeId == msg.FromNodeId))
                        {
                            if (node.Suspects.Any(s => s.NodeId == msg.AccusedNode && s.SuspectedByNodeId == msg.FromNodeId))
                            {
                                node.Suspects.SingleOrDefault(s => s.NodeId == msg.AccusedNode && s.SuspectedByNodeId == msg.FromNodeId).Expires = t + AccusationLife;
                            }
                            else if (!node.Blacklisted.Any(bl => bl.NodeId == msg.AccusedNode))
                            {
                                Trace.WriteLine("Adding suspect: " + msg.AccusedNode
                                    + ", by " + msg.FromNodeId + " and signed by " + string.Join(",",msg.NodesSigned)
                                     + " to node: " + node.NodeId + " at time " + t);
                                node.Suspects.Add(new Suspected(msg.AccusedNode, t + AccusationLife, msg.FromNodeId, msg.NodesSigned));
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
                        var susList = node.Suspects.GroupBy(s => s.NodeId).Where(g => g.Count() > 1).Select(g => g.Key);
                        foreach (var susId in susList)
                        {
                            var signed =
                            node.Suspects.Where(s => s.NodeId == susId).SelectMany(sl => sl.SignedNodes).Union(
                            node.Suspects.Where(s => s.NodeId == susId).Select(sl => sl.SuspectedByNodeId)).ToList();
                            node.Blacklisted.Add(new Blacklisted(susId, signed, t));
                            Trace.WriteLine("Adding Blacklist: " + susId
                                    + ", signed by " + string.Join(",",signed)
                                     + " to node: " + node.NodeId + " at time " + t);
                        }
                        //var sus = node.Suspects.GroupBy(s => s.NodeId).Where(g => g.Count() > 1).SelectMany(s=>s.ToList());
                        //node.Blacklisted.AddRange(
                        //    node.Suspects
                        //        .GroupBy(s => s.NodeId)
                        //        .Where(g => g.Count() > 1)
                        //        .Select(g => new Blacklisted(g.Key, new List<int>(){node.NodeId})));
                        node.Blacklisted = node.Blacklisted.Distinct().ToList();
                    }
                }

                Previous = t;
                totalMsg += messages.Count();
                msgPerSecond += messages.Count();
                maxMsg = maxMsg > messages.Count() ? maxMsg : messages.Count();
                maxMsgPerSecond = maxMsgPerSecond > msgPerSecond ? maxMsgPerSecond : msgPerSecond;
            }
            return (maxTime, totalMsg, maxMsg, maxMsgPerSecond);
        }

        private static void SybilSeeding()
        {
            /// Set up seed sybil detection
            var sybilNodes = SybilSeedFactory.CreateSybilSeedData();
            foreach (var sn in sybilNodes)
            {
                var node = nodes.Where(n => n.NodeId == sn.KnowingNodeId && n.Time != -1).MinBy(n => n.Time);
                node.Blacklisted.Add(new Blacklisted(sn.SybilNodeId, new List<int>(), 0));
                Trace.WriteLine("Initial Blacklist: " + sn.SybilNodeId
                                    + ", no signatures"
                                     + " to node: " + node.NodeId);
            }
        }

        private static void FinalReporting(double maxTime, int totalMsg, int maxMsg, int maxMsgPerSecond)
        {
            Trace.WriteLine("Finished!" + distances.MinBy(d => d.DistanceToNode).DistanceToNode.ToString());

            // find max nodeid and for loop over node ids
            int maxNode = nodes.MaxBy(n => n.NodeId).NodeId;
            // find max time for each node ID and add to list to print out some info
            for (int n = 0; n <= maxNode; n++)
            {
                var node = nodes.Where(nd => nd.NodeId == n).MaxBy(nd => nd.Time);
                Trace.WriteLine("Node: " + node.NodeId);
                Trace.WriteLine("BlackList: ");
                foreach (var blItem in node.Blacklisted)
                {
                    Trace.WriteLine(blItem.NodeId + ", Signatures:");
                    foreach (var sig in blItem.SignedNodes)
                    {
                        Trace.Write(sig + ", ");
                    }
                    Trace.WriteLine("End of Signatures.");
                }
                Trace.WriteLine("End of Blacklist.");
                Trace.WriteLine("End of Node.");
            }
            var blNode = nodes.MaxBy(n => n.Blacklisted.Count());
            Trace.WriteLine("Largest Blacklist: " + blNode.NodeId
            + ", Size: " + blNode.Blacklisted.Count()
            + ", Time: " + blNode.Time);
            Trace.WriteLine(string.Join(", ", blNode.Blacklisted.Select(bl => bl.NodeId)));
            Trace.WriteLine($"Total Messages: {totalMsg}, Messages per second: {totalMsg / maxTime}");
            Trace.WriteLine($"Max number of messages sent at time t: {maxMsg}");
            Trace.WriteLine($"Max number of messages sent in 1 second: {maxMsgPerSecond}");
            /// -----------------------------------------------------------------------
            /// -----------------------------------------------------------------------
            /// Algorithm to verify msgs
            /// 1. msg came from a plausible location
            /// 2. msg came from a location you can see and there is a vehicle there
            /// 3. msg is signed
            /// 4. msgs have been recieved from multiple sources
        }

        private static void CalculateDistances(string arg)
        {
            string filepath = arg + ".dis";
            if (File.Exists(filepath))
            {
                var lines = File.ReadLines(filepath);
                foreach (var line in lines)
                {
                    if(!string.IsNullOrWhiteSpace(line))
                    {
                        var lineArray = line.Split(" ");
                        distances.Add(new NodeDistance(int.Parse(lineArray[1]), int.Parse(lineArray[2]), double.Parse(lineArray[3]), double.Parse(lineArray[0])));
                    }
                }
            }
            else
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
        }

        private static void PrintDistances(string arg)
        {
            string filepath = arg + ".dis";
            if (!File.Exists(filepath))
            {
                using (StreamWriter outputFile = new StreamWriter(filepath))
                {
                    foreach (var dist in distances)
                        outputFile.WriteLine(dist.Time + " "
                        + dist.StartNodeId + " "
                        + dist.DestinationNodeId + " "
                        + dist.DistanceToNode);
                }
            }
        }
        private static void ParseFile(string arg)
        {
            var lines = File.ReadLines(arg);
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