using System;
using Discord;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using System.Threading.Tasks;
using SimDpsBot;
using System.Threading;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SimDpsBot;
using System.IO;
using System.ComponentModel;
using Discord.Rest;
using HtmlAgilityPack;
using CoreFtp;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using CSharp_SimDpsBot;

namespace TemplateBot
{
    class CommandHandler
    {
        
        DateTime SinceLastMsg = new DateTime();
        public System.Threading.Timer _timer;
        private DiscordSocketClient _client;
        private CommandService _service;
        public static ObservableCollection<QueueDTO> Queue = new ObservableCollection<QueueDTO>();
        BackgroundWorker bgw = new BackgroundWorker();

        public static List<SettingsDTO> Settings = new List<SettingsDTO>();
        public CommandHandler(DiscordSocketClient client)
        {
            Queue.CollectionChanged += Queue_CollectionChanged;
            SinceStartedRequest = DateTime.Now;
            SinceLastMsg = DateTime.Now;
            _timer = new System.Threading.Timer(Callback, true, 1000, System.Threading.Timeout.Infinite);
            _client = client;
            _service = new CommandService();
            _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += _client_MessageReceived;
            try
            {

                string json = File.ReadAllText("Settings.json");
                Settings = JsonConvert.DeserializeObject<List<SettingsDTO>>(json);
            }
            catch (Exception)
            {
            }
            _client.SetGameAsync("\".sd -?\" for command");
        }

