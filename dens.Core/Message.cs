namespace dens.Core;

using System.Text;

public static class Utils
{
    public static ushort ToUInt16(byte[] data)
    {
	if(BitConverter.IsLittleEndian)
	{
	    Array.Reverse(data);
	}

	return BitConverter.ToUInt16(data);
    }
}

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

    public static Header Decode(byte[] header)
    {
	ushort[] headerUShort = new ushort[6];
	for (var i = 0; i < header.Length - 2; i += 2)
	{
	    if(BitConverter.IsLittleEndian)
	    {
		headerUShort[i / 2] = BitConverter.ToUInt16(new Byte[2]{header[i + 1], header[i]}, 0);
	    } else
	    {
		headerUShort[i / 2] = BitConverter.ToUInt16(new Byte[2]{header[i], header[i + 1]}, 0);		
	    }

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
            RCODE = QR == 0 ? ResponseType.IsQuery : (ResponseType)RCODE,
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
	flags |= (ushort)(0) << 6; // the Z bit
	flags |= (ushort)((ushort)(RCODE == ResponseType.IsQuery ? 0 : RCODE) << 3);
	
	data[1] = flags;
	data[2] = QDCOUNT;
	data[3] = ANCOUNT;
	data[4] = NSCOUNT;
	data[5] = ARCOUNT;

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

    public static (Question, int) Decode(byte[] message, int pointer)
    {
	var (name, nextPointer) = Message.DecodeName(message, pointer);

	ushort qtype = Utils.ToUInt16(new Byte[2]{message[nextPointer], message[nextPointer + 1]});
	ushort qclass = Utils.ToUInt16(new Byte[2]{message[nextPointer + 2], message[nextPointer + 3]});

	return (new Question(name, (QType)qtype, (QClass)qclass), nextPointer + 4);
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

	header = Header.NewQuery();
	question = new Question(name);
    }

    public static bool IsPointer(byte data)
    {

	return (data >> 6) == 0b_0000_0011;
    }

    public static (string, int) ParseLabel(Byte[] nameByte, int pointer)
    {
	byte length = nameByte[pointer];
	
	int firstCharIndex = pointer + 1;
	string label = "";

	for (var i = firstCharIndex; i < firstCharIndex + length; i++)
	{
	    char letter = (char)nameByte[i];
	    label = string.Concat(label, letter.ToString());
	}

	return (label, firstCharIndex + length);
    }

    public static string ParsePointer(Byte[] message, int pointer)
    {
	Byte[] pointerValue;

	if(BitConverter.IsLittleEndian)
	{
	    pointerValue = new Byte[2]{message[pointer + 1], message[pointer]}; 
	}
	else
	{
	    pointerValue = new Byte[2]{message[pointer], message[pointer + 1]};    
	}

	
	var bitMask = (1 << 14) - 1;

	
	var pointTo = BitConverter.ToUInt16(pointerValue) & bitMask;
	Console.WriteLine(pointTo);
	var (name, _) = DecodeName(message, pointTo);

	return name;
    }

    public enum ParseState
    {
	// Check state is to confirm
	// if next action is to parse a label or a pointer.
	Check, 
	Label,
	Pointer,
	Root,
    }

    public static (string, int) DecodeName(Byte[] message, int pointer)
    {
	var state = ParseState.Check;
	string name = "";

	while (true)
	{
	    if (state == ParseState.Check)
	    {
		var length = message[pointer];

		// root
		if (length == 0)
		{
		    // remove trailing dot
		    name = name.Substring(0, name.Length - 1);
		    pointer++;
		    break;
		}

		if (IsPointer(length))
		{
		    state = ParseState.Pointer;
		}
		else
		{
		    state = ParseState.Label;
		}
	    }
	    else if (state == ParseState.Label)
	    {
		var (label, nextPointer) = ParseLabel(message, pointer);
		name = string.Concat(name, label);
		name = string.Concat(name, ".");
		pointer = nextPointer;
		state = ParseState.Check;
	    }
	    else if (state == ParseState.Pointer)
	    {
		var label = ParsePointer(message, pointer);
		name = string.Concat(name, label);
		pointer += 2;
		break;
	    }
	    else {
		break;
	    }
	}

	return (name, pointer);
    }

    public static Byte[] EncodeName(string name)
    {
	// TODO: Check if name is a valid domain name
	var utf8 = new UTF8Encoding();
	string[] labels = name.Split(".");
	List<byte> NameByte = new List<byte>{};

	foreach (var label in labels)
	{
	    int length = utf8.GetBytes(label).Length;
	    Byte[] data = BitConverter.GetBytes(length);
	    NameByte.Add(data[0]);
	    foreach (byte b in label)
	    {
		NameByte.Add(b);
	    }
	    // if (label.Length % 2 == 0)
	    // {
	    // 	NameByte.Add(0);
	    // }
	}
	NameByte.Add(0);

	return NameByte.ToArray();
    }

    public Byte[] Encode() {
	Byte[] headerByte = header.Encode();
	Byte[] nameByte = EncodeName(question.QNAME);
	Byte[] qType = BitConverter.GetBytes((ushort)question.QTYPE);
	Byte[] qClass = BitConverter.GetBytes((ushort)question.QCLASS);

	int length = headerByte.Length + nameByte.Length + qType.Length + qClass.Length;
        byte[] result = new byte[length];

        int offset = 0;
        Array.Copy(headerByte, 0, result, offset, headerByte.Length);
        offset += headerByte.Length;

        Array.Copy(nameByte, 0, result, offset, nameByte.Length);
        offset += nameByte.Length;

        Array.Copy(qType, 0, result, offset, qType.Length);
        offset += qType.Length;

        Array.Copy(qClass, 0, result, offset, qClass.Length);

	return result;
    }
}
