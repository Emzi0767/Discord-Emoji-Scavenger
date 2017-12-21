using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EmoteScavenger
{
    public class MultiDownloader
    {
        private const string SOURCE_URL = "https://cdn.discordapp.com/emojis/{0}.png";

        private HttpClient Http { get; }
        private SemaphoreSlim DownloadSemaphore { get; }
        private DirectoryInfo TargetDirectory { get; }

        public MultiDownloader(DirectoryInfo target, int concurrencyLevel)
        {
            this.Http = new HttpClient()
            {
                BaseAddress = new Uri("https://cdn.discordapp.com/emojis/")
            };

            this.DownloadSemaphore = new SemaphoreSlim(concurrencyLevel, concurrencyLevel);
            this.TargetDirectory = target;
        }

        public Task DownloadAllAsync(IEnumerable<Emoji> emojis)
        {
            var ivs = Path.GetInvalidFileNameChars();
            var dns = emojis
                .GroupBy(xe => xe.GuildId)
                .Select(xg => (xg.Key, xg.First().GuildName));
            var dirs = new Dictionary<ulong, DirectoryInfo>();
            
            foreach (var xt in dns)
            {
                var s = $"{xt.Key} - {xt.GuildName}";
                s = this.NormalizeString(s, ivs);
                s = Path.Combine(this.TargetDirectory.FullName, s);

                var di = new DirectoryInfo(s);
                dirs[xt.Key] = di;
                di.Create();
            }

            var tasks = new List<Task>();
            foreach (var emoji in emojis)
            {
                var td = dirs[emoji.GuildId];
                var tf = Path.Combine(td.FullName, $"{emoji.Name}.png");

                tasks.Add(DownloadAsync(emoji, new FileInfo(tf)));
            }

            return Task.WhenAll(tasks);
        }

        private async Task DownloadAsync(Emoji emoji, FileInfo targetFile)
        {
            await this.DownloadSemaphore.WaitAsync();

            try
            {
                using (var res = await this.Http.GetAsync(string.Format(SOURCE_URL, emoji.Id)))
                using (var str = await res.Content.ReadAsStreamAsync())
                using (var fs = targetFile.Create())
                    await str.CopyToAsync(fs);

                this.LogCompletion(emoji);
            }
            catch (Exception ex)
            {
                this.LogFailure(emoji, ex);
            }
            finally
            {
                this.DownloadSemaphore.Release();
            }
        }

        private void LogCompletion(Emoji e)
        {
            if (this.MessageLogged == null)
                return;

            this.MessageLogged($"Emoji download completed: {e.Name}/{e.Id}");
        }

        private void LogFailure(Emoji e, Exception ex)
        {
            if (this.MessageLogged == null)
                return;

            this.MessageLogged($"FAIL: {e.Name}/{e.Id} ({ex.GetType()}: {ex.Message})");
        }

        private string NormalizeString(string val, char[] illegals)
        {
            var x = val.ToCharArray();
            var c = false;
            for (var i = 0; i < x.Length; i++)
            {
                if (illegals.Contains(x[i]))
                {
                    c = true;
                    x[i] = '_';
                }
            }
            if (c)
                return new string(x);
            return val;
        }

        public event MessageLoggedEventHandler MessageLogged;
    }
}
