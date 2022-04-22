using DivarCrawler.Database.Domain;
using DivarCrawler.Models;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace DivarCrawler.Services
{
    public class DivarServices
    {
        public DivarServices()
        {

        }


        public string GeneratDivarApiFromUrl(string url)
        {
            var uri = new Uri(url);
            return $"https://api.divar.ir/v8/web-search/tehran/rent-apartment{uri.Query}";
        }

        public async Task CrawleUrl(string url)
        {
            HttpClient client = new HttpClient();
            var res = await client.GetStringAsync(url);

            var data = JsonSerializer.Deserialize<Root>(res);

            var divarItemList = data.widget_list.Select(s => new DivarItem
            {
                Category = s.data.category,
                City = s.data.city,
                Description = s.data.description,
                District = s.data.district,
                Images = $"{s.data.image},{string.Join(",", s.data.web_image.Select(ss => ss.src))}",
                Title = s.data.title,
                Token = s.data.token,
                IsSendToTelegram = false,
                CreationDate = DateTime.Now,
            }).ToList();

            var tokenLists = divarItemList.Select(s => s.Token).ToList();

            using var db = new DatabasServices();
            {
                var existlist = await db.GetExistDivarItems(tokenLists);

                var insertedList = divarItemList.Where(w => !existlist.Contains(w.Token)).ToList();
                await db.InsertManyDivarItem(insertedList);
            }
        }

        public async Task SendToTelegram(List<DivarItem> divarItems)
        {
            TelegramBotClient botClient = new TelegramBotClient("5312553515:AAFfYV_2lfNQBa8H2okf_5TgzdM-3AdfDwM");

            foreach (var item in divarItems)
            {
                await botClient.SendTextMessageAsync(new ChatId(130800314), $"{item.Title} \n {item.GetDetailsUrl()}");
            }
        }
        public async Task<List<DivarItem>> GetItemCanSendToTelegram(List<DivarItem> notSendItems)
        {

            return notSendItems;
        }
    }
}
