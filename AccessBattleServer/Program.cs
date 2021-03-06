﻿using AccessBattle;
using AccessBattle.Networking;
using AccessBattle.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

// Commands:
// -usrdb=".\db\db.txt" 
// -acceptany

namespace AccessBattleServer
{
    class Program
    {
        static GameServer _server;

        static void Main(string[] args)
        {
            // Testcode
            //string hash, salt;
            //PasswordHasher.GetNewHash("passw0rd", out hash, out salt);
            //Console.WriteLine(hash);
            //Console.WriteLine(salt);
            //Console.WriteLine(PasswordHasher.VerifyHash("passw0rd", hash, salt));
            // ----------------

            bool acceptAny = false;
            ushort port = 3221;
            string dbPath = "userdb.txt";

            // Read command line params
            for (int i = 0; i<args.Length; ++i)
            {
                var arg = args[i];
                if (!arg.StartsWith("-", StringComparison.Ordinal) && !arg.StartsWith("/", StringComparison.Ordinal))
                {
                    Console.WriteLine("Parameters must start with '-' or '/'. Use '-?' to show help.");
                    return;
                }
                if (arg.Length == 1) continue;
                arg = arg.Substring(1, arg.Length - 1);

                if (arg == "acceptany")
                {
                    acceptAny = true;
                    continue;
                }
                else if (arg.StartsWith("usrdb=", StringComparison.Ordinal))
                {
                    var spl = arg.Split('=');
                    if (spl.Length == 2)
                    {
                        dbPath = spl[1].Trim(new[] { '\"' });
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Error in parameter 'usrdb'");
                        return;
                    }
                }
                else if (arg.StartsWith("port="))
                {
                    var spl = arg.Split('=');
                    if (spl.Length != 2 || !ushort.TryParse(spl[1], out port))
                    {
                        Console.WriteLine("Error in parameter 'port'");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine(
                        "Access Battle Server\r\n\r\n" +
                        "Usage: AccessBattleServer [-acceptany] [-usrdb=path]\r\n" +
                        "\r\nOptions:\r\n" +
                        "\t-port=3221    Define the port to use. Default: 3221\r\n" +
                        "\t-acceptany    Accepts any client. Disables user database.\r\n" +
                        "\t              Clients require no password.\r\n" +
                        "\t-usrdb=path   Path to text file for user database.\r\n" +
                        "\t              default path is '.\\userdb.txt'.\r\n" +
                        "\r\nUsing '/' instead of '-' is allowed.\r\n"
                        );
                    return;
                }
            }

            Log.SetMode(LogMode.Console);
            Log.Priority = LogPriority.Information;

            // Create userdb folder if not existing
            Console.WriteLine("Using user database file: " + dbPath);
            try
            {
                var dir = System.IO.Path.GetDirectoryName(dbPath);
                if (!string.IsNullOrEmpty(dir) && !System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Could not create directory for user database!");
            }

            try
            {
                var userDb = new TextFileUserDatabaseProvider(dbPath);

                _server = new GameServer(port, userDb)
                {
                    AcceptAnyClient = acceptAny
                };
                _server.Start();

                Console.WriteLine("Game Server started");
                if (acceptAny)
                    Console.WriteLine("! Any client is accepted");

                Console.WriteLine("Type 'help' to show available commands");

                string line;
                while ((line = Console.ReadLine()) != "exit")
                {
                    bool ok = false;
                    line = line.Trim();
                    if (line == ("help"))
                    {
                        Console.WriteLine(
                            "\texit          Close this program.\r\n" +
                            "\tlist          List all games\r\n" +
                            "\tadd user n p  Adds user 'u' with password 'p'\r\n"+
                            "\tdebug         Debug command. Requires additional parameters:\r\n" +
                            "\t  win key=1234   1    Let player 1 win. Uses game key.\r\n" +
                            "\t  win name=gameX 2    Let player 2 win. Uses game name.");
                        ok = true;
                    }
                    else if (line == ("list"))
                    {
                        var games = _server.Games.ToList();
                        if (games.Count == 0)
                        {
                            Console.WriteLine("There are no games"); ;
                        }
                        else 
                        foreach (var game in games)
                            Console.WriteLine(game.Key + " - " + game.Value.Name);
                        ok = true;
                    }
                    else if (line.StartsWith("debug ", StringComparison.Ordinal))
                    {
                        var sp = line.Split(' ');
                        if (sp.Length == 4)
                        {
                            if (sp[1] == "win")
                            {
                                uint k = 0;
                                var spx = sp[2].Split('=');
                                if (spx.Length != 2) continue;
                                if (spx[0] == "key")
                                {
                                    if (!uint.TryParse(spx[1], out k)) continue;
                                }
                                else if (spx[0] == "name")
                                {
                                    k = _server.Games.FirstOrDefault(o => o.Value.Name == spx[1]).Key;
                                }
                                if (k == 0) continue;
                                int p;
                                if (!int.TryParse(sp[3], out p)) continue;
                                _server.Win(p, k);
                                ok = true;
                            }
                        }
                    }
                    else if (line.StartsWith("add user ", StringComparison.Ordinal))
                    {
                        var spl = line.Split(' ');
                        if (spl.Length == 4)
                        {
                            var pw = new System.Security.SecureString();
                            foreach (var c in spl[3]) pw.AppendChar(c);
                            if (userDb.AddUserAsync(spl[2], pw).GetAwaiter().GetResult())
                                Console.WriteLine("user added");
                            else Console.WriteLine("add failed");
                            ok = true;
                        }
                    }

                    if (!ok)
                    {
                        Console.WriteLine("error");
                    }
                }
            }
            finally
            {
                Log.WriteLine(LogPriority.Information, "Stopping Server...");
                _server.Stop();
#if DEBUG
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
#endif
                Log.WriteLine(LogPriority.Information, "Game Server stopped");
            }
        }
    }
}
