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
    public class Stormancer_Plugins_GameSessionResultSerializer : MsgPack.Serialization.MessagePackSerializer<Stormancer.Plugins.GameSessionResult> {
        
        private MsgPack.Serialization.MessagePackSerializer<System.Collections.Generic.Dictionary<string, string>> _serializer0;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Packer, Stormancer.Plugins.GameSessionResult>> _packOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.GameSessionResult>> _packOperationTable;
        
        private System.Collections.Generic.IDictionary<string, System.Func<Stormancer.Plugins.GameSessionResult, bool>> _nullCheckersTable;
        
        private System.Action<Stormancer.Plugins.GameSessionResult, System.Collections.Generic.Dictionary<string, string>> this_SetUnpackedValueOfUsersScoreDelegate;
        
        private System.Collections.Generic.IList<string> _memberNames;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameSessionResult, int, int>> _unpackOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameSessionResult, int, int>> _unpackOperationTable;
        
        public Stormancer_Plugins_GameSessionResultSerializer(MsgPack.Serialization.SerializationContext context) : 
                base(context, (MsgPack.Serialization.SerializerCapabilities.PackTo | MsgPack.Serialization.SerializerCapabilities.UnpackFrom)) {
            MsgPack.Serialization.PolymorphismSchema schema0 = default(MsgPack.Serialization.PolymorphismSchema);
            schema0 = null;
            this._serializer0 = context.GetSerializer<System.Collections.Generic.Dictionary<string, string>>(schema0);
            System.Action<MsgPack.Packer, Stormancer.Plugins.GameSessionResult>[] packOperationList = default(System.Action<MsgPack.Packer, Stormancer.Plugins.GameSessionResult>[]);
            packOperationList = new System.Action<MsgPack.Packer, Stormancer.Plugins.GameSessionResult>[1];
            packOperationList[0] = new System.Action<MsgPack.Packer, Stormancer.Plugins.GameSessionResult>(this.PackValueOfUsersScore);
            this._packOperationList = packOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.GameSessionResult>> packOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.GameSessionResult>>);
            packOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.GameSessionResult>>(1);
            packOperationTable["UsersScore"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.GameSessionResult>(this.PackValueOfUsersScore);
            this._packOperationTable = packOperationTable;
            System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.GameSessionResult, bool>> nullCheckerTable = default(System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.GameSessionResult, bool>>);
            nullCheckerTable = new System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.GameSessionResult, bool>>(1);
            nullCheckerTable["UsersScore"] = new System.Func<Stormancer.Plugins.GameSessionResult, bool>(this.IsUsersScoreNull);
            this._nullCheckersTable = nullCheckerTable;
            System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameSessionResult, int, int>[] unpackOperationList = default(System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameSessionResult, int, int>[]);
            unpackOperationList = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameSessionResult, int, int>[1];
            unpackOperationList[0] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameSessionResult, int, int>(this.UnpackValueOfUsersScore);
            this._unpackOperationList = unpackOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameSessionResult, int, int>> unpackOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameSessionResult, int, int>>);
            unpackOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameSessionResult, int, int>>(1);
            unpackOperationTable["UsersScore"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.GameSessionResult, int, int>(this.UnpackValueOfUsersScore);
            this._unpackOperationTable = unpackOperationTable;
            this._memberNames = new string[] {
                    "UsersScore"};
            this.this_SetUnpackedValueOfUsersScoreDelegate = new System.Action<Stormancer.Plugins.GameSessionResult, System.Collections.Generic.Dictionary<string, string>>(this.SetUnpackedValueOfUsersScore);
        }
        
        private void PackValueOfUsersScore(MsgPack.Packer packer, Stormancer.Plugins.GameSessionResult objectTree) {
            this._serializer0.PackTo(packer, objectTree.UsersScore);
        }
        
        private bool IsUsersScoreNull(Stormancer.Plugins.GameSessionResult objectTree) {
            return (objectTree.UsersScore == null);
        }
        
        protected internal override void PackToCore(MsgPack.Packer packer, Stormancer.Plugins.GameSessionResult objectTree) {
            MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.GameSessionResult> packHelperParameters = default(MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.GameSessionResult>);
            packHelperParameters.Packer = packer;
            packHelperParameters.Target = objectTree;
            packHelperParameters.Operations = this._packOperationList;
            MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.GameSessionResult> packHelperParameters0 = default(MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.GameSessionResult>);
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
        
        private void SetUnpackedValueOfUsersScore(Stormancer.Plugins.GameSessionResult unpackingContext, System.Collections.Generic.Dictionary<string, string> unpackedValue) {
            unpackingContext.UsersScore = unpackedValue;
        }
        
        private void UnpackValueOfUsersScore(MsgPack.Unpacker unpacker, Stormancer.Plugins.GameSessionResult unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.GameSessionResult, System.Collections.Generic.Dictionary<string, string>> unpackHelperParameters = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.GameSessionResult, System.Collections.Generic.Dictionary<string, string>>);
            unpackHelperParameters.Unpacker = unpacker;
            unpackHelperParameters.UnpackingContext = unpackingContext;
            unpackHelperParameters.Serializer = this._serializer0;
            unpackHelperParameters.ItemsCount = itemsCount;
            unpackHelperParameters.Unpacked = indexOfItem;
            unpackHelperParameters.TargetObjectType = typeof(System.Collections.Generic.Dictionary<string, string>);
            unpackHelperParameters.MemberName = "UsersScore";
            unpackHelperParameters.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters.DirectRead = null;
            unpackHelperParameters.Setter = this.this_SetUnpackedValueOfUsersScoreDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters);
        }
        
        protected internal override Stormancer.Plugins.GameSessionResult UnpackFromCore(MsgPack.Unpacker unpacker) {
            Stormancer.Plugins.GameSessionResult result = default(Stormancer.Plugins.GameSessionResult);
            result = new Stormancer.Plugins.GameSessionResult();
            if (unpacker.IsArrayHeader) {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromArray(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.GameSessionResult>(), this._memberNames, this._unpackOperationList);
            }
            else {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromMap(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.GameSessionResult>(), this._unpackOperationTable);
            }
        }
    }
}
