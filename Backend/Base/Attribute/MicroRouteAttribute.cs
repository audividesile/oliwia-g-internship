using System;

namespace VigilantMeerkat.Micro.Base.Attribute
{
    public class MicroRouteAttribute : System.Attribute
    {
        public string Route { get; }
        public Type ProtoType { get; }

        public MicroRouteAttribute(string route)
        {
            Route = route;
        }
        
        public MicroRouteAttribute(string route, Type protoType)
        {
            Route = route;
            ProtoType = protoType;
        }
    }
}