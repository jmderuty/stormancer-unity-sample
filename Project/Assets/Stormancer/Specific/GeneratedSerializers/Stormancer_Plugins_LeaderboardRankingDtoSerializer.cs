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
    public class Stormancer_Plugins_LeaderboardRankingDtoSerializer : MsgPack.Serialization.MessagePackSerializer<Stormancer.Plugins.LeaderboardRankingDto> {
        
        private MsgPack.Serialization.MessagePackSerializer<int> _serializer0;
        
        private MsgPack.Serialization.MessagePackSerializer<Stormancer.Plugins.ScoreDto> _serializer1;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardRankingDto>> _packOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardRankingDto>> _packOperationTable;
        
        private System.Collections.Generic.IDictionary<string, System.Func<Stormancer.Plugins.LeaderboardRankingDto, bool>> _nullCheckersTable;
        
        private System.Action<Stormancer.Plugins.LeaderboardRankingDto, int> this_SetUnpackedValueOfRankingDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, int> MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate;
        
        private System.Action<Stormancer.Plugins.LeaderboardRankingDto, Stormancer.Plugins.ScoreDto> this_SetUnpackedValueOfDocumentDelegate;
        
        private System.Collections.Generic.IList<string> _memberNames;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardRankingDto, int, int>> _unpackOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardRankingDto, int, int>> _unpackOperationTable;
        
        public Stormancer_Plugins_LeaderboardRankingDtoSerializer(MsgPack.Serialization.SerializationContext context) : 
                base(context, (MsgPack.Serialization.SerializerCapabilities.PackTo | MsgPack.Serialization.SerializerCapabilities.UnpackFrom)) {
            MsgPack.Serialization.PolymorphismSchema schema0 = default(MsgPack.Serialization.PolymorphismSchema);
            schema0 = null;
            this._serializer0 = context.GetSerializer<int>(schema0);
            MsgPack.Serialization.PolymorphismSchema schema1 = default(MsgPack.Serialization.PolymorphismSchema);
            schema1 = null;
            this._serializer1 = context.GetSerializer<Stormancer.Plugins.ScoreDto>(schema1);
            System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardRankingDto>[] packOperationList = default(System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardRankingDto>[]);
            packOperationList = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardRankingDto>[2];
            packOperationList[0] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardRankingDto>(this.PackValueOfRanking);
            packOperationList[1] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardRankingDto>(this.PackValueOfDocument);
            this._packOperationList = packOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardRankingDto>> packOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardRankingDto>>);
            packOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardRankingDto>>(2);
            packOperationTable["Ranking"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardRankingDto>(this.PackValueOfRanking);
            packOperationTable["Document"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardRankingDto>(this.PackValueOfDocument);
            this._packOperationTable = packOperationTable;
            System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.LeaderboardRankingDto, bool>> nullCheckerTable = default(System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.LeaderboardRankingDto, bool>>);
            nullCheckerTable = new System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.LeaderboardRankingDto, bool>>(1);
            nullCheckerTable["Document"] = new System.Func<Stormancer.Plugins.LeaderboardRankingDto, bool>(this.IsDocumentNull);
            this._nullCheckersTable = nullCheckerTable;
            System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardRankingDto, int, int>[] unpackOperationList = default(System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardRankingDto, int, int>[]);
            unpackOperationList = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardRankingDto, int, int>[2];
            unpackOperationList[0] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardRankingDto, int, int>(this.UnpackValueOfRanking);
            unpackOperationList[1] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardRankingDto, int, int>(this.UnpackValueOfDocument);
            this._unpackOperationList = unpackOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardRankingDto, int, int>> unpackOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardRankingDto, int, int>>);
            unpackOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardRankingDto, int, int>>(2);
            unpackOperationTable["Ranking"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardRankingDto, int, int>(this.UnpackValueOfRanking);
            unpackOperationTable["Document"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardRankingDto, int, int>(this.UnpackValueOfDocument);
            this._unpackOperationTable = unpackOperationTable;
            this._memberNames = new string[] {
                    "Ranking",
                    "Document"};
            this.this_SetUnpackedValueOfRankingDelegate = new System.Action<Stormancer.Plugins.LeaderboardRankingDto, int>(this.SetUnpackedValueOfRanking);
            this.MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, int>(MsgPack.Serialization.UnpackHelpers.UnpackInt32Value);
            this.this_SetUnpackedValueOfDocumentDelegate = new System.Action<Stormancer.Plugins.LeaderboardRankingDto, Stormancer.Plugins.ScoreDto>(this.SetUnpackedValueOfDocument);
        }
        
        private void PackValueOfRanking(MsgPack.Packer packer, Stormancer.Plugins.LeaderboardRankingDto objectTree) {
            this._serializer0.PackTo(packer, objectTree.Ranking);
        }
        
        private void PackValueOfDocument(MsgPack.Packer packer, Stormancer.Plugins.LeaderboardRankingDto objectTree) {
            this._serializer1.PackTo(packer, objectTree.Document);
        }
        
        private bool IsDocumentNull(Stormancer.Plugins.LeaderboardRankingDto objectTree) {
            return (objectTree.Document == null);
        }
        
        protected internal override void PackToCore(MsgPack.Packer packer, Stormancer.Plugins.LeaderboardRankingDto objectTree) {
            MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.LeaderboardRankingDto> packHelperParameters = default(MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.LeaderboardRankingDto>);
            packHelperParameters.Packer = packer;
            packHelperParameters.Target = objectTree;
            packHelperParameters.Operations = this._packOperationList;
            MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.LeaderboardRankingDto> packHelperParameters0 = default(MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.LeaderboardRankingDto>);
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
        
        private void SetUnpackedValueOfRanking(Stormancer.Plugins.LeaderboardRankingDto unpackingContext, int unpackedValue) {
            unpackingContext.Ranking = unpackedValue;
        }
        
        private void UnpackValueOfRanking(MsgPack.Unpacker unpacker, Stormancer.Plugins.LeaderboardRankingDto unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Plugins.LeaderboardRankingDto, int> unpackHelperParameters = default(MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Plugins.LeaderboardRankingDto, int>);
            unpackHelperParameters.Unpacker = unpacker;
            unpackHelperParameters.UnpackingContext = unpackingContext;
            unpackHelperParameters.Serializer = this._serializer0;
            unpackHelperParameters.ItemsCount = itemsCount;
            unpackHelperParameters.Unpacked = indexOfItem;
            unpackHelperParameters.TargetObjectType = typeof(int);
            unpackHelperParameters.MemberName = "Ranking";
            unpackHelperParameters.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate;
            unpackHelperParameters.Setter = this.this_SetUnpackedValueOfRankingDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackValueTypeValue(ref unpackHelperParameters);
        }
        
        private void SetUnpackedValueOfDocument(Stormancer.Plugins.LeaderboardRankingDto unpackingContext, Stormancer.Plugins.ScoreDto unpackedValue) {
            unpackingContext.Document = unpackedValue;
        }
        
        private void UnpackValueOfDocument(MsgPack.Unpacker unpacker, Stormancer.Plugins.LeaderboardRankingDto unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.LeaderboardRankingDto, Stormancer.Plugins.ScoreDto> unpackHelperParameters0 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.LeaderboardRankingDto, Stormancer.Plugins.ScoreDto>);
            unpackHelperParameters0.Unpacker = unpacker;
            unpackHelperParameters0.UnpackingContext = unpackingContext;
            unpackHelperParameters0.Serializer = this._serializer1;
            unpackHelperParameters0.ItemsCount = itemsCount;
            unpackHelperParameters0.Unpacked = indexOfItem;
            unpackHelperParameters0.TargetObjectType = typeof(Stormancer.Plugins.ScoreDto);
            unpackHelperParameters0.MemberName = "Document";
            unpackHelperParameters0.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters0.DirectRead = null;
            unpackHelperParameters0.Setter = this.this_SetUnpackedValueOfDocumentDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters0);
        }
        
        protected internal override Stormancer.Plugins.LeaderboardRankingDto UnpackFromCore(MsgPack.Unpacker unpacker) {
            Stormancer.Plugins.LeaderboardRankingDto result = default(Stormancer.Plugins.LeaderboardRankingDto);
            result = new Stormancer.Plugins.LeaderboardRankingDto();
            if (unpacker.IsArrayHeader) {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromArray(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.LeaderboardRankingDto>(), this._memberNames, this._unpackOperationList);
            }
            else {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromMap(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.LeaderboardRankingDto>(), this._unpackOperationTable);
            }
        }
    }
}
