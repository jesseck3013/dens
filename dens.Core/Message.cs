namespace dens.Core;

using System.Text;
using System.Net;

public class Message
{
    public Header header { get; set; }
    public Question[] questions { get; set; }
    public RR[] answers { get; set; } = [];
    public RR[] authoritys { get; set; } = [];
    public RR[] additionals { get; set; } = [];

    // create a query message
    public Message(string name, QType qtype = QType.A, MessageType messageType = MessageType.Query)
    {
        if (messageType == MessageType.Response)
        {
            // TODO: implement later
        }

        header = Header.NewQuery();
        questions = [new Question(name, qtype)];
    }

    public Message(Header header, Question[] questions, RR[] answers, RR[] authoritys, RR[] additionals)
    {
        this.header = header;
        this.questions = questions;
        this.answers = answers;
        this.authoritys = authoritys;
        this.additionals = additionals;
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
        ushort pointerValue = Utils.ToUInt16(message[pointer], message[pointer + 1]);
        var bitMask = (1 << 14) - 1;
        var pointTo = pointerValue & bitMask;
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
            else
            {
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
        List<byte> NameByte = new List<byte> { };

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

    public Byte[] Encode()
    {
        Byte[] headerByte = header.Encode();
        Byte[] questionByte = [];


        foreach (var question in questions)
        {
            questionByte = questionByte.Concat(question.Encode()).ToArray();
        }

        Byte[] result = headerByte.Concat(questionByte).ToArray();

        return result;
    }

    public static Message Decode(byte[] message)
    {
        var headerBytes = new ArraySegment<byte>(message);
        var header = Header.Decode(headerBytes.Slice(0, 12).ToArray());

        var questions = new List<Question>();
        var answers = new List<RR>();
        var authoritys = new List<RR>();
        var additionals = new List<RR>();

        int pointer = 12;

        while (pointer < message.Length)
        {
            for (int i = 0; i < header.QDCOUNT; i++)
            {
                var (item, nextPointer) = Question.Decode(message, pointer);
                pointer = nextPointer;
                questions.Add(item);
            }

            for (int i = 0; i < header.ANCOUNT; i++)
            {
                var (item, nextPointer) = RR.Decode(message, pointer);
                pointer = nextPointer;
		answers.Add(item);
            }

            for (int i = 0; i < header.NSCOUNT; i++)
            {
                var (item, nextPointer) = RR.Decode(message, pointer);
                pointer = nextPointer;
                authoritys.Add(item);
            }

            for (int i = 0; i < header.ARCOUNT; i++)
            {
                var (item, nextPointer) = RR.Decode(message, pointer);
                pointer = nextPointer;
                additionals.Add(item);
            }
        }
        return new Message(
            header,
            questions.ToArray(),
            answers.ToArray(),
            authoritys.ToArray(),
            additionals.ToArray()
        );
    }

    public override string ToString()
    {
	string result = "";

	result += "++++ Answer ++++\n";
	foreach (var rr in answers)
	{
	    result += rr.ToString();
	}

	if (authoritys.Length > 0)
	{
	    result += "++++ Authority ++++\n";	    
	}
	foreach (var rr in authoritys)
	{
	    result += rr.ToString();
	}

	if (authoritys.Length > 0)
	{
	    result += "++++ Additional ++++\n";
	}
	foreach (var rr in additionals)
	{
	    result += rr.ToString();
	}

	return result;
    }
}
