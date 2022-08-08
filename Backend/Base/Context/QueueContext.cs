using VigilantMeerkat.Micro.Base.Bind;

namespace VigilantMeerkat.Micro.Base.Context
{
    public class QueueContext
    {
        public MethodBinding Binding { get; set; }
        public object Instance { get; set; }
    }
}