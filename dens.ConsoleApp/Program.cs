using System.Net;
using System.Text;
using System.Net.Sockets;
using dens.Core;

class Program
{
    static async Task Main(string[] args)
    {
	UdpClient udpClient = new UdpClient();
	
	var message = new Message(args[0]);

	var messageBytes = message.Encode();

	var endpoint = IPEndPoint.Parse($"{args[2]}:53");

	await udpClient.SendAsync(messageBytes, messageBytes.Length, endpoint);
	var response = await udpClient.ReceiveAsync();

	var responseMessage = Message.Decode(response.Buffer);
	Console.Write(responseMessage.ToString());
    }
}
