using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TCPCommunicator;

public class SynchronousSocketListener
{

    // Incoming data from the client.  
    public static string data = null;

    public static void StartListening()
    {
        // Data buffer for incoming data.  
        byte[] bytes = new Byte[1024];

        // Establish the local endpoint for the socket.  
        // Dns.GetHostName returns the name of the
        // host running the application.  
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = IPAddress.Any;
        
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 25565);

        // Create a TCP/IP socket.  
        Socket listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
        Console.WriteLine(localEndPoint);
        // Bind the socket to the local endpoint and
        // listen for incoming connections.  
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            // Start listening for connections.  
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                // Program is suspended while waiting for an incoming connection.  
                Socket handler = listener.Accept();
                Console.WriteLine("Connected IP: {0}", handler.LocalEndPoint);
                while(handler.Connected)
                {
                    data = null;
                    Console.WriteLine("Text received : {0}", data);
                    // An incoming connection needs to be processed.  
                    var ns = new NetworkStream(handler);
                    var msg = new Message(ns);
                    // Show the data on the console.  
                    Console.WriteLine("Text received : {0}", msg.Text);

                }
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }

    public static int Main(String[] args)
    {
        StartListening();
        return 0;
    }
}