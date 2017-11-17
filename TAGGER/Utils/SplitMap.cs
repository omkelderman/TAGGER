using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;

namespace TAGGER.Utils
{
    class SplitMap
    {
        public static void Split(int players, string mapId)
        {
            try
            {
                int combo = 0;
                bool shared = true;
                bool ignore = true;
                string path = "Temp/";
                List<StreamWriter> list = new List<StreamWriter>();
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string[] map = File.ReadAllLines(path + mapId);

                for (int i = 0; i < players; i++)
                {
                    FileStream create = File.Create($"{path}{mapId}_{i}");
                    create.Close();
                    try
                    {
                        StreamWriter file = new StreamWriter($"{path}{mapId}_{i}");
                        list.Add(file);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                StreamWriter[] player = list.ToArray();

                for (int i = 0; i < map.Length; i++)
                {
                    if (!shared)
                    {
                        string[] split = map[i].Split(',');
                        string checkCombo = split[3];
                        if (checkCombo == "4" || checkCombo == "5" || checkCombo == "6" || checkCombo == "70")
                        {
                            if (!ignore)
                            {
                                if (combo == players - 1)
                                {
                                    combo = 0;
                                }
                                else
                                {
                                    combo++;
                                }
                            }
                            player[combo].WriteLine(map[i]);
                            ignore = false;
                        }
                        else
                        {
                            if (ignore)
                                ignore = false;
                            if (checkCombo == "8" || checkCombo == "12")
                            {
                                for (int j = 0; j < players; j++)
                                {
                                    player[j].WriteLine(map[i]);
                                }
                            }
                            else
                            {
                                player[combo].WriteLine(map[i]);
                            }
                        }
                    }
                    if (map[i].Contains("[HitObjects]"))
                    {
                        for (int j = 0; j < players; j++)
                        {
                            player[j].WriteLine(map[i]);
                        }
                        shared = false;
                    }
                    if (shared)
                    {
                        for (int j = 0; j < players; j++)
                        {
                            player[j].WriteLine(map[i]);
                        }
                    }
                }

                for (int j = 0; j < player.Length; j++)
                {
                    player[j].Close();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
