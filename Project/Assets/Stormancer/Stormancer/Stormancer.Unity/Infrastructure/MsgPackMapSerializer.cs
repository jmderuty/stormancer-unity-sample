
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stormancer.Client45.Infrastructure
{
    public class MsgPackMapSerializer : MsgPackSerializer
    {
        public new const string NAME = "msgpack/map";

        public MsgPackMapSerializer(ClientConfiguration config) : base(config) { }

        public MsgPackMapSerializer(ClientConfiguration config, IEnumerable<IMsgPackSerializationPlugin> plugins) : base(config, plugins) { }

        protected override MsgPack.Serialization.SerializationContext CreateSerializationContext()
        {
            var ctx = base.CreateSerializationContext();

            ctx.SerializationMethod = MsgPack.Serialization.SerializationMethod.Map;
            return ctx;
        }

        public override string Name
        {
            get { return NAME; }
        }
    }
}
