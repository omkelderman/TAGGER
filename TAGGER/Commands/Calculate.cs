using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

using DSharpPlus.CommandsNext;

using TAGGER.Utils;

namespace TAGGER.Commands
{
    class Calculate
    {
        private static Process proc;

        public static double Sum(double[] starList)
        {
            double result = 0;
            for (int i = 0; i < starList.Length; i++)
            {
                result += starList[i];
            }
            return result;
        }

        public static double Average(double[] avgStars)
        {
            double sum = Sum(avgStars);
            double result = (double)sum / avgStars.Length;
            return result;
        }

        public static string GetOsuFile(string link)
        {
            string newlink = link;

            if (link.Contains("beatmapsets"))
            {
                link = link.Replace("beatmapsets", "b[eatmapsets").Replace("#osu", "#osu]");
                string regex = "(\\[.*\\])";
                newlink = Regex.Replace(link, regex, "");
                Console.WriteLine(link);
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
            string id;
            string osulink = GetOsuFile(link);
            JObject json;
            double stars = 0;
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
                    stars = Math.Round(Average(tagStars), 2);
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
            }
            else
            {
                await ctx.RespondAsync($"{ctx.Member.Mention} the link you have provided is not a beatmap or invalid.");
            }
        }
    }
}
