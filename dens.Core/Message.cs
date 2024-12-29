namespace dens.Core;

public enum MessageType
{
    Query = 0,
    Response = 1,
}

public enum QueryType
{
    Query = 0,
    IQuery = 1,
    Status = 2,
}

public enum ResponseType
{
    Ok = 0,
    FormatError = 1,
    ServerFailure = 2,
    NameError = 3,
    NotImplemented = 4,
    Refused = 5,
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
    public RR[] Answer { get; set; }
    public RR[] Authority { get; set; }
    public RR[] Additional { get; set; }

    public Message()
    {
	
    }
}
