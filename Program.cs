using Discord;
using Discord.API;
using Discord.Audio;
using Discord.Audio.Streams;
using Discord.Commands.Builders;
using Discord.Commands;
using Discord.Interactions.Builders;
using Discord.Interactions;
using Discord.Net.Converters;
using Discord.Net;
using Discord.Net.ED25519;
using Discord.Net.Queue;
using Discord.Net.Rest;
using Discord.Net.Udp;
using Discord.Net.WebSockets;
using Discord.Rest;
using Discord.Utils;
using Discord.Webhook;
using Discord.WebSocket;
using DotNetEnv;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net;


namespace HouseBot;

static class Program
{
    static void StartWebServer()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://*:8000/");
        listener.Start();
        Console.WriteLine("Web server running on port 8000");

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerResponse response = context.Response;

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Your bot is running");
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.Close();
        }  
    }
    static DiscordSocketClient client = new DiscordSocketClient(new DiscordSocketConfig()
    {
        GatewayIntents = GatewayIntents.All
    });
    static void Main() => MainAsync().GetAwaiter().GetResult();

    static async Task MainAsync()
    {
        StartWebServer();
        Env.Load();
        await SubscribeToEvents();
        await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("TOKEN"));
        await client.StartAsync();
        await Task.Delay(-1);
    }
    static Task SubscribeToEvents()
    {
        client.Ready += BotReady;
        client.MessageReceived += MessageReceived;
        client.Log += Log;
        return Task.CompletedTask;
    }

    static Task Log(LogMessage message)
    {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }

    static async Task MessageReceived(SocketMessage message)
    {
        if (message.Author.IsBot || message.Author.IsWebhook) return;
        if (new Random().Next(0,100) == 50)
        {
            message.Channel.SendMessageAsync(responses[new Random().Next(0,responses.Length)]);
        }

    }
    static string[] responses = {"yes", "no", "maybe","chicken","not no", "not yes","idk","how am i supposed to know"};
    static async Task BotReady()
    {
        new Thread(() => 
        {
            Thread.Sleep(new Random().Next(50,10000000));
            foreach(SocketGuild guild in client.Guilds)
            {
                foreach(SocketTextChannel channel in guild.Channels)
                {
                    channel.SendMessageAsync(responses[new Random().Next(0, responses.Length)]);
                }
            }

        }).Start();
    }
}