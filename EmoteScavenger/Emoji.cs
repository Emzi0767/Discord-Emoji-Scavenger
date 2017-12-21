using System;

namespace EmoteScavenger
{
    public struct Emoji
    {
        public string Name { get; }
        public ulong Id { get; }
        public string GuildName { get; }
        public ulong GuildId { get; }

        public Emoji(string name, ulong id, string guildName, ulong guildId)
        {
            this.Name = name;
            this.Id = id;
            this.GuildName = guildName;
            this.GuildId = guildId;
        }

        public Uri GetUri()
            => new Uri($"https://cdn.discordapp.com/emojis/{this.Id}.png");
    }
}
