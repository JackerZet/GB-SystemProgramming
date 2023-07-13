using System.Text;

namespace Homeworks.homework_3
{
    internal static class MessageIDs
    {
        public const string RENAMING_COMMAND = "/n";


        public static string MessageToCommand(string message, string command)
        {
            StringBuilder sb = new StringBuilder(message);
            sb.Insert(0, command);
            return sb.ToString();
        }
        public static string CommandToMessage(string message, string command)
        {
            StringBuilder sb = new StringBuilder(message);
            sb.Remove(0, command.Length);
            return sb.ToString();
        }
    }
}
