using Google.Protobuf;

namespace VigilantMeerkat.Micro.Base.Transformation
{
    public static class ProtoTransformation
    {
        public static AmqpMessage TransformMessage(byte[] msg)
        {
            return AmqpMessage.Parser.ParseFrom(msg);
        }
    }
}