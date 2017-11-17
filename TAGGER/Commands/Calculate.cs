using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json.Linq;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;

using TAGGER.Utils;

namespace TAGGER.Commands
{
    class Calculate
    {
        private static Process proc;

        public static string GetOsuFile(string link)
        {
            string newlink = link;

            if (link.Contains("beatmapsets"))
            {
                link = link.Replace("beatmapsets", "b[eatmapsets").Replace("#osu", "#osu]");
                string regex = "(\\[.*\\])";
                newlink = Regex.Replace(link, regex, "");
            }

            string cleanlink = newlink;
            if (newlink.Contains("&m=") || newlink.Contains("?m="))
            {
                cleanlink = newlink.Remove(newlink.Length - 4);
            }
            string finallink = cleanlink.Replace("/b/", "/osu/");
            finallink = finallink.Replace("https://", "http://");
            return finallink;
        }

        public static JObject Oppai(string name)
        {
            JObject json;
            string line = "";
            proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "oppai.exe",
                    Arguments = $"Temp/{name} -ojson",
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
        public static void Curl(string link, string id)
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
            try
            {
                string id;
                string osulink = GetOsuFile(link);
                JObject json;
                double stars = 0;
                DiscordEmbedBuilder playerStars = new DiscordEmbedBuilder();
                if (osulink.Contains("http://osu.ppy.sh") && !osulink.Contains("/s/"))
                {
                    id = osulink.Substring(22);
                    Curl(osulink, id);
                    proc.WaitForExit();
                    if (tag != 1 || tag != 0)
                    {
                        List<double> tagStarsList = new List<double>();
                        SplitMap.Split(tag, id);
                        for (int i = 0; i < tag; i++)
                        {
                            json = Oppai($"{id}_{i}");
                            proc.WaitForExit();
                            stars = Convert.ToDouble(json.GetValue("stars"));
                            tagStarsList.Add(stars);
                            File.Delete($"Temp/{id}_{i}");
                        }
                        double[] tagStars = tagStarsList.ToArray();
                        for (int i = 0; i < tag; i++)
                        {
                            int j = i + 1;
                            playerStars.AddField("Player " + j, Math.Round(tagStars[i], 2).ToString(), true);
                        }
                        playerStars.WithFooter("Provided by http://tag.tayo.ws/");
                        stars = Math.Round(tagStars.Average(), 2);
                        File.Delete($"Temp/{id}");
                    }
                    else
                    {
                        json = Oppai(id);
                        proc.WaitForExit();
                        stars = Convert.ToDouble(json.GetValue("stars"));
                        stars = Math.Round(stars, 2);
                        File.Delete($"Temp/{id}");
                    }
                    await ctx.RespondAsync($"{ctx.Member.Mention} the amount of stars for the given beatmap with {tag} player(s) is {stars}*");
                    if (tag != 1 || tag != 0)
                        await ctx.RespondAsync(null, false, playerStars);
                }
                else
                {
                    await ctx.RespondAsync($"{ctx.Member.Mention} the link you have provided is not a beatmap or invalid.");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
