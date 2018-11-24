namespace SimpleAbilityLeveling
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;

    using Ensage;
    using Ensage.Common.Extensions;

    internal class AbilityBuilder
    {
        private readonly AbilityNames abilityNames;

        private readonly Hero hero;

        private readonly List<Tuple<float, Dictionary<uint, string>>> rawBuilds = new List<Tuple<float, Dictionary<uint, string>>>();

        private Dictionary<uint, Ability> bestBuild = new Dictionary<uint, Ability>();

        private bool error;

        public AbilityBuilder(Hero hero)
        {
            this.hero = hero;
            abilityNames = new AbilityNames();

            SaveAbilityBuild(GetDotabuffName(hero.NetworkName));
        }

        public string BestBuildWinRate { get; private set; }

        public Ability GetAbility()
        {
            if (error)
            {
                return null;
            }

            Ability ability;

            var abilityLevels = (uint)hero.Spellbook.Spells
                                    .Where(x => !x.IsHidden && !x.AbilityBehavior.HasFlag(AbilityBehavior.NotLearnable))
                                    .Sum(x => x.Level) + 1;

            return bestBuild.TryGetValue(abilityLevels, out ability) ? ability : hero.Spellbook.Spells.FirstOrDefault();
        }

        public IEnumerable<Ability> GetBestBuild()
        {
            return bestBuild.OrderBy(x => x.Key).Select(x => x.Value);
        }

        private static string GetDotabuffName(string networkName)
        {
            switch (networkName)
            {
                case "CDOTA_Unit_Hero_DoomBringer":
                    return "doom";
                case "CDOTA_Unit_Hero_Furion":
                    return "natures-prophet";
                case "CDOTA_Unit_Hero_Magnataur":
                    return "magnus";
                case "CDOTA_Unit_Hero_Necrolyte":
                    return "necrophos";
                case "CDOTA_Unit_Hero_Nevermore":
                    return "shadow-fiend";
                case "CDOTA_Unit_Hero_Obsidian_Destroyer":
                    return "outworld-devourer";
                case "CDOTA_Unit_Hero_Rattletrap":
                    return "clockwerk";
                case "CDOTA_Unit_Hero_Shredder":
                    return "timbersaw";
                case "CDOTA_Unit_Hero_SkeletonKing":
                    return "wraith-king";
                case "CDOTA_Unit_Hero_Wisp":
                    return "io";
                case "CDOTA_Unit_Hero_Zuus":
                    return "zeus";
                case "CDOTA_Unit_Hero_Windrunner":
                    return "windranger";
                case "CDOTA_Unit_Hero_Life_Stealer":
                    return "lifestealer";
                case "CDOTA_Unit_Hero_Treant":
                    return "treant-protector";
                case "CDOTA_Unit_Hero_MonkeyKing":
                    return "monkey-king";
                case "CDOTA_Unit_Hero_AbyssalUnderlord":
                    return "underlord";
            }

            var name = networkName.Substring("CDOTA_Unit_Hero_".Length).Replace("_", string.Empty);
            var newName = new StringBuilder(name[0].ToString());

            foreach (var ch in name.Skip(1))
            {
                if (char.IsUpper(ch))
                {
                    newName.Append('-');
                }
                newName.Append(ch);
            }

            return newName.ToString().ToLower();
        }

        private void GetBestWinRateBuild()
        {
            var best = rawBuilds.OrderByDescending(x => x.Item1).First();
            bestBuild = best.Item2.ToDictionary(x => x.Key, x => hero.FindSpell(x.Value));
            BestBuildWinRate = best.Item1 + "%";
        }

        private void SaveAbilityBuild(string heroName)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                string html;
                var webRequest = WebRequest.CreateHttp("http://www.dotabuff.com/heroes/" + heroName + "/builds");
                webRequest.UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";

                using (var responseStream = webRequest.GetResponse().GetResponseStream())
                {
                    using (var streamReader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        html = streamReader.ReadToEnd();
                    }
                }

                var abilityBuild = Regex.Match(html, @"<div class=""skill-build"">.+?</div></div></div></div>");

                while (abilityBuild.Success)
                {
                    var winRate = Regex.Match(abilityBuild.Value, @"\>(\d{1,3}\.\d{1,2})\%\<").NextMatch();
                    var ability = Regex.Match(abilityBuild.Value, @"<div class=""skill"">.+?</div></div></div>");

                    var saveBuild = new Dictionary<uint, string>();

                    while (ability.Success)
                    {
                        var abilityName = Regex.Match(ability.Value, @"<img alt=""(.+?)""");
                        var dotaAbilityName = string.Empty;

                        if (abilityName.Success)
                        {
                            var name = abilityName.Groups[1].Value.Replace("&#39;", "\'");

                            if (name.Contains("Talent:"))
                            {
                                break;
                            }

                            if (heroName == "queen-of-pain" && name == "Blink")
                            {
                                // Anti-Mage Blink conflict fix
                                name = "Queen of Pain Blink";
                            }
                            else if (heroName == "shadow-shaman" && name == "Hex")
                            {
                                // Lion Hex conflict fix
                                name = "Shadow Shaman Hex";
                            }

                            abilityNames.Names.TryGetValue(name, out dotaAbilityName);
                        }

                        if (string.IsNullOrEmpty(dotaAbilityName))
                        {
                            Game.PrintMessage(
                                "<font color='#FF0000'>[Simple Ability Leveling] Ability " + abilityName.Groups[1].Value + " not found</font>");
                            error = true;
                        }

                        var level = Regex.Match(ability.Value, @"\>(\d{1,2})\<");

                        while (level.Success)
                        {
                            saveBuild.Add(uint.Parse(level.Groups[1].Value), dotaAbilityName);
                            level = level.NextMatch();
                        }

                        ability = ability.NextMatch();
                    }

                    rawBuilds.Add(Tuple.Create(float.Parse(winRate.Groups[1].Value), saveBuild));
                    abilityBuild = abilityBuild.NextMatch();
                }
                GetBestWinRateBuild();
            }
            catch (Exception)
            {
                Game.PrintMessage(
                    "<font color='#FF0000'>[Simple Ability Leveling] Something went wrong with " + hero.GetRealName() + " build</font>");
                error = true;
            }
        }
    }
}