namespace Test.Entities
{
    public class Post
    {
        public int id { get; set; }
        public string author { get; set; }
        public int authorId { get; set; }
        public int likes { get; set; }
        public decimal popularity { get; set; }
        public int reads { get; set; }
        public string [] tags { get; set; }
    }
}
