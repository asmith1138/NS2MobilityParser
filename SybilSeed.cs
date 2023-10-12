namespace Parser{
    public class SybilSeed
    {
        public SybilSeed(int knowingNodeId, int sybilNodeId)
        {
            this.KnowingNodeId = knowingNodeId;
            this.SybilNodeId = sybilNodeId;
        }

        public int KnowingNodeId{ get; set; }
        public int SybilNodeId { get; set; }
    }

    public static class SybilSeedFactory{
        public static List<SybilSeed> CreateSybilSeedData(){
            return new List<SybilSeed>{
                new SybilSeed(0,5),
                new SybilSeed(7,5),
                new SybilSeed(3,5),
                new SybilSeed(1,5),
                new SybilSeed(2,5),
                new SybilSeed(4,5),
                new SybilSeed(6,5),
                new SybilSeed(7,5),
                new SybilSeed(8,5),
                new SybilSeed(9,5),
                new SybilSeed(11,5),
                new SybilSeed(12,5),
                new SybilSeed(10,5),
                new SybilSeed(13,5),
                new SybilSeed(14,5),
                new SybilSeed(15,5),
                new SybilSeed(16,5),
                new SybilSeed(17,5),
                new SybilSeed(18,5),
                new SybilSeed(19,5),
                new SybilSeed(20,5),
                new SybilSeed(21,5),
                new SybilSeed(22,5),
                new SybilSeed(23,5),
                new SybilSeed(0,11),
                new SybilSeed(2,11),
                new SybilSeed(12,11),
                new SybilSeed(1,11)
            };
        }
    }
}