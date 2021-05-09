using System;
using System.Threading.Tasks;

namespace SausageRolls
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string token = Environment.GetEnvironmentVariable("TOKEN");
            string channel = Environment.GetEnvironmentVariable("CHANNEL");
            string imgur = Environment.GetEnvironmentVariable("IMGUR");
            Bot b = new Bot(token, channel, imgur);
            await b.Run();
        }
    }
}
