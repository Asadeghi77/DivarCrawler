using DivarCrawler.Database.Domain;
using DivarCrawler.Models;
using DivarCrawler.Services;
using System.ComponentModel;

namespace DivarCrawler;

public class Program
{
    private static bool CanContinue = true;
    private static BackgroundWorker worker = new BackgroundWorker();
    private static DivarServices divarServices = new DivarServices();
    private static ConsoleColor defaultColor = Console.ForegroundColor;

    static async Task Main(string[] args)
    {
        worker.DoWork += Worker_DoWork;
        Timer timer = new Timer(Timer_Elapsed, null, 20, 300000);


        var commandList = new List<CommandItem>()
        {
           new CommandItem {Index=0, Command = "Exit", Method = Exit },
           new CommandItem {Index=1, Command = "Add new Request", Method = AddRequest },
           new CommandItem {Index=2, Command = "Edit Requests", Method = EditRequests },
        };


        while (CanContinue)
        {
            Console.Clear();

            commandList.ForEach(cmd => Console.WriteLine($"{cmd.Index} - {cmd.Command}"));

            int.TryParse(Console.ReadLine(), out int index);

            await commandList.FirstOrDefault(w => w.Index == index).Method();
        }
    }


    public static async Task AddRequest()
    {
        Console.WriteLine("Wite url Request");
        var url = Console.ReadLine();
        Console.WriteLine("Wite Name Request");
        var name = Console.ReadLine();

        using (var db = new DatabasServices())
            await db.InsertGeneral(new Request
            {
                Name = name,
                IsActive = true,
                LastCheck = null,
                Url = divarServices.GeneratDivarApiFromUrl(url)
            });
    }
    public static async Task EditRequests()
    {
        var defaultColor = Console.ForegroundColor;
        Console.WriteLine("Requests :");
        using (var db = new DatabasServices())
        {
            var requests = await db.GetActiveRequestList();
            requests.ForEach(f =>
            {
                var status = f.IsActive ? "Active" : "Deactive";
                Console.ForegroundColor = f.IsActive ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"{f.Id} - {f.Name} - {status}");
            });
            Console.ForegroundColor = defaultColor;

            Console.WriteLine($"write id to edit?");
            int.TryParse(Console.ReadLine(), out int id);

            var selected = requests.FirstOrDefault(f => f.Id == id);

            if (selected is null)
            {
                Console.WriteLine($"Id not true");
                return;
            }

            var reverseStatus = selected.IsActive ? "Deactive" : "Active";
            Console.WriteLine($"Do you want  {reverseStatus} {selected.Name}? (Y/N)");
            var response = Console.ReadLine();
            if (response.ToLower().StartsWith("y"))
            {
                selected.IsActive = !selected.IsActive;
                await db.EditDivarItem(selected);
            }
        }
    }
    public static async Task Exit()
    {
        CanContinue = false;
    }
    public static void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
        Write($"BG WORKER: Run at {DateTime.Now}", ConsoleColor.Yellow, defaultColor);

        List<DivarItem> notSendItems;
        List<Request> requests;

        using (var db = new DatabasServices())
            requests = db.GetActiveRequestList().GetAwaiter().GetResult();

        Write("BG WORKER: Get Requests", ConsoleColor.Yellow, defaultColor);

        requests.ForEach(item =>
        {
            divarServices.CrawleUrl(item.Url).GetAwaiter().GetResult();

            Write($"BG WORKER: CrawleUrl {item.Name} - {DateTime.Now}", ConsoleColor.Yellow, defaultColor);

            item.LastCheck = DateTime.Now;
        });


        using (var db = new DatabasServices())
        {
            db.UpdateRequests(requests).GetAwaiter().GetResult();
            notSendItems = db.GetNotSendDivarItems().GetAwaiter().GetResult();
        }

        Write("BG WORKER: Get Not Send Divar Items", ConsoleColor.Yellow, defaultColor);

        var itemsToSend = divarServices.GetItemCanSendToTelegram(notSendItems).GetAwaiter().GetResult();
        
        divarServices.SendToTelegram(itemsToSend).GetAwaiter().GetResult();

        Write("BG WORKER: Sent Divar Items To Telegram Bot", ConsoleColor.Yellow, defaultColor);

        using (var db = new DatabasServices())
            db.ChangeToSentStatusDivarItems(notSendItems).GetAwaiter().GetResult();

        Write("BG WORKER: Change Divar Items Status To Sent", ConsoleColor.Yellow, defaultColor);
    }

    private static void Write(string msg, ConsoleColor beforeColor, ConsoleColor defaultColor)
    {
        Console.ForegroundColor = beforeColor;
        Console.WriteLine(msg);
        Console.ForegroundColor = defaultColor;
    }
    public static void Timer_Elapsed(object sender)
    {
        if (!worker.IsBusy)
            worker.RunWorkerAsync();
    }
}


