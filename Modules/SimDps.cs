using Discord.Commands;
using SimDpsBot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Discord.Rest;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;

namespace TemplateBot.Modules
{
    public class SimDpsCommands : ModuleBase<SocketCommandContext>
    {
        public SettingsDTO nullSettings(SettingsDTO settings, SocketCommandContext context)
        {
            settings.DefaultRealm = null;
            settings.Fight_Length = null;
            settings.Fight_Style = null;
            settings.FTP = new FTPDTO();
            settings.FTPWebAdress = null;
            settings.GuildID = context.Guild.Id;
            settings.Interations = null;
            settings.Pawn = false;
            settings.SendFileAfterSim = false;
            settings.Targets = null;
            settings.UseFTP = false;
            settings.World = null;

            return settings;
        }
        [Command("skip")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task skip()
        {
            CommandHandler.skip = true;
            await Context.Channel.SendMessageAsync("Skipping sim");
        }
        [Command("fightstyles")]
        [Alias("fs")]

        public async Task fightstyles()
        {


            EmbedBuilder eb = new EmbedBuilder();
            eb.WithColor(new Color(1f, 1f, 1f));
            eb.Title = "Fight Styles";

            eb.Description =
            "```" +
            "1 = Patchwerk" + Environment.NewLine +
            "2 = HecticAddCleave" + Environment.NewLine +
            "3 = LightMovement" + Environment.NewLine +
            "4 = HeavyMovement" + Environment.NewLine +
            "5 = CastingPatchwerk" + Environment.NewLine +
            "6 = HelterSkelter" + Environment.NewLine +
            "7 = Ultraxion" + Environment.NewLine +
            "8 = Beastlord" + Environment.NewLine +
            "```"

            ;

            await Context.Channel.SendMessageAsync("", false, eb.Build());


        }
        [Command("world")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task worldSetting()
        {

            if (CommandHandler.Settings.Exists(x => x.GuildID == Context.Guild.Id))
            {
                var item = CommandHandler.Settings.Find(x => x.GuildID == Context.Guild.Id);
                if (string.IsNullOrEmpty(item.World) || item.World.ToLower() == "eu")
                {
                    item.World = "us";
                    await SaveSettings();
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "World have been set to  US", ""));

                }
                else if (item.World.ToLower() == "us")
                {
                    item.World = "eu";
                    await SaveSettings();
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "World have been set to  EU", ""));
                }
            }
            else
            {
                var item = new SettingsDTO();
                CopyProperties(item, nullSettings(item, Context));
                if (string.IsNullOrEmpty(item.World) || item.World.ToLower() == "eu")
                {
                    item.World = "us";
                    CommandHandler.Settings.Add(item);
                    await SaveSettings();
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "World have been set to  US", ""));

                }
                else if (item.World.ToLower() == "us")
                {
                    item.World = "eu";
                    CommandHandler.Settings.Add(item);
                    await SaveSettings();
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "World have been set to  EU", ""));
                }
            }


        }
        [Command("DefaultRealm")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task DefaultRealmSetting(string json)
        {
            if (CommandHandler.Settings.Exists(x => x.GuildID == Context.Guild.Id))
            {
                var item = CommandHandler.Settings.Find(x => x.GuildID == Context.Guild.Id);
                item.DefaultRealm = json.ToString();
                await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Default Realm is now set to "+ json, ""));
                await SaveSettings();
            }
            else
            {
                var item = new SettingsDTO();
                CopyProperties(item, nullSettings(item, Context));
                item.DefaultRealm = json.ToString();
                CommandHandler.Settings.Add(item);
                await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Default Realm is now set to " + json, ""));
                await SaveSettings();
            }

        }
        [Command("Interations")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task InterationsSetting(int json)
        {
            if (CommandHandler.Settings.Exists(x => x.GuildID == Context.Guild.Id))
            {
                var item = CommandHandler.Settings.Find(x => x.GuildID == Context.Guild.Id);
                item.Interations = json.ToString();
                await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Interations is now set to " + json.ToString(), ""));
                await SaveSettings();
            }
            else
            {
                var item = new SettingsDTO();
                CopyProperties(item, nullSettings(item, Context));
                item.Interations = json.ToString();
                CommandHandler.Settings.Add(item);
                await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Interations is now set to " + json.ToString(), ""));
                await SaveSettings();
            }

        }
        [Command("Fight_Style")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Fight_StyleSetting( string json)
        {
            if (CommandHandler.Settings.Exists(x => x.GuildID == Context.Guild.Id))
            {
                var item = CommandHandler.Settings.Find(x => x.GuildID == Context.Guild.Id);
                item.Fight_Style = json.ToString();
                await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Fight Style is now set to " + json , ""));
                await SaveSettings();
            }
            else
            {
                var item = new SettingsDTO();
                CopyProperties(item, nullSettings(item, Context));
                item.Fight_Style = json.ToString();
                CommandHandler.Settings.Add(item);
                await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Fight Style is now set to " + json, ""));
                await SaveSettings();
            }

        }
        [Command("Fight_Length")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Fight_LengthSetting(int json)
        {

            if (CommandHandler.Settings.Exists(x => x.GuildID == Context.Guild.Id))
            {
                var item = CommandHandler.Settings.Find(x => x.GuildID == Context.Guild.Id);
                item.Fight_Length = json.ToString();
                await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Fight Length is now set to " + json, ""));
                await SaveSettings();
            }
            else
            {
                var item = new SettingsDTO();
                CopyProperties(item, nullSettings(item, Context));
                item.Fight_Length = json.ToString();
                CommandHandler.Settings.Add(item);
                await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Fight Length is now set to " + json, ""));
                await SaveSettings();
            }

        }
        [Command("pawn")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task pawnSetting()
        {
            if (CommandHandler.Settings.Exists(x => x.GuildID == Context.Guild.Id))
            {
                var item = CommandHandler.Settings.Find(x => x.GuildID == Context.Guild.Id);
                if (item.Pawn)
                {
                    item.Pawn = false;
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Pawn is now by default off!", ""));
                }
                else if (!item.Pawn)
                {
                    item.Pawn = true;
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Pawn is now by default on!", ""));
                }
                await SaveSettings();
            }
            else
            {
                var item = new SettingsDTO();
                CopyProperties(item, nullSettings(item, Context));
                if (item.Pawn)
                {
                    item.Pawn = false;
                    CommandHandler.Settings.Add(item);
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Pawn is now by default off!", ""));
                }
                else if (!item.Pawn)
                {
                    item.Pawn = true;
                    CommandHandler.Settings.Add(item);
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Pawn is now by default on!", ""));
                }
                await SaveSettings();
            }

        }
        [Command("SendFileAfterSim")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SendFileAfterSimSetting()
        {
            if (CommandHandler.Settings.Exists(x => x.GuildID == Context.Guild.Id))
            {
                var item = CommandHandler.Settings.Find(x => x.GuildID == Context.Guild.Id);
                if (item.SendFileAfterSim)
                {
                    item.SendFileAfterSim = false;
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "The bot will now not send the HTML report after the sim!", ""));
                }
                else if (!item.SendFileAfterSim)
                {
                    item.SendFileAfterSim = true;
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "The bot will now by default send the HTML report after the sim!", ""));
                }
                await SaveSettings();
            }
            else
            {
                var item = new SettingsDTO();
                CopyProperties(item, nullSettings(item, Context));
                if (item.SendFileAfterSim)
                {
                    item.SendFileAfterSim = false;
                    CommandHandler.Settings.Add(item);
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "The bot will now not send the HTML report after the sim!", ""));
                }
                else if (!item.SendFileAfterSim)
                {
                    item.SendFileAfterSim = true;
                    CommandHandler.Settings.Add(item);
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "The bot will now by default send the HTML report after the sim!", ""));
                }
                await SaveSettings();
            }

        }
        [Command("UseFTP")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task UseFTPSetting()
        {
            if (CommandHandler.Settings.Exists(x => x.GuildID == Context.Guild.Id))
            {
                var item = CommandHandler.Settings.Find(x => x.GuildID == Context.Guild.Id);
                if (item.UseFTP)
                {
                    item.UseFTP = false;
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Uploading to FTP after SIm is now off!", ""));
                }
                else if (!item.UseFTP)
                {
                    item.UseFTP = true;
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Uploading to FTP after SIm is now on!", ""));
                }
                await SaveSettings();
            }
            else
            {
                var item = new SettingsDTO();
                CopyProperties(item, nullSettings(item, Context));
                if (item.UseFTP)
                {
                    item.UseFTP = false;
                    CommandHandler.Settings.Add(item);
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Uploading to FTP after SIm is now off!", ""));
                }
                else if (!item.UseFTP)
                {
                    item.UseFTP = true;
                    CommandHandler.Settings.Add(item);
                    await Context.Channel.SendMessageAsync("", false, SimpleEmbed(Color.DarkMagenta, "Uploading to FTP after SIm is now on!", ""));
                }
                await SaveSettings();
            }

        }
        [Command("Settings")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Settings([Optional][Remainder]  string json)
        {
            if (json == null)
            {
                if (CommandHandler.Settings.Exists(x => x.GuildID == Context.Guild.Id))
                {
                    var item = CommandHandler.Settings.Find(x => x.GuildID == Context.Guild.Id);
                    item.FTP.Password = "******";
                    string resturnjeson = JsonConvert.SerializeObject(item, Formatting.Indented);
                    await Context.Channel.SendMessageAsync("```" + resturnjeson + "```");


                }
                else
                {
                    await Context.Channel.SendMessageAsync("There are no settings for this guild!");
                }
            }
            else
            {
                try
                {
                    SettingsDTO item = null;
                    if (CommandHandler.Settings.Exists(x => x.GuildID == Context.Guild.Id))
                    {
                        item = CommandHandler.Settings.Find(x => x.GuildID == Context.Guild.Id);
                        CommandHandler.Settings.Remove(item);
                    }
                    var item2 = JsonConvert.DeserializeObject<SettingsDTO>(json);
                    item2.GuildID = Context.Guild.Id;
                    if (item != null && item2.FTP.Password == "******")
                    {
                        item2.FTP.Password = item.FTP.Password;
                    }
                    CommandHandler.Settings.Add(item2);
                    await Context.Channel.SendMessageAsync("Settings have been added!");
                    await SaveSettings();
                }
                catch (Exception)
                {
                    await Context.Channel.SendMessageAsync("Unknown error!");
                }
            }

        }
        [Command("Settings-template")]
        public async Task Settingstemp()
        {

            SettingsDTO bla = new SettingsDTO();
            bla.DefaultRealm = "silvermoon";
            bla.Fight_Style = "Patchwerk";
            bla.GuildID = Context.Guild.Id;
            bla.Interations = "1000";
            bla.Pawn = false;
            bla.Targets = "1";
            bla.World = "eu";
            FTPDTO ftp = new FTPDTO();
            bla.FTP = ftp;
            bla.FTP.Server = "test.com";
            bla.FTP.Password = "pass123";
            bla.FTP.Username = "username";
            bla.FTPWebAdress = "http://test.com";
            bla.FTP.Port = 23;

            string resturnjeson = JsonConvert.SerializeObject(bla, Formatting.Indented);
            await Context.Channel.SendMessageAsync("```" + resturnjeson + "```");



        }
        [Command("simdps")]
        [Alias("sd")]
        public async Task simdps([Remainder] string text)
        {
            if (text == "-?")
            {
                EmbedBuilder eb = new EmbedBuilder();
                eb.WithColor(new Color(1f, 1f, 1f));
                eb.Title = "How to use the .simdps command";
                eb.AddField("** **", "** **");
                eb.AddField(
                    "Usage : ", ".simdps \"USERNAME\" [\"Realm\"] [i=x] [-p] [t=x] [fs=x] [fl=x]" + Environment.NewLine +
                    ".simdps \"[SimcTEXT]\" [i=x] [-p] [t=x] [fs=x] [fl=x]");
                eb.AddField("** **", "** **");


                eb.AddField("Modifiers: ", "** **");

                eb.AddField("i=x ", "Sets the amount of runs it should do.");
                eb.AddField("t=x ", "Sets the amount of targets");
                eb.AddField("fs=x ", "Sets fight style the simulation should run on. use command .fightstyles or .fs to get the full list of supported fight styles.");
                eb.AddField("fl=x ", "Sets the fight length for the simulation in seconds.");
                eb.AddField("-p", "Enables scaleing. This makes the simulation return a pawn string and and stat prio.");



                await Context.Channel.SendMessageAsync("", false, eb.Build());

            }
            else
            {


                QueueDTO queue = new QueueDTO();
                SettingsDTO settings = new SettingsDTO();
                settings.Pawn = false;
                settings.Targets = "1";
                settings.Fight_Style = "Patchwerk";
                settings.Interations = "10000";
                settings.GuildID = Context.Guild.Id;
                settings.World = "eu";
                settings.DefaultRealm = "Silvermoon";

                if (CommandHandler.Settings.Exists(x => x.GuildID == Context.Guild.Id))
                {
                    CopyProperties(settings, CommandHandler.Settings.Find(x => x.GuildID == Context.Guild.Id));
                    //settings = CommandHandler.Settings.Find(x => x.GuildID == Context.Guild.Id);

                }

                string Text = text;
                foreach (var item in text.ToLower().Split(' '))
                {
                    if (item.StartsWith("i="))
                    {
                        Text = Text.Replace(" " + item, "");
                        settings.Interations = item.Replace("i=", "");

                    }
                    else if (item == ("-p"))
                    {
                        Text = Text.Replace(" " + item, "");
                        settings.Pawn = true;
                    }
                    else if (item.StartsWith("fs="))
                    {
                        
                        Text = Text.ToLower().Replace(" " + item, "");
                        var fightstyle = item.Replace("fs=", "");
                        if (fightstyle == "1" || item == "fs=Patchwerk".ToLower())
                        {
                            settings.Fight_Style = "Patchwerk";
                        }
                        else if (fightstyle == "2" || item == "fs=HecticAddCleave".ToLower())
                        {
                            settings.Fight_Style = "HecticAddCleave";
                        }
                        else if (fightstyle == "3" || item == "fs=LightMovement".ToLower())
                        {
                            settings.Fight_Style = "LightMovement";
                        }
                        else if (fightstyle == "4" || item == "fs=HeavyMovement".ToLower())
                        {
                            settings.Fight_Style = "HeavyMovement";
                        }
                        else if (fightstyle == "5" || item == "fs=CastingPatchwerk".ToLower())
                        {
                            settings.Fight_Style = "CastingPatchwerk";
                        }
                        else if (fightstyle == "6" || item == "fs=HelterSkelter".ToLower())
                        {
                            settings.Fight_Style = "HelterSkelter";
                        }
                        else if (fightstyle == "7" || item == "fs=Ultraxion".ToLower())
                        {
                            settings.Fight_Style = "Ultraxion";
                        }
                        else if (fightstyle == "8" || item == "fs=Beastlord".ToLower())
                        {
                            settings.Fight_Style = "Beastlord";
                        }

                    }
                    else if (item.StartsWith("fl="))
                    {
                        Text = Text.Replace(" " + item, "");
                        settings.Fight_Length = item.Replace("fl=", "");
                    }
                    else if (item.StartsWith("t="))
                    {
                        Text = Text.Replace(" " + item, "");
                        settings.Targets = item.Replace("t=", "");
                    }
                }


                string[] Split = Text.Split(' ');
                if (Split.Count() == 2)
                {
                    queue.Username = Split[0];
                    queue.Realm = Split[1];
                }
                else if (Text.Split(' ').Count() > 5 || Text.Split('\n').Count() > 5)
                {
                    queue.Text = Text;

                }
                else if (Text.Split(' ').Count() == 1)
                {
                    queue.Username = Split[0];
                    queue.Realm = settings.DefaultRealm;
                }

                string Title = "**#################=Discord_SimDPS_Bot=###################**";


                RestUserMessage msg = null;
                if (queue.Text != null)
                {
                    await Context.Message.DeleteAsync();
                    EmbedBuilder eb = new EmbedBuilder();
                    eb.WithColor(new Color(1f, 1f, 1f));
                    eb.Title = "Simulation of Text";
                    eb.Description = Title;

                    eb.AddField("Status", "Adding request...");

                    msg = await Context.Channel.SendMessageAsync("", false, eb.Build());
                }
                else
                {
                    EmbedBuilder eb = new EmbedBuilder();
                    eb.WithColor(new Color(1f, 1f, 1f));
                    eb.Title = "Simulation of " + queue.Username;
                    eb.Description = Title;

                    eb.AddField("Status", "Adding request...");

                    msg = await Context.Channel.SendMessageAsync("", false, eb.Build());
                }

                queue.Settings = settings;
                queue.MessageID = msg.Id;
                queue.ChannelID = msg.Channel.Id;
                if (queue.Text == null && !DoesCharacterExist(queue.Realm, queue.Username, queue))
                {
                    await msg.ModifyAsync(x => x.Embed = SimpleEmbed(new Color(1f, 1f, 1f), msg.Embeds.ToList()[0].Title, "Cant find player!" + Environment.NewLine + "Check that the name and realm is correct."));
                }
                else
                {
                    if (GetRole(queue.Realm, queue.Username, queue) == "HEALING")
                    {
                        await msg.ModifyAsync(x => x.Embed = SimpleEmbed(new Color(1f, 1f, 1f), msg.Embeds.ToList()[0].Title, "Simdps for healer is not supported!"));

                    }
                    else
                    {


                        int i = CommandHandler.Queue.Count + 1;
                        queue.queueSpot = i;
                        if (int.Parse(settings.Interations) < 30000)
                        {


                            if (queue.Text == null && DoesCharacterExist(queue.Realm, queue.Username, queue))
                            {
                                var character = GetCharacter(queue.Realm, queue.Username, queue);


                                var eb = CommandHandler.ChangeStatus(msg, "Your Request is number " + i + " in queue.");
                                if (queue.Settings.World.ToLower() == "eu")
                                {
                                    eb.ThumbnailUrl = @"https://render-eu.worldofwarcraft.com/character/" + character.thumbnail;
                                }
                                if (queue.Settings.World.ToLower() == "us")
                                {
                                    eb.ThumbnailUrl = @"https://render-us.worldofwarcraft.com/character/" + character.thumbnail;
                                }


                                await msg.ModifyAsync(x => x.Embed = eb.Build());

                            }
                            else
                            {
                                var eb = CommandHandler.ChangeStatus(msg, "Your Request is number " + i + " in queue.");
                                await msg.ModifyAsync(x => x.Embed = eb.Build());
                            }
                            CommandHandler.Queue.Add(queue);
                        }
                        else
                        {
                            var eb = CommandHandler.ChangeStatus(msg, "The request have to many Interations!");
                            await msg.ModifyAsync(x => x.Embed = eb.Build());

                        }
                    }
                }

            }
        }


        public static Embed SimpleEmbed(Color c, string title, string description)
        {
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithColor(c);

            eb.Title = title;
            eb.WithDescription(description);


            return eb.Build();
        }
        public static Embed SimpleEmbed(Color c, string title, string description, string[] inline)
        {
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithColor(c);
            foreach (var item in inline)
            {
                var split = item.Split('#');
                eb.AddField(split[0], split[1], true);
            }
            eb.Title = title;
            eb.WithDescription(description);


            return eb.Build();
        }
        public WowCharacter GetCharacter(string realm, string player, QueueDTO queue)
        {
            try
            {
                if (queue.Settings.World.ToLower() == "eu")
                {
                    var UserString = HTTPGET("https://eu.api.battle.net/wow/character/", realm + "/" + player + "?locale=en_GB&apikey=######################################");
                    var c = JsonConvert.DeserializeObject<WowCharacter>(UserString);
                    return c;
                }
                else if (queue.Settings.World.ToLower() == "us")
                {
                    var UserString = HTTPGET("https://us.api.battle.net/wow/character/", realm + "/" + player + "?locale=en_US&apikey=######################################");
                    var c = JsonConvert.DeserializeObject<WowCharacter>(UserString);
                    return c;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception)
            {
                return null;
            }
        }
        public string GetRole(string realm, string player, QueueDTO queue)
        {
            try
            {
                if (queue.Settings.World.ToLower() == "eu")
                {

                    var UserString = HTTPGET("https://eu.api.battle.net/wow/character/", realm + "/" + player + "?fields=talents&locale=en_GB&apikey=######################################");
                    var c = JsonConvert.DeserializeObject<WowTalentCharacter>(UserString);
                    return c.talents[0].spec.role;
                }
                else if (queue.Settings.World.ToLower() == "us")
                {
                    var UserString = HTTPGET("https://us.api.battle.net/wow/character/", realm + "/" + player + "?fields=talents&locale=en_US&apikey=######################################");
                    var c = JsonConvert.DeserializeObject<WowTalentCharacter>(UserString);
                    return c.talents[0].spec.role;
                }
                else
                {
                    return "";
                }


            }
            catch { return ""; }

        }
        public bool DoesCharacterExist(string realm, string player, QueueDTO queue)
        {
            try
            {
                if (queue.Settings.World.ToLower() == "eu")
                {
                    var UserString = HTTPGET("https://eu.api.battle.net/wow/character/", realm + "/" + player + "?locale=en_GB&apikey=######################################");
                    var c = JsonConvert.DeserializeObject<WowCharacter>(UserString);
                    return true;
                }
                else if (queue.Settings.World.ToLower() == "us")
                {
                    var UserString = HTTPGET("https://us.api.battle.net/wow/character/", realm + "/" + player + "?locale=en_US&apikey=######################################");
                    var c = JsonConvert.DeserializeObject<WowCharacter>(UserString);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }
        public string GetRole(string realm, string player)
        {
            /*

            var UserString = HTTPGET("https://eu.api.battle.net/wow/character/", realm + "/" + player + "?locale=en_GB&apikey=######################################");
            var ClassString = HTTPGET("https://eu.api.battle.net/wow/character/", realm + "/" + player + "?{locale=en_GB&jsonp=4&apikey="++"######################################");
            var c = JsonConvert.DeserializeObject(b);
            var be = c["name"];
            return true;
            */

            return "";
        }
        public bool DoesContainPawn(string message)
        {
            return false;
        }
        public string HTTPGET(string baseadress, string request)
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri(baseadress);
                HttpResponseMessage response = client.GetAsync(request).Result;
                response.EnsureSuccessStatusCode();
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            };


        }
        public async Task SaveSettings()
        {
            string jsonfile = "Settings.json";
            if (File.Exists(jsonfile))
            {
                File.Delete(jsonfile);
            }
            File.WriteAllText(jsonfile, JsonConvert.SerializeObject(CommandHandler.Settings));
        }
        static void CopyProperties(object dest, object src)

        {

            foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(src))

            {

                item.SetValue(dest, item.GetValue(src));

            }

        }
    }
}


