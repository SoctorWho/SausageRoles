using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Rest;

using ScottPlot;
using ScottPlot.Drawing;

using Imgur.API.Endpoints;
using Imgur.API.Authentication;


namespace SausageRolls
{
    public class Bot
    {
        string[] WantedRoles;

        ulong channel;
        ulong logChannel;
        public Bot(string Token, string Channel, string Imgur,string ImgurSecret, string LogChannel)
        {
            _client = new DiscordSocketClient();
            loginAwaiter = _client.LoginAsync(TokenType.Bot, Token);

            _client.GuildMemberUpdated += PostGraph;

            WantedRoles = File.ReadAllLines("roles");

            channel = Convert.ToUInt64(Channel);
            logChannel = Convert.ToUInt64(LogChannel);
            apiClient = new ApiClient(Imgur, ImgurSecret);
        }

        private async Task PostGraph(SocketGuildUser a, SocketGuildUser b)
        {
            var lchan = a.Guild.GetTextChannel(logChannel);

            await lchan.SendMessageAsync($"Update in Server: {a.Guild.Name}");
            var roles = a.Guild.Roles
                .Where(r => WantedRoles.Contains(r.Name))
                .OrderBy(r => Array.IndexOf(WantedRoles, r.Name));
            string[] labels = roles.Select(r => r.Name).ToArray();
            double[] values = roles.Select(r => (double)r.Members.Count()).ToArray();

            string file = MakeGraph(labels, values);
            await lchan.SendMessageAsync($"Made Graph");

            string url;
               using(var wc = new WebClient()){
                var x = wc.UploadFile("transfer.sh/members.png", "PUT", file);
                url = Encoding.UTF8.GetString(x);
               }
            
            await lchan.SendMessageAsync($"Uploaded image to {url}");

            var chan = a.Guild.GetTextChannel(channel);
            bool processed = false;
            var ms = chan.GetPinnedMessagesAsync();
            foreach (var msg in await ms)
            {
                
                if (msg.Author.Id == _client.CurrentUser.Id)
                    {
                        await lchan.SendMessageAsync($"Updating message in <#{channel}>");
                        processed = true;

                        await (msg as RestUserMessage).ModifyAsync(x => { x.Content = url; });
                        break;
                    }
                
            }
            if (!processed)
            {
                await lchan.SendMessageAsync($"Sending message to <#{channel}>");
                var m = await chan.SendMessageAsync(url);
                await m.PinAsync();
            }

        }

        private string MakeGraph(string[] labels, double[] values)
        {

            double[] xs = DataGen.Consecutive(labels.Count());


            var plt = new ScottPlot.Plot(1920, 1080);
            plt.PlotBar(xs, values);

            // customize the plot to make it look nicer
            plt.Grid(enableVertical: false, lineStyle: LineStyle.Dot);
            plt.XTicks(xs, labels);
            plt.Ticks(fontSize: 25, xTickRotation: 45);
            plt.Layout(yScaleWidth: 80, titleHeight: 50, xLabelHeight: 200, y2LabelWidth: 20);
            string path = "graph.png";
            plt.SaveFig(path);

            return path;
        }

        private DiscordSocketClient _client;
        private ApiClient apiClient;
        private Task loginAwaiter;

        public async Task Run()
        {
            await loginAwaiter;
            await _client.StartAsync();
            await Task.Delay(-1);
        }

    }
}