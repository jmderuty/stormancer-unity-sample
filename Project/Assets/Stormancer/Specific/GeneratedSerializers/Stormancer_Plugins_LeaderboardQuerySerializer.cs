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
    public class Stormancer_Plugins_LeaderboardQuerySerializer : MsgPack.Serialization.MessagePackSerializer<Stormancer.Plugins.LeaderboardQuery> {
        
        private MsgPack.Serialization.MessagePackSerializer<string> _serializer0;
        
        private MsgPack.Serialization.MessagePackSerializer<System.Collections.Generic.List<Stormancer.Plugins.ScoreFilter>> _serializer1;
        
        private MsgPack.Serialization.MessagePackSerializer<System.Collections.Generic.List<Stormancer.Plugins.FieldFilter>> _serializer2;
        
        private MsgPack.Serialization.MessagePackSerializer<int> _serializer3;
        
        private MsgPack.Serialization.MessagePackSerializer<System.Collections.Generic.List<string>> _serializer4;
        
        private MsgPack.Serialization.MessagePackSerializer<Stormancer.Plugins.LeaderboardOrdering> _serializer5;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>> _packOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>> _packOperationTable;
        
        private System.Collections.Generic.IDictionary<string, System.Func<Stormancer.Plugins.LeaderboardQuery, bool>> _nullCheckersTable;
        
        private System.Action<Stormancer.Plugins.LeaderboardQuery, string> this_SetUnpackedValueOfStartIdDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, string> MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
        
        private System.Action<Stormancer.Plugins.LeaderboardQuery, System.Collections.Generic.List<Stormancer.Plugins.ScoreFilter>> this_SetUnpackedValueOfScoreFiltersDelegate;
        
        private System.Action<Stormancer.Plugins.LeaderboardQuery, System.Collections.Generic.List<Stormancer.Plugins.FieldFilter>> this_SetUnpackedValueOfFieldFiltersDelegate;
        
        private System.Action<Stormancer.Plugins.LeaderboardQuery, int> this_SetUnpackedValueOfSizeDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, int> MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate;
        
        private System.Action<Stormancer.Plugins.LeaderboardQuery, int> this_SetUnpackedValueOfSkipDelegate;
        
        private System.Action<Stormancer.Plugins.LeaderboardQuery, string> this_SetUnpackedValueOfLeaderboardNameDelegate;
        
        private System.Action<Stormancer.Plugins.LeaderboardQuery, System.Collections.Generic.List<string>> this_SetUnpackedValueOfFriendsIdsDelegate;
        
        private System.Action<Stormancer.Plugins.LeaderboardQuery, Stormancer.Plugins.LeaderboardOrdering> this_SetUnpackedValueOfOrderDelegate;
        
        private System.Collections.Generic.IList<string> _memberNames;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>> _unpackOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>> _unpackOperationTable;
        
        public Stormancer_Plugins_LeaderboardQuerySerializer(MsgPack.Serialization.SerializationContext context) : 
                base(context, (MsgPack.Serialization.SerializerCapabilities.PackTo | MsgPack.Serialization.SerializerCapabilities.UnpackFrom)) {
            MsgPack.Serialization.PolymorphismSchema schema0 = default(MsgPack.Serialization.PolymorphismSchema);
            schema0 = null;
            this._serializer0 = context.GetSerializer<string>(schema0);
            MsgPack.Serialization.PolymorphismSchema schema1 = default(MsgPack.Serialization.PolymorphismSchema);
            schema1 = null;
            this._serializer1 = context.GetSerializer<System.Collections.Generic.List<Stormancer.Plugins.ScoreFilter>>(schema1);
            MsgPack.Serialization.PolymorphismSchema schema2 = default(MsgPack.Serialization.PolymorphismSchema);
            schema2 = null;
            this._serializer2 = context.GetSerializer<System.Collections.Generic.List<Stormancer.Plugins.FieldFilter>>(schema2);
            MsgPack.Serialization.PolymorphismSchema schema3 = default(MsgPack.Serialization.PolymorphismSchema);
            schema3 = null;
            this._serializer3 = context.GetSerializer<int>(schema3);
            MsgPack.Serialization.PolymorphismSchema schema4 = default(MsgPack.Serialization.PolymorphismSchema);
            schema4 = null;
            this._serializer4 = context.GetSerializer<System.Collections.Generic.List<string>>(schema4);
            this._serializer5 = context.GetSerializer<Stormancer.Plugins.LeaderboardOrdering>(MsgPack.Serialization.EnumMessagePackSerializerHelpers.DetermineEnumSerializationMethod(context, typeof(Stormancer.Plugins.LeaderboardOrdering), MsgPack.Serialization.EnumMemberSerializationMethod.Default));
            System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>[] packOperationList = default(System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>[]);
            packOperationList = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>[8];
            packOperationList[0] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfStartId);
            packOperationList[1] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfScoreFilters);
            packOperationList[2] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfFieldFilters);
            packOperationList[3] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfSize);
            packOperationList[4] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfSkip);
            packOperationList[5] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfLeaderboardName);
            packOperationList[6] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfFriendsIds);
            packOperationList[7] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfOrder);
            this._packOperationList = packOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>> packOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>>);
            packOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>>(8);
            packOperationTable["StartId"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfStartId);
            packOperationTable["ScoreFilters"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfScoreFilters);
            packOperationTable["FieldFilters"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfFieldFilters);
            packOperationTable["Size"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfSize);
            packOperationTable["Skip"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfSkip);
            packOperationTable["LeaderboardName"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfLeaderboardName);
            packOperationTable["FriendsIds"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfFriendsIds);
            packOperationTable["Order"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.LeaderboardQuery>(this.PackValueOfOrder);
            this._packOperationTable = packOperationTable;
            System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.LeaderboardQuery, bool>> nullCheckerTable = default(System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.LeaderboardQuery, bool>>);
            nullCheckerTable = new System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.LeaderboardQuery, bool>>(5);
            nullCheckerTable["StartId"] = new System.Func<Stormancer.Plugins.LeaderboardQuery, bool>(this.IsStartIdNull);
            nullCheckerTable["ScoreFilters"] = new System.Func<Stormancer.Plugins.LeaderboardQuery, bool>(this.IsScoreFiltersNull);
            nullCheckerTable["FieldFilters"] = new System.Func<Stormancer.Plugins.LeaderboardQuery, bool>(this.IsFieldFiltersNull);
            nullCheckerTable["LeaderboardName"] = new System.Func<Stormancer.Plugins.LeaderboardQuery, bool>(this.IsLeaderboardNameNull);
            nullCheckerTable["FriendsIds"] = new System.Func<Stormancer.Plugins.LeaderboardQuery, bool>(this.IsFriendsIdsNull);
            this._nullCheckersTable = nullCheckerTable;
            System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>[] unpackOperationList = default(System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>[]);
            unpackOperationList = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>[8];
            unpackOperationList[0] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfStartId);
            unpackOperationList[1] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfScoreFilters);
            unpackOperationList[2] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfFieldFilters);
            unpackOperationList[3] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfSize);
            unpackOperationList[4] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfSkip);
            unpackOperationList[5] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfLeaderboardName);
            unpackOperationList[6] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfFriendsIds);
            unpackOperationList[7] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfOrder);
            this._unpackOperationList = unpackOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>> unpackOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>>);
            unpackOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>>(8);
            unpackOperationTable["StartId"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfStartId);
            unpackOperationTable["ScoreFilters"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfScoreFilters);
            unpackOperationTable["FieldFilters"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfFieldFilters);
            unpackOperationTable["Size"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfSize);
            unpackOperationTable["Skip"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfSkip);
            unpackOperationTable["LeaderboardName"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfLeaderboardName);
            unpackOperationTable["FriendsIds"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfFriendsIds);
            unpackOperationTable["Order"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.LeaderboardQuery, int, int>(this.UnpackValueOfOrder);
            this._unpackOperationTable = unpackOperationTable;
            this._memberNames = new string[] {
                    "StartId",
                    "ScoreFilters",
                    "FieldFilters",
                    "Size",
                    "Skip",
                    "LeaderboardName",
                    "FriendsIds",
                    "Order"};
            this.this_SetUnpackedValueOfStartIdDelegate = new System.Action<Stormancer.Plugins.LeaderboardQuery, string>(this.SetUnpackedValueOfStartId);
            this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, string>(MsgPack.Serialization.UnpackHelpers.UnpackStringValue);
            this.this_SetUnpackedValueOfScoreFiltersDelegate = new System.Action<Stormancer.Plugins.LeaderboardQuery, System.Collections.Generic.List<Stormancer.Plugins.ScoreFilter>>(this.SetUnpackedValueOfScoreFilters);
            this.this_SetUnpackedValueOfFieldFiltersDelegate = new System.Action<Stormancer.Plugins.LeaderboardQuery, System.Collections.Generic.List<Stormancer.Plugins.FieldFilter>>(this.SetUnpackedValueOfFieldFilters);
            this.this_SetUnpackedValueOfSizeDelegate = new System.Action<Stormancer.Plugins.LeaderboardQuery, int>(this.SetUnpackedValueOfSize);
            this.MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, int>(MsgPack.Serialization.UnpackHelpers.UnpackInt32Value);
            this.this_SetUnpackedValueOfSkipDelegate = new System.Action<Stormancer.Plugins.LeaderboardQuery, int>(this.SetUnpackedValueOfSkip);
            this.this_SetUnpackedValueOfLeaderboardNameDelegate = new System.Action<Stormancer.Plugins.LeaderboardQuery, string>(this.SetUnpackedValueOfLeaderboardName);
            this.this_SetUnpackedValueOfFriendsIdsDelegate = new System.Action<Stormancer.Plugins.LeaderboardQuery, System.Collections.Generic.List<string>>(this.SetUnpackedValueOfFriendsIds);
            this.this_SetUnpackedValueOfOrderDelegate = new System.Action<Stormancer.Plugins.LeaderboardQuery, Stormancer.Plugins.LeaderboardOrdering>(this.SetUnpackedValueOfOrder);
        }
        
        private void PackValueOfStartId(MsgPack.Packer packer, Stormancer.Plugins.LeaderboardQuery objectTree) {
            this._serializer0.PackTo(packer, objectTree.StartId);
        }
        
        private bool IsStartIdNull(Stormancer.Plugins.LeaderboardQuery objectTree) {
            return (objectTree.StartId == null);
        }
        
        private void PackValueOfScoreFilters(MsgPack.Packer packer, Stormancer.Plugins.LeaderboardQuery objectTree) {
            this._serializer1.PackTo(packer, objectTree.ScoreFilters);
        }
        
        private bool IsScoreFiltersNull(Stormancer.Plugins.LeaderboardQuery objectTree) {
            return (objectTree.ScoreFilters == null);
        }
        
        private void PackValueOfFieldFilters(MsgPack.Packer packer, Stormancer.Plugins.LeaderboardQuery objectTree) {
            this._serializer2.PackTo(packer, objectTree.FieldFilters);
        }
        
        private bool IsFieldFiltersNull(Stormancer.Plugins.LeaderboardQuery objectTree) {
            return (objectTree.FieldFilters == null);
        }
        
        private void PackValueOfSize(MsgPack.Packer packer, Stormancer.Plugins.LeaderboardQuery objectTree) {
            this._serializer3.PackTo(packer, objectTree.Size);
        }
        
        private void PackValueOfSkip(MsgPack.Packer packer, Stormancer.Plugins.LeaderboardQuery objectTree) {
            this._serializer3.PackTo(packer, objectTree.Skip);
        }
        
        private void PackValueOfLeaderboardName(MsgPack.Packer packer, Stormancer.Plugins.LeaderboardQuery objectTree) {
            this._serializer0.PackTo(packer, objectTree.LeaderboardName);
        }
        
        private bool IsLeaderboardNameNull(Stormancer.Plugins.LeaderboardQuery objectTree) {
            return (objectTree.LeaderboardName == null);
        }
        
        private void PackValueOfFriendsIds(MsgPack.Packer packer, Stormancer.Plugins.LeaderboardQuery objectTree) {
            this._serializer4.PackTo(packer, objectTree.FriendsIds);
        }
        
        private bool IsFriendsIdsNull(Stormancer.Plugins.LeaderboardQuery objectTree) {
            return (objectTree.FriendsIds == null);
        }
        
        private void PackValueOfOrder(MsgPack.Packer packer, Stormancer.Plugins.LeaderboardQuery objectTree) {
            this._serializer5.PackTo(packer, objectTree.Order);
        }
        
        protected internal override void PackToCore(MsgPack.Packer packer, Stormancer.Plugins.LeaderboardQuery objectTree) {
            MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.LeaderboardQuery> packHelperParameters = default(MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.LeaderboardQuery>);
            packHelperParameters.Packer = packer;
            packHelperParameters.Target = objectTree;
            packHelperParameters.Operations = this._packOperationList;
            MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.LeaderboardQuery> packHelperParameters0 = default(MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.LeaderboardQuery>);
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
        
        private void SetUnpackedValueOfStartId(Stormancer.Plugins.LeaderboardQuery unpackingContext, string unpackedValue) {
            unpackingContext.StartId = unpackedValue;
        }
        
        private void UnpackValueOfStartId(MsgPack.Unpacker unpacker, Stormancer.Plugins.LeaderboardQuery unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, string> unpackHelperParameters = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, string>);
            unpackHelperParameters.Unpacker = unpacker;
            unpackHelperParameters.UnpackingContext = unpackingContext;
            unpackHelperParameters.Serializer = this._serializer0;
            unpackHelperParameters.ItemsCount = itemsCount;
            unpackHelperParameters.Unpacked = indexOfItem;
            unpackHelperParameters.TargetObjectType = typeof(string);
            unpackHelperParameters.MemberName = "StartId";
            unpackHelperParameters.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
            unpackHelperParameters.Setter = this.this_SetUnpackedValueOfStartIdDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters);
        }
        
        private void SetUnpackedValueOfScoreFilters(Stormancer.Plugins.LeaderboardQuery unpackingContext, System.Collections.Generic.List<Stormancer.Plugins.ScoreFilter> unpackedValue) {
            unpackingContext.ScoreFilters = unpackedValue;
        }
        
        private void UnpackValueOfScoreFilters(MsgPack.Unpacker unpacker, Stormancer.Plugins.LeaderboardQuery unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, System.Collections.Generic.List<Stormancer.Plugins.ScoreFilter>> unpackHelperParameters0 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, System.Collections.Generic.List<Stormancer.Plugins.ScoreFilter>>);
            unpackHelperParameters0.Unpacker = unpacker;
            unpackHelperParameters0.UnpackingContext = unpackingContext;
            unpackHelperParameters0.Serializer = this._serializer1;
            unpackHelperParameters0.ItemsCount = itemsCount;
            unpackHelperParameters0.Unpacked = indexOfItem;
            unpackHelperParameters0.TargetObjectType = typeof(System.Collections.Generic.List<Stormancer.Plugins.ScoreFilter>);
            unpackHelperParameters0.MemberName = "ScoreFilters";
            unpackHelperParameters0.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters0.DirectRead = null;
            unpackHelperParameters0.Setter = this.this_SetUnpackedValueOfScoreFiltersDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters0);
        }
        
        private void SetUnpackedValueOfFieldFilters(Stormancer.Plugins.LeaderboardQuery unpackingContext, System.Collections.Generic.List<Stormancer.Plugins.FieldFilter> unpackedValue) {
            unpackingContext.FieldFilters = unpackedValue;
        }
        
        private void UnpackValueOfFieldFilters(MsgPack.Unpacker unpacker, Stormancer.Plugins.LeaderboardQuery unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, System.Collections.Generic.List<Stormancer.Plugins.FieldFilter>> unpackHelperParameters1 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, System.Collections.Generic.List<Stormancer.Plugins.FieldFilter>>);
            unpackHelperParameters1.Unpacker = unpacker;
            unpackHelperParameters1.UnpackingContext = unpackingContext;
            unpackHelperParameters1.Serializer = this._serializer2;
            unpackHelperParameters1.ItemsCount = itemsCount;
            unpackHelperParameters1.Unpacked = indexOfItem;
            unpackHelperParameters1.TargetObjectType = typeof(System.Collections.Generic.List<Stormancer.Plugins.FieldFilter>);
            unpackHelperParameters1.MemberName = "FieldFilters";
            unpackHelperParameters1.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters1.DirectRead = null;
            unpackHelperParameters1.Setter = this.this_SetUnpackedValueOfFieldFiltersDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters1);
        }
        
        private void SetUnpackedValueOfSize(Stormancer.Plugins.LeaderboardQuery unpackingContext, int unpackedValue) {
            unpackingContext.Size = unpackedValue;
        }
        
        private void UnpackValueOfSize(MsgPack.Unpacker unpacker, Stormancer.Plugins.LeaderboardQuery unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, int> unpackHelperParameters2 = default(MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, int>);
            unpackHelperParameters2.Unpacker = unpacker;
            unpackHelperParameters2.UnpackingContext = unpackingContext;
            unpackHelperParameters2.Serializer = this._serializer3;
            unpackHelperParameters2.ItemsCount = itemsCount;
            unpackHelperParameters2.Unpacked = indexOfItem;
            unpackHelperParameters2.TargetObjectType = typeof(int);
            unpackHelperParameters2.MemberName = "Size";
            unpackHelperParameters2.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate;
            unpackHelperParameters2.Setter = this.this_SetUnpackedValueOfSizeDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackValueTypeValue(ref unpackHelperParameters2);
        }
        
        private void SetUnpackedValueOfSkip(Stormancer.Plugins.LeaderboardQuery unpackingContext, int unpackedValue) {
            unpackingContext.Skip = unpackedValue;
        }
        
        private void UnpackValueOfSkip(MsgPack.Unpacker unpacker, Stormancer.Plugins.LeaderboardQuery unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, int> unpackHelperParameters3 = default(MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, int>);
            unpackHelperParameters3.Unpacker = unpacker;
            unpackHelperParameters3.UnpackingContext = unpackingContext;
            unpackHelperParameters3.Serializer = this._serializer3;
            unpackHelperParameters3.ItemsCount = itemsCount;
            unpackHelperParameters3.Unpacked = indexOfItem;
            unpackHelperParameters3.TargetObjectType = typeof(int);
            unpackHelperParameters3.MemberName = "Skip";
            unpackHelperParameters3.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackInt32ValueDelegate;
            unpackHelperParameters3.Setter = this.this_SetUnpackedValueOfSkipDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackValueTypeValue(ref unpackHelperParameters3);
        }
        
        private void SetUnpackedValueOfLeaderboardName(Stormancer.Plugins.LeaderboardQuery unpackingContext, string unpackedValue) {
            unpackingContext.LeaderboardName = unpackedValue;
        }
        
        private void UnpackValueOfLeaderboardName(MsgPack.Unpacker unpacker, Stormancer.Plugins.LeaderboardQuery unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, string> unpackHelperParameters4 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, string>);
            unpackHelperParameters4.Unpacker = unpacker;
            unpackHelperParameters4.UnpackingContext = unpackingContext;
            unpackHelperParameters4.Serializer = this._serializer0;
            unpackHelperParameters4.ItemsCount = itemsCount;
            unpackHelperParameters4.Unpacked = indexOfItem;
            unpackHelperParameters4.TargetObjectType = typeof(string);
            unpackHelperParameters4.MemberName = "LeaderboardName";
            unpackHelperParameters4.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters4.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
            unpackHelperParameters4.Setter = this.this_SetUnpackedValueOfLeaderboardNameDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters4);
        }
        
        private void SetUnpackedValueOfFriendsIds(Stormancer.Plugins.LeaderboardQuery unpackingContext, System.Collections.Generic.List<string> unpackedValue) {
            unpackingContext.FriendsIds = unpackedValue;
        }
        
        private void UnpackValueOfFriendsIds(MsgPack.Unpacker unpacker, Stormancer.Plugins.LeaderboardQuery unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, System.Collections.Generic.List<string>> unpackHelperParameters5 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, System.Collections.Generic.List<string>>);
            unpackHelperParameters5.Unpacker = unpacker;
            unpackHelperParameters5.UnpackingContext = unpackingContext;
            unpackHelperParameters5.Serializer = this._serializer4;
            unpackHelperParameters5.ItemsCount = itemsCount;
            unpackHelperParameters5.Unpacked = indexOfItem;
            unpackHelperParameters5.TargetObjectType = typeof(System.Collections.Generic.List<string>);
            unpackHelperParameters5.MemberName = "FriendsIds";
            unpackHelperParameters5.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters5.DirectRead = null;
            unpackHelperParameters5.Setter = this.this_SetUnpackedValueOfFriendsIdsDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters5);
        }
        
        private void SetUnpackedValueOfOrder(Stormancer.Plugins.LeaderboardQuery unpackingContext, Stormancer.Plugins.LeaderboardOrdering unpackedValue) {
            unpackingContext.Order = unpackedValue;
        }
        
        private void UnpackValueOfOrder(MsgPack.Unpacker unpacker, Stormancer.Plugins.LeaderboardQuery unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, Stormancer.Plugins.LeaderboardOrdering> unpackHelperParameters6 = default(MsgPack.Serialization.UnpackValueTypeValueParameters<Stormancer.Plugins.LeaderboardQuery, Stormancer.Plugins.LeaderboardOrdering>);
            unpackHelperParameters6.Unpacker = unpacker;
            unpackHelperParameters6.UnpackingContext = unpackingContext;
            unpackHelperParameters6.Serializer = this._serializer5;
            unpackHelperParameters6.ItemsCount = itemsCount;
            unpackHelperParameters6.Unpacked = indexOfItem;
            unpackHelperParameters6.TargetObjectType = typeof(Stormancer.Plugins.LeaderboardOrdering);
            unpackHelperParameters6.MemberName = "Order";
            unpackHelperParameters6.DirectRead = null;
            unpackHelperParameters6.Setter = this.this_SetUnpackedValueOfOrderDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackValueTypeValue(ref unpackHelperParameters6);
        }
        
        protected internal override Stormancer.Plugins.LeaderboardQuery UnpackFromCore(MsgPack.Unpacker unpacker) {
            Stormancer.Plugins.LeaderboardQuery result = default(Stormancer.Plugins.LeaderboardQuery);
            result = new Stormancer.Plugins.LeaderboardQuery();
            if (unpacker.IsArrayHeader) {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromArray(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.LeaderboardQuery>(), this._memberNames, this._unpackOperationList);
            }
            else {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromMap(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.LeaderboardQuery>(), this._unpackOperationTable);
            }
        }
    }
}

