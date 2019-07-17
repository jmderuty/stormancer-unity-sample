using MsgPack.Serialization;
using System.Collections.Generic;

namespace Stormancer.Infrastructure
{
    public class MsgPackMapSerializerNativeDates : MsgPackMapSerializer
    {
        protected override void InitializeSerializationContext(SerializationContext ctx)
        {
            base.InitializeSerializationContext(ctx);
            ctx.DefaultDateTimeConversionMethod = DateTimeConversionMethod.Native;
        }

        protected override MsgPack.Serialization.SerializationContext GetSerializationContext()
        {
            if (System.Threading.Interlocked.CompareExchange(ref _initialized, 1, 0) == 0)
            {
                InitializeSerializationContext(_ctx);

            }
            return _ctx;
        }
        private static int _initialized = 0;
        private static MsgPack.Serialization.SerializationContext _ctx = new MsgPack.Serialization.SerializationContext();
        public override string Name {
            get { return "msgpack/map-nativeDates"; }
        }
    }
    public class MsgPackMapSerializer : MsgPackSerializer
    {
        public const string NAME = "msgpack/map";

        public MsgPackMapSerializer() : base()
        {

        }

        public MsgPackMapSerializer(IEnumerable<IMsgPackSerializationPlugin> plugins) : base(plugins) { }

        protected override MsgPack.Serialization.SerializationContext GetSerializationContext()
        {
            if (System.Threading.Interlocked.CompareExchange(ref _initialized, 1, 0) == 0)
            {
                InitializeSerializationContext(_ctx);

            }
            return _ctx;
        }
        private static int _initialized = 0;
        private static MsgPack.Serialization.SerializationContext _ctx = new MsgPack.Serialization.SerializationContext();


        protected override void InitializeSerializationContext(SerializationContext ctx)
        {
            base.InitializeSerializationContext(ctx);
            ctx.SerializationMethod = MsgPack.Serialization.SerializationMethod.Map;

        }

        public override string Name {
            get { return NAME; }
        }
    }
}
