using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using InfinityScript;
using Meebey.SmartIrc4net;

namespace IRCBridge
{
    public class IRCBridge : BaseScript
    {
        public IrcClient irc = new IrcClient();
        public string server;
        public int port;
        public string channel;
        public string nick;
        public string password;
        public bool sendstartmsg;
        public Thread thread;
        public const string BOLD = "";
        public const string NORMAL = "";
        public const string UNDERLINE = "";
        public const string COLOUR = "";

        public IRCBridge()
        {
            /*AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                                                              {
                                                                  Log.Error("Unhandled exception:");
                                                                  Log.Error(args.ExceptionObject.ToString());
                                                                  Environment.Exit(1);
                                                              };*/
            if (File.Exists("irc.lock"))
            {
                Log.Error("IRC lock file found, plugin was not properly disposed of.");
                //return;
            }
            File.WriteAllText("irc.lock", "");
            try
            {
                var settings = File.ReadAllLines("scripts\\ircbridge\\settings.txt");
                if (settings.Length < 5)
                    throw new Exception();
                server = settings[0];
                port = int.Parse(settings[1]);
                channel = settings[2];
                nick = settings[3];
                password = settings[4];
            }
            catch (Exception e)
            {
                Log.Error("settings.txt not found or is invalid.");
                return;
            }
            irc.Encoding = Encoding.ASCII;
            irc.SendDelay = 300;
            irc.AutoReconnect = true;
            irc.AutoRejoinOnKick = true;
            irc.SupportNonRfc = true;
            irc.ActiveChannelSyncing = true;
            irc.OnChannelMessage += IRCOnOnChannelMessage;
#if DEBUG
            irc.OnRawMessage += (sender, args) => Log.Debug(args.Data.RawMessage);
#endif
            Tick += () =>
                        {
                            /*if (irc.IsConnected)
                                irc.ListenOnce(false);
                            /*if (!init)
                            {
                                Connect();
                                init = true;
                            }*/
                            if (sendstartmsg)
                            {
                                sendstartmsg = false;
                                OnStartGameType();
                            }
                        };
            PlayerConnected +=
                entity => SendMessage(entity.GetField<string>("name") + " has connected to the game.");
            PlayerConnecting +=
                entity => SendMessage(entity.GetField<string>("name") + " is connecting to the game.");
            PlayerDisconnected +=
                entity => SendMessage(entity.GetField<string>("name") + " has disconnected from the game.");
            OnNotify("exitLevel_called", OnExitLevel);
            OnNotify("game_ended", OnExitLevel);
            Log.Info("Connecting to " + server + ":" + port + "/" + channel);
            thread = new Thread(Connect);
            thread.Start();
        }

        ~IRCBridge()
        {
            SendMessage("Plugin getting destroyed...");
            thread.Abort();
            irc.Disconnect();
            irc = null;
            File.Delete("irc.lock");
        }

        private void IRCOnOnChannelMessage(object sender, IrcEventArgs ircEventArgs)
        {
            //TODO: Find out how to say stuff without resorting to renaming players
            switch (ircEventArgs.Data.Message)
            {
                case "!players":
                    BuildScores();
                    break;
            }
        }

        private void BuildScores()
        {
            SendMessage("Player - Score - Kills - Assists - Death");
            var scoreList = (from p in Players
                             orderby p.GetField<int>("score") descending, p.GetField<int>("deaths") ascending
                             select p).ToArray();
            for (int i = 0; i < scoreList.Length; i++)
            {
                SendMessage(string.Format("{0} - {1} - {2} - {3} - {4}", scoreList[i].GetField<string>("name"),
                                          scoreList[i].GetField<string>("score"),
                                          scoreList[i].GetField<string>("kills"),
                                          scoreList[i].GetField<string>("assists"),
                                          scoreList[i].GetField<string>("deaths")));
            }
        }

        public override void OnExitLevel()
        {
            SendMessage("Match ended, level is exiting...");
            SendMessage("Scoreboard: ");
            BuildScores();
            irc.Disconnect();
            thread.Abort();
            irc = null;
            File.Delete("irc.lock");
        }

        public override void OnStartGameType()
        {
            SendMessage("Match starting. Map: " + Call<string>("GetDvar", new Parameter("mapname")) + ", Game Type: " +
                        Call<string>("GetDvar", new Parameter("g_gametype")));
        }

        public override void OnSay(Entity player, string name, string message)
        {
            SendMessage(name + NORMAL + ": " + message);
        }

        public override void OnPlayerKilled(Entity player, Entity inflictor, Entity attacker, int damage, string mod, string weapon, Vector3 dir, string hitLoc)
        {
            if (mod == "MOD_SUICIDE" || mod == "MOD_TRIGGER_HURT" || mod == "MOD_FALLING")
                SendMessage(string.Format("{0}({1}) suicided.", player.GetField<string>("name"),
                                          player.GetField<string>("sessionteam")));
            else
                SendMessage(string.Format("{0}({3}) was killed by {1}({4}) with {2}.", player.GetField<string>("name"),
                                          attacker.GetField<string>("name"), weapon, player.GetField<string>("sessionteam"),
                                          attacker.GetField<string>("sessionteam")));
        }


        public void SendMessage(string message)
        {
            if (irc.IsConnected)
                irc.SendMessage(SendType.Message, channel, ReplaceQuakeColorCodes(message));
        }

        public void Connect()
        {
            try
            {
                Log.Info("Attempting to connect...");
                irc.Connect(server, port);
                irc.ListenOnce(false);
                Log.Info("Logging in.");
                irc.Login(nick, "IW5M IRCBridge Bot");
                irc.ListenOnce(false);
                Log.Info("Joining channel.");
                irc.RfcJoin(channel);
                irc.ListenOnce(false);
                Log.Info("Identifying.");
                irc.RfcPrivmsg("NickServ", "identify " + password);
                irc.ListenOnce(false);
                //Log.Info("Sending match info.");
                //OnStartGameType();
                sendstartmsg = true;
                Log.Info("Listening.");
                irc.Listen();
            }
            catch (Exception e)
            {
                //throw;
                Log.Error(e.ToString());
                return;
            }
        }

        public static string ReplaceQuakeColorCodes(string remove)
        {
            var filteredout = "";
            var array = remove.Split('^');
            foreach (var part in array)
            {
                if (part.StartsWith("0"))
                    filteredout += part.Substring(1) + COLOUR + "01";
                else if (part.StartsWith("1"))
                    filteredout += part.Substring(1) + COLOUR + "04";
                else if (part.StartsWith("2"))
                    filteredout += part.Substring(1) + COLOUR + "03";
                else if (part.StartsWith("3"))
                    filteredout += part.Substring(1) + COLOUR + "08";
                else if (part.StartsWith("4"))
                    filteredout += part.Substring(1) + COLOUR + "02";
                else if (part.StartsWith("5"))
                    filteredout += part.Substring(1) + COLOUR + "12";
                else if (part.StartsWith("6"))
                    filteredout += part.Substring(1) + COLOUR + "06";
                else if (part.StartsWith("7"))
                    //filteredout += part.Substring(1) + COLOUR + "00";
                    filteredout += part.Substring(1) + NORMAL;
                else if (part.StartsWith("8"))
                    filteredout += part.Substring(1) + COLOUR + "14";
                else if (part.StartsWith("9"))
                    filteredout += part.Substring(1) + COLOUR + "05";
                else
                    filteredout += "^" + part;
            }
            return filteredout.Substring(1);
        }
    }
}
