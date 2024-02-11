using System.Runtime.InteropServices;
using System.Text;

namespace DistanceMeasure.Utils
{
    public static class MessageBuilder
    {
        public static byte[] BuildMessage(MessagesEnum value, byte[]? message = null)
        {
            message ??= [];

            int valueSize = sizeof(MessagesEnum);
            int valueSizeSize = sizeof(int);

            byte[] newMessage = new byte[valueSize + valueSizeSize + message.Length];
            
            Buffer.BlockCopy(BitConverter.GetBytes(valueSize),      0, newMessage, 0,                           valueSizeSize);
            Buffer.BlockCopy(BitConverter.GetBytes((Int32)value),   0, newMessage, valueSizeSize,               valueSize);
            Buffer.BlockCopy(message,                               0, newMessage, valueSize + valueSizeSize,   message.Length);

            return newMessage;
        }

        public static TemplateType GetMessage<TemplateType>(ref byte[] message)
        {
            Type templateType = typeof(TemplateType);

            if (templateType == typeof(byte[]) && templateType.IsArray)
            {
                return (TemplateType)(object)GetByteArray(ref message);
            }

            return templateType.Name switch
            {
                nameof(MessagesEnum) => (TemplateType)(object)GetMessagesEnumVar(ref message),
                nameof(ValueEnum) =>    (TemplateType)(object)GetValueEnumVar   (ref message),
                nameof(UInt64) =>       (TemplateType)(object)GetUInt64Var      (ref message),
                nameof(Int32) =>        (TemplateType)(object)GetInt32Var       (ref message),
                nameof(Boolean) =>      (TemplateType)(object)GetBooleanVar     (ref message),
                nameof(String) =>       (TemplateType)(object)GetStringVar      (ref message),
                _ => throw new NotImplementedException(),
            };
        }
        private static Int32 GetSize(ref byte[] message)
        {
            Int32 size = BitConverter.ToInt32(message, 0);

            message = message[sizeof(Int32)..];

            return size;
        }

        private static MessagesEnum GetMessagesEnumVar(ref byte[] message)
        {
            return (MessagesEnum)GetInt32Var(ref message);
        }
        private static ValueEnum GetValueEnumVar(ref byte[] message)
        {
            return (ValueEnum)GetInt32Var(ref message);
        }
        private static byte[] GetByteArray(ref byte[] message)
        {
            Int32 length = GetSize(ref message);
            byte[] value = message[..length];
            message = message[length..];
            return value;
        }   
        private static Int32 GetInt32Var(ref byte[] message)
        {
            int valueSize = GetSize(ref message);
            if (valueSize != sizeof(Int32))
            {
                throw new Exception("Invalid size");
            }

            return GetSize(ref message);
        }
        private static UInt64 GetUInt64Var(ref byte[] message)
        {
            int valueSize = GetSize(ref message);
            if (valueSize != sizeof(UInt64))
            {
                throw new Exception("Invalid size");
            }

            UInt64 value = BitConverter.ToUInt64(message, 0);

            message = message[valueSize..];

            return value;
        }   
        private static Boolean GetBooleanVar(ref byte[] message)
        {
            int valueSize = GetSize(ref message);
            if (valueSize != sizeof(Boolean))
            {
                throw new Exception("Invalid size");
            }

            Boolean value = BitConverter.ToBoolean(message, 0);

            message = message[valueSize..];

            return value;
        }
        private static string GetStringVar(ref byte[] message)
        {
            Int32 length = GetSize(ref message);
            string value = Encoding.UTF8.GetString(message, 0, length);
            message = message[length..];
            return value;
        }
    }
}
