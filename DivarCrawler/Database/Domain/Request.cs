namespace DivarCrawler.Database.Domain
{
    public class Request : IBaseEntity
    {
        public int Id { get; set; }
        public string Name { get; internal set; }
        public string Url { get; set; }
        public DateTime? LastCheck { get; set; }
        public bool IsActive { get; set; }
    }
}
