using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TCPCommunicator;

public class SynchronousSocketClient
{

    public static void StartClient()
    {
        // Data buffer for incoming data.  
        byte[] bytes = new byte[1024];

        // Connect to a remote device.  
        try
        {
            // Establish the remote endpoint for the socket.  
            // This example uses port 11000 on the local computer.  
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            
            // Create a TCP/IP  socket.  
            Socket sender = new Socket(SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                sender.Connect("192.168.55.107", 25565);

                Console.WriteLine("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString());
                while (true) 
                {
                    var msg = new Message(Console.ReadLine());
                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg.Serialize());
                    //int bytesSent = sender.Send(msg);
                }
                // Encode the data string into a byte array.  
                

                // Release the socket.  
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

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

    public static int Main(String[] args)
    {
        StartClient();
        return 0;
    }
}