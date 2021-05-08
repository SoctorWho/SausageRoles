using System;
using System.Threading.Tasks;

namespace SausageRolls
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string token = Environment.GetEnvironmentVariable("TOKEN");

            Bot b = new Bot(token);
            await b.Run();
        }
    }
}
