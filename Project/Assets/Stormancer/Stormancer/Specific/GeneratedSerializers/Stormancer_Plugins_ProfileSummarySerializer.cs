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
    public class Stormancer_Plugins_ProfileSummarySerializer : MsgPack.Serialization.MessagePackSerializer<Stormancer.Plugins.ProfileSummary> {
        
        private MsgPack.Serialization.MessagePackSerializer<string> _serializer0;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Packer, Stormancer.Plugins.ProfileSummary>> _packOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.ProfileSummary>> _packOperationTable;
        
        private System.Collections.Generic.IDictionary<string, System.Func<Stormancer.Plugins.ProfileSummary, bool>> _nullCheckersTable;
        
        private System.Action<Stormancer.Plugins.ProfileSummary, string> this_SetUnpackedValueOfProfileNameDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, string> MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
        
        private System.Action<Stormancer.Plugins.ProfileSummary, string> this_SetUnpackedValueOfIdDelegate;
        
        private System.Collections.Generic.IList<string> _memberNames;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Unpacker, Stormancer.Plugins.ProfileSummary, int, int>> _unpackOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.ProfileSummary, int, int>> _unpackOperationTable;
        
        public Stormancer_Plugins_ProfileSummarySerializer(MsgPack.Serialization.SerializationContext context) : 
                base(context, (MsgPack.Serialization.SerializerCapabilities.PackTo | MsgPack.Serialization.SerializerCapabilities.UnpackFrom)) {
            MsgPack.Serialization.PolymorphismSchema schema0 = default(MsgPack.Serialization.PolymorphismSchema);
            schema0 = null;
            this._serializer0 = context.GetSerializer<string>(schema0);
            System.Action<MsgPack.Packer, Stormancer.Plugins.ProfileSummary>[] packOperationList = default(System.Action<MsgPack.Packer, Stormancer.Plugins.ProfileSummary>[]);
            packOperationList = new System.Action<MsgPack.Packer, Stormancer.Plugins.ProfileSummary>[2];
            packOperationList[0] = new System.Action<MsgPack.Packer, Stormancer.Plugins.ProfileSummary>(this.PackValueOfProfileName);
            packOperationList[1] = new System.Action<MsgPack.Packer, Stormancer.Plugins.ProfileSummary>(this.PackValueOfId);
            this._packOperationList = packOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.ProfileSummary>> packOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.ProfileSummary>>);
            packOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.ProfileSummary>>(2);
            packOperationTable["ProfileName"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.ProfileSummary>(this.PackValueOfProfileName);
            packOperationTable["Id"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.ProfileSummary>(this.PackValueOfId);
            this._packOperationTable = packOperationTable;
            System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.ProfileSummary, bool>> nullCheckerTable = default(System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.ProfileSummary, bool>>);
            nullCheckerTable = new System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.ProfileSummary, bool>>(2);
            nullCheckerTable["ProfileName"] = new System.Func<Stormancer.Plugins.ProfileSummary, bool>(this.IsProfileNameNull);
            nullCheckerTable["Id"] = new System.Func<Stormancer.Plugins.ProfileSummary, bool>(this.IsIdNull);
            this._nullCheckersTable = nullCheckerTable;
            System.Action<MsgPack.Unpacker, Stormancer.Plugins.ProfileSummary, int, int>[] unpackOperationList = default(System.Action<MsgPack.Unpacker, Stormancer.Plugins.ProfileSummary, int, int>[]);
            unpackOperationList = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.ProfileSummary, int, int>[2];
            unpackOperationList[0] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.ProfileSummary, int, int>(this.UnpackValueOfProfileName);
            unpackOperationList[1] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.ProfileSummary, int, int>(this.UnpackValueOfId);
            this._unpackOperationList = unpackOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.ProfileSummary, int, int>> unpackOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.ProfileSummary, int, int>>);
            unpackOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.ProfileSummary, int, int>>(2);
            unpackOperationTable["ProfileName"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.ProfileSummary, int, int>(this.UnpackValueOfProfileName);
            unpackOperationTable["Id"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.ProfileSummary, int, int>(this.UnpackValueOfId);
            this._unpackOperationTable = unpackOperationTable;
            this._memberNames = new string[] {
                    "ProfileName",
                    "Id"};
            this.this_SetUnpackedValueOfProfileNameDelegate = new System.Action<Stormancer.Plugins.ProfileSummary, string>(this.SetUnpackedValueOfProfileName);
            this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, string>(MsgPack.Serialization.UnpackHelpers.UnpackStringValue);
            this.this_SetUnpackedValueOfIdDelegate = new System.Action<Stormancer.Plugins.ProfileSummary, string>(this.SetUnpackedValueOfId);
        }
        
        private void PackValueOfProfileName(MsgPack.Packer packer, Stormancer.Plugins.ProfileSummary objectTree) {
            this._serializer0.PackTo(packer, objectTree.ProfileName);
        }
        
        private bool IsProfileNameNull(Stormancer.Plugins.ProfileSummary objectTree) {
            return (objectTree.ProfileName == null);
        }
        
        private void PackValueOfId(MsgPack.Packer packer, Stormancer.Plugins.ProfileSummary objectTree) {
            this._serializer0.PackTo(packer, objectTree.Id);
        }
        
        private bool IsIdNull(Stormancer.Plugins.ProfileSummary objectTree) {
            return (objectTree.Id == null);
        }
        
        protected internal override void PackToCore(MsgPack.Packer packer, Stormancer.Plugins.ProfileSummary objectTree) {
            MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.ProfileSummary> packHelperParameters = default(MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.ProfileSummary>);
            packHelperParameters.Packer = packer;
            packHelperParameters.Target = objectTree;
            packHelperParameters.Operations = this._packOperationList;
            MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.ProfileSummary> packHelperParameters0 = default(MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.ProfileSummary>);
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
        
        private void SetUnpackedValueOfProfileName(Stormancer.Plugins.ProfileSummary unpackingContext, string unpackedValue) {
            unpackingContext.ProfileName = unpackedValue;
        }
        
        private void UnpackValueOfProfileName(MsgPack.Unpacker unpacker, Stormancer.Plugins.ProfileSummary unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.ProfileSummary, string> unpackHelperParameters = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.ProfileSummary, string>);
            unpackHelperParameters.Unpacker = unpacker;
            unpackHelperParameters.UnpackingContext = unpackingContext;
            unpackHelperParameters.Serializer = this._serializer0;
            unpackHelperParameters.ItemsCount = itemsCount;
            unpackHelperParameters.Unpacked = indexOfItem;
            unpackHelperParameters.TargetObjectType = typeof(string);
            unpackHelperParameters.MemberName = "ProfileName";
            unpackHelperParameters.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
            unpackHelperParameters.Setter = this.this_SetUnpackedValueOfProfileNameDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters);
        }
        
        private void SetUnpackedValueOfId(Stormancer.Plugins.ProfileSummary unpackingContext, string unpackedValue) {
            unpackingContext.Id = unpackedValue;
        }
        
        private void UnpackValueOfId(MsgPack.Unpacker unpacker, Stormancer.Plugins.ProfileSummary unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.ProfileSummary, string> unpackHelperParameters0 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.ProfileSummary, string>);
            unpackHelperParameters0.Unpacker = unpacker;
            unpackHelperParameters0.UnpackingContext = unpackingContext;
            unpackHelperParameters0.Serializer = this._serializer0;
            unpackHelperParameters0.ItemsCount = itemsCount;
            unpackHelperParameters0.Unpacked = indexOfItem;
            unpackHelperParameters0.TargetObjectType = typeof(string);
            unpackHelperParameters0.MemberName = "Id";
            unpackHelperParameters0.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters0.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
            unpackHelperParameters0.Setter = this.this_SetUnpackedValueOfIdDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters0);
        }
        
        protected internal override Stormancer.Plugins.ProfileSummary UnpackFromCore(MsgPack.Unpacker unpacker) {
            Stormancer.Plugins.ProfileSummary result = default(Stormancer.Plugins.ProfileSummary);
            result = new Stormancer.Plugins.ProfileSummary();
            if (unpacker.IsArrayHeader) {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromArray(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.ProfileSummary>(), this._memberNames, this._unpackOperationList);
            }
            else {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromMap(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.ProfileSummary>(), this._unpackOperationTable);
            }
        }
    }
}
