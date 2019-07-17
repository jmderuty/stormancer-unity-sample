using MsgPack;
using MsgPack.Serialization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Stormancer.Infrastructure
{
    /// <summary>
    /// Serializer based on MsgPack.
    /// </summary>
    public class MsgPackSerializer : ISerializer
    {
        internal static ISerializer Instance = new MsgPackSerializer();

        private readonly IEnumerable<IMsgPackSerializationPlugin> _plugins;

        private ConcurrentDictionary<Type, object> _serializersCache = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Creates a new MsgPackSerializer object
        /// </summary>
        public MsgPackSerializer() : this(null) { }

        /// <summary>
        /// Creates a new MsgPackSerializer object with plugins
        /// </summary>
        /// <param name="plugins">A collection of serialization plugins</param>
        public MsgPackSerializer(IEnumerable<IMsgPackSerializationPlugin> plugins)
        {
            if (plugins == null)
            {
                plugins = Enumerable.Empty<IMsgPackSerializationPlugin>();
            }

            this._plugins = plugins;
        }

        /// <summary>
        /// Serializes an object into a stream
        /// </summary>
        /// <typeparam name="T">The Type of the object to deserialize</typeparam>
        /// <param name="data">An object to serialize</param>
        /// <param name="stream">A Stream into which the object will be serialized</param>
        /// <remarks>
        /// The method doesn't close the stream
        /// </remarks>
        public void Serialize<T>(T data, System.IO.Stream stream)
        {
            var ctx = GetSerializationContext();
            var serializer = ctx.GetSerializer<T>();// (MsgPack.Serialization.MessagePackSerializer<T>)_serializersCache.GetOrAdd(typeof(T),k=> MsgPack.Serialization.MessagePackSerializer.Get<T>(GetSerializationContext()));


            serializer.PackTo(Packer.Create(stream, PackerCompatibilityOptions.None, false), data);
        }

        /// <summary>
        /// Deserialize an instance of T from a stream.
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="stream">A binary stream the object will be deserialized from</param>
        /// <remarks>
        /// The method don't close the stream
        /// </remarks>
        /// <returns>An instance of T deserialized from the stream</returns>
        public T Deserialize<T>(System.IO.Stream stream)
        {
            var ctx = GetSerializationContext();
            var serializer = ctx.GetSerializer<T>();// (MsgPack.Serialization.MessagePackSerializer<T>)_serializersCache.GetOrAdd(typeof(T),k=> MsgPack.Serialization.MessagePackSerializer.Get<T>(GetSerializationContext()));

            var unpacker = Unpacker.Create(stream, false);
            unpacker.Read();
            return serializer.UnpackFrom(unpacker);
        }

        /// <summary>
        /// Builds the msgpack serialization context for this serializer.
        /// </summary>
        /// <returns>
        /// The new serialization context.
        /// </returns>
        protected virtual SerializationContext GetSerializationContext()
        {
            if (System.Threading.Interlocked.CompareExchange(ref _initialized, 1, 0) == 0)
            {
                InitializeSerializationContext(_ctx);

            }
            return _ctx;
        }
        private static int _initialized = 0;
        private static MsgPack.Serialization.SerializationContext _ctx = new MsgPack.Serialization.SerializationContext();

        protected virtual void InitializeSerializationContext(SerializationContext ctx)
        {


            ctx.Serializers.Register(new MsgPackLambdaTypeSerializer<JToken>(packJToken, unpackJToken, ctx));
            ctx.Serializers.Register(new MsgPackLambdaTypeSerializer<JArray>(packJArray, unpackJArray, ctx));
            ctx.Serializers.Register(new MsgPackLambdaTypeSerializer<JObject>(packJObject, unpackJObject, ctx));
            ctx.Serializers.Register(new MsgPackLambdaTypeSerializer<JValue>(packJValue, unpackJValue, ctx));

            foreach (var plugin in _plugins)
            {
                plugin.OnCreatingSerializationContext(ctx);
            }
        }

        /// <summary>
        /// Name of the serializer
        /// </summary>
        /// <remarks>
        /// Returns 'msgpack/array'
        /// </remarks>
        public virtual string Name {
            get { return "msgpack/array"; }
        }

        private void packJArray(Packer packer, JArray array, SerializationContext ctx)
        {
            packer.PackArray(array, ctx);
        }

        private void packJToken(Packer packer, JToken token, SerializationContext ctx)
        {
            if (token is JValue)
            {
                packJValue(packer, (JValue)token, ctx);
            }
            else if (token is JArray)
            {
                packJArray(packer, (JArray)token, ctx);
            }
            else if (token is JObject)
            {
                packJObject(packer, (JObject)token, ctx);
            }
        }

        private static void packJValue(Packer packer, JValue value, SerializationContext ctx)
        {
            switch (value.Type)
            {
                case JTokenType.Null:
                    packer.PackNull();
                    break;
                //case JTokenType.Date:
                //    packer.Pack(((DateTime)value.Value).Ticks);
                //    break;
                //case JTokenType.String:
                //    packer.PackString((string)value.Value);
                //    break;
                //case JTokenType.Float:

                //    break;
                default:
                    packer.Pack(value.Value);
                    break;

            }
        }

        private void packJObject(Packer packer, JObject obj, SerializationContext ctx)
        {
            packer.PackMap(obj, ctx);
        }

        private JToken unpackJToken(Unpacker unpacker, SerializationContext ctx)
        {
            if (unpacker.IsArrayHeader)
            {
                return unpackJArray(unpacker, ctx);
            }
            else if (unpacker.IsMapHeader)
            {
                return unpackJObject(unpacker, ctx);
            }
            else
            {

                return unpackJValue(unpacker, ctx);
            }
        }

        private JValue unpackJValue(Unpacker unpacker, SerializationContext ctx)
        {
            var data = unpacker.LastReadData;
            if (data.IsTypeOf<string>() ?? false)
            {
                return new JValue(data.AsString());
            }
            else if (data.IsTypeOf<long>() ?? false)
            {
                return new JValue(data.AsInt64());
            }
            else if (data.IsTypeOf<float>() ?? false)
            {
                return new JValue(data.AsSingle());
            }
            else if (data.IsTypeOf<double>() ?? false)
            {
                return new JValue(data.AsDouble());
            }
            else if (data.IsNil)
            {
                return JValue.CreateNull();
            }
            else if (data.IsTypeOf<bool>() ?? false)
            {
                return new JValue(data.AsBoolean());
            }
            else if (data.IsTypeOf<byte[]>() ?? false)
            {
                return new JValue((byte[])data.ToObject());
            }
            else if (data.IsTypeOf<MessagePackExtendedTypeObject>() ?? false)
            {
                var ext = data.AsMessagePackExtendedTypeObject();
                if (ext.TypeCode == 0xff)
                {
                    return new JValue(MsgPack.Timestamp.Decode(ext).ToDateTime());
                }
            }
            throw new NotSupportedException($"Couldn't unpack {data} into JValue");
        }

        private JArray unpackJArray(Unpacker unpacker, SerializationContext ctx)
        {
            //backward compatibility
            if (unpacker.LastReadData.IsTypeOf<string>() ?? false)
            {
                return (JArray)JToken.Parse(unpacker.LastReadData.AsString());
            }

            var array = new JArray();
            long length = unpacker.ItemsCount;

            for (int i = 0; i < length; i++)
            {
                unpacker.Read();
                JToken token = unpacker.Unpack<JToken>(ctx);
                if (token == null)
                {
                    token = JValue.CreateNull();
                }
                array.Add(token);
            }
            return array;
        }

        private JObject unpackJObject(Unpacker unpacker, SerializationContext ctx)
        {
            //backward compatibility
            if (unpacker.LastReadData.IsTypeOf<string>() ?? false)
            {
                return (JObject)JToken.Parse(unpacker.LastReadData.AsString());
            }

            var map = new JObject();
            long length = unpacker.ItemsCount;

            //var subtree = data.ReadSubtree();
            //var length = subtree.LastReadData.AsInt64();

            for (long i = 0; i < length; i++)
            {
                unpacker.Read();
                string key = unpacker.LastReadData.AsString();

                unpacker.Read();
                JToken token = unpacker.Unpack<JToken>(ctx);
                if (token == null)
                {
                    token = JValue.CreateNull();
                }
                map.Add(key, token);
            }

            return map;
        }

    }


    /// <summary>
    /// A custom msgPack serializer that allows to declare its serialization logic using lambda methods
    /// </summary>
    /// <typeparam name="T">The type that this serializer will serialize/deserialize</typeparam>
    public class MsgPackLambdaTypeSerializer<T> : MessagePackSerializer<T>
    {
        private readonly Action<MsgPack.Packer, T, SerializationContext> _pack;
        private readonly Func<MsgPack.Unpacker, SerializationContext, T> _unpack;

        /// <summary>
        /// Creates a MsgPackLambdaTypeSerializer instance
        /// </summary>
        /// <param name="pack">An action that is executed when an instance of T has to be serialized</param>
        /// <param name="unpack">A function that is executed when an instance of T has to be deserialized</param>
        /// <param name="ctx">The serialization context</param>
        public MsgPackLambdaTypeSerializer(Action<MsgPack.Packer, T, SerializationContext> pack, Func<MsgPack.Unpacker, SerializationContext, T> unpack, SerializationContext ctx)
            : base(ctx)
        {
            _pack = pack;
            _unpack = unpack;
        }

        /// <summary>
        /// Serializes the target object
        /// </summary>
        /// <param name="packer"></param>
        /// <param name="objectTree"></param>
        protected internal override void PackToCore(MsgPack.Packer packer, T objectTree)
        {
            _pack(packer, objectTree, this.OwnerContext);
        }

        /// <summary>
        /// Deserializes the target object
        /// </summary>
        /// <param name="unpacker"></param>
        /// <returns></returns>
        protected internal override T UnpackFromCore(MsgPack.Unpacker unpacker)
        {
            return _unpack(unpacker, this.OwnerContext);
        }
    }

    /// <summary>
    /// Declares a plugin that can customize the msgpack serialization process
    /// </summary>
    public interface IMsgPackSerializationPlugin
    {
        /// <summary>
        /// Registers custom serializers into the msgpack serializer
        /// </summary>
        /// <param name="ctx">A SerializationContext instance that allows the plugin to register custom serialization logic</param>
        void OnCreatingSerializationContext(SerializationContext ctx);
    }


}
