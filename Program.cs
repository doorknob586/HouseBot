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
using RestSharp;


namespace HouseBot;

static class Program
{
    static DiscordSocketClient client = new DiscordSocketClient(new DiscordSocketConfig()
    {
        GatewayIntents = GatewayIntents.All
    });
    static void Main() => MainAsync().GetAwaiter().GetResult();

    static async Task MainAsync()
    {
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
        await message.Channel.SendMessageAsync(await GetResponceAsync(message.Content));

    }
    static async Task<string> GetResponceAsync(string message)
    {
        string apiKey = Environment.GetEnvironmentVariable("APIKEY");
        const string apiUrl = "https://api.openai.com/v1/engines/davinci-codex/completions";

        RestClient restClient = new RestClient(apiUrl);
        restClient.AddDefaultHeader("Authorization", $"Bearer {apiKey}");
        RestSharp.RestRequest request = new RestSharp.RestRequest();
        request.AddJsonBody(new
        {
            prompt = $">>User:{message}",
            max_tokens = 50,
            n = 1,
            stop = "\n",
            temperature = 0.5,
        });
        request.Method = Method.Post;
        var response = await restClient.ExecuteAsync(request);
        if (response.IsSuccessful)
        {
            JObject responseData = JObject.Parse(response.Content);
            string chatGptResponse = responseData["choices"][0]["text"].ToString().Trim();
            return chatGptResponse;
        }
        else
        {
            return "An error occureds";
        }
    }
    static async Task BotReady()
    {
    }
}