using KeyboardTrainer.Server;

namespace KeyboardTrainer;

class Program
{
    public static MyTcpClient Client { get; set; }
    
    static void Main(string[] args)
    {
        
        KeyboardTrainer.Menu.Menu.Display();
    }
}