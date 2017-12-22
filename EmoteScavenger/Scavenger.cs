using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace EmoteScavenger
{
    public class Scavenger
    {
        public DiscordClient Discord { get; }
        private TaskCompletionSource<IEnumerable<Emoji>> EmojiTask { get; }
        private ConcurrentQueue<DiscordGuild> GuildsToSync { get; }
        private ConcurrentBag<Emoji> Emojis { get; }

        public Scavenger(string token)
        {
            // init the task
            this.EmojiTask = new TaskCompletionSource<IEnumerable<Emoji>>();

            // init data holders
            this.GuildsToSync = new ConcurrentQueue<DiscordGuild>();
            this.Emojis = new ConcurrentBag<Emoji>();

            // init discord
            this.Discord = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.User,
                LogLevel = LogLevel.Info,
                AutomaticGuildSync = false
            });

            this.Discord.Ready += this.Discord_Ready;
            this.Discord.GuildAvailable += this.Discord_GuildAvailable;
            this.Discord.DebugLogger.LogMessageReceived += this.DebugLogger_LogMessageReceived;
        }

        public Task BeginAsync()
            => this.Discord.ConnectAsync();

        public Task EndAsync()
            => this.Discord.DisconnectAsync();

        public Task<IEnumerable<Emoji>> GetEmojiAsync()
            => this.EmojiTask.Task;

        private Task Discord_Ready(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Scavenger", "Ready to process emotes", DateTime.Now);

            foreach (var xg in e.Client.Guilds.Values)
                this.GuildsToSync.Enqueue(xg);

            if (this.GuildsToSync.TryDequeue(out var gld))
                return this.Discord.SyncGuildsAsync(gld);

            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Scavenger", "No guilds to process", DateTime.Now);
            this.EmojiTask.SetResult(new List<Emoji>());
            return Task.CompletedTask;
        }

        private Task Discord_GuildAvailable(GuildCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Scavenger", $"Found guild {e.Guild.Name} with {e.Guild.Emojis.Count:#,##0} emojis", DateTime.Now);

            var xg = e.Guild;
            foreach (var xe in xg.Emojis)
                this.Emojis.Add(new Emoji(xe.Name, xe.Id, xg.Name, xg.Id, xe.Url, xe.IsAnimated));

            if (this.GuildsToSync.TryDequeue(out var gld))
                return e.Client.SyncGuildsAsync(gld);

            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Scavenger", "All emotes processed", DateTime.Now);
            this.EmojiTask.SetResult(new List<Emoji>(this.Emojis));
            return Task.CompletedTask;
        }

        private void DebugLogger_LogMessageReceived(object sender, DebugLogMessageEventArgs e)
        {
            var msg = $"DISCORD: [{e.Application}] {e.Message}";
            if (this.MessageLogged != null)
                this.MessageLogged(msg);
        }

        public event MessageLoggedEventHandler MessageLogged;
    }
}
