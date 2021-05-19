using System;
using System.Net.Sockets;
using System.Collections.Generic;
using Simple.Net.InternalEvents;

namespace Simple.Net.Client {
    public class Client {
        public TcpClient socket { get; private set; }
        NetworkStream networkStream;
        public bool Connected { get => socket != null ? socket.Connected : false; }
        byte[] buffer;

        Action onConnect;
        Action onDisconnect;
        Dictionary<long, object> Events = new Dictionary<long, object>();

        string host;
        int port;
        public RSADecryptComponent RSADecryptionComponent = new RSADecryptComponent();
        public RSAEncryptCompoenent RSAEncryptionComponent; // Defined in Authenticator.Process => user.on<RSAPublicKey>

        public Client(string host, int port, Action onConnect, Action onDisconnect, int bufferSize = 2048) {
            socket = new TcpClient() {
                ReceiveBufferSize = bufferSize,
                SendBufferSize = bufferSize
            };

            this.onConnect = onConnect;
            this.onDisconnect = onDisconnect;
            buffer = new byte[bufferSize];

            this.host = host;
            this.port = port;
            
            on<HeartBeat>((HeartBeat packet) => emit(packet));
        }

        public void Connect()
            => socket.BeginConnect(this.host, this.port, OnConnect, null);

        void OnConnect(IAsyncResult asyncResult) {
            try {
                socket.EndConnect(asyncResult);
                if (!Connected) throw new Exception();
                networkStream = socket.GetStream();
                networkStream.BeginRead(buffer, 0, buffer.Length, OnReceive, null);
                onConnect();
            }
            catch {
                Console.WriteLine("DB Connect timeout, trying again");
                socket.BeginConnect(host, port, OnConnect, null);
            }
        }

        void OnReceive(IAsyncResult asyncResult) {
            byte[] newBuffer = null;
            try {
                newBuffer = new byte[networkStream.EndRead(asyncResult)];
            }
            catch {
                Disconnect();
                return;
            }
            if (newBuffer.Length == 0) {
                Disconnect();
                return;
            }

            Array.Copy(buffer, newBuffer, newBuffer.Length);
            
            NetReader reader = new NetReader(newBuffer);
            Type eventType = HashCache.GetType(reader.hashCode);
            if (eventType == null) {
                Console.WriteLine($"ERR: Unknown event code [hash: {reader.hashCode}]");
                return;
            }
            if (!Events.ContainsKey(reader.hashCode)) {
                Console.WriteLine($"ERR: Got packet with no callback [name: {eventType.Name}, hash: {reader.hashCode}]");
                return;
            }

            dynamic packetInstance = Activator.CreateInstance(eventType);
            packetInstance.Deserialize(reader);
            
            object eventHandler = Events[reader.hashCode];
            Type callbackType = eventHandler.GetType();
            if (callbackType == typeof(Action))
                ((Action)eventHandler)();
            else if (callbackType == typeof(Action<Client>))
                ((Action<Client>)eventHandler)(this);
            else if (callbackType == typeof(Action<object>))
                ((Action<object>)eventHandler)(packetInstance);
            else if (callbackType == typeof(Action<Client, object>))
                ((Action<Client, object>)eventHandler)(this, packetInstance);
            else Console.WriteLine($"Got unknown event callback for packet! [name: {eventType.Name}, hash: {reader.hashCode}, callback: {eventHandler}]");

            try { networkStream.BeginRead(buffer, 0, buffer.Length, OnReceive, null); }
            catch { return; }
        }

        public void emit<T>(T packet) where T : struct, INetSerializable {
            NetWriter writer = new NetWriter(HashCache.Get<T>());
            packet.Serialize(writer);
            try { networkStream.Write(writer.ToByteArray()); } catch {}
        }

        public bool eventExists<T>() where T : struct, INetSerializable
            => Events.ContainsKey(HashCache.Get<T>());

        public void on<T>(Action callback) where T : struct, INetSerializable {
            if (callback == null) return;

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = callback;
            else Events.Add(HashCache.Get<T>(), callback);
        }

        public void on<T>(Action<Client> callback) where T : struct, INetSerializable {
            if (callback == null) throw new ArgumentNullException();

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = callback;
            else Events.Add(HashCache.Get<T>(), callback);
        }
        
        public void on<T>(Action<T> callback) where T : struct, INetSerializable {
            if (callback == null) throw new ArgumentNullException();

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = new Action<object>(o => callback((T)o));
            else Events.Add(HashCache.Get<T>(), new Action<object>(o => callback((T)o)));
        }
        
        public void on<T>(Action<Client, T> callback) where T : struct, INetSerializable {
            if (callback == null) throw new ArgumentNullException();

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = new Action<object>(o => callback(this, (T)o));
            else Events.Add(HashCache.Get<T>(), new Action<object>(o => callback(this, (T)o)));
        }

        public void onOnce<T>(Action<Client> callback) where T : struct, INetSerializable {
            if (callback == null) throw new ArgumentNullException();

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = new Action(() => { removeEvent<T>(); callback(this);});
            else Events.Add(HashCache.Get<T>(), new Action(() => { removeEvent<T>(); callback(this);}));
        }
        
        public void onOnce<T>(Action<T> callback) where T : struct, INetSerializable {
            if (callback == null) throw new ArgumentNullException();

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = new Action<object>(o => { removeEvent<T>(); callback((T)o);});
            else Events.Add(HashCache.Get<T>(), new Action<object>(o => { removeEvent<T>(); callback((T)o);}));
        }
        
        public void onOnce<T>(Action<Client, T> callback) where T : struct, INetSerializable {
            if (callback == null) throw new ArgumentNullException();

            if (eventExists<T>())
                Events[HashCache.Get<T>()] = new Action<object>(o => { removeEvent<T>(); callback(this, (T)o); });
            else Events.Add(HashCache.Get<T>(), new Action<object>(o => { removeEvent<T>(); callback(this, (T)o); }));
        }

        public void removeEvent<T>() where T : struct, INetSerializable {
            if (eventExists<T>())
                Events.Remove(HashCache.Get<T>());
        }

        public void Disconnect() {
            if (Connected) {
                networkStream.Close();
                socket.Close();
                onDisconnect();
            }
        }
    }
}
