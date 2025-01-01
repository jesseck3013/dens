namespace dens.Tests;

using dens.Core;

public class HeaderTest
{
    [Fact]
    public void HeaderEncodeTest1()
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
    public void HeaderDecodeTest1()
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

    public string domain = "F.ISI.ARPA";
    public byte[] domainByte = new Byte[] {0x01, 0x46, 0x03, 0x49, 0x53, 0x49, 0x04, 0x41, 0x52, 0x50, 0x41, 0x00};

    [Fact]
    public void ParseLabelTest1()
    {
	var (actual, pointer) = Message.ParseLabel(domainByte, 0);

	Assert.Equal("F", actual);
	Assert.Equal(2, pointer);
    }

    [Fact]
    public void ParseLabelTest2()
    {
	var (actual, pointer) = Message.ParseLabel(domainByte, 2);

	Assert.Equal("ISI", actual);
	Assert.Equal(6, pointer);
    }

    [Fact]
    public void DecodeNameTest1()
    {
	var (actual, pointer) = Message.DecodeName(domainByte, 0);

	Assert.Equal(domain, actual);
	Assert.Equal(domainByte.Length, pointer);
    }

    public byte[] exampleResponse = { 0x36, 0x9F, 0x81, 0xA0, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x07, 0x65, 0x78, 0x61, 0x6D, 0x70, 0x6C, 0x65, 0x03, 0x63, 0x6F, 0x6D, 0x00, 0x00, 0x01, 0x00, 0x01, 0xC0, 0x0C, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x08, 0xCC, 0x00, 0x04, 0x5D, 0xB8, 0xD7, 0x0E, 0x00, 0x00, 0x29, 0x04, 0xD0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

    [Fact]
    public void DecodeNameTest2()
    {
	var (actual, pointer) = Message.DecodeName(exampleResponse, 29);

	Assert.Equal("example.com", actual);
	Assert.Equal(31, pointer);
    }
}
