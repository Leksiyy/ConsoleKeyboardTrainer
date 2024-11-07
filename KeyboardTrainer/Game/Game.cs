using System.Text.Json;
using KeyboardTrainer.Models;

namespace KeyboardTrainer.Game;

public static class Game
{
    public static async Task Play()
    {
        Dictionary<int, bool> letterStatus = new Dictionary<int, bool>(); // словарь для хранения статуса каждой буквы (правильная/неправильная)
        (int, int, TimeSpan) stats = (0, 0, new TimeSpan()); // правильные, ошибки, время.
        Console.Clear();
        Console.WriteLine("Начните вводить текст:");

        string word = "hello world";
        Console.Write(word);
        int index = 0;
        DateTime startTime = default;
        ConsoleColor consoleColor = Console.BackgroundColor;

        while (index <= word.Length)
        {
            Console.SetCursorPosition(index, 1);
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
            
            if (index == 0) startTime = DateTime.Now;
            
            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (index > 0)
                {
                    index--;
                    Console.SetCursorPosition(index, 1);
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(word[index]);
                    Console.SetCursorPosition(index, 1);
                    Console.ResetColor();
                }
                continue;
            }

            if (index >= word.Length) break;
            
            char inputChar = keyInfo.KeyChar;

            if (keyInfo.Key == ConsoleKey.Spacebar)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write(inputChar);
                Console.BackgroundColor = consoleColor;
            }
            
            if (inputChar == word[index])
            {
                stats.Item1++;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(inputChar);
            }
            else
            {
                stats.Item2++;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(inputChar);
            }

            index++;

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                break;
            }
        }

        stats.Item3 = DateTime.Now - startTime;
        
        Console.ResetColor();
        
        Console.WriteLine($"\nSTATS:\nTime taken: {stats.Item3.Seconds} seconds\nNumber of incorrect letters: {stats.Item2}\nAccuracy: {((double)stats.Item1 / word.Length)*100:F2}%");

        Program.Client.SendAsync("SEND record");
        string recordJson = await Program.Client.ReceiveAsync();
        Statistics statistics = JsonSerializer.Deserialize<Statistics>(recordJson);
        if (statistics is not null && word.Length / stats.Item3.TotalSeconds > (double)statistics.NumOfLetters / statistics.CorrectLetters && // по вермени
            ((double)stats.Item1 / word.Length) * 100 >= 80) // по процентам
        {
            Console.WriteLine("\n\t\tNEW WORLD RECORD!\n");
            Statistics newStatistics = new Statistics { Time = stats.Item3 , CorrectLetters = stats.Item1, WrongLetters = stats.Item2, NumOfLetters = word.Length };
            Program.Client.SendAsync($"UPDATE record|{JsonSerializer.Serialize(newStatistics)}");
            
            bool statusCode = Convert.ToBoolean(await Program.Client.ReceiveAsync());
            Console.WriteLine(statusCode ? "World record successfully updated!" : "Can't update world record!");
        }
    
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}