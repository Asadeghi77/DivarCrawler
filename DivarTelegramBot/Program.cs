
using DivarDataAccess.Database.Domain;
using DivarDataAccess.Repositories;
using DivarTelegramBot.Services;
using System.ComponentModel;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace DivarTelegramBot;

public class Program
{
    private static BackgroundWorker worker = new BackgroundWorker();
    private static DivarServices divarServices = new DivarServices();
    private static TelegramService telegramService = new TelegramService();
    
    private static ReceiverOptions? options = new ReceiverOptions();
    private static ConsoleColor defaultColor = Console.ForegroundColor;
    private static string tokenKey = "5312553515:AAFfYV_2lfNQBa8H2okf_5TgzdM-3AdfDwM";

    public static void Main()
    {
        worker.DoWork += Worker_DoWork;
        Timer timer = new Timer(Timer_Elapsed, null, 0, 1000);
        Console.WriteLine("Start BG worker");


        TelegramBotClient botClient = new TelegramBotClient(tokenKey);

        botClient.StartReceiving(updatehandler,
            async Task (a, b, c) => { },
            options);
        Console.WriteLine("Start Telegram Engine");

        Console.ReadLine();
    }

    private static async Task updatehandler(ITelegramBotClient client, Update model, CancellationToken cancellationToken)
    {
        var text = model.Message.Text;
        var chatId = model.Message.Chat.Id;

        if (text.Contains("/start"))
            await telegramService.Start(client, chatId);
        else if (text.Contains("/reg"))
            await telegramService.RegisterRequest(client, chatId, text);
        else
            await telegramService.NotDetectedMessage(client, chatId);
    }


    public static void Timer_Elapsed(object sender)
    {
        if (!worker.IsBusy)
            worker.RunWorkerAsync();
    }

    public static void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
        List<DivarItem> notSendItems;
        List<Request> requests;

        using (var db = new RequestRepository())
            requests = db.GetActiveList().GetAwaiter().GetResult();

        Write("BG WORKER: Get Requests", ConsoleColor.Yellow, defaultColor);

        requests.ForEach(item =>
        {
            divarServices.CrawleUrl(item).GetAwaiter().GetResult();

            Write($"BG WORKER: CrawleUrl {item.Name} - {DateTime.Now}", ConsoleColor.Yellow, defaultColor);

            item.LastCheck = DateTime.Now;
        });


        using (var db = new RequestRepository())
            db.UpdateMany(requests).GetAwaiter().GetResult();

        using (var db = new DivarItemRepository())
            notSendItems = db.GetNotSend().GetAwaiter().GetResult();

        Write("BG WORKER: Get Not Send Divar Items", ConsoleColor.Yellow, defaultColor);

        telegramService.SendToTelegram(notSendItems, tokenKey).GetAwaiter().GetResult();

        Write("BG WORKER: Sent Divar Items To Telegram Bot", ConsoleColor.Yellow, defaultColor);

        using (var db = new DivarItemRepository())
            db.ChangeToSentStatus(notSendItems).GetAwaiter().GetResult();

        Write("BG WORKER: Change Divar Items Status To Sent", ConsoleColor.Yellow, defaultColor);
    }

    private static void Write(string msg, ConsoleColor beforeColor, ConsoleColor defaultColor)
    {
        Console.ForegroundColor = beforeColor;
       // Console.WriteLine(msg);
        Console.ForegroundColor = defaultColor;
    }

    


}