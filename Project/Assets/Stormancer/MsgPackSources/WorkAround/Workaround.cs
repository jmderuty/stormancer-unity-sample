using MsgPack.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Stormancer
{
    public static class Workaround
    {
        public static void AotHelper()
        {
            var serializingMembers = default(IEnumerable<SerializingMember>);

            List<SerializingMember> list = new List<SerializingMember>();
            list.Add(new SerializingMember());
            list.ToLookup<SerializingMember, KeyValuePair<string, Type>>( k => new KeyValuePair<string, Type>( k.MemberName,k.GetType())  );

            var collectionType = default(IEnumerable<MsgPack.CollectionType>);

            List<MsgPack.CollectionType> listcollectionType = new List<MsgPack.CollectionType>();
            listcollectionType.Add(MsgPack.CollectionType.None);
            MsgPack.CollectionType[] arraycollectionType = new MsgPack.CollectionType[1];
            arraycollectionType[0] = MsgPack.CollectionType.Array;

            var collectionTypes = new[] { MsgPack.CollectionType.Array, MsgPack.CollectionType.Map, MsgPack.CollectionType.None };
            var s = "";
            foreach (var c in collectionTypes)
            {
                s += c;
            }

            IDisposable idisposableValue = null;
            var exchangeEventJIT = System.Threading.Interlocked.Exchange(ref idisposableValue, null);

            var readValueResult = default(IEnumerable<MsgPack.ReadValueResult>);

  


        }
    }
}
