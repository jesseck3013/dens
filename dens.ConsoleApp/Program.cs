using System.Net;
using System.Text;
using System.Net.Sockets;
using dens.Core;

enum Type : int
{
    x = 1,
}

class Program
{
    static void Main()
    {
	var msg = new Message("example.com");

        var byteArray = msg.header.Encode();

	var header = Header.Decode(byteArray);
	Console.WriteLine(header.ID);
	Console.WriteLine(header.QR);
	Console.WriteLine(header.OPCODE);
	Console.WriteLine(header.AA);
	Console.WriteLine(header.TC);
	Console.WriteLine(header.RD);
	Console.WriteLine(header.RA);
	Console.WriteLine(header.RCODE);
	Console.WriteLine(header.QDCOUNT);
	//string hexString = BitConverter.ToString(byteArray);
	//Console.WriteLine(hexString);

	var secondLineByte = new Byte[2] {byteArray[3], byteArray[2]} ;

	ushort secondLine = BitConverter.ToUInt16(secondLineByte, 0);

	var QR = (secondLine >> 15) & 1;
	var OPCODE = (secondLine >> 11) & (1 << 4 - 1);
	var AA = (secondLine >> 10) & 1;
	var TC = (secondLine >> 9) & 1;
	var RD = (secondLine >> 8) & 1;
	var RA = (secondLine >> 7) & 1;
	var RCODE = (secondLine >> 3) & (1 << 4 - 1);

	Console.WriteLine(QR);
	Console.WriteLine(OPCODE);
	Console.WriteLine(AA);
	Console.WriteLine(TC);
	Console.WriteLine(RD);
	Console.WriteLine(RA);
	Console.WriteLine(RCODE);

	// question header in bytes:
	//     00-01-01-00-00-01-00-00-00-00-00-00

	// Header header = Header.NewQuery();
	
	// foreach (var item in header.Encode())
	// {
	//     Console.WriteLine(Convert.ToString(item, 2).PadLeft(8, '0'));
	// }
	
	// UdpClient udpClient = new UdpClient();
	
	// //	Byte[] data = Encoding.Unicode.GetBytes("hello");

	// var endpoint = IPEndPoint.Parse("127.0.0.0:9090");

	// byte[] data = { 0x32, 0xD8, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x67, 0x6F, 0x6F, 0x67, 0x6C, 0x65, 0x03, 0x63, 0x6F, 0x6D, 0x00, 0x00, 0x1C, 0x00, 0x01 };
	
	// await udpClient.SendAsync(data, data.Length, endpoint);
	// var response = await udpClient.ReceiveAsync();
	// foreach (var b in response.Buffer)
	// {
	//     Console.Write(b.ToString("X2") + " ");
	// }
	// Console.Write("\n");
    }
}
