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
    public class Stormancer_Plugins_GameFinderRequestSerializer : MsgPack.Serialization.MessagePackSerializer<Stormancer.Plugins.GameFinderRequest> {
        
        private MsgPack.Serialization.MessagePackSerializer<System.Collections.Generic.Dictionary<string, string>> _serializer0;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Packer, Stormancer.Plugins.GameFinderRequest>> _packOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.GameFinderRequest>> _packOperationTable;
        
        private System.Collections.Generic.IDictionary<string, System.Func<Stormancer.Plugins.GameFinderRequest, bool>> _nullCheckersTable;
        
        private System.Action<Stormancer.Plugins.GameFinderRequest, System.Collections.Generic.Dictionary<string, string>> this_SetUnpackedValueOfProfileIdsDelegate;
        
        private System.Collections.Generic.IList<string> _memberNames;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameFinderRequest, int, int>> _unpackOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameFinderRequest, int, int>> _unpackOperationTable;
        
        public Stormancer_Plugins_GameFinderRequestSerializer(MsgPack.Serialization.SerializationContext context) : 
                base(context, (MsgPack.Serialization.SerializerCapabilities.PackTo | MsgPack.Serialization.SerializerCapabilities.UnpackFrom)) {
            MsgPack.Serialization.PolymorphismSchema schema0 = default(MsgPack.Serialization.PolymorphismSchema);
            schema0 = null;
            this._serializer0 = context.GetSerializer<System.Collections.Generic.Dictionary<string, string>>(schema0);
            System.Action<MsgPack.Packer, Stormancer.Plugins.GameFinderRequest>[] packOperationList = default(System.Action<MsgPack.Packer, Stormancer.Plugins.GameFinderRequest>[]);
            packOperationList = new System.Action<MsgPack.Packer, Stormancer.Plugins.GameFinderRequest>[1];
            packOperationList[0] = new System.Action<MsgPack.Packer, Stormancer.Plugins.GameFinderRequest>(this.PackValueOfProfileIds);
            this._packOperationList = packOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.GameFinderRequest>> packOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.GameFinderRequest>>);
            packOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.GameFinderRequest>>(1);
            packOperationTable["ProfileIds"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.GameFinderRequest>(this.PackValueOfProfileIds);
            this._packOperationTable = packOperationTable;
            System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.GameFinderRequest, bool>> nullCheckerTable = default(System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.GameFinderRequest, bool>>);
            nullCheckerTable = new System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.GameFinderRequest, bool>>(1);
            nullCheckerTable["ProfileIds"] = new System.Func<Stormancer.Plugins.GameFinderRequest, bool>(this.IsProfileIdsNull);
            this._nullCheckersTable = nullCheckerTable;
            System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameFinderRequest, int, int>[] unpackOperationList = default(System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameFinderRequest, int, int>[]);
            unpackOperationList = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameFinderRequest, int, int>[1];
            unpackOperationList[0] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameFinderRequest, int, int>(this.UnpackValueOfProfileIds);
            this._unpackOperationList = unpackOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameFinderRequest, int, int>> unpackOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameFinderRequest, int, int>>);
            unpackOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameFinderRequest, int, int>>(1);
            unpackOperationTable["ProfileIds"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameFinderRequest, int, int>(this.UnpackValueOfProfileIds);
            this._unpackOperationTable = unpackOperationTable;
            this._memberNames = new string[] {
                    "ProfileIds"};
            this.this_SetUnpackedValueOfProfileIdsDelegate = new System.Action<Stormancer.Plugins.GameFinderRequest, System.Collections.Generic.Dictionary<string, string>>(this.SetUnpackedValueOfProfileIds);
        }
        
        private void PackValueOfProfileIds(MsgPack.Packer packer, Stormancer.Plugins.GameFinderRequest objectTree) {
            this._serializer0.PackTo(packer, objectTree.ProfileIds);
        }
        
        private bool IsProfileIdsNull(Stormancer.Plugins.GameFinderRequest objectTree) {
            return (objectTree.ProfileIds == null);
        }
        
        protected internal override void PackToCore(MsgPack.Packer packer, Stormancer.Plugins.GameFinderRequest objectTree) {
            MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.GameFinderRequest> packHelperParameters = default(MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.GameFinderRequest>);
            packHelperParameters.Packer = packer;
            packHelperParameters.Target = objectTree;
            packHelperParameters.Operations = this._packOperationList;
            MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.GameFinderRequest> packHelperParameters0 = default(MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.GameFinderRequest>);
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
        
        private void SetUnpackedValueOfProfileIds(Stormancer.Plugins.GameFinderRequest unpackingContext, System.Collections.Generic.Dictionary<string, string> unpackedValue) {
            unpackingContext.ProfileIds = unpackedValue;
        }
        
        private void UnpackValueOfProfileIds(MsgPack.Unpacker unpacker, Stormancer.Plugins.GameFinderRequest unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.GameFinderRequest, System.Collections.Generic.Dictionary<string, string>> unpackHelperParameters = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.GameFinderRequest, System.Collections.Generic.Dictionary<string, string>>);
            unpackHelperParameters.Unpacker = unpacker;
            unpackHelperParameters.UnpackingContext = unpackingContext;
            unpackHelperParameters.Serializer = this._serializer0;
            unpackHelperParameters.ItemsCount = itemsCount;
            unpackHelperParameters.Unpacked = indexOfItem;
            unpackHelperParameters.TargetObjectType = typeof(System.Collections.Generic.Dictionary<string, string>);
            unpackHelperParameters.MemberName = "ProfileIds";
            unpackHelperParameters.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters.DirectRead = null;
            unpackHelperParameters.Setter = this.this_SetUnpackedValueOfProfileIdsDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters);
        }
        
        protected internal override Stormancer.Plugins.GameFinderRequest UnpackFromCore(MsgPack.Unpacker unpacker) {
            Stormancer.Plugins.GameFinderRequest result = default(Stormancer.Plugins.GameFinderRequest);
            result = new Stormancer.Plugins.GameFinderRequest();
            if (unpacker.IsArrayHeader) {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromArray(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.GameFinderRequest>(), this._memberNames, this._unpackOperationList);
            }
            else {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromMap(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.GameFinderRequest>(), this._unpackOperationTable);
            }
        }
    }
}
