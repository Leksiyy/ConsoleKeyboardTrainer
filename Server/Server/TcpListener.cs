using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using BookStore.Data;
using KeyboardTrainer.Models;

namespace Server.Server;

public class MyTcpListener
{
    private const int ServerPort = 8888;
    private static IPAddress ServerIP = IPAddress.Parse("127.0.0.1");
    private readonly TcpListener _tcpListener = new TcpListener(ServerIP, ServerPort);

    public async Task StartAsync()
    {
        _tcpListener.Start();
        Console.WriteLine($"Listening on port {ServerPort}");
        while (true)
        {
            TcpClient client = await _tcpListener.AcceptTcpClientAsync();
            Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");
            _ = Task.Run(() => HandleClientAsync(client));
        }
    }

    public async Task HandleClientAsync(TcpClient tcpClient)
    {
        while (true)
        {
            byte[] buffer = new byte[1024];
            try
            {
                NetworkStream networkStream = tcpClient.GetStream();
                int readBytes = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, readBytes);
                Console.WriteLine("Receive message:" + message);
                
                await ProcessMessageAsync(message, networkStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public async Task ProcessMessageAsync(string message, NetworkStream networkStream)
    {
        int splitMessage = message.IndexOf(' ');
        string firstPart, secondPart;
        if (splitMessage != -1)
        {
            firstPart = message.Substring(0, splitMessage);
            secondPart = message.Substring(splitMessage + 1);
        }
        else
        {
            return; // TODO: response to client
        }
        
        switch (firstPart.ToLower())
        {
            case "send":
                byte[] sendBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(await GetStatisticsAsync()));
                await networkStream.WriteAsync(sendBytes, 0, sendBytes.Length);
                break;
            case "update":
                byte[] updateBytes = Encoding.UTF8.GetBytes(Convert.ToString(await UpdateStatisticsAsync(message)));
                await networkStream.WriteAsync(updateBytes, 0, updateBytes.Length);
                break;
            default:
                break;
        }
    }

    public async Task<Statistics?> GetStatisticsAsync()
    {
        using (ApplicationContext context = Program.DbContext())
        {
            Statistics statistics = context.Statistics.FirstOrDefault();
            return statistics;
        }
    }

    public async Task<bool> UpdateStatisticsAsync(string statistics)
    {
        Statistics newStatistics = JsonSerializer.Deserialize<Statistics>(statistics);
        using (ApplicationContext context = Program.DbContext())
        {
            Statistics oldStatistics = context.Statistics.FirstOrDefault();
            if (newStatistics is not null && oldStatistics is not null)
            {
                oldStatistics.Time = newStatistics.Time;
                oldStatistics.CorrectLetters = newStatistics.CorrectLetters;
                oldStatistics.WrongLetters = newStatistics.WrongLetters;
                oldStatistics.NumOfLetters = newStatistics.NumOfLetters;
                
                await context.SaveChangesAsync();
                return true;
            }
        }

        return false;
    }
}