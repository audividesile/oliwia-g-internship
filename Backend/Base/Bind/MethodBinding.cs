using System;
using System.Reflection;

namespace VigilantMeerkat.Micro.Base.Bind
{
    public class MethodBinding
    {
        public MethodInfo Method { get; set; }
        public Type ProtoType { get; set; }
        public bool IsAuth { get; set; }
    }
}