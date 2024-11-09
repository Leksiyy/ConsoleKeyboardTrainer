using System.Text.Json;
using KeyboardTrainer.Models;

namespace KeyboardTrainer.Game;

public class Game
{
    private static Random random = new Random();

    public async Task PlayAsync()
    {
        string[] wordsDictionary = {
            "galaxy", "whisper", "mountain", "river", "storm",
            "dream", "symphony", "puzzle", "journey", "flame",
            "ocean", "voyage", "crystal", "echo", "twilight",
            "memory", "horizon", "shadow", "silence", "mystery",
            "ancient", "breeze", "canvas", "labyrinth", "whistle",
            "ember", "miracle", "phantom", "realm", "adventure",
            "fortune", "wander", "rhythm", "midnight", "wonder",
            "compass", "myth", "forest", "glow", "riddle",
            "treasure", "destiny", "whirlpool", "glimpse", "solitude",
            "spark", "enigma", "legend", "haven", "cosmos"
        };
        
        Dictionary<int, bool> letterStatus = new Dictionary<int, bool>(); // словарь для хранения статуса каждой буквы (правильная/неправильная)
        (int, int, TimeSpan) stats = (0, 0, new TimeSpan()); // правильные, ошибки, время.
        Console.Clear();
        Console.WriteLine("Начните вводить текст:");

        string testWords = string.Join(" ", GenerateWords(wordsDictionary));
        Console.Write(testWords);
        int index = 0;
        DateTime startTime = default;
        ConsoleColor consoleColor = Console.BackgroundColor;

        while (index <= testWords.Length)
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
                    Console.Write(testWords[index]);
                    Console.SetCursorPosition(index, 1);
                    Console.ResetColor();
                }
                continue;
            }

            if (index >= testWords.Length) break;
            
            char inputChar = keyInfo.KeyChar;

            if (keyInfo.Key == ConsoleKey.Spacebar && inputChar != testWords[index])
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write(inputChar);
                Console.BackgroundColor = consoleColor;
            }
            
            if (inputChar == testWords[index])
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
        
        Console.WriteLine($"\nSTATS:\nTime taken: {stats.Item3.Seconds} seconds\nNumber of incorrect letters: {stats.Item2}\nAccuracy: {((double)stats.Item1 / testWords.Length)*100:F2}%");
        Program.Client.SendAsync("SEND record");
        string recordJson = await Program.Client.ReceiveAsync();
        Statistics statistics;
        if (recordJson != "False")
        {
            statistics = JsonSerializer.Deserialize<Statistics>(recordJson);
        }
        else
        {
            statistics = null;
        }
        Statistics newStatistics = new Statistics { Time = stats.Item3 , CorrectLetters = stats.Item1, WrongLetters = stats.Item2, NumOfLetters = testWords.Length };
        if (statistics is not null && testWords.Length / stats.Item3.TotalSeconds > (double)statistics.NumOfLetters / statistics.CorrectLetters && // по вермени
            ((double)stats.Item1 / testWords.Length) * 100 >= 80) // по процентам
        {
            Console.WriteLine("\n\t\tNEW WORLD RECORD!\n");
            Program.Client.SendAsync($"UPDATE record|{JsonSerializer.Serialize(newStatistics)}");
            
            bool statusCode = Convert.ToBoolean(await Program.Client.ReceiveAsync());
            Console.WriteLine(statusCode ? "World record successfully updated!" : "Can't update world record!");
        } else if (statistics is null)
        {
            Console.WriteLine("\n\t\tNEW WORLD RECORD!\n");
            Program.Client.SendAsync($"UPDATE record|{JsonSerializer.Serialize(newStatistics)}");
        }
    
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    private string[] GenerateWords(string[] wordsDictionary)
    {
        return wordsDictionary.OrderBy(x => random.Next())
            .Take(random.Next(6, 8))
            .ToArray();
    }
}