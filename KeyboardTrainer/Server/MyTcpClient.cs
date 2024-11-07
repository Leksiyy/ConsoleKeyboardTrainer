using System.Net.Sockets;
using System.Text;

namespace KeyboardTrainer.Server;

public class MyTcpClient : IDisposable
{
    private const string Host = "127.0.0.1";
    private const int Port = 8888;
    private TcpClient _tcpClient = null!;
    private NetworkStream _networkStream = null!;

    public async void ConnectAsync()
    {
        try
        {
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(Host, Port);
            _networkStream = _tcpClient.GetStream();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while connecting: " + ex.Message);
        }
    }

    public async void SendAsync(string message)
    {
        try
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            await _networkStream.WriteAsync(messageBytes, 0, messageBytes.Length);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<string> ReceiveAsync()
    {
        List<byte> receivedData = new List<byte>();
        byte[] buffer = new byte[1024];
        int bytesRead;
            
        while ((bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            receivedData.AddRange(buffer.Take(bytesRead));

            if (bytesRead < buffer.Length)
            {
                break;
            }
        }

        if (receivedData.Count == 0)
        {
            Console.WriteLine("Server not responding.");
            return string.Empty;
        }

        string response = Encoding.UTF8.GetString(receivedData.ToArray());
        return response;
    }
    
    public bool IsConnected()
    {
        try
        {
            return _tcpClient?.Connected == true && _networkStream?.CanRead == true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void Disconnect()
    {
        _networkStream?.Close();
        _tcpClient?.Close();
        Console.WriteLine("Disconnected from the server.");
    }

    public void Dispose()
    {
        Disconnect();
    }
}