using System.Text.Json;
using KeyboardTrainer.Models;
using KeyboardTrainer.Server;

namespace KeyboardTrainer.Menu;

public static class Menu
{
    public static Game.Game Game = new Game.Game();
    public static async Task Display()
    {
        Console.Clear();
        Console.WriteLine("Welcome to Keyboard Trainer!");
        Console.WriteLine("Connecting to the server...");
        
        await Program.Client.ConnectAsync();
        
        Thread.Sleep(1000);
        
        if (Program.Client.IsConnected())
        {
            Console.WriteLine("Connected!");
            

            Thread.Sleep(2000);
            while (true)
            {
                Console.WriteLine("Keyboard trainer menu:\n1. Start training.\n2. Show \"world\" record.\n3. Exit");
                
                Console.WriteLine("Enter your choice: ");
                string choose = Console.ReadLine() ?? "";
                switch (choose)
                {
                    case "1":
                        await Game.PlayAsync();
                        continue;
                    case "2":
                        ShowRecord();
                        continue;
                    case "3":
                        break;
                    default:
                        continue;
                }
            }
        }
        else
        {
            Console.WriteLine("Not connected. Try again.");
        }
    }

    private static async void ShowRecord()
    {
        if (Program.Client.IsConnected())
        {
            Program.Client.SendAsync("SEND record");
            string recordJson = await Program.Client.ReceiveAsync();
            Statistics statistics = JsonSerializer.Deserialize<Statistics>(recordJson) ?? new Statistics();
            
            Console.WriteLine($"Statistics:" +
                              $"\nTaken time: {statistics.Time}" +
                              $"\nCorrect letters: {statistics.CorrectLetters}" +
                              $"\nWrong letters: {statistics.WrongLetters}" +
                              $"\nNumbers of letters: {statistics.NumOfLetters}" +
                              $"\nAccuracy: {((double)statistics.CorrectLetters / statistics.NumOfLetters)*100:F2}");
        }
    }
}