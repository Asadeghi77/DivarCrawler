namespace DivarDataAccess.Database.Domain
{
    public class Request : IBaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime? LastCheck { get; set; }
        public bool IsActive { get; set; }
        public long ChatId { get; set; }

        public virtual IList<DivarItem> DivarItems { get; set; }

    }
}
