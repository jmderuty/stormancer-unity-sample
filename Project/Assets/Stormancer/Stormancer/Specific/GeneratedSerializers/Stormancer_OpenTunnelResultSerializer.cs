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
    public class Stormancer_OpenTunnelResultSerializer : MsgPack.Serialization.MessagePackSerializer<Stormancer.OpenTunnelResult> {
        
        private MsgPack.Serialization.MessagePackSerializer<bool> _serializer0;
        
        private MsgPack.Serialization.MessagePackSerializer<string> _serializer1;
        
        private MsgPack.Serialization.MessagePackSerializer<byte> _serializer2;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Packer, Stormancer.OpenTunnelResult>> _packOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Packer, Stormancer.OpenTunnelResult>> _packOperationTable;
        
        private System.Collections.Generic.IDictionary<string, System.Func<Stormancer.OpenTunnelResult, bool>> _nullCheckersTable;
        
        private System.Action<Stormancer.OpenTunnelResult, bool> this_SetUnpackedValueOfUseTunnelDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, bool> MsgPack_Serialization_UnpackHelpers_UnpackBooleanValueDelegate;
        
        private System.Action<Stormancer.OpenTunnelResult, string> this_SetUnpackedValueOfEndpointDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, string> MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
        
        private System.Action<Stormancer.OpenTunnelResult, byte> this_SetUnpackedValueOfHandleDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, byte> MsgPack_Serialization_UnpackHelpers_UnpackByteValueDelegate;
        
        private System.Collections.Generic.IList<string> _memberNames;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Unpacker, Stormancer.OpenTunnelResult, int, int>> _unpackOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Unpacker, Stormancer.OpenTunnelResult, int, int>> _unpackOperationTable;
        
        public Stormancer_OpenTunnelResultSerializer(MsgPack.Serialization.SerializationContext context) : 
                base(context, (MsgPack.Serialization.SerializerCapabilities.PackTo | MsgPack.Serialization.SerializerCapabilities.UnpackFrom)) {
            MsgPack.Serialization.PolymorphismSchema schema0 = default(MsgPack.Serialization.PolymorphismSchema);
            schema0 = null;
            this._serializer0 = context.GetSerializer<bool>(schema0);
            MsgPack.Serialization.PolymorphismSchema schema1 = default(MsgPack.Serialization.PolymorphismSchema);
            schema1 = null;
            this._serializer1 = context.GetSerializer<string>(schema1);
            MsgPack.Serialization.PolymorphismSchema schema2 = default(MsgPack.Serialization.PolymorphismSchema);
            schema2 = null;
            this._serializer2 = context.GetSerializer<byte>(schema2);
            System.Action<MsgPack.Packer, Stormancer.OpenTunnelResult>[] packOperationList = default(System.Action<MsgPack.Packer, Stormancer.OpenTunnelResult>[]);
            packOperationList = new System.Action<MsgPack.Packer, Stormancer.OpenTunnelResult>[3];
            packOperationList[0] = new System.Action<MsgPack.Packer, Stormancer.OpenTunnelResult>(this.PackValueOfUseTunnel);
            packOperationList[1] = new System.Action<MsgPack.Packer, Stormancer.OpenTunnelResult>(this.PackValueOfEndpoint);
            packOperationList[2] = new System.Action<MsgPack.Packer, Stormancer.OpenTunnelResult>(this.PackValueOfHandle);
            this._packOperationList = packOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.OpenTunnelResult>> packOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.OpenTunnelResult>>);
            packOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.OpenTunnelResult>>(3);
            packOperationTable["UseTunnel"] = new System.Action<MsgPack.Packer, Stormancer.OpenTunnelResult>(this.PackValueOfUseTunnel);
            packOperationTable["Endpoint"] = new System.Action<MsgPack.Packer, Stormancer.OpenTunnelResult>(this.PackValueOfEndpoint);
            packOperationTable["Handle"] = new System.Action<MsgPack.Packer, Stormancer.OpenTunnelResult>(this.PackValueOfHandle);
            this._packOperationTable = packOperationTable;
            System.Collections.Generic.Dictionary<string, System.Func<Stormancer.OpenTunnelResult, bool>> nullCheckerTable = default(System.Collections.Generic.Dictionary<string, System.Func<Stormancer.OpenTunnelResult, bool>>);
            nullCheckerTable = new System.Collections.Generic.Dictionary<string, System.Func<Stormancer.OpenTunnelResult, bool>>(1);
            nullCheckerTable["Endpoint"] = new System.Func<Stormancer.OpenTunnelResult, bool>(this.IsEndpointNull);
            this._nullCheckersTable = nullCheckerTable;
            System.Action<MsgPack.Unpacker, Stormancer.OpenTunnelResult, int, int>[] unpackOperationList = default(System.Action<MsgPack.Unpacker, Stormancer.OpenTunnelResult, int, int>[]);
            unpackOperationList = new System.Action<MsgPack.Unpacker, Stormancer.OpenTunnelResult, int, int>[3];
            unpackOperationList[0] = new System.Action<MsgPack.Unpacker, Stormancer.OpenTunnelResult, int, int>(this.UnpackValueOfUseTunnel);
            unpackOperationList[1] = new System.Action<MsgPack.Unpacker, Stormancer.OpenTunnelResult, int, int>(this.UnpackValueOfEndpoint);
            unpackOperationList[2] = new System.Action<MsgPack.Unpacker, Stormancer.OpenTunnelResult, int, int>(this.UnpackValueOfHandle);
            this._unpackOperationList = unpackOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.OpenTunnelResult, int, int>> unpackOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.OpenTunnelResult, int, int>>);
            unpackOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.OpenTunnelResult, int, int>>(3);
            unpackOperationTable["UseTunnel"] = new System.Action<MsgPack.Unpacker, Stormancer.OpenTunnelResult, int, int>(this.UnpackValueOfUseTunnel);
            unpackOperationTable["Endpoint"] = new System.Action<MsgPack.Unpacker, Stormancer.OpenTunnelResult, int, int>(this.UnpackValueOfEndpoint);
            unpackOperationTable["Handle"] = new System.Action<MsgPack.Unpacker, Stormancer.OpenTunnelResult, int, int>(this.UnpackValueOfHandle);
            this._unpackOperationTable = unpackOperationTable;
            this._memberNames = new string[] {
                    "UseTunnel",
                    "Endpoint",
                    "Handle"};
            this.this_SetUnpackedValueOfUseTunnelDelegate = new System.Action<Stormancer.OpenTunnelResult, bool>(this.SetUnpackedValueOfUseTunnel);
            this.MsgPack_Serialization_UnpackHelpers_UnpackBooleanValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, bool>(MsgPack.Serialization.UnpackHelpers.UnpackBooleanValue);
            this.this_SetUnpackedValueOfEndpointDelegate = new System.Action<Stormancer.OpenTunnelResult, string>(this.SetUnpackedValueOfEndpoint);
            this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, string>(MsgPack.Serialization.UnpackHelpers.UnpackStringValue);
            this.this_SetUnpackedValueOfHandleDelegate = new System.Action<Stormancer.OpenTunnelResult, byte>(this.SetUnpackedValueOfHandle);
            this.MsgPack_Serialization_UnpackHelpers_UnpackByteValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, byte>(MsgPack.Serialization.UnpackHelpers.UnpackByteValue);
        }
        
        private void PackValueOfUseTunnel(MsgPack.Packer packer, Stormancer.OpenTunnelResult objectTree) {
            this._serializer0.PackTo(packer, objectTree.UseTunnel);
        }
        
        private void PackValueOfEndpoint(MsgPack.Packer packer, Stormancer.OpenTunnelResult objectTree) {
            this._serializer1.PackTo(packer, objectTree.Endpoint);
        }
        
        private bool IsEndpointNull(Stormancer.OpenTunnelResult objectTree) {
            return (objectTree.Endpoint == null);
        }
        
        private void PackValueOfHandle(MsgPack.Packer packer, Stormancer.OpenTunnelResult objectTree) {
            this._serializer2.PackTo(packer, objectTree.Handle);
        }
        
        protected internal override void PackToCore(MsgPack.Packer packer, Stormancer.OpenTunnelResult objectTree) {
            MsgPack.Serialization.PackToArrayParameters<Stormancer.OpenTunnelResult> packHelperParameters = default(MsgPack.Serialization.PackToArrayParameters<Stormancer.OpenTunnelResult>);
            packHelperParameters.Packer = packer;
            packHelperParameters.Target = objectTree;
            packHelperParameters.Operations = this._packOperationList;
            MsgPack.Serialization.PackToMapParameters<Stormancer.OpenTunnelResult> packHelperParameters0 = default(MsgPack.Serialization.PackToMapParameters<Stormancer.OpenTunnelResult>);
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
        
        private void SetUnpackedValueOfUseTunnel(Stormancer.OpenTunnelResult unpackingContext, bool unpackedValue) {
            unpackingContext.UseTunnel = unpackedValue;
        }
        
        private void UnpackValueOfUseTunnel(MsgPack.Unpacker unpacker, Stormancer.OpenTunnelResult unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.OpenTunnelResult, bool> unpackHelperParameters = default(MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.OpenTunnelResult, bool>);
            unpackHelperParameters.Unpacker = unpacker;
            unpackHelperParameters.UnpackingContext = unpackingContext;
            unpackHelperParameters.Serializer = this._serializer0;
            unpackHelperParameters.ItemsCount = itemsCount;
            unpackHelperParameters.Unpacked = indexOfItem;
            unpackHelperParameters.TargetObjectType = typeof(bool);
            unpackHelperParameters.MemberName = "UseTunnel";
            unpackHelperParameters.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackBooleanValueDelegate;
            unpackHelperParameters.Setter = this.this_SetUnpackedValueOfUseTunnelDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackValueTypeValue(ref unpackHelperParameters);
        }
        
        private void SetUnpackedValueOfEndpoint(Stormancer.OpenTunnelResult unpackingContext, string unpackedValue) {
            unpackingContext.Endpoint = unpackedValue;
        }
        
        private void UnpackValueOfEndpoint(MsgPack.Unpacker unpacker, Stormancer.OpenTunnelResult unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.OpenTunnelResult, string> unpackHelperParameters0 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.OpenTunnelResult, string>);
            unpackHelperParameters0.Unpacker = unpacker;
            unpackHelperParameters0.UnpackingContext = unpackingContext;
            unpackHelperParameters0.Serializer = this._serializer1;
            unpackHelperParameters0.ItemsCount = itemsCount;
            unpackHelperParameters0.Unpacked = indexOfItem;
            unpackHelperParameters0.TargetObjectType = typeof(string);
            unpackHelperParameters0.MemberName = "Endpoint";
            unpackHelperParameters0.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters0.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
            unpackHelperParameters0.Setter = this.this_SetUnpackedValueOfEndpointDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters0);
        }
        
        private void SetUnpackedValueOfHandle(Stormancer.OpenTunnelResult unpackingContext, byte unpackedValue) {
            unpackingContext.Handle = unpackedValue;
        }
        
        private void UnpackValueOfHandle(MsgPack.Unpacker unpacker, Stormancer.OpenTunnelResult unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.OpenTunnelResult, byte> unpackHelperParameters1 = default(MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.OpenTunnelResult, byte>);
            unpackHelperParameters1.Unpacker = unpacker;
            unpackHelperParameters1.UnpackingContext = unpackingContext;
            unpackHelperParameters1.Serializer = this._serializer2;
            unpackHelperParameters1.ItemsCount = itemsCount;
            unpackHelperParameters1.Unpacked = indexOfItem;
            unpackHelperParameters1.TargetObjectType = typeof(byte);
            unpackHelperParameters1.MemberName = "Handle";
            unpackHelperParameters1.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackByteValueDelegate;
            unpackHelperParameters1.Setter = this.this_SetUnpackedValueOfHandleDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackValueTypeValue(ref unpackHelperParameters1);
        }
        
        protected internal override Stormancer.OpenTunnelResult UnpackFromCore(MsgPack.Unpacker unpacker) {
            Stormancer.OpenTunnelResult result = default(Stormancer.OpenTunnelResult);
            result = new Stormancer.OpenTunnelResult();
            if (unpacker.IsArrayHeader) {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromArray(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.OpenTunnelResult>(), this._memberNames, this._unpackOperationList);
            }
            else {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromMap(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.OpenTunnelResult>(), this._unpackOperationTable);
            }
        }
    }
}

