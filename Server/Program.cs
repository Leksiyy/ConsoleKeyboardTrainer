using Server.Data;
using Server.Factories;
using Server.Server;

namespace Server;

class Program
{
    public static ApplicationContext DbContext() => new ApplicationContextFactory().CreateDbContext();
    static async Task Main(string[] args)
    {
        MyTcpListener myServer = new MyTcpListener();
        await myServer.StartAsync();
    }
} //TODO: сервер некорректно получает рекорД, (думаю что нужно пересмотреть модли или джсоны)