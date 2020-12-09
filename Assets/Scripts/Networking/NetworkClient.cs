using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel.Design;
using System.Threading;
using System.IO;
using Assets.Server;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System.Collections.Concurrent;

public class NetworkClient
{
    public static NetworkClient instance = new NetworkClient();
    public static int BufferSize = 1024;
    public int myId = 0;
    public string ip = "127.0.0.1";
    public int port = 9000;
    public UDP udpInstance;

    public ConcurrentQueue<Message> MessageQueue { get; private set; } = new ConcurrentQueue<Message>();

    public static NetworkClient GetInstance() { return instance; }

    public void Init()
    {
        udpInstance = new UDP();
        udpInstance.Connect(0);
    }

    public void FixedUpdate()
    {
        /*Debug.Log("Sending packet");
        Player p = GameObject.Find("Player(Clone)").GetComponent<Player>();
        float x = p.x;
        float y = p.y;
        float xp = p.vx;
        float yp = p.vy;
        float rot = 0f;

        MovementMessage mm = new MovementMessage(0, 0, 0, 0, x, y, rot, xp, yp);
        UDPPacket packet = new UDPPacket();
        packet.AddMessage(mm);

        this.udpInstance.SendPacket(packet);*/

        UDPPacket p = BuildPacket();
        this.udpInstance.SendPacket(p);

    }

    private UDPPacket BuildPacket()
    {
        UDPPacket p = new UDPPacket();
        int space = p.SizeLeft();

        while (this.MessageQueue.Count > 0)
        {
            this.MessageQueue.TryPeek(out Message n);
            if (n.Size() < space)
            {
                this.MessageQueue.TryDequeue(out Message m);
                p.AddMessage(m);
            } 
            else
            {
                break;
            }
        }

        return p;
    }

    public void Destroy()
    {
        this.udpInstance.listenerTaskCancellationToken.Cancel();
        this.udpInstance.listenerTaskCancellationToken.Dispose();
    }

    public class UDP
    {
        public CancellationTokenSource listenerTaskCancellationToken;
        public UdpClient socket;
        public IPEndPoint endPoint;
        public bool connected = false;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }
        
        public void Connect(int clientPort)
        {
            socket = new UdpClient();
            socket.Connect(endPoint);

            this.listenerTaskCancellationToken = new CancellationTokenSource();
            CancellationToken ct = this.listenerTaskCancellationToken.Token;

            Task.Run(() =>
            {
                try
                {
                    if (this.connected == false)
                    {
                        this.Handshake();
                    }

                    while (true)
                    {
                        if(ct.IsCancellationRequested)
                        {
                            ct.ThrowIfCancellationRequested();
                        }

                        // TODO: Should try catch this as well / instead
                        byte[] data = new byte[1024];
                        //Debug.Log("feed me data");
                        data = this.socket.Receive(ref this.endPoint);
                        //Debug.Log("*nom* " + data);
                        HandleRawPacket(data);
                    }
                }
                catch (System.OperationCanceledException)
                {
                    Debug.Log("Closed network listener");
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            });
        }

        /*
         * BLOCKING - Must be called in a thread
         */
        public void Handshake()
        {
            int retries = 0;
            String knock = "Knock, knock";
            byte[] d = ASCIIEncoding.ASCII.GetBytes(knock);

            Task handshake_listener = Task.Run(() =>
            {
                byte[] data = new byte[1024];
                data = this.socket.Receive(ref this.endPoint);

                if(data.SequenceEqual(ASCIIEncoding.ASCII.GetBytes("VAMPIRES!")))
                {
                    Debug.Log("Successfully connected");
                    this.connected = true;
                    return;
                }

                if (retries >= 3) { return; }
            });

            while (this.connected == false)
            {
                if(retries >= 3) 
                {
                    Debug.LogWarning("Failed to connect");
                    break; 
                }

                Debug.Log("Trying to connect...");
                this.socket.Send(d, d.Length);
                retries += 1;

                Thread.Sleep(1000);
            }
        }

        public void HandleRawPacket(byte[] data)
        {
            UDPPacket packet = new UDPPacket(data);
            List<Message> messages = packet.GetMessages();

            // TODO: Refactor out from here
            MessageVisitorGameStateUpdater v = new MessageVisitorGameStateUpdater();
            foreach (Message message in messages)
            {
                message.Accept(v);
            }
        }

        public void SendPacket(UDPPacket packet)
        {
            byte[] p = packet.Serialize();
            this.socket.Send(p, p.Length);
        }
    }
}