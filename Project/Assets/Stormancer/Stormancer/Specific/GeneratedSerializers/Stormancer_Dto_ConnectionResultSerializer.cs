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
    public class Stormancer_Dto_ConnectionResultSerializer : MsgPack.Serialization.MessagePackSerializer<Stormancer.Dto.ConnectionResult> {
        
        private MsgPack.Serialization.MessagePackSerializer<System.Collections.Generic.Dictionary<string, ushort>> _serializer0;
        
        private MsgPack.Serialization.MessagePackSerializer<byte> _serializer1;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Packer, Stormancer.Dto.ConnectionResult>> _packOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Packer, Stormancer.Dto.ConnectionResult>> _packOperationTable;
        
        private System.Collections.Generic.IDictionary<string, System.Func<Stormancer.Dto.ConnectionResult, bool>> _nullCheckersTable;
        
        private System.Action<Stormancer.Dto.ConnectionResult, System.Collections.Generic.Dictionary<string, ushort>> this_SetUnpackedValueOfRouteMappingsDelegate;
        
        private System.Action<Stormancer.Dto.ConnectionResult, byte> this_SetUnpackedValueOfSceneHandleDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, byte> MsgPack_Serialization_UnpackHelpers_UnpackByteValueDelegate;
        
        private System.Collections.Generic.IList<string> _memberNames;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectionResult, int, int>> _unpackOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectionResult, int, int>> _unpackOperationTable;
        
        public Stormancer_Dto_ConnectionResultSerializer(MsgPack.Serialization.SerializationContext context) : 
                base(context, (MsgPack.Serialization.SerializerCapabilities.PackTo | MsgPack.Serialization.SerializerCapabilities.UnpackFrom)) {
            MsgPack.Serialization.PolymorphismSchema schema0 = default(MsgPack.Serialization.PolymorphismSchema);
            schema0 = null;
            this._serializer0 = context.GetSerializer<System.Collections.Generic.Dictionary<string, ushort>>(schema0);
            MsgPack.Serialization.PolymorphismSchema schema1 = default(MsgPack.Serialization.PolymorphismSchema);
            schema1 = null;
            this._serializer1 = context.GetSerializer<byte>(schema1);
            System.Action<MsgPack.Packer, Stormancer.Dto.ConnectionResult>[] packOperationList = default(System.Action<MsgPack.Packer, Stormancer.Dto.ConnectionResult>[]);
            packOperationList = new System.Action<MsgPack.Packer, Stormancer.Dto.ConnectionResult>[2];
            packOperationList[0] = new System.Action<MsgPack.Packer, Stormancer.Dto.ConnectionResult>(this.PackValueOfRouteMappings);
            packOperationList[1] = new System.Action<MsgPack.Packer, Stormancer.Dto.ConnectionResult>(this.PackValueOfSceneHandle);
            this._packOperationList = packOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Dto.ConnectionResult>> packOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Dto.ConnectionResult>>);
            packOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Dto.ConnectionResult>>(2);
            packOperationTable["RouteMappings"] = new System.Action<MsgPack.Packer, Stormancer.Dto.ConnectionResult>(this.PackValueOfRouteMappings);
            packOperationTable["SceneHandle"] = new System.Action<MsgPack.Packer, Stormancer.Dto.ConnectionResult>(this.PackValueOfSceneHandle);
            this._packOperationTable = packOperationTable;
            System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Dto.ConnectionResult, bool>> nullCheckerTable = default(System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Dto.ConnectionResult, bool>>);
            nullCheckerTable = new System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Dto.ConnectionResult, bool>>(1);
            nullCheckerTable["RouteMappings"] = new System.Func<Stormancer.Dto.ConnectionResult, bool>(this.IsRouteMappingsNull);
            this._nullCheckersTable = nullCheckerTable;
            System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectionResult, int, int>[] unpackOperationList = default(System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectionResult, int, int>[]);
            unpackOperationList = new System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectionResult, int, int>[2];
            unpackOperationList[0] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectionResult, int, int>(this.UnpackValueOfRouteMappings);
            unpackOperationList[1] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectionResult, int, int>(this.UnpackValueOfSceneHandle);
            this._unpackOperationList = unpackOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectionResult, int, int>> unpackOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectionResult, int, int>>);
            unpackOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectionResult, int, int>>(2);
            unpackOperationTable["RouteMappings"] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectionResult, int, int>(this.UnpackValueOfRouteMappings);
            unpackOperationTable["SceneHandle"] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectionResult, int, int>(this.UnpackValueOfSceneHandle);
            this._unpackOperationTable = unpackOperationTable;
            this._memberNames = new string[] {
                    "RouteMappings",
                    "SceneHandle"};
            this.this_SetUnpackedValueOfRouteMappingsDelegate = new System.Action<Stormancer.Dto.ConnectionResult, System.Collections.Generic.Dictionary<string, ushort>>(this.SetUnpackedValueOfRouteMappings);
            this.this_SetUnpackedValueOfSceneHandleDelegate = new System.Action<Stormancer.Dto.ConnectionResult, byte>(this.SetUnpackedValueOfSceneHandle);
            this.MsgPack_Serialization_UnpackHelpers_UnpackByteValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, byte>(MsgPack.Serialization.UnpackHelpers.UnpackByteValue);
        }
        
        private void PackValueOfRouteMappings(MsgPack.Packer packer, Stormancer.Dto.ConnectionResult objectTree) {
            this._serializer0.PackTo(packer, objectTree.RouteMappings);
        }
        
        private bool IsRouteMappingsNull(Stormancer.Dto.ConnectionResult objectTree) {
            return (objectTree.RouteMappings == null);
        }
        
        private void PackValueOfSceneHandle(MsgPack.Packer packer, Stormancer.Dto.ConnectionResult objectTree) {
            this._serializer1.PackTo(packer, objectTree.SceneHandle);
        }
        
        protected internal override void PackToCore(MsgPack.Packer packer, Stormancer.Dto.ConnectionResult objectTree) {
            MsgPack.Serialization.PackToArrayParameters<Stormancer.Dto.ConnectionResult> packHelperParameters = default(MsgPack.Serialization.PackToArrayParameters<Stormancer.Dto.ConnectionResult>);
            packHelperParameters.Packer = packer;
            packHelperParameters.Target = objectTree;
            packHelperParameters.Operations = this._packOperationList;
            MsgPack.Serialization.PackToMapParameters<Stormancer.Dto.ConnectionResult> packHelperParameters0 = default(MsgPack.Serialization.PackToMapParameters<Stormancer.Dto.ConnectionResult>);
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
        
        private void SetUnpackedValueOfRouteMappings(Stormancer.Dto.ConnectionResult unpackingContext, System.Collections.Generic.Dictionary<string, ushort> unpackedValue) {
            unpackingContext.RouteMappings = unpackedValue;
        }
        
        private void UnpackValueOfRouteMappings(MsgPack.Unpacker unpacker, Stormancer.Dto.ConnectionResult unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.ConnectionResult, System.Collections.Generic.Dictionary<string, ushort>> unpackHelperParameters = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.ConnectionResult, System.Collections.Generic.Dictionary<string, ushort>>);
            unpackHelperParameters.Unpacker = unpacker;
            unpackHelperParameters.UnpackingContext = unpackingContext;
            unpackHelperParameters.Serializer = this._serializer0;
            unpackHelperParameters.ItemsCount = itemsCount;
            unpackHelperParameters.Unpacked = indexOfItem;
            unpackHelperParameters.TargetObjectType = typeof(System.Collections.Generic.Dictionary<string, ushort>);
            unpackHelperParameters.MemberName = "RouteMappings";
            unpackHelperParameters.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters.DirectRead = null;
            unpackHelperParameters.Setter = this.this_SetUnpackedValueOfRouteMappingsDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters);
        }
        
        private void SetUnpackedValueOfSceneHandle(Stormancer.Dto.ConnectionResult unpackingContext, byte unpackedValue) {
            unpackingContext.SceneHandle = unpackedValue;
        }
        
        private void UnpackValueOfSceneHandle(MsgPack.Unpacker unpacker, Stormancer.Dto.ConnectionResult unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Dto.ConnectionResult, byte> unpackHelperParameters0 = default(MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Dto.ConnectionResult, byte>);
            unpackHelperParameters0.Unpacker = unpacker;
            unpackHelperParameters0.UnpackingContext = unpackingContext;
            unpackHelperParameters0.Serializer = this._serializer1;
            unpackHelperParameters0.ItemsCount = itemsCount;
            unpackHelperParameters0.Unpacked = indexOfItem;
            unpackHelperParameters0.TargetObjectType = typeof(byte);
            unpackHelperParameters0.MemberName = "SceneHandle";
            unpackHelperParameters0.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackByteValueDelegate;
            unpackHelperParameters0.Setter = this.this_SetUnpackedValueOfSceneHandleDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackValueTypeValue(ref unpackHelperParameters0);
        }
        
        protected internal override Stormancer.Dto.ConnectionResult UnpackFromCore(MsgPack.Unpacker unpacker) {
            Stormancer.Dto.ConnectionResult result = default(Stormancer.Dto.ConnectionResult);
            result = new Stormancer.Dto.ConnectionResult();
            if (unpacker.IsArrayHeader) {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromArray(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Dto.ConnectionResult>(), this._memberNames, this._unpackOperationList);
            }
            else {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromMap(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Dto.ConnectionResult>(), this._unpackOperationTable);
            }
        }
    }
}

