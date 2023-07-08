using Discord;
using Discord.WebSocket;
using Discord.Audio;
using Discord.Commands;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Channels;
using System.Reflection;
using System;
using Microsoft.Extensions.DependencyInjection;
using OpenAI_API;

public class Bot
{


    //Discord
    private DiscordSocketClient _client;
    private CommandService _commands;
    private IServiceProvider _services;
    //OpenAI
    static public ChatGPT gpt = null;

    //System
    public static Task Main(string[] args) => new Bot().MainAsync();

    public async Task MainAsync()
    {

        DiscordSocketConfig config = new DiscordSocketConfig();
        config.GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.GuildVoiceStates;

        _client = new DiscordSocketClient(config);
        _commands = new CommandService();

        _client.MessageReceived += CommandsHandler;
        _client.Log += Log;

        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        var bot_key = File.ReadAllText("DISCORD_KEY.txt");

        //Start of Bot
        await _client.LoginAsync(TokenType.Bot, bot_key);
        await _client.StartAsync();
        await _client.SetStatusAsync(UserStatus.Online);

        //INITIALIZATION
        OpenAIAPI openAI_key = new OpenAIAPI(File.ReadAllText("OPENAI_KEY.TXT"));
        gpt = new ChatGPT(openAI_key);

        MyCommands my_cmd = new MyCommands();




        await Task.Delay(-1);
    }
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private async Task CommandsHandler(SocketMessage msg)
    {

        try
        {
            if (msg is not SocketUserMessage message) return;
            var context = new SocketCommandContext(_client, message);

            int argPos = 0;
            if (message.HasStringPrefix("$", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }

            switch (msg.Content)
            {
                default:
                    {
                        bool isBotMy = false;
                        foreach (var user in message.MentionedUsers)
                        {
                            if (user.Id == 1125797038932627506) { isBotMy = true; }
                        }
                        if (message.Author.Id == 464892954050560000 || (isBotMy && !message.Author.IsBot))
                        {

                            await message.ReplyAsync($"Подожди блять, я зараз тобі відповім {message.Author.Mention}, під кличкою \"Блядота\"...");
                            await gpt.Response(message);
                        }
                    }
                    break;
            }

        }
        catch (Exception ex)
        {

        }

    }


}