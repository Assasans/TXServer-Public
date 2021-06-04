using System;
using Simple.Net;
using Simple.Net.Client;
using TXDatabase.NetworkEvents.Security;
using TXServer.Core.Logging;

namespace TXServer.Core.Database
{
    public class DatabaseNetwork
    {
        private static DatabaseNetworkConfig Config => Server.Config.DatabaseNetwork;
        public static DatabaseNetwork Instance { get; private set; }
        public Client Socket { get; private set; }
        public bool Connected => Socket != null && Instance.Socket.Connected;
        private bool ready;
        public bool IsReady => Connected && ready;
        public static bool AuthError { get; private set; }
        public Action OnReady;

        public DatabaseNetwork()
        {
            Instance?.Dispose();
            Instance = this;
            if (!Config.Enabled) return;
            Logger.Debug("Opening new instance (DatabaseNetwork)");
            Socket = new Client(Config.HostAddress,
                                Config.HostPort,
                                () => { },
                                () =>
                                {
                                    Logger.Debug("[DB] Disconnected");
                                    if (!AuthError) new DatabaseNetwork().Connect(null);
                                });
            Socket.onOnce<RSAPublicKey>(initPacket =>
            {
                //Logger.Debug("Got RSA Key");
                Socket.RSAEncryptionComponent = new RSAEncryptCompoenent(initPacket.Key);
                Socket.emit(new RSAPublicKey() { Key = Socket.RSADecryptionComponent.publicKey });
                Socket.on((RSAPublicKey packet) =>
                {
                    Socket.RSAEncryptionComponent = new RSAEncryptCompoenent(packet.Key);
                    Socket.RSADecryptionComponent = new RSADecryptComponent();
                    Socket.emit(new RSAPublicKey() { Key = Socket.RSADecryptionComponent.publicKey });
                });
                Socket.emit(new LoginEvent()
                {
                    encryptedAPIKey = Socket.RSAEncryptionComponent.Encrypt(Config.Key),
                    encryptedAPIToken = Socket.RSAEncryptionComponent.Encrypt(Config.Token)
                });
                Socket.on((LoginFailedEvent reason) =>
                {
                    AuthError = true;
                    Logger.Debug($"Database Logon Error: {(string.IsNullOrEmpty(reason.reason) ? "No reason specified" : reason.reason)}");
                });
                Socket.on<LoginSuccessEvent>(() =>
                {
                    Logger.Debug("DB Connected and Ready");
                    ready = true;
                    OnReady();
                });
            });
        }

        public DatabaseNetwork Connect(Action onReady)
        {
            if (!Config.Enabled) return this;
            //Logger.Debug("Connecting");
            OnReady = onReady ?? (() => { });
            Socket.Connect();
            return this;
        }

        private void Dispose()
        {
            //Logger.Debug("Disposing");
            if (Socket == null) return;
            Socket.socket.Close();
            Socket = null;
            if (this == Instance)
                Instance = null;
        }
    }
}
