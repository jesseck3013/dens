namespace dens.Core;

using System.Net;

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
    TXT = 16,   
    AAAA = 28,
}

public enum RRClass : ushort
{
    IN = 1,
    CS = 2,
    CH = 3,
    HS = 4,
}

public class RR
{
    public string NAME { get; set; } = String.Empty;
    public RRType TYPE { get; set; }
    public RRClass CLASS { get; set; }
    public uint TTL { get; set; }
    public ushort RDLENGTH { get; set; }
    public string RDATA { get; set; } = String.Empty;

    public override string ToString()
    {
	return $"{TYPE}    {CLASS}    {RDATA}\n";
    }

    public static bool IsName(RRType type)
    {
	switch (type)
	{
	    case RRType.CNAME:
		return true;
	    case RRType.NS:
		return true;
	    case RRType.PTR:
		return true;
	    case RRType.TXT:
		return true;
	    default:
		return false;
	}
    }

    public static bool IsIp(RRType type)
    {
	switch (type)
	{
	    case RRType.A:
		return true;
	    case RRType.AAAA:
		return true;
	    default:
		return false;
	}
    }

    // TODO: only support A record now, add support for other types later.
    public static (RR, int) Decode(byte[] message, int pointer)
    {
        var (name, nextPointer) = Message.DecodeName(message, pointer);

        var type = (RRType)Utils.ToUInt16(
            message[nextPointer],
            message[nextPointer + 1]);

        var recordClass = (RRClass)Utils.ToUInt16(
            message[nextPointer + 2],
            message[nextPointer + 3]);

        var ttl = Utils.ToUInt32(
            message[nextPointer + 4],
            message[nextPointer + 5],
            message[nextPointer + 6],
            message[nextPointer + 7]);

        var rdLength = Utils.ToUInt16(
            message[nextPointer + 8],
            message[nextPointer + 9]);

        var dataByte = new ArraySegment<byte>(message).Slice(nextPointer + 10, rdLength).ToArray();
        var data = String.Empty;
        if (IsIp(type))
        {
            var ip = new IPAddress(dataByte);
	    data = ip.ToString();
        }
        if (IsName(type))
        {
            var (nameData, _) = Message.DecodeName(message, nextPointer + 10);
            data = nameData;
        }

        var rr = new RR
        {
            NAME = name,
            TYPE = type,
            CLASS = recordClass,
            TTL = ttl,
            RDLENGTH = rdLength,
            RDATA = data,
        };
        return (rr, nextPointer + 10 + rdLength);
    }
}
