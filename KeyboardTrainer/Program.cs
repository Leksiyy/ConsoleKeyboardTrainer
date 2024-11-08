using KeyboardTrainer.Server;

namespace KeyboardTrainer;

class Program
{
    public static MyTcpClient Client { get; set; } = new MyTcpClient();
    
    static async Task Main(string[] args)
    {
        
        await KeyboardTrainer.Menu.Menu.Display();
    } //TODO: у меня с сервера не стягиваеться стата потому что ее нету, и приложению не с чем сравнить текущую стату, сделай так что если с сервера ничего не приходит то показываеться что ничего
      // не найдено, ав игре сделай так что бы игнорировать загрузку на сервер при условии что сервер не отправил нормальную статистику.
}