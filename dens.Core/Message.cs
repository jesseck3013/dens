namespace dens.Core;

public enum MessageType : ushort
{
    Query = 0,
    Response = 1,
}

public enum QueryType : ushort
{
    Query = 0,
    IQuery = 1,
    Status = 2,
}

public enum ResponseType : ushort
{
    Ok = 0,
    FormatError = 1,
    ServerFailure = 2,
    NameError = 3,
    NotImplemented = 4,
    Refused = 5,
    IsQuery = 6,
}

public class Header
{
    public ushort ID { get; set; }
    public MessageType QR { get; set; }
    public QueryType OPCODE { get; set; }
    public bool AA { get; set; }
    public bool TC { get; set; }
    public bool RD { get; set; }
    public bool RA { get; set; }
    public ResponseType RCODE { get; set; }
    public ushort QDCOUNT { get; set; }
    public ushort ANCOUNT { get; set; }
    public ushort NSCOUNT { get; set; }
    public ushort ARCOUNT { get; set; }

    public static Header NewQuery()
    {
	return new Header
        {
            ID = 0x01, // TODO: generate random ID
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
    }

    public byte[] Encode()
    {
	ushort[] data = new ushort[6];
	
	data[0] = ID;
	ushort flags = 0;
	flags |= (ushort)((ushort)(QR) << 15);
	flags |= (ushort)((ushort)(OPCODE) << 11);
	flags |= (ushort)((ushort)(AA ? 1 : 0) << 10);
	flags |= (ushort)((ushort)(TC ? 1 : 0) << 9);
	flags |= (ushort)((ushort)(RD ? 1 : 0) << 8);
	flags |= (ushort)((ushort)(RA ? 1 : 0) << 7);
	flags |= (ushort)(0) << 6; // the Z bit
	flags |= (ushort)((ushort)(RCODE == ResponseType.IsQuery ? 0 : RCODE) << 3);
	
	data[1] = flags;
	data[2] = QDCOUNT;
	data[3] = ANCOUNT;
	data[4] = NSCOUNT;
	data[5] = ARCOUNT;

	foreach (var item in data)
	{
	    Console.WriteLine(Convert.ToString(item, 2).PadLeft(16, '0'));
	}

	Byte[] result = new Byte[12];
	for (int i = 0; i < data.Length; i++)
	{
	    byte[] bytes = BitConverter.GetBytes(data[i]);
	    
	    if(BitConverter.IsLittleEndian)
	    {
		result[i * 2] = bytes[1];
		result[i * 2 + 1] = bytes[0];
	    }
	    else
	    {
		result[i * 2] = bytes[0];
		result[i * 2 + 1] = bytes[1];	
	    }
	}

	return result;
    }
}

public enum RRType : ushort
{
    A = 1,
    NS = 2,
    MD = 3,
    MF = 4,
    CNAME = 5,
    SOA = 6,
    MB = 7,
    MG = 8,
    MR = 9,
    NULL = 10,
    WKS = 11,
    PTR = 12,
    HINFO = 13,
    MINFO = 14,
    MX = 15,
    TXT = 16
}

// QType is a super set of RRType
public enum QType : ushort
{
    A = RRType.A,
    NS = RRType.NS,
    MD = RRType.MD,
    MF = RRType.MF,
    CNAME = RRType.CNAME,
    SOA = RRType.SOA,
    MB = RRType.MB,
    MG = RRType.MG,
    MR = RRType.MR,
    NULL = RRType.NULL,
    WKS = RRType.WKS,
    PTR = RRType.PTR,
    HINFO = RRType.HINFO,
    MINFO = RRType.MINFO,
    MX = RRType.MX,
    TXT = RRType.TXT,
    AXFR = 252,
    MAILB = 253,
    MAILA = 254,
    ANY = 255,
}

public enum RecordClass : ushort
{
    IN = 1,
    CS = 2,
    CH = 3,
    HS = 4,
}

public enum QClass : ushort
{
    IN = RecordClass.IN,
    CS = RecordClass.CS,
    CH = RecordClass.CH,
    HS = RecordClass.HS,
    ANY = 255
}

public class Question
{
    public string QNAME { get; set; }
    public QType QTYPE { get; set; }
    public QClass QCLASS { get; set; }

    public Question(string name, QType qtype = QType.A, QClass qclass = QClass.IN)
    {
	QNAME = name;
	QTYPE = qtype;
	QCLASS = qclass;
    }
}

public class RR
{
    public string NAME { get; set; }
    public RRType TYPE { get; set; }
    public RecordClass CLASS { get; set; }
    public int TTL { get; set; }
    public ushort RDLENGTH { get; set; }
    public byte[] RDATA { get; set; }
}

public class Message
{
    public Header header { get; set; }
    public Question question { get; set; }
    public RR[] Answer { get; set; } = [];
    public RR[] Authority { get; set; } = [];
    public RR[] Additional { get; set; } = [];

    // create a query message
    public Message(string name, MessageType messageType =  MessageType.Query)
    {
	if (messageType == MessageType.Response)
	{
	    // TODO: implement later
	}

	Header header = Header.NewQuery();
	Question question = new Question(name);
    }

    // public Encode() {
	
    // }
}
