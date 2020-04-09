using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using TCPCommunicator;

public class SynchronousSocketClient
{
    

    public static int Main(String[] args)
    {
        var c = new TCPClient();
        c.Connect();
        return 0;
    }
}

public class TCPClient
{
    private ManualResetEvent ClientConnected = new ManualResetEvent(false);
    private readonly IPEndPoint ServerEndpoint;
    public TCPClient(string serverAddress = "192.168.55.107", string serverPort = "25565")
    {
        ServerEndpoint = IPEndPoint.Parse($"{serverAddress}:{serverPort}");     
    }
    public void Connect()
    {

        // Connect to a remote device.  
        try
        {
            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                var sender = new Socket(SocketType.Stream, ProtocolType.Tcp);
                sender.BeginConnect(ServerEndpoint, ConnectionCallback, sender);
                ClientConnected.WaitOne();

            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public void ConnectionCallback(IAsyncResult ar)
    {
        Socket connection = (Socket)ar.AsyncState;
        connection.EndConnect(ar);
        Console.WriteLine("Socket connected to {0}", connection.RemoteEndPoint.ToString());
        var ns = new NetworkStream(connection);
        while (connection.Connected)
        {
            var msg = new Message(Console.ReadLine());
            // Send the data through the socket.  
            connection.Send(msg.Serialize());
            while (ns.DataAvailable) 
            {
                msg = new Message(ns);
                Console.WriteLine("Text received from server : {0}", msg.Text);
            }
            
        }
        ClientConnected.Set();
    }
}