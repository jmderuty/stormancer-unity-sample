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
    public class Stormancer_Plugins_PlayerUpdateSerializer : MsgPack.Serialization.MessagePackSerializer<Stormancer.Plugins.PlayerUpdate> {
        
        private MsgPack.Serialization.MessagePackSerializer<string> _serializer0;
        
        private MsgPack.Serialization.MessagePackSerializer<int> _serializer1;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Packer, Stormancer.Plugins.PlayerUpdate>> _packOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.PlayerUpdate>> _packOperationTable;
        
        private System.Collections.Generic.IDictionary<string, System.Func<Stormancer.Plugins.PlayerUpdate, bool>> _nullCheckersTable;
        
        private System.Action<Stormancer.Plugins.PlayerUpdate, string> this_SetUnpackedValueOfUserIdDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, string> MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
        
        private System.Action<Stormancer.Plugins.PlayerUpdate, int> this_SetUnpackedValueOfStatusDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, int> MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate;
        
        private System.Action<Stormancer.Plugins.PlayerUpdate, string> this_SetUnpackedValueOfDataDelegate;
        
        private System.Collections.Generic.IList<string> _memberNames;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Unpacker, Stormancer.Plugins.PlayerUpdate, int, int>> _unpackOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.PlayerUpdate, int, int>> _unpackOperationTable;
        
        public Stormancer_Plugins_PlayerUpdateSerializer(MsgPack.Serialization.SerializationContext context) : 
                base(context, (MsgPack.Serialization.SerializerCapabilities.PackTo | MsgPack.Serialization.SerializerCapabilities.UnpackFrom)) {
            MsgPack.Serialization.PolymorphismSchema schema0 = default(MsgPack.Serialization.PolymorphismSchema);
            schema0 = null;
            this._serializer0 = context.GetSerializer<string>(schema0);
            MsgPack.Serialization.PolymorphismSchema schema1 = default(MsgPack.Serialization.PolymorphismSchema);
            schema1 = null;
            this._serializer1 = context.GetSerializer<int>(schema1);
            System.Action<MsgPack.Packer, Stormancer.Plugins.PlayerUpdate>[] packOperationList = default(System.Action<MsgPack.Packer, Stormancer.Plugins.PlayerUpdate>[]);
            packOperationList = new System.Action<MsgPack.Packer, Stormancer.Plugins.PlayerUpdate>[3];
            packOperationList[0] = new System.Action<MsgPack.Packer, Stormancer.Plugins.PlayerUpdate>(this.PackValueOfUserId);
            packOperationList[1] = new System.Action<MsgPack.Packer, Stormancer.Plugins.PlayerUpdate>(this.PackValueOfStatus);
            packOperationList[2] = new System.Action<MsgPack.Packer, Stormancer.Plugins.PlayerUpdate>(this.PackValueOfData);
            this._packOperationList = packOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.PlayerUpdate>> packOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.PlayerUpdate>>);
            packOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.PlayerUpdate>>(3);
            packOperationTable["UserId"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.PlayerUpdate>(this.PackValueOfUserId);
            packOperationTable["Status"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.PlayerUpdate>(this.PackValueOfStatus);
            packOperationTable["Data"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.PlayerUpdate>(this.PackValueOfData);
            this._packOperationTable = packOperationTable;
            System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.PlayerUpdate, bool>> nullCheckerTable = default(System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.PlayerUpdate, bool>>);
            nullCheckerTable = new System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.PlayerUpdate, bool>>(2);
            nullCheckerTable["UserId"] = new System.Func<Stormancer.Plugins.PlayerUpdate, bool>(this.IsUserIdNull);
            nullCheckerTable["Data"] = new System.Func<Stormancer.Plugins.PlayerUpdate, bool>(this.IsDataNull);
            this._nullCheckersTable = nullCheckerTable;
            System.Action<MsgPack.Unpacker, Stormancer.Plugins.PlayerUpdate, int, int>[] unpackOperationList = default(System.Action<MsgPack.Unpacker, Stormancer.Plugins.PlayerUpdate, int, int>[]);
            unpackOperationList = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.PlayerUpdate, int, int>[3];
            unpackOperationList[0] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.PlayerUpdate, int, int>(this.UnpackValueOfUserId);
            unpackOperationList[1] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.PlayerUpdate, int, int>(this.UnpackValueOfStatus);
            unpackOperationList[2] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.PlayerUpdate, int, int>(this.UnpackValueOfData);
            this._unpackOperationList = unpackOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.PlayerUpdate, int, int>> unpackOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.PlayerUpdate, int, int>>);
            unpackOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.PlayerUpdate, int, int>>(3);
            unpackOperationTable["UserId"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.PlayerUpdate, int, int>(this.UnpackValueOfUserId);
            unpackOperationTable["Status"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.PlayerUpdate, int, int>(this.UnpackValueOfStatus);
            unpackOperationTable["Data"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.PlayerUpdate, int, int>(this.UnpackValueOfData);
            this._unpackOperationTable = unpackOperationTable;
            this._memberNames = new string[] {
                    "UserId",
                    "Status",
                    "Data"};
            this.this_SetUnpackedValueOfUserIdDelegate = new System.Action<Stormancer.Plugins.PlayerUpdate, string>(this.SetUnpackedValueOfUserId);
            this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, string>(MsgPack.Serialization.UnpackHelpers.UnpackStringValue);
            this.this_SetUnpackedValueOfStatusDelegate = new System.Action<Stormancer.Plugins.PlayerUpdate, int>(this.SetUnpackedValueOfStatus);
            this.MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, int>(MsgPack.Serialization.UnpackHelpers.UnpackInt32Value);
            this.this_SetUnpackedValueOfDataDelegate = new System.Action<Stormancer.Plugins.PlayerUpdate, string>(this.SetUnpackedValueOfData);
        }
        
        private void PackValueOfUserId(MsgPack.Packer packer, Stormancer.Plugins.PlayerUpdate objectTree) {
            this._serializer0.PackTo(packer, objectTree.UserId);
        }
        
        private bool IsUserIdNull(Stormancer.Plugins.PlayerUpdate objectTree) {
            return (objectTree.UserId == null);
        }
        
        private void PackValueOfStatus(MsgPack.Packer packer, Stormancer.Plugins.PlayerUpdate objectTree) {
            this._serializer1.PackTo(packer, objectTree.Status);
        }
        
        private void PackValueOfData(MsgPack.Packer packer, Stormancer.Plugins.PlayerUpdate objectTree) {
            this._serializer0.PackTo(packer, objectTree.Data);
        }
        
        private bool IsDataNull(Stormancer.Plugins.PlayerUpdate objectTree) {
            return (objectTree.Data == null);
        }
        
        protected internal override void PackToCore(MsgPack.Packer packer, Stormancer.Plugins.PlayerUpdate objectTree) {
            MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.PlayerUpdate> packHelperParameters = default(MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.PlayerUpdate>);
            packHelperParameters.Packer = packer;
            packHelperParameters.Target = objectTree;
            packHelperParameters.Operations = this._packOperationList;
            MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.PlayerUpdate> packHelperParameters0 = default(MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.PlayerUpdate>);
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
        
        private void SetUnpackedValueOfUserId(Stormancer.Plugins.PlayerUpdate unpackingContext, string unpackedValue) {
            unpackingContext.UserId = unpackedValue;
        }
        
        private void UnpackValueOfUserId(MsgPack.Unpacker unpacker, Stormancer.Plugins.PlayerUpdate unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.PlayerUpdate, string> unpackHelperParameters = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.PlayerUpdate, string>);
            unpackHelperParameters.Unpacker = unpacker;
            unpackHelperParameters.UnpackingContext = unpackingContext;
            unpackHelperParameters.Serializer = this._serializer0;
            unpackHelperParameters.ItemsCount = itemsCount;
            unpackHelperParameters.Unpacked = indexOfItem;
            unpackHelperParameters.TargetObjectType = typeof(string);
            unpackHelperParameters.MemberName = "UserId";
            unpackHelperParameters.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
            unpackHelperParameters.Setter = this.this_SetUnpackedValueOfUserIdDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters);
        }
        
        private void SetUnpackedValueOfStatus(Stormancer.Plugins.PlayerUpdate unpackingContext, int unpackedValue) {
            unpackingContext.Status = unpackedValue;
        }
        
        private void UnpackValueOfStatus(MsgPack.Unpacker unpacker, Stormancer.Plugins.PlayerUpdate unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Plugins.PlayerUpdate, int> unpackHelperParameters0 = default(MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Plugins.PlayerUpdate, int>);
            unpackHelperParameters0.Unpacker = unpacker;
            unpackHelperParameters0.UnpackingContext = unpackingContext;
            unpackHelperParameters0.Serializer = this._serializer1;
            unpackHelperParameters0.ItemsCount = itemsCount;
            unpackHelperParameters0.Unpacked = indexOfItem;
            unpackHelperParameters0.TargetObjectType = typeof(int);
            unpackHelperParameters0.MemberName = "Status";
            unpackHelperParameters0.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate;
            unpackHelperParameters0.Setter = this.this_SetUnpackedValueOfStatusDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackValueTypeValue(ref unpackHelperParameters0);
        }
        
        private void SetUnpackedValueOfData(Stormancer.Plugins.PlayerUpdate unpackingContext, string unpackedValue) {
            unpackingContext.Data = unpackedValue;
        }
        
        private void UnpackValueOfData(MsgPack.Unpacker unpacker, Stormancer.Plugins.PlayerUpdate unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.PlayerUpdate, string> unpackHelperParameters1 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.PlayerUpdate, string>);
            unpackHelperParameters1.Unpacker = unpacker;
            unpackHelperParameters1.UnpackingContext = unpackingContext;
            unpackHelperParameters1.Serializer = this._serializer0;
            unpackHelperParameters1.ItemsCount = itemsCount;
            unpackHelperParameters1.Unpacked = indexOfItem;
            unpackHelperParameters1.TargetObjectType = typeof(string);
            unpackHelperParameters1.MemberName = "Data";
            unpackHelperParameters1.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters1.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
            unpackHelperParameters1.Setter = this.this_SetUnpackedValueOfDataDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters1);
        }
        
        protected internal override Stormancer.Plugins.PlayerUpdate UnpackFromCore(MsgPack.Unpacker unpacker) {
            Stormancer.Plugins.PlayerUpdate result = default(Stormancer.Plugins.PlayerUpdate);
            result = new Stormancer.Plugins.PlayerUpdate();
            if (unpacker.IsArrayHeader) {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromArray(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.PlayerUpdate>(), this._memberNames, this._unpackOperationList);
            }
            else {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromMap(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.PlayerUpdate>(), this._unpackOperationTable);
            }
        }
    }
}

