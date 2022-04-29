using DivarDataAccess.Database.Domain;
using DivarDataAccess.Repositories;
using DivarTelegramBot.Models;
using System.Text.Json;

namespace DivarTelegramBot.Services
{
    public class DivarServices
    {
        public DivarServices()
        {

        }


        public string GeneratDivarApiFromUrl(string url)
        {
            try
            {
                var queryIndex = url.ToLower().IndexOf("/s/") + 3;
                var query = url.Substring(queryIndex);
                return $"https://api.divar.ir/v8/web-search/{query}";
            }
            catch (Exception ex)
            {
                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"----------------------------------------------------------");
                Console.WriteLine($"GeneratDivarApiFromUrl : {ex.Message} \n url: {url}");
                Console.WriteLine($"----------------------------------------------------------");
                Console.ForegroundColor = defaultColor;
                return null;
            }
        }

        public async Task CrawleUrl(Request request)
        {
            try
            {
                HttpClient client = new HttpClient();
                var res = await client.GetStringAsync(request.Url);

                var data = JsonSerializer.Deserialize<Root>(res);

                var divarItemList = data.widget_list.Select(s => new DivarItem
                {
                    District = s.data.district,
                    Title = s.data.title,
                    Token = s.data.token,
                    IsSendToTelegram = false,
                    CreationDate = DateTime.Now,
                    RequestId = request.Id,
                    ChatId = request.ChatId
                }).ToList();

                using var db = new DivarItemRepository();
                {
                    var existlist = await db.GetExistItems(divarItemList);

                    var insertedList = divarItemList.Where(w => !existlist.Contains(w.Token)).ToList();
                    await db.InsertMany(insertedList);
                }
            }
            catch (Exception ex)
            {
                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"----------------------------------------------------------");
                Console.WriteLine($"CrawleUrl : {ex.Message} \n request: {request.Id} - {request.Name}");
                Console.WriteLine($"----------------------------------------------------------");
                Console.ForegroundColor = defaultColor;
            }
        }

    }
}
