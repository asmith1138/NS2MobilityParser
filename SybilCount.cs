namespace Parser{
    public class SybilCount
    {
        public SybilCount(int node, int count)
        {
            Node = node;
            Count = count;
        }

        public void Increment(){
            Count++;
        }
        
        public int Node { get; set; }
        public int Count { get; set; }
    }
}