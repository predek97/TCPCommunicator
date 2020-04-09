using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TCPCommunicator;

public class SynchronousSocketListener
{
    public static int Main(String[] args)
    {
        var s = new TCPAsyncServer();
        s.StartListening();
        return 0;
    }
}

public class TCPAsyncServer
{
    public TCPAsyncServer()
    {
        // Establish the local endpoint for the socket.  
        // Dns.GetHostName returns the name of the
        // host running the application.  
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = IPAddress.Any;

        ServerLocalEndpoint = new IPEndPoint(ipAddress, 25565);
        // Create a TCP/IP socket.  
        Listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
    }

    public void StartListening()
    {
        
        // Bind the socket to the local endpoint and
        // listen for incoming connections.  
        try
        {
            Listener.Bind(ServerLocalEndpoint);
            Listener.Listen(10);

            // Start listening for connections.  
            Listen();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    // Thread signal.  
    private IPEndPoint ServerLocalEndpoint;
    private Socket Listener;
    private ManualResetEvent anotherClientConnected = new ManualResetEvent(false);

    private void Listen()
    {
        while (true)
        {
            anotherClientConnected.Reset();
            Console.WriteLine("Waiting for a connection...");
            // Program is suspended while waiting for an incoming connection.  
            Listener.BeginAccept(new AsyncCallback(ConnectionAcceptedCallback), Listener);
            anotherClientConnected.WaitOne();
        }
    }
    private void ConnectionAcceptedCallback(IAsyncResult ar)
    {
        // Signal the main thread to continue.  
        anotherClientConnected.Set();

        // Get the socket that handles the client request.  
        Socket listener = (Socket)ar.AsyncState;
        listener = listener.EndAccept(ar);
        Console.WriteLine("Connected IP: {0}", listener.LocalEndPoint);
        var ns = new NetworkStream(listener);
        while (listener.Connected)
        {
            // An incoming connection needs to be processed.  
            try
            {
                var msg = new Message(ns);
                // Show the data on the console.  
                Console.WriteLine("Text received : {0}", msg.Text);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

        }
        listener.Shutdown(SocketShutdown.Both);
        listener.Close();
    }
}