        private void Queue_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                int i = 0;
                foreach (var item in Queue)
                {
                    i++;
                    //int i = item.queueSpot - 1;
                    //item.queueSpot = i;
                    Thread.Sleep(200);
                    var Channel = _client.GetChannel(item.ChannelID) as SocketTextChannel;
                    var msg = Channel.GetMessageAsync(item.MessageID).Result as RestUserMessage;
                    var eb = CommandHandler.ChangeStatus(msg, "Your Request is number " + i + " in queue.");
                    msg.ModifyAsync(x => x.Embed = eb.Build());

                }
            }
        }

        private void Callback(Object state)
        {
            TimerEvent();
            _timer.Change(1000, Timeout.Infinite);
        }
        private void SimDPS(QueueDTO QueueItem, RestUserMessage Msg, SocketTextChannel channel)
        {

            Process sim = new Process();
            QueueItem.Process = sim;
            Random r = new Random();
            string HtmlFile = @"c:\simbot\sims\"+r.Next(100000000, 900000000 )+ ".html";
            QueueItem.HTMLFilelocation = HtmlFile;

            sim.StartInfo.FileName = @"C:\SimBot\simc.exe";
            string interations = "iterations=\"1000\" ";
            if (QueueItem.Settings.Interations != null)
            {
                interations = "iterations=\"" + QueueItem.Settings.Interations + "\" ";
            }
            string fightstyle = "fight_style=\"Patchwerk\" ";
            if (QueueItem.Settings.Fight_Style != null)
            {
                fightstyle = "fight_style=\"" + QueueItem.Settings.Fight_Style + "\" ";
            }
            string desired_targets = "desired_targets=\"1\" ";
            if (QueueItem.Settings.Targets != null)
            {
                desired_targets = "desired_targets=\"" + QueueItem.Settings.Targets + "\" ";
            }
            string max_time = "max_time=\"300\" ";
            if (QueueItem.Settings.Fight_Length != null)
            {
                max_time = "max_time=\"" + QueueItem.Settings.Fight_Length + "\" ";
            }
            string single_actor_batch = "single_actor_batch=\"1\" ";
            string pawn = "calculate_scale_factors=\"0\" ";
            if (QueueItem.Settings.Pawn)
            {
                pawn = "calculate_scale_factors=\"1\" ";
            }
            string scale = "scale_only=\"strength,intellect,agility,crit,mastery,vers,haste\" ";
            string html = "html=\"" + HtmlFile + "\" ";
            string threads = "threads=\"" + Environment.ProcessorCount+"\"";

            if (QueueItem.Text != null && QueueItem.Username == null)
            {
                string simname = "name=\"" + QueueItem.Username + "\" ";
                sim.StartInfo.Arguments = "" +
                    QueueItem.Text.Replace("\n", " ") + " " +
                    interations +
                    fightstyle +
                    desired_targets +
                    max_time +
                    single_actor_batch +
                    pawn +
                    scale +
                    html+
                    threads;
            }
            else if (QueueItem.Text == null && QueueItem.Username != null)
            {

                string armory = "armory=\"eu," + QueueItem.Realm + "," + QueueItem.Username + "\" ";
                if (QueueItem.Settings.World != null && QueueItem.Settings.World.ToLower() == "us")
                {
                    armory = "armory=\"us," + QueueItem.Realm + "," + QueueItem.Username + "\" ";
                }
                string simname = "name=\"" + QueueItem.Username + "\" ";
                sim.StartInfo.Arguments =
                    armory +
                    simname +
                    interations +
                    fightstyle +
                    desired_targets +
                    max_time +
                    single_actor_batch +
                    pawn +
                    scale +
                    html +
                    threads ;
            }


            sim.StartInfo.CreateNoWindow = true;
            sim.StartInfo.UseShellExecute = false;
            sim.StartInfo.RedirectStandardError = true;
            sim.StartInfo.RedirectStandardInput = true;
            sim.StartInfo.RedirectStandardOutput = true;
            sim.EnableRaisingEvents = true;
            sim.OutputDataReceived += (sender, e) => Sim_OutputDataReceived(sender, e, Msg, QueueItem,HtmlFile,sim);
            sim.ErrorDataReceived += (sender, e) => Sim_OutputDataReceived(sender, e, Msg, QueueItem, HtmlFile,sim);
            //sim.Exited += process_Exited;
            sim.Start();
            sim.BeginErrorReadLine();
            sim.BeginOutputReadLine();
            sim.WaitForExit();
        }
        public static bool skip = false;
        public bool isSimProgress(string i)
        {
            if (i.Contains("Generating Baseline") || i.Contains("Generating Agi") || i.Contains("Generating Crit") || i.Contains("Generating Haste") || i.Contains("Generating Mastery") || i.Contains("Generating Vers") || i.Contains("Generating Int") || i.Contains("Generating St"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool isSimProgressMinusBaseline(string i)
        {
            if (i.Contains("Generating Agi") || i.Contains("Generating Crit") || i.Contains("Generating Haste") || i.Contains("Generating Mastery") || i.Contains("Generating Vers") || i.Contains("Generating Int") || i.Contains("Generating St"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public EmbedBuilder Rebuild(RestUserMessage msg, bool RemoveStatus)
        {
            EmbedBuilder eb = new EmbedBuilder();
            var oldEmbed = msg.Embeds.ToList()[0];

            eb.WithColor(msg.Embeds.ToList()[0].Color.Value);
            eb.Title = msg.Embeds.ToList()[0].Title;
            foreach (var fields in msg.Embeds.ToList()[0].Fields)
            {
                if (!RemoveStatus && fields.Name != "Status")
                {
                    if (fields.Inline)
                    {
                        eb.AddField(fields.Name, fields.Value, true);
                    }
                    else
                    {
                        eb.AddField(fields.Name, fields.Value);
                    }
                }
            }
            if (oldEmbed.Description != null)
            {
                eb.Description = msg.Embeds.ToList()[0].Description;
            }
            if (oldEmbed.Image != null)
            {
                eb.ImageUrl = oldEmbed.Image.Value.Url;
            }
            if (oldEmbed.Thumbnail != null)
            {
                eb.ThumbnailUrl = oldEmbed.Thumbnail.Value.Url;
            }
            return eb;
        }
        public EmbedBuilder Rebuild(RestUserMessage msg)
        {
            return Rebuild(msg, false);
        }
        static public EmbedBuilder ChangeStatus(RestUserMessage msg, string NewStatus)
        {
            EmbedBuilder eb = new EmbedBuilder();
            var oldEmbed = msg.Embeds.ToList()[0];
            eb.WithColor(msg.Embeds.ToList()[0].Color.Value);
            eb.Title = msg.Embeds.ToList()[0].Title;
            foreach (var fields in msg.Embeds.ToList()[0].Fields)
            {
                if (fields.Inline)
                {
                    if (fields.Name == "Status")
                    {
                        eb.AddField("Status", NewStatus, true);
                    }
                    else
                    {
                        eb.AddField(fields.Name, fields.Value, true);


                    }
                }
                else
                {
                    if (fields.Name == "Status")
                    {
                        eb.AddField("Status", NewStatus);
                    }
                    else
                    {
                        eb.AddField(fields.Name, fields.Value);
                    }
                }


            }
            if (oldEmbed.Description != null)
            {
                eb.Description = msg.Embeds.ToList()[0].Description;
            }
            if (oldEmbed.Image != null)
            {
                eb.ImageUrl = oldEmbed.Image.Value.Url;
            }
            if (oldEmbed.Thumbnail != null)
            {
                eb.ThumbnailUrl = oldEmbed.Thumbnail.Value.Url;
            }

            return eb;
        }

        private async void Sim_OutputDataReceived(object sender, DataReceivedEventArgs e, RestUserMessage msg, QueueDTO queueitem, string HtmlFile, Process sim)
        {
            try
            {

            

            Console.WriteLine(e.Data);
            if (e.Data != null && isSimProgress(e.Data.Replace(@"1/1", "Baseline")))
            {
                if (DateTime.Now.Subtract(SinceLastMsg).TotalSeconds > 1 || (e.Data.Contains("[===================>]") && DateTime.Now.Subtract(SinceLastMsg).TotalMilliseconds > 200))
                {
                    SinceLastMsg = DateTime.Now;
                    var eb = ChangeStatus(msg, e.Data.Replace("  ", "").Replace("Generating ", ""));
                    await msg.ModifyAsync(x => x.Embed = eb.Build());

                }
            }
            if (e.Data != null && e.Data.Contains("html report took"))
            {
                queueitem.IsComplete = true;
                var eb1 = ChangeStatus(msg, "Generating HTML file");
                await msg.ModifyAsync(x => x.Embed = eb1.Build());
                var result = ParseSim(HtmlFile, queueitem, HtmlFile);

                var eb = Rebuild(msg, true);

                eb.AddField("DPS", result.DPS,true);
                eb.AddField("Ilvl", result.ItemLevel, true);
                eb.AddField("Role", result.Role, true);
                eb.AddField("Race", result.Race, true);
                eb.AddField("Class", result.Class, true);
                eb.AddField("Spec", result.Spec, true);
                eb.AddField("Interations", result.intervals, true);
                eb.AddField("FightLength", result.FightLength, true);
                eb.AddField("Fight Style", result.FightStyle, true);
                if (queueitem.Settings.Pawn && result.Role != "Heal")
                {
                    eb.AddField("Stat Prio", result.StatRanks);
                    eb.AddField("Pawn string", "```" + result.PawnString + "```");
                }

                eb.WithFooter("Simulation took " + DateTime.Now.Subtract(queueitem.TimeStarted).TotalSeconds.ToString().Split('.')[0] + " Sec");
                await msg.ModifyAsync(x => x.Embed = eb.Build());

                if (queueitem.Settings.UseFTP)
                {
                    try
                    {

                        await uploadFile(queueitem, HtmlFile, msg);
                        eb.Url = queueitem.Settings.FTPWebAdress + HtmlFile.Split('\\').Last();
                        await msg.ModifyAsync(x => x.Embed = eb.Build());
                    }
                    catch (Exception es)
                    {
                        eb.AddField("Status", es.Message);
                        await msg.ModifyAsync(x => x.Embed = eb.Build());
                    }

                }
                if (queueitem.Settings.SendFileAfterSim)
                {
                    await msg.Channel.SendFileAsync(HtmlFile);
                }

            }
            if (skip)
            {
                Queue.Remove(queueitem);
                sim.Kill();
                skip = false;
                var eb = ChangeStatus(msg, "Your request have been skipped by user!");
                await msg.ModifyAsync(x => x.Embed = eb.Build());
            }
            }
            catch (Exception ss)
            {
                await Program.Log(ss.Message, ConsoleColor.Red);
            }
            if (e.Data != null && e.Data.Contains(" is not currently supported."))
            {
                Queue.Remove(queueitem);
                sim.Kill();
                skip = false;
                var eb = ChangeStatus(msg, e.Data);
                await msg.ModifyAsync(x => x.Embed = eb.Build());
            }
        }
        public HTMLParse ParseSim(string file, QueueDTO queueitem, string  HtmlFile)
        {
            HtmlAgilityPack.HtmlNode.ElementsFlags.Remove("form");

            HTMLParse item = new HTMLParse();
            var doc = new HtmlDocument();
            doc.Load(HtmlFile);

            var doc2 = new HtmlDocument();
            doc2.Load(HtmlFile);
            HtmlNode MainNode = null;
            if (queueitem.Text == null)
            {
                MainNode = doc2.DocumentNode.SelectSingleNode(@"//*[@id=""player1""]").ChildNodes[1].ChildNodes[4];
            }
            else
            {
                var sd = doc2.DocumentNode.SelectSingleNode(@"//*[@id=""player1""]");
                MainNode = doc2.DocumentNode.SelectSingleNode(@"//*[@id=""player1""]").ChildNodes.ToList().First(x => x.OuterHtml.Count() > 200);
                    
            }

            var dps = doc.DocumentNode.SelectNodes("//*[@id=\"player1toggle\"]").First();

            item.DPS = dps.InnerText.Split(';')[2].Split(',')[0];

            item.ItemLevel = MainNode.ChildNodes.FirstOrDefault(x => x.InnerText.Contains("Average Item Level:")).ChildNodes.FirstOrDefault(x => x.InnerText.Contains("Average Item Level:")).ChildNodes.FirstOrDefault(x => x.InnerText.Contains("Average Item Level:")).ChildNodes.FirstOrDefault(x => x.InnerText.Contains("Average Item Level:")).ChildNodes.FirstOrDefault(x => x.InnerText.Contains("Average Item Level:")).ChildNodes.First().InnerText.Split(' ')[3];
            item.Role = MainNode.ChildNodes[1].ChildNodes[9].ChildNodes[1].InnerText;
            item.Level = int.Parse(MainNode.ChildNodes[1].ChildNodes[7].ChildNodes[1].InnerText);
            item.Spec = MainNode.ChildNodes[1].ChildNodes[5].ChildNodes[1].InnerText;
            item.Race = MainNode.ChildNodes[1].ChildNodes[1].ChildNodes[1].InnerText;
            item.Class = MainNode.ChildNodes[1].ChildNodes[3].ChildNodes[1].InnerText;
            item.FightStyle = doc.DocumentNode.SelectNodes("//*[@id=\"masthead\"]/ul/li[4]").First().ChildNodes[1].InnerText;
            item.FightLength = doc.DocumentNode.SelectNodes("//*[@id=\"masthead\"]/ul/li[3]/a").First().ChildNodes[1].InnerText;
            item.intervals = doc.DocumentNode.SelectNodes("//*[@id=\"masthead\"]/ul/li[2]").First().ChildNodes[1].InnerText;
            if (MainNode.InnerHtml.Contains("Pawn") && item.Role != "Heal")
            {
                item.StatRanks = MainNode.ChildNodes[5].ChildNodes[3].ChildNodes[7].ChildNodes[13].ChildNodes[3].ChildNodes[1].ChildNodes[1].InnerText;
                item.PawnString = MainNode.ChildNodes.FirstOrDefault(x => x.InnerText.Contains("Pawn: v1:")).ChildNodes.FirstOrDefault(x => x.InnerText.Contains("Pawn: v1:")).ChildNodes.FirstOrDefault(x => x.InnerText.Contains("Pawn: v1:")).ChildNodes.FirstOrDefault(x => x.InnerText.Contains("Pawn: v1:")).ChildNodes.FirstOrDefault(x => x.InnerText.Contains("Pawn: v1:")).InnerText.Replace(Environment.NewLine, "");
            }
    
            return item;
        }
        public DateTime SinceStartedRequest = new DateTime();
        private void TimerEvent()
        {

            if (DateTime.Now.Subtract(SinceStartedRequest).TotalMinutes > 5  || !bgw.IsBusy)
            {
                SinceStartedRequest = DateTime.Now;
                if (bgw.IsBusy)
                {
                    if (Queue.ToList().Exists(x => x.Process != null))
                    {
                        var queueitem = Queue.ToList().Find(x => x.Process != null);
                        queueitem.Process.Kill();

                        var Channel = _client.GetChannel(queueitem.ChannelID) as SocketTextChannel;
                        var msg = Channel.GetMessageAsync(queueitem.MessageID).Result as RestUserMessage;
                        var eb = ChangeStatus(msg, "Simulation took to long, and have been stopped!");
                        msg.ModifyAsync(x => x.Embed = eb.Build());
                        Thread.Sleep(1000);
                    }
                }
                else
                {


                    bgw.DoWork += (_, __) =>
                    {
                        if (Queue.Count != 0)
                        {
                            var item = Queue[0];

                            var Channel = _client.GetChannel(item.ChannelID) as SocketTextChannel;
                            var msg = Channel.GetMessageAsync(item.MessageID).Result as RestUserMessage;
                            var eb = ChangeStatus(msg, "Starting the simulation!");
                            msg.ModifyAsync(x => x.Embed = eb.Build());
                            item.TimeStarted = DateTime.Now;
                            SimDPS(item, msg, Channel);
                            Queue.Remove(item);
                        }

                    };
                    bgw.RunWorkerAsync();
                }
            }
        }
        private async Task _client_MessageReceived(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            var context = new SocketCommandContext(_client, msg);
            int argPost = 0;
            if (msg.HasCharPrefix('.', ref argPost))
            {


                var result = _service.ExecuteAsync(context, argPost);
                if (!result.Result.IsSuccess && result.Result.Error != CommandError.UnknownCommand)
                {
                    await context.Channel.SendMessageAsync(result.Result.ErrorReason);
                }
                await Program.Log("Invoked " + msg + " in " + context.Channel + " with " + result.Result, ConsoleColor.Magenta);
            }
            else
            {
                await Program.Log(context.Channel + "-" + context.User.Username + " : " + msg, ConsoleColor.White);
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
        public string[] SplitOnString(string input, string Spliton)
        {
            return input.Split(new string[] { Spliton }, StringSplitOptions.None);
        }
        static void DisplayPercentage(double ratio)
        {
            string percentage = string.Format("Percentage is {0:0.0%}", ratio);
        }
        public async Task uploadFile(QueueDTO item, string file, RestUserMessage msg)
        {
            if (item.Settings.FTP.Password != null && item.Settings.FTP.Server != null && item.Settings.FTP.Username != null)
            {

                if (File.Exists(file))
                {


                    using (var ftpClient = new FtpClient(new FtpClientConfiguration
                    {
                        Host = item.Settings.FTP.Server,
                        Username = item.Settings.FTP.Username,
                        Password = item.Settings.FTP.Password,
                        Port = item.Settings.FTP.Port
                    }))
                    {
                        var fileinfo = new FileInfo(file);
                        await ftpClient.LoginAsync();

                        using (var writeStream = await ftpClient.OpenFileWriteStreamAsync(file.Split('\\').Last()))
                        {
                            var fileReadStream = fileinfo.OpenRead();
                            await fileReadStream.CopyToAsync(writeStream);
                        }
                    }
                }
                else
                {
                    var eb = Rebuild(msg);
                    eb.AddField("Status", "Cant find HTML File!");
                    await msg.ModifyAsync(x => x.Embed = eb.Build());
                }
            }
            else
            {
                var eb = Rebuild(msg);
                eb.AddField("Status", "Missing FTP info!");
                await msg.ModifyAsync(x => x.Embed = eb.Build());
            }

        }
        /// <summary>
        /// This method writes the percentage of the top number to the bottom number.
        /// </summary>
        static void DisplayPercentage(int top, int bottom)
        {
            DisplayPercentage((double)top / bottom);
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
    }
}
