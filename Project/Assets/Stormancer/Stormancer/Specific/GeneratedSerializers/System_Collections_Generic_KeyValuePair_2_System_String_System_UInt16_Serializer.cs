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
    public class System_Collections_Generic_KeyValuePair_2_System_String_System_UInt16_Serializer : MsgPack.Serialization.MessagePackSerializer<System.Collections.Generic.KeyValuePair<string, ushort>> {
        
        private MsgPack.Serialization.MessagePackSerializer<string> _serializer0;
        
        private MsgPack.Serialization.MessagePackSerializer<ushort> _serializer1;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Packer, System.Collections.Generic.KeyValuePair<string, ushort>>> _packOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Packer, System.Collections.Generic.KeyValuePair<string, ushort>>> _packOperationTable;
        
        private System.Collections.Generic.IDictionary<string, System.Func<System.Collections.Generic.KeyValuePair<string, ushort>, bool>> _nullCheckersTable;
        
        private System.Func<UnpackingContext, System.Collections.Generic.KeyValuePair<string, ushort>> this_CreateInstanceFromContextDelegate;
        
        private System.Action<UnpackingContext, string> this_SetUnpackedValueOfKeyDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, string> MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
        
        private System.Action<UnpackingContext, ushort> this_SetUnpackedValueOfValueDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, ushort> MsgPack_Serialization_UnpackHelpers_UnpackUInt16ValueDelegate;
        
        private System.Collections.Generic.IList<string> _memberNames;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Unpacker, UnpackingContext, int, int>> _unpackOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Unpacker, UnpackingContext, int, int>> _unpackOperationTable;
        
        public System_Collections_Generic_KeyValuePair_2_System_String_System_UInt16_Serializer(MsgPack.Serialization.SerializationContext context) : 
                base(context, (MsgPack.Serialization.SerializerCapabilities.PackTo | MsgPack.Serialization.SerializerCapabilities.UnpackFrom)) {
            MsgPack.Serialization.PolymorphismSchema schema0 = default(MsgPack.Serialization.PolymorphismSchema);
            schema0 = null;
            this._serializer0 = context.GetSerializer<string>(schema0);
            MsgPack.Serialization.PolymorphismSchema schema1 = default(MsgPack.Serialization.PolymorphismSchema);
            schema1 = null;
            this._serializer1 = context.GetSerializer<ushort>(schema1);
            System.Action<MsgPack.Packer, System.Collections.Generic.KeyValuePair<string, ushort>>[] packOperationList = default(System.Action<MsgPack.Packer, System.Collections.Generic.KeyValuePair<string, ushort>>[]);
            packOperationList = new System.Action<MsgPack.Packer, System.Collections.Generic.KeyValuePair<string, ushort>>[2];
            packOperationList[0] = new System.Action<MsgPack.Packer, System.Collections.Generic.KeyValuePair<string, ushort>>(this.PackValueOfKey);
            packOperationList[1] = new System.Action<MsgPack.Packer, System.Collections.Generic.KeyValuePair<string, ushort>>(this.PackValueOfValue);
            this._packOperationList = packOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, System.Collections.Generic.KeyValuePair<string, ushort>>> packOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, System.Collections.Generic.KeyValuePair<string, ushort>>>);
            packOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, System.Collections.Generic.KeyValuePair<string, ushort>>>(2);
            packOperationTable["Key"] = new System.Action<MsgPack.Packer, System.Collections.Generic.KeyValuePair<string, ushort>>(this.PackValueOfKey);
            packOperationTable["Value"] = new System.Action<MsgPack.Packer, System.Collections.Generic.KeyValuePair<string, ushort>>(this.PackValueOfValue);
            this._packOperationTable = packOperationTable;
            System.Collections.Generic.Dictionary<string, System.Func<System.Collections.Generic.KeyValuePair<string, ushort>, bool>> nullCheckerTable = default(System.Collections.Generic.Dictionary<string, System.Func<System.Collections.Generic.KeyValuePair<string, ushort>, bool>>);
            nullCheckerTable = new System.Collections.Generic.Dictionary<string, System.Func<System.Collections.Generic.KeyValuePair<string, ushort>, bool>>(1);
            nullCheckerTable["Key"] = new System.Func<System.Collections.Generic.KeyValuePair<string, ushort>, bool>(this.IsKeyNull);
            this._nullCheckersTable = nullCheckerTable;
            System.Action<MsgPack.Unpacker, UnpackingContext, int, int>[] unpackOperationList = default(System.Action<MsgPack.Unpacker, UnpackingContext, int, int>[]);
            unpackOperationList = new System.Action<MsgPack.Unpacker, UnpackingContext, int, int>[2];
            unpackOperationList[0] = new System.Action<MsgPack.Unpacker, UnpackingContext, int, int>(this.UnpackValueOfKey);
            unpackOperationList[1] = new System.Action<MsgPack.Unpacker, UnpackingContext, int, int>(this.UnpackValueOfValue);
            this._unpackOperationList = unpackOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, UnpackingContext, int, int>> unpackOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, UnpackingContext, int, int>>);
            unpackOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, UnpackingContext, int, int>>(2);
            unpackOperationTable["Key"] = new System.Action<MsgPack.Unpacker, UnpackingContext, int, int>(this.UnpackValueOfKey);
            unpackOperationTable["Value"] = new System.Action<MsgPack.Unpacker, UnpackingContext, int, int>(this.UnpackValueOfValue);
            this._unpackOperationTable = unpackOperationTable;
            this._memberNames = new string[] {
                    "Key",
                    "Value"};
            this.this_CreateInstanceFromContextDelegate = new System.Func<UnpackingContext, System.Collections.Generic.KeyValuePair<string, ushort>>(this.CreateInstanceFromContext);
            this.this_SetUnpackedValueOfKeyDelegate = new System.Action<UnpackingContext, string>(this.SetUnpackedValueOfKey);
            this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, string>(MsgPack.Serialization.UnpackHelpers.UnpackStringValue);
            this.this_SetUnpackedValueOfValueDelegate = new System.Action<UnpackingContext, ushort>(this.SetUnpackedValueOfValue);
            this.MsgPack_Serialization_UnpackHelpers_UnpackUInt16ValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, ushort>(MsgPack.Serialization.UnpackHelpers.UnpackUInt16Value);
        }
        
        private void PackValueOfKey(MsgPack.Packer packer, System.Collections.Generic.KeyValuePair<string, ushort> objectTree) {
            this._serializer0.PackTo(packer, objectTree.Key);
        }
        
        private bool IsKeyNull(System.Collections.Generic.KeyValuePair<string, ushort> objectTree) {
            return (objectTree.Key == null);
        }
        
        private void PackValueOfValue(MsgPack.Packer packer, System.Collections.Generic.KeyValuePair<string, ushort> objectTree) {
            this._serializer1.PackTo(packer, objectTree.Value);
        }
        
        protected internal override void PackToCore(MsgPack.Packer packer, System.Collections.Generic.KeyValuePair<string, ushort> objectTree) {
            MsgPack.Serialization.PackToArrayParameters<System.Collections.Generic.KeyValuePair<string, ushort>> packHelperParameters = default(MsgPack.Serialization.PackToArrayParameters<System.Collections.Generic.KeyValuePair<string, ushort>>);
            packHelperParameters.Packer = packer;
            packHelperParameters.Target = objectTree;
            packHelperParameters.Operations = this._packOperationList;
            MsgPack.Serialization.PackToMapParameters<System.Collections.Generic.KeyValuePair<string, ushort>> packHelperParameters0 = default(MsgPack.Serialization.PackToMapParameters<System.Collections.Generic.KeyValuePair<string, ushort>>);
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
        
        private System.Collections.Generic.KeyValuePair<string, ushort> CreateInstanceFromContext(UnpackingContext unpackingContext) {
            System.Collections.Generic.KeyValuePair<string, ushort> result = default(System.Collections.Generic.KeyValuePair<string, ushort>);
            result = new System.Collections.Generic.KeyValuePair<string, ushort>(unpackingContext.Key, unpackingContext.Value);
            return result;
        }
        
        private void SetUnpackedValueOfKey(UnpackingContext unpackingContext, string unpackedValue) {
            unpackingContext.Key = unpackedValue;
        }
        
        private void UnpackValueOfKey(MsgPack.Unpacker unpacker, UnpackingContext unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<UnpackingContext, string> unpackHelperParameters = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<UnpackingContext, string>);
            unpackHelperParameters.Unpacker = unpacker;
            unpackHelperParameters.UnpackingContext = unpackingContext;
            unpackHelperParameters.Serializer = this._serializer0;
            unpackHelperParameters.ItemsCount = itemsCount;
            unpackHelperParameters.Unpacked = indexOfItem;
            unpackHelperParameters.TargetObjectType = typeof(string);
            unpackHelperParameters.MemberName = "Key";
            unpackHelperParameters.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
            unpackHelperParameters.Setter = this.this_SetUnpackedValueOfKeyDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters);
        }
        
        private void SetUnpackedValueOfValue(UnpackingContext unpackingContext, ushort unpackedValue) {
            unpackingContext.Value = unpackedValue;
        }
        
        private void UnpackValueOfValue(MsgPack.Unpacker unpacker, UnpackingContext unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackValueTypeValueParameters<UnpackingContext, ushort> unpackHelperParameters0 = default(MsgPack.Serialization.UnpackValueTypeValueParameters<UnpackingContext, ushort>);
            unpackHelperParameters0.Unpacker = unpacker;
            unpackHelperParameters0.UnpackingContext = unpackingContext;
            unpackHelperParameters0.Serializer = this._serializer1;
            unpackHelperParameters0.ItemsCount = itemsCount;
            unpackHelperParameters0.Unpacked = indexOfItem;
            unpackHelperParameters0.TargetObjectType = typeof(ushort);
            unpackHelperParameters0.MemberName = "Value";
            unpackHelperParameters0.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackUInt16ValueDelegate;
            unpackHelperParameters0.Setter = this.this_SetUnpackedValueOfValueDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackValueTypeValue(ref unpackHelperParameters0);
        }
        
        protected internal override System.Collections.Generic.KeyValuePair<string, ushort> UnpackFromCore(MsgPack.Unpacker unpacker) {
            UnpackingContext unpackingContext = default(UnpackingContext);
            string ctorArg0 = default(string);
            ctorArg0 = null;
            ushort ctorArg1 = default(ushort);
            ctorArg1 = 0;
            unpackingContext = new UnpackingContext(ctorArg0, ctorArg1);
            if (unpacker.IsArrayHeader) {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromArray(unpacker, unpackingContext, this.this_CreateInstanceFromContextDelegate, this._memberNames, this._unpackOperationList);
            }
            else {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromMap(unpacker, unpackingContext, this.this_CreateInstanceFromContextDelegate, this._unpackOperationTable);
            }
        }
        
        public class UnpackingContext {
            
            public string Key;
            
            public ushort Value;
            
            public UnpackingContext(string Key, ushort Value) {
                this.Key = Key;
                this.Value = Value;
            }
        }
    }
}

