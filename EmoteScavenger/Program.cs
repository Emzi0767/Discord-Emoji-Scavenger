using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmoteScavenger.Database;

namespace EmoteScavenger
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
                throw new Exception("You need to specify target directory.");

            var di = new DirectoryInfo(args[0]);
            if (!di.Exists)
                di.Create();

            Console.WriteLine("Beginning run");
            var exec = new AsyncExecutor();

            Console.WriteLine("Locating Discord token");
            var dbf = GetTokenLocation();
            Console.WriteLine("Token located at '{0}'", dbf.FullName);

            Console.WriteLine("Extracting token");
            var token = ExtractToken(dbf);
            Console.WriteLine("Extracted token");

            Console.WriteLine("Obtaining emoji list");
            var emojis = exec.Execute(GetEmojiListAsync, token);
            Console.WriteLine("Obtained a list of {0:#,##0} emojis", emojis.Count());

            Console.WriteLine("Beginning download");
            exec.Execute(DownloadAllEmojisAsync, (emojis, di));
            Console.WriteLine("Download completed");

            Console.WriteLine("Emoji download completed. All emojis were placed in '{0}'", di.FullName);
        }

        private static FileInfo GetTokenLocation()
        {
            var dproc = Process.GetProcesses()
                .Where(xp => xp.ProcessName.ToLowerInvariant().StartsWith("discord"))
                .FirstOrDefault(xp => xp.MainModule?.FileName != null);
            if (dproc == null)
                throw new FileNotFoundException("Could not locate Discord installation.");

            var loc = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            loc = Path.Combine(loc, dproc.ProcessName.ToLowerInvariant(), "Local Storage");

            var di = new DirectoryInfo(loc);
            var floc = di.GetFiles("*.localstorage").FirstOrDefault(xf => xf.Name.Contains("https_discordapp.com"));

            return floc;
        }

        private static string ExtractToken(FileInfo dbf)
        {
            var tfn = string.Concat(dbf.FullName, ".tmp");
            dbf = dbf.CopyTo(tfn, true);

            string token = null;
            using (var db = new StorageContext(dbf))
            {
                var tk = db.Items.FirstOrDefault(xi => xi.Key == "token");
                if (tk == null)
                    throw new KeyNotFoundException("Token not found in local storage of located Discord instance.");

                token = tk.Value?.Trim();
                if (string.IsNullOrWhiteSpace(token))
                    throw new KeyNotFoundException("Invalid token found in local storage of located Discord instance.");

                token = token.Substring(1, token.Length - 2);
            }

            dbf.Delete();

            return token;
        }

        private static async Task<IEnumerable<Emoji>> GetEmojiListAsync(string token)
        {
            var scav = new Scavenger(token);
            scav.MessageLogged += OnLog;
            await scav.BeginAsync();
            var emojis = await scav.GetEmojiAsync();
            await scav.EndAsync();

            return emojis;
        }

        private static async Task DownloadAllEmojisAsync((IEnumerable<Emoji> emojis, DirectoryInfo target) data)
        {
            var mdl = new MultiDownloader(data.target, Environment.ProcessorCount * 2);
            mdl.MessageLogged += OnLog;
            await mdl.DownloadAllAsync(data.emojis);
        }

        private static void OnLog(string msg)
            => Console.WriteLine(msg);
    }
}
