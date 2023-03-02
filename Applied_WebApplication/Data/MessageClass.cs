using System.Drawing;

namespace Applied_WebApplication.Data
{
    public class MessageClass
    {



        public static Message SetMessage(string _Message)
        {
            Message message = new Message
            {
                Msg = _Message
            };
            return message;
        }

        public static Message SetMessage(string _Message, int _MessageID)
        {
            Message message = new Message
            {
                ErrorID = _MessageID,
                Msg = _Message
            };
            return message;
        }

        public static Message SetMessage(string _Message, int _MessageID, Color _Colour)
        {
            Message message = new Message
            {
                ErrorID = _MessageID,
                Msg = _Message,
                Colour = _Colour
            };
            return message;
        }

        public static Message SetMessage(string _Message, int _MessageID, Color _Colour, bool Success)
        {
            Message message = new Message
            {
                ErrorID = _MessageID,
                Msg = _Message,
                Colour = _Colour,
                Success = Success
            };
            return message;
        }

        public static Message SetMessage(string _Message, Color _Colour)
        {
            Message message = new Message
            {

                Msg = _Message,
                Colour = _Colour,

            };
            return message;
        }

    }


    public class Message
    {

        public bool Success { get; set; } = false;
        public string Msg { get; set; } = string.Empty;
        public int ErrorID { get; set; } = 0;
        public Color Colour { get; set; } = Color.Orange;
    }





}
