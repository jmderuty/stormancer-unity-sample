﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GeneratedSerializers {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("MsgPack.Serialization.CodeDomSerializers.CodeDomSerializerBuilder", "1.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public class Stormancer_ConnectivityCandidateSerializer : MsgPack.Serialization.MessagePackSerializer<Stormancer.ConnectivityCandidate> {
        
        private MsgPack.Serialization.MessagePackSerializer<ulong> _serializer0;
        
        private MsgPack.Serialization.MessagePackSerializer<Stormancer.EndpointCandidate> _serializer1;
        
        private MsgPack.Serialization.MessagePackSerializer<byte[]> _serializer2;
        
        private MsgPack.Serialization.MessagePackSerializer<int> _serializer3;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>> _packOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>> _packOperationTable;
        
        private System.Collections.Generic.IDictionary<string, System.Func<Stormancer.ConnectivityCandidate, bool>> _nullCheckersTable;
        
        private System.Action<Stormancer.ConnectivityCandidate, ulong> this_SetUnpackedValueOfListeningPeerDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, ulong> MsgPack_Serialization_UnpackHelpers_UnpackUInt64ValueDelegate;
        
        private System.Action<Stormancer.ConnectivityCandidate, ulong> this_SetUnpackedValueOfClientPeerDelegate;
        
        private System.Action<Stormancer.ConnectivityCandidate, Stormancer.EndpointCandidate> this_SetUnpackedValueOfListeningEndpointCandidateDelegate;
        
        private System.Action<Stormancer.ConnectivityCandidate, Stormancer.EndpointCandidate> this_SetUnpackedValueOfClientEndpointCandidateDelegate;
        
        private System.Action<Stormancer.ConnectivityCandidate, byte[]> this_SetUnpackedValueOfSessionIdDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, byte[]> MsgPack_Serialization_UnpackHelpers_UnpackBinaryValueDelegate;
        
        private System.Action<Stormancer.ConnectivityCandidate, byte[]> this_SetUnpackedValueOfIdDelegate;
        
        private System.Action<Stormancer.ConnectivityCandidate, int> this_SetUnpackedValueOfPingDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, int> MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate;
        
        private System.Collections.Generic.IList<string> _memberNames;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>> _unpackOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>> _unpackOperationTable;
        
        public Stormancer_ConnectivityCandidateSerializer(MsgPack.Serialization.SerializationContext context) : 
                base(context, (MsgPack.Serialization.SerializerCapabilities.PackTo | MsgPack.Serialization.SerializerCapabilities.UnpackFrom)) {
            MsgPack.Serialization.PolymorphismSchema schema0 = default(MsgPack.Serialization.PolymorphismSchema);
            schema0 = null;
            this._serializer0 = context.GetSerializer<ulong>(schema0);
            MsgPack.Serialization.PolymorphismSchema schema1 = default(MsgPack.Serialization.PolymorphismSchema);
            schema1 = null;
            this._serializer1 = context.GetSerializer<Stormancer.EndpointCandidate>(schema1);
            MsgPack.Serialization.PolymorphismSchema schema2 = default(MsgPack.Serialization.PolymorphismSchema);
            schema2 = null;
            this._serializer2 = context.GetSerializer<byte[]>(schema2);
            MsgPack.Serialization.PolymorphismSchema schema3 = default(MsgPack.Serialization.PolymorphismSchema);
            schema3 = null;
            this._serializer3 = context.GetSerializer<int>(schema3);
            System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>[] packOperationList = default(System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>[]);
            packOperationList = new System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>[7];
            packOperationList[0] = new System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>(this.PackValueOfListeningPeer);
            packOperationList[1] = new System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>(this.PackValueOfClientPeer);
            packOperationList[2] = new System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>(this.PackValueOfListeningEndpointCandidate);
            packOperationList[3] = new System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>(this.PackValueOfClientEndpointCandidate);
            packOperationList[4] = new System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>(this.PackValueOfSessionId);
            packOperationList[5] = new System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>(this.PackValueOfId);
            packOperationList[6] = new System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>(this.PackValueOfPing);
            this._packOperationList = packOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>> packOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>>);
            packOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>>(7);
            packOperationTable["ListeningPeer"] = new System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>(this.PackValueOfListeningPeer);
            packOperationTable["ClientPeer"] = new System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>(this.PackValueOfClientPeer);
            packOperationTable["ListeningEndpointCandidate"] = new System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>(this.PackValueOfListeningEndpointCandidate);
            packOperationTable["ClientEndpointCandidate"] = new System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>(this.PackValueOfClientEndpointCandidate);
            packOperationTable["SessionId"] = new System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>(this.PackValueOfSessionId);
            packOperationTable["Id"] = new System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>(this.PackValueOfId);
            packOperationTable["Ping"] = new System.Action<MsgPack.Packer, Stormancer.ConnectivityCandidate>(this.PackValueOfPing);
            this._packOperationTable = packOperationTable;
            System.Collections.Generic.Dictionary<string, System.Func<Stormancer.ConnectivityCandidate, bool>> nullCheckerTable = default(System.Collections.Generic.Dictionary<string, System.Func<Stormancer.ConnectivityCandidate, bool>>);
            nullCheckerTable = new System.Collections.Generic.Dictionary<string, System.Func<Stormancer.ConnectivityCandidate, bool>>(4);
            nullCheckerTable["ListeningEndpointCandidate"] = new System.Func<Stormancer.ConnectivityCandidate, bool>(this.IsListeningEndpointCandidateNull);
            nullCheckerTable["ClientEndpointCandidate"] = new System.Func<Stormancer.ConnectivityCandidate, bool>(this.IsClientEndpointCandidateNull);
            nullCheckerTable["SessionId"] = new System.Func<Stormancer.ConnectivityCandidate, bool>(this.IsSessionIdNull);
            nullCheckerTable["Id"] = new System.Func<Stormancer.ConnectivityCandidate, bool>(this.IsIdNull);
            this._nullCheckersTable = nullCheckerTable;
            System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>[] unpackOperationList = default(System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>[]);
            unpackOperationList = new System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>[7];
            unpackOperationList[0] = new System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>(this.UnpackValueOfListeningPeer);
            unpackOperationList[1] = new System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>(this.UnpackValueOfClientPeer);
            unpackOperationList[2] = new System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>(this.UnpackValueOfListeningEndpointCandidate);
            unpackOperationList[3] = new System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>(this.UnpackValueOfClientEndpointCandidate);
            unpackOperationList[4] = new System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>(this.UnpackValueOfSessionId);
            unpackOperationList[5] = new System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>(this.UnpackValueOfId);
            unpackOperationList[6] = new System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>(this.UnpackValueOfPing);
            this._unpackOperationList = unpackOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>> unpackOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>>);
            unpackOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>>(7);
            unpackOperationTable["ListeningPeer"] = new System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>(this.UnpackValueOfListeningPeer);
            unpackOperationTable["ClientPeer"] = new System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>(this.UnpackValueOfClientPeer);
            unpackOperationTable["ListeningEndpointCandidate"] = new System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>(this.UnpackValueOfListeningEndpointCandidate);
            unpackOperationTable["ClientEndpointCandidate"] = new System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>(this.UnpackValueOfClientEndpointCandidate);
            unpackOperationTable["SessionId"] = new System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>(this.UnpackValueOfSessionId);
            unpackOperationTable["Id"] = new System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>(this.UnpackValueOfId);
            unpackOperationTable["Ping"] = new System.Action<MsgPack.Unpacker, Stormancer.ConnectivityCandidate, int, int>(this.UnpackValueOfPing);
            this._unpackOperationTable = unpackOperationTable;
            this._memberNames = new string[] {
                    "ListeningPeer",
                    "ClientPeer",
                    "ListeningEndpointCandidate",
                    "ClientEndpointCandidate",
                    "SessionId",
                    "Id",
                    "Ping"};
            this.this_SetUnpackedValueOfListeningPeerDelegate = new System.Action<Stormancer.ConnectivityCandidate, ulong>(this.SetUnpackedValueOfListeningPeer);
            this.MsgPack_Serialization_UnpackHelpers_UnpackUInt64ValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, ulong>(MsgPack.Serialization.UnpackHelpers.UnpackUInt64Value);
            this.this_SetUnpackedValueOfClientPeerDelegate = new System.Action<Stormancer.ConnectivityCandidate, ulong>(this.SetUnpackedValueOfClientPeer);
            this.this_SetUnpackedValueOfListeningEndpointCandidateDelegate = new System.Action<Stormancer.ConnectivityCandidate, Stormancer.EndpointCandidate>(this.SetUnpackedValueOfListeningEndpointCandidate);
            this.this_SetUnpackedValueOfClientEndpointCandidateDelegate = new System.Action<Stormancer.ConnectivityCandidate, Stormancer.EndpointCandidate>(this.SetUnpackedValueOfClientEndpointCandidate);
            this.this_SetUnpackedValueOfSessionIdDelegate = new System.Action<Stormancer.ConnectivityCandidate, byte[]>(this.SetUnpackedValueOfSessionId);
            this.MsgPack_Serialization_UnpackHelpers_UnpackBinaryValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, byte[]>(MsgPack.Serialization.UnpackHelpers.UnpackBinaryValue);
            this.this_SetUnpackedValueOfIdDelegate = new System.Action<Stormancer.ConnectivityCandidate, byte[]>(this.SetUnpackedValueOfId);
            this.this_SetUnpackedValueOfPingDelegate = new System.Action<Stormancer.ConnectivityCandidate, int>(this.SetUnpackedValueOfPing);
            this.MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, int>(MsgPack.Serialization.UnpackHelpers.UnpackInt32Value);
        }
        
        private void PackValueOfListeningPeer(MsgPack.Packer packer, Stormancer.ConnectivityCandidate objectTree) {
            this._serializer0.PackTo(packer, objectTree.ListeningPeer);
        }
        
        private void PackValueOfClientPeer(MsgPack.Packer packer, Stormancer.ConnectivityCandidate objectTree) {
            this._serializer0.PackTo(packer, objectTree.ClientPeer);
        }
        
        private void PackValueOfListeningEndpointCandidate(MsgPack.Packer packer, Stormancer.ConnectivityCandidate objectTree) {
            this._serializer1.PackTo(packer, objectTree.ListeningEndpointCandidate);
        }
        
        private bool IsListeningEndpointCandidateNull(Stormancer.ConnectivityCandidate objectTree) {
            return (objectTree.ListeningEndpointCandidate == null);
        }
        
        private void PackValueOfClientEndpointCandidate(MsgPack.Packer packer, Stormancer.ConnectivityCandidate objectTree) {
            this._serializer1.PackTo(packer, objectTree.ClientEndpointCandidate);
        }
        
        private bool IsClientEndpointCandidateNull(Stormancer.ConnectivityCandidate objectTree) {
            return (objectTree.ClientEndpointCandidate == null);
        }
        
        private void PackValueOfSessionId(MsgPack.Packer packer, Stormancer.ConnectivityCandidate objectTree) {
            this._serializer2.PackTo(packer, objectTree.SessionId);
        }
        
        private bool IsSessionIdNull(Stormancer.ConnectivityCandidate objectTree) {
            return (objectTree.SessionId == null);
        }
        
        private void PackValueOfId(MsgPack.Packer packer, Stormancer.ConnectivityCandidate objectTree) {
            this._serializer2.PackTo(packer, objectTree.Id);
        }
        
        private bool IsIdNull(Stormancer.ConnectivityCandidate objectTree) {
            return (objectTree.Id == null);
        }
        
        private void PackValueOfPing(MsgPack.Packer packer, Stormancer.ConnectivityCandidate objectTree) {
            this._serializer3.PackTo(packer, objectTree.Ping);
        }
        
        protected internal override void PackToCore(MsgPack.Packer packer, Stormancer.ConnectivityCandidate objectTree) {
            MsgPack.Serialization.PackToArrayParameters<Stormancer.ConnectivityCandidate> packHelperParameters = default(MsgPack.Serialization.PackToArrayParameters<Stormancer.ConnectivityCandidate>);
            packHelperParameters.Packer = packer;
            packHelperParameters.Target = objectTree;
            packHelperParameters.Operations = this._packOperationList;
            MsgPack.Serialization.PackToMapParameters<Stormancer.ConnectivityCandidate> packHelperParameters0 = default(MsgPack.Serialization.PackToMapParameters<Stormancer.ConnectivityCandidate>);
            packHelperParameters0.Packer = packer;
            packHelperParameters0.Target = objectTree;
            packHelperParameters0.Operations = this._packOperationTable;
            packHelperParameters0.SerializationContext = this.OwnerContext;
            packHelperParameters0.NullCheckers = this._nullCheckersTable;
            if ((this.OwnerContext.SerializationMethod == MsgPack.Serialization.SerializationMethod.Array)) {
                MsgPack.Serialization.PackHelpers.PackToArray(ref packHelperParameters);
            }
            else {
                MsgPack.Serialization.PackHelpers.PackToMap(ref packHelperParameters0);
            }
        }
        
        private void SetUnpackedValueOfListeningPeer(Stormancer.ConnectivityCandidate unpackingContext, ulong unpackedValue) {
            unpackingContext.ListeningPeer = unpackedValue;
        }
        
        private void UnpackValueOfListeningPeer(MsgPack.Unpacker unpacker, Stormancer.ConnectivityCandidate unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.ConnectivityCandidate, ulong> unpackHelperParameters = default(MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.ConnectivityCandidate, ulong>);
            unpackHelperParameters.Unpacker = unpacker;
            unpackHelperParameters.UnpackingContext = unpackingContext;
            unpackHelperParameters.Serializer = this._serializer0;
            unpackHelperParameters.ItemsCount = itemsCount;
            unpackHelperParameters.Unpacked = indexOfItem;
            unpackHelperParameters.TargetObjectType = typeof(ulong);
            unpackHelperParameters.MemberName = "ListeningPeer";
            unpackHelperParameters.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackUInt64ValueDelegate;
            unpackHelperParameters.Setter = this.this_SetUnpackedValueOfListeningPeerDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackValueTypeValue(ref unpackHelperParameters);
        }
        
        private void SetUnpackedValueOfClientPeer(Stormancer.ConnectivityCandidate unpackingContext, ulong unpackedValue) {
            unpackingContext.ClientPeer = unpackedValue;
        }
        
        private void UnpackValueOfClientPeer(MsgPack.Unpacker unpacker, Stormancer.ConnectivityCandidate unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.ConnectivityCandidate, ulong> unpackHelperParameters0 = default(MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.ConnectivityCandidate, ulong>);
            unpackHelperParameters0.Unpacker = unpacker;
            unpackHelperParameters0.UnpackingContext = unpackingContext;
            unpackHelperParameters0.Serializer = this._serializer0;
            unpackHelperParameters0.ItemsCount = itemsCount;
            unpackHelperParameters0.Unpacked = indexOfItem;
            unpackHelperParameters0.TargetObjectType = typeof(ulong);
            unpackHelperParameters0.MemberName = "ClientPeer";
            unpackHelperParameters0.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackUInt64ValueDelegate;
            unpackHelperParameters0.Setter = this.this_SetUnpackedValueOfClientPeerDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackValueTypeValue(ref unpackHelperParameters0);
        }
        
        private void SetUnpackedValueOfListeningEndpointCandidate(Stormancer.ConnectivityCandidate unpackingContext, Stormancer.EndpointCandidate unpackedValue) {
            unpackingContext.ListeningEndpointCandidate = unpackedValue;
        }
        
        private void UnpackValueOfListeningEndpointCandidate(MsgPack.Unpacker unpacker, Stormancer.ConnectivityCandidate unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.ConnectivityCandidate, Stormancer.EndpointCandidate> unpackHelperParameters1 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.ConnectivityCandidate, Stormancer.EndpointCandidate>);
            unpackHelperParameters1.Unpacker = unpacker;
            unpackHelperParameters1.UnpackingContext = unpackingContext;
            unpackHelperParameters1.Serializer = this._serializer1;
            unpackHelperParameters1.ItemsCount = itemsCount;
            unpackHelperParameters1.Unpacked = indexOfItem;
            unpackHelperParameters1.TargetObjectType = typeof(Stormancer.EndpointCandidate);
            unpackHelperParameters1.MemberName = "ListeningEndpointCandidate";
            unpackHelperParameters1.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters1.DirectRead = null;
            unpackHelperParameters1.Setter = this.this_SetUnpackedValueOfListeningEndpointCandidateDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters1);
        }
        
        private void SetUnpackedValueOfClientEndpointCandidate(Stormancer.ConnectivityCandidate unpackingContext, Stormancer.EndpointCandidate unpackedValue) {
            unpackingContext.ClientEndpointCandidate = unpackedValue;
        }
        
        private void UnpackValueOfClientEndpointCandidate(MsgPack.Unpacker unpacker, Stormancer.ConnectivityCandidate unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.ConnectivityCandidate, Stormancer.EndpointCandidate> unpackHelperParameters2 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.ConnectivityCandidate, Stormancer.EndpointCandidate>);
            unpackHelperParameters2.Unpacker = unpacker;
            unpackHelperParameters2.UnpackingContext = unpackingContext;
            unpackHelperParameters2.Serializer = this._serializer1;
            unpackHelperParameters2.ItemsCount = itemsCount;
            unpackHelperParameters2.Unpacked = indexOfItem;
            unpackHelperParameters2.TargetObjectType = typeof(Stormancer.EndpointCandidate);
            unpackHelperParameters2.MemberName = "ClientEndpointCandidate";
            unpackHelperParameters2.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters2.DirectRead = null;
            unpackHelperParameters2.Setter = this.this_SetUnpackedValueOfClientEndpointCandidateDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters2);
        }
        
        private void SetUnpackedValueOfSessionId(Stormancer.ConnectivityCandidate unpackingContext, byte[] unpackedValue) {
            unpackingContext.SessionId = unpackedValue;
        }
        
        private void UnpackValueOfSessionId(MsgPack.Unpacker unpacker, Stormancer.ConnectivityCandidate unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.ConnectivityCandidate, byte[]> unpackHelperParameters3 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.ConnectivityCandidate, byte[]>);
            unpackHelperParameters3.Unpacker = unpacker;
            unpackHelperParameters3.UnpackingContext = unpackingContext;
            unpackHelperParameters3.Serializer = this._serializer2;
            unpackHelperParameters3.ItemsCount = itemsCount;
            unpackHelperParameters3.Unpacked = indexOfItem;
            unpackHelperParameters3.TargetObjectType = typeof(byte[]);
            unpackHelperParameters3.MemberName = "SessionId";
            unpackHelperParameters3.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters3.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackBinaryValueDelegate;
            unpackHelperParameters3.Setter = this.this_SetUnpackedValueOfSessionIdDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters3);
        }
        
        private void SetUnpackedValueOfId(Stormancer.ConnectivityCandidate unpackingContext, byte[] unpackedValue) {
            unpackingContext.Id = unpackedValue;
        }
        
        private void UnpackValueOfId(MsgPack.Unpacker unpacker, Stormancer.ConnectivityCandidate unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.ConnectivityCandidate, byte[]> unpackHelperParameters4 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.ConnectivityCandidate, byte[]>);
            unpackHelperParameters4.Unpacker = unpacker;
            unpackHelperParameters4.UnpackingContext = unpackingContext;
            unpackHelperParameters4.Serializer = this._serializer2;
            unpackHelperParameters4.ItemsCount = itemsCount;
            unpackHelperParameters4.Unpacked = indexOfItem;
            unpackHelperParameters4.TargetObjectType = typeof(byte[]);
            unpackHelperParameters4.MemberName = "Id";
            unpackHelperParameters4.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters4.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackBinaryValueDelegate;
            unpackHelperParameters4.Setter = this.this_SetUnpackedValueOfIdDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters4);
        }
        
        private void SetUnpackedValueOfPing(Stormancer.ConnectivityCandidate unpackingContext, int unpackedValue) {
            unpackingContext.Ping = unpackedValue;
        }
        
        private void UnpackValueOfPing(MsgPack.Unpacker unpacker, Stormancer.ConnectivityCandidate unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.ConnectivityCandidate, int> unpackHelperParameters5 = default(MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.ConnectivityCandidate, int>);
            unpackHelperParameters5.Unpacker = unpacker;
            unpackHelperParameters5.UnpackingContext = unpackingContext;
            unpackHelperParameters5.Serializer = this._serializer3;
            unpackHelperParameters5.ItemsCount = itemsCount;
            unpackHelperParameters5.Unpacked = indexOfItem;
            unpackHelperParameters5.TargetObjectType = typeof(int);
            unpackHelperParameters5.MemberName = "Ping";
            unpackHelperParameters5.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate;
            unpackHelperParameters5.Setter = this.this_SetUnpackedValueOfPingDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackValueTypeValue(ref unpackHelperParameters5);
        }
        
        protected internal override Stormancer.ConnectivityCandidate UnpackFromCore(MsgPack.Unpacker unpacker) {
            Stormancer.ConnectivityCandidate result = default(Stormancer.ConnectivityCandidate);
            result = new Stormancer.ConnectivityCandidate();
            if (unpacker.IsArrayHeader) {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromArray(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.ConnectivityCandidate>(), this._memberNames, this._unpackOperationList);
            }
            else {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromMap(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.ConnectivityCandidate>(), this._unpackOperationTable);
            }
        }
    }
}
