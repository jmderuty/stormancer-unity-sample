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
    public class Stormancer_Plugins_ScoreFilterSerializer : MsgPack.Serialization.MessagePackSerializer<Stormancer.Plugins.ScoreFilter> {
        
        private MsgPack.Serialization.MessagePackSerializer<Stormancer.Plugins.ComparisonOperator> _serializer0;
        
        private MsgPack.Serialization.MessagePackSerializer<int> _serializer1;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Packer, Stormancer.Plugins.ScoreFilter>> _packOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.ScoreFilter>> _packOperationTable;
        
        private System.Collections.Generic.IDictionary<string, System.Func<Stormancer.Plugins.ScoreFilter, bool>> _nullCheckersTable;
        
        private System.Action<Stormancer.Plugins.ScoreFilter, Stormancer.Plugins.ComparisonOperator> this_SetUnpackedValueOfTypeDelegate;
        
        private System.Action<Stormancer.Plugins.ScoreFilter, int> this_SetUnpackedValueOfValueDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, int> MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate;
        
        private System.Collections.Generic.IList<string> _memberNames;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Unpacker, Stormancer.Plugins.ScoreFilter, int, int>> _unpackOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.ScoreFilter, int, int>> _unpackOperationTable;
        
        public Stormancer_Plugins_ScoreFilterSerializer(MsgPack.Serialization.SerializationContext context) : 
                base(context, (MsgPack.Serialization.SerializerCapabilities.PackTo | MsgPack.Serialization.SerializerCapabilities.UnpackFrom)) {
            this._serializer0 = context.GetSerializer<Stormancer.Plugins.ComparisonOperator>(MsgPack.Serialization.EnumMessagePackSerializerHelpers.DetermineEnumSerializationMethod(context, typeof(Stormancer.Plugins.ComparisonOperator), MsgPack.Serialization.EnumMemberSerializationMethod.Default));
            MsgPack.Serialization.PolymorphismSchema schema0 = default(MsgPack.Serialization.PolymorphismSchema);
            schema0 = null;
            this._serializer1 = context.GetSerializer<int>(schema0);
            System.Action<MsgPack.Packer, Stormancer.Plugins.ScoreFilter>[] packOperationList = default(System.Action<MsgPack.Packer, Stormancer.Plugins.ScoreFilter>[]);
            packOperationList = new System.Action<MsgPack.Packer, Stormancer.Plugins.ScoreFilter>[2];
            packOperationList[0] = new System.Action<MsgPack.Packer, Stormancer.Plugins.ScoreFilter>(this.PackValueOfType);
            packOperationList[1] = new System.Action<MsgPack.Packer, Stormancer.Plugins.ScoreFilter>(this.PackValueOfValue);
            this._packOperationList = packOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.ScoreFilter>> packOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.ScoreFilter>>);
            packOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.ScoreFilter>>(2);
            packOperationTable["Type"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.ScoreFilter>(this.PackValueOfType);
            packOperationTable["Value"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.ScoreFilter>(this.PackValueOfValue);
            this._packOperationTable = packOperationTable;
            System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.ScoreFilter, bool>> nullCheckerTable = default(System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.ScoreFilter, bool>>);
            nullCheckerTable = new System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.ScoreFilter, bool>>(0);
            this._nullCheckersTable = nullCheckerTable;
            System.Action<MsgPack.Unpacker, Stormancer.Plugins.ScoreFilter, int, int>[] unpackOperationList = default(System.Action<MsgPack.Unpacker, Stormancer.Plugins.ScoreFilter, int, int>[]);
            unpackOperationList = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.ScoreFilter, int, int>[2];
            unpackOperationList[0] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.ScoreFilter, int, int>(this.UnpackValueOfType);
            unpackOperationList[1] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.ScoreFilter, int, int>(this.UnpackValueOfValue);
            this._unpackOperationList = unpackOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.ScoreFilter, int, int>> unpackOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.ScoreFilter, int, int>>);
            unpackOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.ScoreFilter, int, int>>(2);
            unpackOperationTable["Type"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.ScoreFilter, int, int>(this.UnpackValueOfType);
            unpackOperationTable["Value"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.ScoreFilter, int, int>(this.UnpackValueOfValue);
            this._unpackOperationTable = unpackOperationTable;
            this._memberNames = new string[] {
                    "Type",
                    "Value"};
            this.this_SetUnpackedValueOfTypeDelegate = new System.Action<Stormancer.Plugins.ScoreFilter, Stormancer.Plugins.ComparisonOperator>(this.SetUnpackedValueOfType);
            this.this_SetUnpackedValueOfValueDelegate = new System.Action<Stormancer.Plugins.ScoreFilter, int>(this.SetUnpackedValueOfValue);
            this.MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, int>(MsgPack.Serialization.UnpackHelpers.UnpackInt32Value);
        }
        
        private void PackValueOfType(MsgPack.Packer packer, Stormancer.Plugins.ScoreFilter objectTree) {
            this._serializer0.PackTo(packer, objectTree.Type);
        }
        
        private void PackValueOfValue(MsgPack.Packer packer, Stormancer.Plugins.ScoreFilter objectTree) {
            this._serializer1.PackTo(packer, objectTree.Value);
        }
        
        protected internal override void PackToCore(MsgPack.Packer packer, Stormancer.Plugins.ScoreFilter objectTree) {
            MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.ScoreFilter> packHelperParameters = default(MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.ScoreFilter>);
            packHelperParameters.Packer = packer;
            packHelperParameters.Target = objectTree;
            packHelperParameters.Operations = this._packOperationList;
            MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.ScoreFilter> packHelperParameters0 = default(MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.ScoreFilter>);
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
        
        private void SetUnpackedValueOfType(Stormancer.Plugins.ScoreFilter unpackingContext, Stormancer.Plugins.ComparisonOperator unpackedValue) {
            unpackingContext.Type = unpackedValue;
        }
        
        private void UnpackValueOfType(MsgPack.Unpacker unpacker, Stormancer.Plugins.ScoreFilter unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Plugins.ScoreFilter, Stormancer.Plugins.ComparisonOperator> unpackHelperParameters = default(MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Plugins.ScoreFilter, Stormancer.Plugins.ComparisonOperator>);
            unpackHelperParameters.Unpacker = unpacker;
            unpackHelperParameters.UnpackingContext = unpackingContext;
            unpackHelperParameters.Serializer = this._serializer0;
            unpackHelperParameters.ItemsCount = itemsCount;
            unpackHelperParameters.Unpacked = indexOfItem;
            unpackHelperParameters.TargetObjectType = typeof(Stormancer.Plugins.ComparisonOperator);
            unpackHelperParameters.MemberName = "Type";
            unpackHelperParameters.DirectRead = null;
            unpackHelperParameters.Setter = this.this_SetUnpackedValueOfTypeDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackValueTypeValue(ref unpackHelperParameters);
        }
        
        private void SetUnpackedValueOfValue(Stormancer.Plugins.ScoreFilter unpackingContext, int unpackedValue) {
            unpackingContext.Value = unpackedValue;
        }
        
        private void UnpackValueOfValue(MsgPack.Unpacker unpacker, Stormancer.Plugins.ScoreFilter unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Plugins.ScoreFilter, int> unpackHelperParameters0 = default(MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Plugins.ScoreFilter, int>);
            unpackHelperParameters0.Unpacker = unpacker;
            unpackHelperParameters0.UnpackingContext = unpackingContext;
            unpackHelperParameters0.Serializer = this._serializer1;
            unpackHelperParameters0.ItemsCount = itemsCount;
            unpackHelperParameters0.Unpacked = indexOfItem;
            unpackHelperParameters0.TargetObjectType = typeof(int);
            unpackHelperParameters0.MemberName = "Value";
            unpackHelperParameters0.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate;
            unpackHelperParameters0.Setter = this.this_SetUnpackedValueOfValueDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackValueTypeValue(ref unpackHelperParameters0);
        }
        
        protected internal override Stormancer.Plugins.ScoreFilter UnpackFromCore(MsgPack.Unpacker unpacker) {
            Stormancer.Plugins.ScoreFilter result = default(Stormancer.Plugins.ScoreFilter);
            result = new Stormancer.Plugins.ScoreFilter();
            if (unpacker.IsArrayHeader) {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromArray(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.ScoreFilter>(), this._memberNames, this._unpackOperationList);
            }
            else {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromMap(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.ScoreFilter>(), this._unpackOperationTable);
            }
        }
    }
}

