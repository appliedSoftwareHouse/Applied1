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

        public static Message SetMessage(string _Message, ConsoleColor _Colour)
        {
            Message message = new Message
            {
                ErrorID = -1,
                Msg = _Message,
                Colour = GetColor(_Colour)
            };
            return message;
        }

        public static Message SetMessage(string _Message, int _MessageID, ConsoleColor _Colour)
        {
            Message message = new Message
            {
                ErrorID = _MessageID,
                Msg = _Message,
                Colour = GetColor(_Colour)
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




        public class Message
        {

            public bool Success { get; set; } = false;
            public string Msg { get; set; } = string.Empty;
            public int ErrorID { get; set; } = 0;
            public Color Colour { get; set; } = Color.Orange;
        }

        private static Color GetColor(ConsoleColor _Color)
        {
            return _Color switch
            {
                ConsoleColor.Black => Color.Black,
                ConsoleColor.DarkBlue => Color.DarkBlue,
                ConsoleColor.DarkGreen => Color.DarkGreen,
                ConsoleColor.DarkCyan => Color.DarkCyan,
                ConsoleColor.DarkRed => Color.DarkRed,
                ConsoleColor.DarkMagenta => Color.DarkMagenta,
                ConsoleColor.DarkYellow => Color.YellowGreen,
                ConsoleColor.Gray => Color.Gray,
                ConsoleColor.DarkGray => Color.DarkGray,
                ConsoleColor.Blue => Color.Blue,
                ConsoleColor.Green => Color.Green,
                ConsoleColor.Cyan => Color.Cyan,
                ConsoleColor.Red => Color.Red,
                ConsoleColor.Magenta => Color.Magenta,
                ConsoleColor.Yellow => Color.Yellow,
                ConsoleColor.White => Color.White,
                _ => Color.Brown,
            };
        }

    }
}
