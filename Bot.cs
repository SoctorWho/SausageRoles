using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;

namespace SausageRolls
{
    public class Bot
    {
        List<string> WantedRoles;
        public Bot(string Token)
        {
            _client = new DiscordSocketClient();
            login_awaiter = _client.LoginAsync(TokenType.Bot, Token);

            _client.GuildMemberUpdated += PostGraph;

        }

        private Task PostGraph(SocketGuildUser a, SocketGuildUser b){
            return Task.CompletedTask;
        }

        private DiscordSocketClient _client;
        private Task login_awaiter;

        public async Task Run()
        {
            await login_awaiter;
            await _client.StartAsync();
            await Task.Delay(-1);
        }

    }
}