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

    public byte[] exampleResponse = { 0x00, 0x02, 0x81, 0x80, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x07, 0x65, 0x78, 0x61, 0x6d, 0x70, 0x6c, 0x65, 0x03, 0x63, 0x6f, 0x6d, 0x00, 0x00, 0x01, 0x00, 0x01, 0xc0, 0x0c, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x04, 0x79, 0x00, 0x04, 0x5d, 0xb8, 0xd7, 0x0e };

    [Fact]
    public void DecodeNameTest2()
    {
	var (actual, pointer) = Message.DecodeName(exampleResponse, 29);

	Assert.Equal("example.com", actual);
	Assert.Equal(31, pointer);
    }

    [Fact]
    public void DecodeQuestionTest1()
    {
	var (question, pointer) = Question.Decode(exampleResponse, 12);

	Assert.Equal("example.com", question.QNAME);
	Assert.Equal(QType.A, question.QTYPE);
	Assert.Equal(QClass.IN, question.QCLASS);
	Assert.Equal(29, pointer);
    }

    [Fact]
    public void DecodeRRTest1()
    {
	var (rr, pointer) = RR.Decode(exampleResponse, 29);
	Assert.Equal("example.com", rr.NAME);
	Assert.Equal(RRType.A, rr.TYPE);
	Assert.Equal(RecordClass.IN, rr.CLASS);
	Assert.Equal((uint)1145, rr.TTL);
	Assert.Equal(4, rr.RDLENGTH);
	Assert.Equal("93.184.215.14", rr.RDATA);
	Assert.Equal(exampleResponse.Length, pointer);
    }

    public byte[] PTRResponse = {0x0, 0x1, 0x81, 0x80, 0x0, 0x1, 0x0, 0x1, 0x0, 0x0, 0x0, 0x0, 0x1, 0x31, 0x1, 0x31, 0x1, 0x31, 0x1, 0x31, 0x7, 0x69, 0x6e, 0x2d, 0x61, 0x64, 0x64, 0x72, 0x4, 0x61, 0x72, 0x70, 0x61, 0x0, 0x0, 0xc, 0x0, 0x1, 0xc0, 0xc, 0x0, 0xc, 0x0, 0x1, 0x0, 0x0, 0x1, 0xbf, 0x0, 0x11, 0x3, 0x6f, 0x6e, 0x65, 0x3, 0x6f, 0x6e, 0x65, 0x3, 0x6f, 0x6e, 0x65, 0x3, 0x6f, 0x6e, 0x65, 0x0};

    [Fact]
    public void DecodeRRTest2()
    {
	var (rr, pointer) = RR.Decode(PTRResponse, 38);
	Assert.Equal("1.1.1.1.in-addr.arpa", rr.NAME);
	Assert.Equal(RRType.PTR, rr.TYPE);
	Assert.Equal(RecordClass.IN, rr.CLASS);
	Assert.Equal((uint)447, rr.TTL);
	Assert.Equal(17, rr.RDLENGTH);
	Assert.Equal("one.one.one.one", rr.RDATA);
	Assert.Equal(PTRResponse.Length, pointer);
    }
}
