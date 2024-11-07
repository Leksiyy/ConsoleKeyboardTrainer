using BookStore.Data;

namespace Server;

class Program
{
    public static ApplicationContext DbContext() => new ApplicationContextFactory().CreateDbContext();
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}