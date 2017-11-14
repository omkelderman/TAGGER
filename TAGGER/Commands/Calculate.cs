using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

using DSharpPlus.CommandsNext;

namespace TAGGER.Commands
{
    class Calculate
    {
        private static Process proc;

        public static string GetOsuFile(string link)
        {
            string cleanlink = link;
            if (link.Contains("&m="))
            {
                cleanlink = link.Remove(link.Length - 4);
            }
            string finallink = cleanlink.Replace("/b/", "/osu/");
            finallink = finallink.Replace("https://", "http://");
            return finallink;
        }

        public static JObject Oppai(string id)
        {
            JObject json;
            string line = "";
            proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "oppai.exe",
                    Arguments = $"Temp/{id} -ojson",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                line = proc.StandardOutput.ReadLine();
            }
            json = JObject.Parse(line);
            return json;
        }
        public static async Task Curl(string link, string id)
        {
            proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "curl.exe",
                    Arguments = $"{link} -o Temp/{id}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
        }

        public async static Task SR(CommandContext ctx, string link, int tag)
        {
            string id;
            string osulink = GetOsuFile(link);
            JObject json;
            double stars;
            if (osulink.Contains("http://osu.ppy.sh") && !osulink.Contains("/s/"))
            {
                id = osulink.Substring(22);
                await Curl(osulink, id);
                proc.WaitForExit();
                json = Oppai(id);
                proc.WaitForExit();
                stars = Convert.ToDouble(json.GetValue("stars"));
                stars = Math.Round(stars, 2);
                File.Delete($"Temp/{id}");
                await ctx.RespondAsync($"{ctx.Member.Mention} the amount of stars for the given beatmap with {tag} player(s) is {stars}*");
            }
            else
            {
                await ctx.RespondAsync($"{ctx.Member.Mention} the link you have provided is not a beatmap or invalid.");
            }
        }
    }
}
