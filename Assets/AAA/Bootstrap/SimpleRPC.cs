using Unity.NetCode;

namespace AAA.Bootstrap
{
    public struct SimpleRPC : IRpcCommand
    {
        public int Value;
    }
}