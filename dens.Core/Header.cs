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
            RCODE = ResponseType.Ok,
            QDCOUNT = 0x01,
            ANCOUNT = 0x00,
            NSCOUNT = 0x00,
            ARCOUNT = 0x00
        };
    }

    public static Header Decode(byte[] header)
    {
        ushort[] headerUShort = new ushort[6];
        for (var i = 0; i < header.Length - 2; i += 2)
        {
            headerUShort[i / 2] = Utils.ToUInt16(header[i], header[i + 1]);
        }

        ushort ID = headerUShort[0];
        ushort secondLineFlags = headerUShort[1];
        ushort QDCOUNT = headerUShort[2];
        ushort ANCOUNT = headerUShort[3];
        ushort NSCOUNT = headerUShort[4];
        ushort ARCOUNT = headerUShort[5];

        var QR = (secondLineFlags >> 15) & 1;
        var OPCODE = (secondLineFlags >> 11) & ((1 << 4) - 1);
        var AA = (secondLineFlags >> 10) & 1;
        var TC = (secondLineFlags >> 9) & 1;
        var RD = (secondLineFlags >> 8) & 1;
        var RA = (secondLineFlags >> 7) & 1;
        var RCODE = (secondLineFlags >> 3) & ((1 << 4) - 1);

        return new Header
        {
            ID = ID,
            QR = (MessageType)QR,
            OPCODE = (QueryType)OPCODE,
            AA = AA == 1,
            TC = TC == 1,
            RD = RD == 1,
            RA = RA == 1,
            RCODE = (ResponseType)RCODE,
            QDCOUNT = QDCOUNT,
            ANCOUNT = ANCOUNT,
            NSCOUNT = NSCOUNT,
            ARCOUNT = ARCOUNT
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
        flags |= (ushort)(0 << 6); // the Z bit
        flags |= (ushort)((ushort)RCODE << 3);

        data[1] = flags;
        data[2] = QDCOUNT;
        data[3] = ANCOUNT;
        data[4] = NSCOUNT;
        data[5] = ARCOUNT;

        Byte[] result = new Byte[12];

        for (int i = 0; i < data.Length; i++)
        {
            byte[] bytes = Utils.GetBytes(data[i]);

            result[i * 2] = bytes[0];
            result[i * 2 + 1] = bytes[1];
        }

        return result;
    }
}
