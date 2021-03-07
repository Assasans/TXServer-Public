using System;
using System.Net;
using TXServer.Core;
using TXServer.Core.Data.Database.Impl;

namespace TXServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.Instance = new Server(new LocalDatabase());
            Server.Instance.Start(IPAddress.Parse(args[0]), Int16.Parse(args[1]), Int32.Parse(args[2]));
        }
    }
}
