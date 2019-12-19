using System.Collections.Generic;
using LiteNetLib.Utils;

namespace IAHNetCoreServer.Server
{
    public class SimplePoolWriter
    {
        // My Collection Pool
        private readonly Queue<NetDataWriter> _FreeWriters;

        public SimplePoolWriter(int initSize = 10)
        {
            _FreeWriters = new Queue<NetDataWriter>(initSize);
            for (int i = 0; i < initSize; i++)
            {
                _FreeWriters.Enqueue(MakeNew());
            }
        }

        private NetDataWriter MakeNew()
        {
            return new NetDataWriter();
        }

        public NetDataWriter Spawn()
        {
            lock (_FreeWriters)
            {
                if (_FreeWriters.Count > 0)
                {
                    // Retrieve from pool
                    return _FreeWriters.Dequeue();
                }
                else
                {
                    return MakeNew();
                }
            }
        }

        public void UnSpawn(NetDataWriter writer)
        {
            writer.Reset();
            lock (_FreeWriters)
            {
                _FreeWriters.Enqueue(writer);
            }
        }
    }
}