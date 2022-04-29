using DivarCrawler.Models;
using DivarDataAccess.Database.Domain;
using DivarDataAccess.Repositories;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

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
                District = s.data.district,
                Title = s.data.title,
                Token = s.data.token,
                IsSendToTelegram = false,
                CreationDate = DateTime.Now,
                  //RequestId 
            }).ToList();

            var tokenLists = divarItemList.Select(s => s.Token).ToList();

            using var db = new DivarItemRepository();
            {
                var existlist = await db.GetExistItems(tokenLists);

                var insertedList = divarItemList.Where(w => !existlist.Contains(w.Token)).ToList();
                await db.InsertMany(insertedList);
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
