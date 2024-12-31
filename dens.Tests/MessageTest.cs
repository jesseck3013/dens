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
	Byte[] headerByte = new Byte[12] { 0xAA, 0xAA, 0x81, 0x80, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 };
	
	var actual = Header.Decode(headerByte);

	Assert.Equal(0xAAAA, actual.ID);
	Assert.Equal(MessageType.Response, actual.QR);
	Assert.Equal(QueryType.Query, actual.OPCODE);
	Assert.False(actual.AA);
	Assert.False(actual.TC);
	Assert.True(actual.RD);
	Assert.True(actual.RA);
	Assert.Equal(1, actual.QDCOUNT);
	Assert.Equal(1, actual.ANCOUNT);
	Assert.Equal(0, actual.NSCOUNT);
	Assert.Equal(0, actual.ARCOUNT);
    }

    [Fact]
    public void PointerTest1()
    {
	Assert.True(Message.IsPointer(0b_1100_0000));
	Assert.False(Message.IsPointer(0b_0000_0000));
    }
}
