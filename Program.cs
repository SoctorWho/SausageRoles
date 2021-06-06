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
            string imgurSecret = Environment.GetEnvironmentVariable("IMGUR_SECRET");
            string logChannel = Environment.GetEnvironmentVariable("LOG_CHANNEL");
            Bot b = new Bot(token, channel, imgur,imgurSecret, logChannel);
            await b.Run();
        }
    }
}
