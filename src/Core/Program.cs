﻿using CmlLib.Core.Auth;
using MeloncherCore.Launcher;
using MeloncherCore.Version;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MeloncherCore
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 512;
            Console.WriteLine("Hello Meloncher!");
            Console.Write("Version: ");
            string versionName = Console.ReadLine();
            bool offline = Confirm("Offline?");
            bool optifine = Confirm("Optifine?");
            var version = new McVersion(versionName, "Test", "Test-" + versionName);
            var launcher = new McLauncher();
            var session = MSession.GetOfflineSession("tester123");
            await launcher.Launch(version, session, offline, optifine);
        }

        static bool Confirm(string text)
        {
            ConsoleKey response;
            do
            {
                Console.Write(text + " [y/N] ");
                response = Console.ReadKey(false).Key;   // true is intercept key (dont show), false is show
                if (response != ConsoleKey.Enter)
                    Console.WriteLine();

            } while (response != ConsoleKey.Y && response != ConsoleKey.N && response != ConsoleKey.Enter);
            return response == ConsoleKey.Y;
        }
    }
}
