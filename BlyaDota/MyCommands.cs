using Discord.Commands;
using Discord;
using System.Reflection;
using System.Windows.Input;
using Discord.WebSocket;

public class MyCommands : ModuleBase<SocketCommandContext>
{
    [Command("join", RunMode = RunMode.Async)]
    public async Task JoinChannel(IVoiceChannel channel = null)
    {
        channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
        if (channel == null) { await Context.Channel.SendMessageAsync("Ти дебіл."); return; }

        var audioClient = await channel.ConnectAsync();
    }
    
    [Command("restart", RunMode = RunMode.Async)]
    public async Task Restart()
    {
        Bot.gpt.CreateChat();
        await Context.Message.ReplyAsync("Зроблено мій володарю.");
        
    }
}
