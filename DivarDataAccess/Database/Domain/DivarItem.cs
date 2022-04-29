namespace DivarDataAccess.Database.Domain
{
    public class DivarItem : IBaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string District { get; set; }
        public string Token { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsSendToTelegram { get; set; }
        public long ChatId { get; set; }

        public int RequestId { get; set; }
        public virtual Request Request { get; set; }

        
        public string GetDetailsUrl()
        {
            return $"https://divar.ir/v/test/{Token}";
        }
    }
}
