using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.Net;
using Simple.Net.Client;
using TXServer.Core.Database.NetworkEvents.Security;

namespace TXServer.Core.Database
{
    public class DatabaseNetwork
    {
        static DatabaseNetworkConfig Config => Server.Config.DatabaseNetwork;
        public static DatabaseNetwork Instance { get; private set; }
        public Client Socket { get; private set; }
        public bool Connected => Socket != null && Instance.Socket.Connected;
        bool ready;
        public bool IsReady => Connected && ready;
        public static bool AuthError { get; private set; }
        public Action OnReady;

        public DatabaseNetwork()
        {
            Instance?.Dispose();
            Instance = this;
            if (!Config.Enabled) return;
            Console.WriteLine("Opening new instance (DatabaseNetwork)");
            Socket = new Client(Config.HostAddress, Config.HostPort, () => { }, () => { Console.WriteLine("[DB] Disconnected"); if (!AuthError) new DatabaseNetwork().Connect(null); });
            Socket.onOnce<RSAPublicKey>(initPacket => {
                //Console.WriteLine("Got RSA Key");
                Socket.RSAEncryptionComponent = new RSAEncryptCompoenent(initPacket.Key);
                Socket.emit(new RSAPublicKey() { Key = Socket.RSADecryptionComponent.publicKey });
                Socket.on((RSAPublicKey packet) => {
                    Socket.RSAEncryptionComponent = new RSAEncryptCompoenent(packet.Key);
                    Socket.RSADecryptionComponent = new RSADecryptComponent();
                    Socket.emit(new RSAPublicKey() { Key = Socket.RSADecryptionComponent.publicKey });
                });
                Socket.emit(new LoginEvent()
                {
                    encryptedAPIKey = Socket.RSAEncryptionComponent.Encrypt(Config.Key),
                    encryptedAPIToken = Socket.RSAEncryptionComponent.Encrypt(Config.Token)
                });
                Socket.on((LoginFailedEvent reason) => {
                    AuthError = true;
                    Console.WriteLine($"Database Logon Error: {(string.IsNullOrEmpty(reason.reason) ? "No reason specified" : reason.reason)}");
                });
                Socket.on<LoginSuccessEvent>(() => { Console.WriteLine("DB Connected and Ready"); ready = true; OnReady(); });
            });
        }

        public DatabaseNetwork Connect(Action onReady)
        {
            if (!Config.Enabled) return this;
            //Console.WriteLine("Connecting");
            OnReady = onReady ?? (() => { });
            Socket.Connect();
            return this;
        }

        private void Dispose()
        {
            //Console.WriteLine("Disposing");
            if (Socket == null) return;
            Socket.socket.Close();
            Socket = null;
            if (this == Instance)
                Instance = null;
        }
    }
}
