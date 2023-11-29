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
        public static List<SybilSeed> CreateSybilSeedData(int arg){
            switch (arg){
                case 1:
                    return new List<SybilSeed>{
                        new SybilSeed(2,13),
                        new SybilSeed(12,13),
                        new SybilSeed(4,25),
                        new SybilSeed(5,25),
                        new SybilSeed(8,25),
                        new SybilSeed(9,25),
                        new SybilSeed(4,28),
                        new SybilSeed(9,28),
                        new SybilSeed(16,28),
                        new SybilSeed(14,28),
                        new SybilSeed(2,19),
                        new SybilSeed(6,19),
                        new SybilSeed(23,19)
                    };
                    break;
                case 2:
                    return new List<SybilSeed>{
                        new SybilSeed(3,16),
                        new SybilSeed(7,16),
                        new SybilSeed(2,27),
                        new SybilSeed(3,27),
                        new SybilSeed(7,27),
                        new SybilSeed(9,27),
                        new SybilSeed(5,30),
                        new SybilSeed(11,30),
                        new SybilSeed(13,30),
                        new SybilSeed(19,30),
                        new SybilSeed(1,18),
                        new SybilSeed(8,18),
                        new SybilSeed(12,18)
                    };
                    break;
                case 3:
                    return new List<SybilSeed>{
                        new SybilSeed(5,8),
                        new SybilSeed(6,8),
                        new SybilSeed(4,14),
                        new SybilSeed(7,14),
                        new SybilSeed(9,14),
                        new SybilSeed(13,14),
                        new SybilSeed(4,15),
                        new SybilSeed(7,15),
                        new SybilSeed(9,15),
                        new SybilSeed(13,15),
                        new SybilSeed(0,16),
                        new SybilSeed(11,16),
                        new SybilSeed(12,16)
                    };
                    break;
                default:
                    return new List<SybilSeed>{
                        new SybilSeed(0,5),
                        new SybilSeed(7,5),
                        new SybilSeed(3,5),
                        new SybilSeed(10,5),
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
                    break;
            }
            return new List<SybilSeed>{
                new SybilSeed(0,5),
                new SybilSeed(3,5),
                new SybilSeed(1,5),
                new SybilSeed(2,5),
                new SybilSeed(7,5),
                new SybilSeed(13,5),
                new SybilSeed(17,5),
                new SybilSeed(18,5),
                new SybilSeed(0,15),
                new SybilSeed(2,15),
                new SybilSeed(12,15),
                new SybilSeed(1,15),
                new SybilSeed(6,16),
                new SybilSeed(3,19),
                new SybilSeed(14,16),
                new SybilSeed(13,19)
            };
        }
        public static List<SybilCount> CreateSybilCount(){
            return new List<SybilCount>{
                new SybilCount(5,0),
                new SybilCount(15,0),
                new SybilCount(16,0),
                new SybilCount(19,0)
            };
        }
    }
}