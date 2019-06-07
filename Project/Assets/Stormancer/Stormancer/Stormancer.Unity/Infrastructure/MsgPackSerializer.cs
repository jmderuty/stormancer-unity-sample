using GeneratedSerializers;
using MsgPack;
using MsgPack.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

// >To avoid compilation error when no generated serializer
// have beed created
namespace GeneratedSerializers
{

}

namespace Stormancer.Client45.Infrastructure
{
    /// <summary>
    /// Serializer based on MsgPack.
    /// </summary>
    public class MsgPackSerializer : ISerializer
    {
        public const string NAME = "msgpack/array";

        private readonly IEnumerable<IMsgPackSerializationPlugin> _plugins;

        private Dictionary<RuntimeTypeHandle, Func<SerializationContext, MessagePackSerializer>> _serializersFactory = new Dictionary<RuntimeTypeHandle, Func<SerializationContext, MessagePackSerializer>>();
        
        public void RegisterSerializerFactory<T>(Func<SerializationContext, MessagePackSerializer<T>> factory)
        {
            _serializersFactory[typeof(T).TypeHandle] = (ctx) => factory(ctx);
        }

        private readonly ClientConfiguration _config;
        private readonly Lazy<SerializationContext> _serializationContextLazy;

        public MsgPackSerializer(ClientConfiguration config) : this(config, null) { }
        public MsgPackSerializer(ClientConfiguration config, IEnumerable<IMsgPackSerializationPlugin> plugins)
        {
            _config = config;

            if (plugins == null)
            {
                plugins = Enumerable.Empty<IMsgPackSerializationPlugin>();
            }

            this._plugins = plugins;

            _serializationContextLazy = new Lazy<SerializationContext>(CreateSerializationContext);

            RegisterSerializerFactories();
        }
        public void Serialize<T>(T data, System.IO.Stream stream)
        {
            //var serializer = (MessagePackSerializer<T>)_serializersCache.GetOrAdd(typeof(T), k => MessagePackSerializer.Get<T>(GetSerializationContext()));
            var serializer =MessagePackSerializer.Get<T>(SerializationContext);

            serializer.PackTo(Packer.Create(stream, false), data);
        }

        public T Deserialize<T>(System.IO.Stream stream)
        {

            //var serializer = (MessagePackSerializer<T>)_serializersCache.GetOrAdd(typeof(T), k => MessagePackSerializer.Get<T>(GetSerializationContext()));
            var serializer = MessagePackSerializer.Get<T>(SerializationContext);

            var unpacker = Unpacker.Create(stream, false);
            unpacker.Read();
            return serializer.UnpackFrom(unpacker);
        }

        private SerializationContext SerializationContext
        {
            get
            {
                return _serializationContextLazy.Value;
            }
        }

        private void RegisterSerializerFactories()
        {
            foreach (var plugin in _plugins)
            {
                plugin.RegisterSerializerFactories(this);
            }

            #region GeneratedSerializers	
			RegisterSerializerFactory(ctx => new Stormancer_Cluster_Application_ConnectionDataSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_ConnectivityCandidateSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Core_Infrastructure_Messages_SystemResponseSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Dto_ConnectionResultSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Dto_ConnectToSceneMsgSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Dto_EmptySerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Dto_RouteDtoSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Dto_SceneInfosDtoSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Dto_SceneInfosRequestDtoSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_EndpointCandidateSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_EndpointCandidateTypeSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_OpenRelayParametersSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_OpenTunnelResultSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_P2PConnectToSceneMessageSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_P2PSessionSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_AuthParametersSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_ComparisonOperatorSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_EndGameDtoSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_FieldFilterSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_GameFinderRequestSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_GameFinderResponseDTOSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_GameResultSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_GameSessionResultSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_LeaderboardOrderingSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_LeaderboardQuerySerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_LeaderboardRankingSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_LeaderboardResultSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_LoginResultSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_PartyRequestDtoSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_PartySettingsDtoSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_PartyUserDataSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_PartyUserDtoSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_PartyUserStatusSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_PlayerDTOSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_PlayerProfileSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_PlayerSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_PlayerUpdateSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_ProfileSummarySerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_ReadinessSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_ReadyVerificationRequestDtoSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_ScoreFilterSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_ScoreRecordSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_ServerStartedMessageSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_SetResultSerializer(ctx));
			RegisterSerializerFactory(ctx => new Stormancer_Plugins_TeamDTOSerializer(ctx));
			RegisterSerializerFactory(ctx => new System_Collections_Generic_KeyValuePair_2_System_String_System_Int32_Serializer(ctx));
			RegisterSerializerFactory(ctx => new System_Collections_Generic_KeyValuePair_2_System_String_System_String_Serializer(ctx));
			RegisterSerializerFactory(ctx => new System_Collections_Generic_KeyValuePair_2_System_String_System_UInt16_Serializer(ctx));
            #endregion GeneratedSerializers
        }

        protected virtual SerializationContext CreateSerializationContext()
        {
            var context = new SerializationContext();

            context.ResolveSerializer += (sender, args) =>
            {
                Func<SerializationContext, MessagePackSerializer> factory;
                if(_serializersFactory.TryGetValue(args.TargetType.TypeHandle, out factory))
                {
                    args.SetSerializer(args.TargetType, factory(args.Context));
                    return;
                }

                _config.Logger.Log(Diagnostics.LogLevel.Warn, "MsgPackSerializer", "Requesting a serializer for unregistered type " + args.TargetType.FullName + ". A serializer will be generated by relection. This will not work on AOT platforms");
            };    

            foreach (var plugin in _plugins)
            {
                plugin.OnCreatingSerializationContext(context);
            }
            return context;
        }

        public virtual string Name
        {
            get { return NAME; }
        }


    }

    public class MsgPackLambdaTypeSerializer<T> 
        : MessagePackSerializer<T>
        {
        private readonly Action<MsgPack.Packer, T> _pack;
        private readonly Func<MsgPack.Unpacker, T> _unpack;
        public MsgPackLambdaTypeSerializer(Action<MsgPack.Packer, T> pack, Func<MsgPack.Unpacker, T> unpack, SerializationContext ctx)
#if UNITY_IOS && false
            :base(typeof(T), ctx.CompatibilityOptions.PackerCompatibilityOptions)
#else
            : base(ctx, ctx.CompatibilityOptions.PackerCompatibilityOptions)
#endif
            {
            _pack = pack;
            _unpack = unpack;
        }
#if UNITY_IOS && false
        protected internal override void PackToCore(Packer packer, object objectTree)
#else
        protected internal override void PackToCore(MsgPack.Packer packer, T objectTree)
#endif
        {
            _pack(packer, (T)objectTree);
        }

#if UNITY_IOS && false
        protected internal override object UnpackFromCore(MsgPack.Unpacker unpacker)
#else
        protected internal override T UnpackFromCore(MsgPack.Unpacker unpacker)
#endif
        {
            return _unpack(unpacker);
        }
    }

    public interface IMsgPackSerializationPlugin
    {
        void OnCreatingSerializationContext(SerializationContext ctx);
        void RegisterSerializerFactories(MsgPackSerializer msgPackSerializer);
    }


}
