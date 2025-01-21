namespace dens.Core;

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
    AAAA = RRType.AAAA,
    AXFR = 252,
    MAILB = 253,
    MAILA = 254,
    ANY = 255,
}

public enum QClass : ushort
{
    IN = RRClass.IN,
    CS = RRClass.CS,
    CH = RRClass.CH,
    HS = RRClass.HS,
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

    public static (Question, int) Decode(byte[] message, int pointer)
    {
        var (name, nextPointer) = Message.DecodeName(message, pointer);

        ushort qtype = Utils.ToUInt16(message[nextPointer], message[nextPointer + 1]);
        ushort qclass = Utils.ToUInt16(message[nextPointer + 2], message[nextPointer + 3]);

        return (new Question(name, (QType)qtype, (QClass)qclass), nextPointer + 4);
    }

    public byte[] Encode()
    {
        Byte[] nameByte = Message.EncodeName(QNAME);
        Byte[] qType = Utils.GetBytes((ushort)QTYPE);
        Byte[] qClass = Utils.GetBytes((ushort)QCLASS);

        return nameByte.Concat(qType).Concat(qClass).ToArray();
    }

}
