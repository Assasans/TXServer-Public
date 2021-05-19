using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.Net;
using Simple.Net.Client;
using TXServer.Core.Database.NetworkEvents.Security;
using SimpleJSON;

namespace TXServer.Core.Database
{
    public class DatabaseNetwork
    {
        static JSONNode Config { get => Server.Config["DatabaseNetwork"]; }
        public static DatabaseNetwork instance { get; private set; }
        public Client Socket { get; private set; }
        public bool Connected { get => Socket != null ? instance.Socket.Connected : false; }
        bool ready = false;
        public bool isReady { get => Connected && ready; }
        public static bool authError { get; private set; }
        public Action OnReady;

        public DatabaseNetwork()
        {
            if (!Config["Enabled"].AsBool) return;
            Console.WriteLine("Opening new instance (DatabaseNetwork)");
            if (instance != null)
                instance.Dispose();
            instance = this;
            Socket = new Client(Config["HostAddress"], Config["HostPort"], () => { }, () => { Console.WriteLine("[DB] Disconnected"); if (!authError) new DatabaseNetwork().Connect(null); });
            Socket.onOnce<RSAPublicKey>((RSAPublicKey initPacket) => {
                //Console.WriteLine("Got RSA Key");
                Socket.RSAEncryptionComponent = new RSAEncryptCompoenent(initPacket.Key);
                Socket.emit<RSAPublicKey>(new RSAPublicKey() { Key = Socket.RSADecryptionComponent.publicKey });
                Socket.on<RSAPublicKey>((RSAPublicKey packet) => {
                    Socket.RSAEncryptionComponent = new RSAEncryptCompoenent(packet.Key);
                    Socket.RSADecryptionComponent = new RSADecryptComponent();
                    Socket.emit<RSAPublicKey>(new RSAPublicKey() { Key = Socket.RSADecryptionComponent.publicKey });
                });
                Socket.emit<LoginEvent>(new LoginEvent()
                {
                    encryptedAPIKey = Socket.RSAEncryptionComponent.Encrypt(Config["Key"]),
                    encryptedAPIToken = Socket.RSAEncryptionComponent.Encrypt(Config["Token"])
                });
                Socket.on<LoginFailedEvent>((LoginFailedEvent reason) => {
                    authError = true;
                    Console.WriteLine($"Database Logon Error: {(string.IsNullOrEmpty(reason.reason) ? "No reason specified" : reason.reason)}");
                });
                Socket.on<LoginSuccessEvent>(() => { Console.WriteLine("DB Connected and Ready"); ready = true; OnReady(); });
            });
        }

        public DatabaseNetwork Connect(Action onReady)
        {
            if (!Config["Enabled"].AsBool) return this;
            //Console.WriteLine("Connecting");
            OnReady = onReady != null ? onReady : () => { };
            Socket.Connect();
            return this;
        }

        public void Dispose()
        {
            //Console.WriteLine("Disposing");
            if (Socket != null)
            {
                Socket.socket.Close();
                Socket = null;
                if (this == instance)
                    instance = null;
            }
        }
    }
}
