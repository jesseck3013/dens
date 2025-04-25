# dens

dens is a DNS message encoder/decoder based on RFC 1035.

## Example Ussage

### Encode a DNS question

```csharp
using dens.Core;

var message = new Message("github.com");
var messageBytes = message.Encode();
```

### Decode a DNS message

```csharp
public byte[] exampleResponse = { 0x00, 0x02, 0x81, 0x80, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x07, 0x65, 0x78, 0x61, 0x6d, 0x70, 0x6c, 0x65, 0x03, 0x63, 0x6f, 0x6d, 0x00, 0x00, 0x01, 0x00, 0x01, 0xc0, 0x0c, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x04, 0x79, 0x00, 0x04, 0x5d, 0xb8, 0xd7, 0x0e };

var message = Message.Decode(exampleResponse);

// output:
//
// ++++ Answer ++++
// A    IN    93.184.215.14
```

### Simple DNS client with UDP

Create a new dotnet project and save the below script.

```csharp
using System.Net;
using System.Net.Sockets;
using dens.Core;

class Program
{
    static async Task Main(string[] args)
    {
	UdpClient udpClient = new UdpClient();

	QType QTYPE = (QType)Enum.Parse(typeof(RRType), args[1]);

	var message = new Message(args[0], QTYPE);
	var messageBytes = message.Encode();

	var endpoint = IPEndPoint.Parse($"{args[2]}:53");

	await udpClient.SendAsync(messageBytes, messageBytes.Length, endpoint);
	var response = await udpClient.ReceiveAsync();

	var responseMessage = Message.Decode(response.Buffer);
	Console.Write(responseMessage.ToString());
    }
}
```

Run the script with:

dotnet run <domain name> <Record Type> <DNS Server IP>

```sh
dotnet run example.com A 1.1.1.1

# Example output
#++++ Answer ++++
#A    IN    23.215.0.138
#A    IN    23.215.0.136
#A    IN    96.7.128.198
#A    IN    96.7.128.175
#A    IN    23.192.228.84
#A    IN    23.192.228.80
```
