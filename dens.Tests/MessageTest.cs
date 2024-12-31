namespace dens.Tests;

using dens.Core;

public class HeaderTest
{
    [Fact]
    public void EncodeTest1()
    {
	var header = new Header
        {
            ID = 0xAAAA,
            QR = MessageType.Query,
            OPCODE = QueryType.Query,
            AA = false,
            TC = false,
            RD = true,
            RA = false,
            RCODE = ResponseType.IsQuery,
            QDCOUNT = 0x01,
            ANCOUNT = 0x00,
            NSCOUNT = 0x00,
            ARCOUNT = 0x00
        };

        var byteArray = header.Encode();

	Byte[] expectedArray = new Byte[12] { 0xAA, 0xAA, 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
	Assert.Equal(expectedArray, byteArray);
    }

    [Fact]
    public void DecodeTest1()
    {

    }
}
