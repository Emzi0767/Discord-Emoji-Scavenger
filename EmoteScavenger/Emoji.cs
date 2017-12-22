namespace EmoteScavenger
{
    public struct Emoji
    {
        public string Name { get; }
        public ulong Id { get; }
        public string GuildName { get; }
        public ulong GuildId { get; }
        public string Url { get; }
        public bool IsAnimated { get; }

        public Emoji(string name, ulong id, string guildName, ulong guildId, string url, bool anim)
        {
            this.Name = name;
            this.Id = id;
            this.GuildName = guildName;
            this.GuildId = guildId;
            this.Url = url;
            this.IsAnimated = anim;
        }
    }
}
