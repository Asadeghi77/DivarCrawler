namespace DivarCrawler.Database.Domain
{
    public class DivarItem : IBaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Images { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Token { get; set; }
        public string Category { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsSendToTelegram { get; set; }

        public string GetDetailsUrl()
        {
            return $"https://divar.ir/v/test/{Token}";
        }
    }
}
