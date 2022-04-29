using DivarDataAccess.Database.Domain;
using DivarDataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DivarTelegramBot.Services
{
    public class TelegramService
    {
        public async Task SendToTelegram(List<DivarItem> divarItems, string tokenKey)
        {
            TelegramBotClient botClient = new TelegramBotClient(tokenKey);

            foreach (var item in divarItems)
                await botClient.SendTextMessageAsync(new ChatId(item.ChatId), $"{item.Title} - {item.District} \n\n {item.GetDetailsUrl()} \n\n #{item.Request?.Name}");
        }

        public async Task NotDetectedMessage(ITelegramBotClient client, long chatId)
        {
            await client.SendTextMessageAsync(chatId, $"shutUp!!!!");
        }


        public async Task Start(ITelegramBotClient client, long chatId)
        {
            await client.SendTextMessageAsync(chatId, $"fuck you bitch, give me your fucking request like next message");
            await client.SendTextMessageAsync(chatId, $"/reg \n Name \n Url");
            await client.SendTextMessageAsync(chatId, $"/reg \n myFuckingRequestName \n http://myFuckingRequestUrl.divar/blablabla?type=fuck&fuck=type");
            await client.SendTextMessageAsync(chatId, $"Be quick, stupid");
        }

        public async Task RegisterRequest(ITelegramBotClient client, long chatId, string message)
        {
            try
            {
                var divarServices = new DivarServices();
                var commandInputs = message.Trim().Split('\n');
                if (commandInputs.Length != 3)
                    await client.SendTextMessageAsync(chatId, $"send me true mesage");

                var mainUrl = divarServices.GeneratDivarApiFromUrl(commandInputs[2]?.Trim());
                if (string.IsNullOrEmpty(mainUrl))
                {
                    await client.SendTextMessageAsync(chatId, $"url is wrong, call ali");
                    return;
                }

                using (var db = new RequestRepository())
                {
                    await db.Insert(new Request
                    {
                        ChatId = chatId,
                        IsActive = true,
                        LastCheck = DateTime.UtcNow,
                        Name = commandInputs[1]?.Trim(),
                        Url = mainUrl
                    });
                }
                await client.SendTextMessageAsync(chatId, $"ok");
            }
            catch (Exception ex)
            {
                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"----------------------------------------------------------");
                Console.WriteLine($"reg command : {ex.Message} \n chatId: {chatId} \n msg: {message}");
                Console.WriteLine($"----------------------------------------------------------");
                Console.ForegroundColor = defaultColor;
            }
        }
    }
}
