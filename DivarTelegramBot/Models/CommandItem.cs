namespace DivarTelegramBot.Models
{
    public class CommandItem
    {
        public int Index { get; set; }
        public string Command { get; set; }
        public Func<Task> Method { get; set; }
    }
}
