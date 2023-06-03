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
        await Task.Delay(-1);
    }
    static Task SubscribeToEvents()
    {
        return Task.CompletedTask;
    }
}