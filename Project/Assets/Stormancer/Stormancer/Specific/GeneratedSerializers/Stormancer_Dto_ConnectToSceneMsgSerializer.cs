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
    public class Stormancer_Dto_ConnectToSceneMsgSerializer : MsgPack.Serialization.MessagePackSerializer<Stormancer.Dto.ConnectToSceneMsg> {
        
        private MsgPack.Serialization.MessagePackSerializer<System.Collections.Generic.Dictionary<string, string>> _serializer0;
        
        private MsgPack.Serialization.MessagePackSerializer<System.Collections.Generic.List<Stormancer.Dto.RouteDto>> _serializer1;
        
        private MsgPack.Serialization.MessagePackSerializer<string> _serializer2;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Packer, Stormancer.Dto.ConnectToSceneMsg>> _packOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Packer, Stormancer.Dto.ConnectToSceneMsg>> _packOperationTable;
        
        private System.Collections.Generic.IDictionary<string, System.Func<Stormancer.Dto.ConnectToSceneMsg, bool>> _nullCheckersTable;
        
        private System.Action<Stormancer.Dto.ConnectToSceneMsg, System.Collections.Generic.Dictionary<string, string>> this_SetUnpackedValueOfConnectionMetadataDelegate;
        
        private System.Action<Stormancer.Dto.ConnectToSceneMsg, System.Collections.Generic.List<Stormancer.Dto.RouteDto>> this_SetUnpackedValueOfRoutesDelegate;
        
        private System.Action<Stormancer.Dto.ConnectToSceneMsg, string> this_SetUnpackedValueOfTokenDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, string> MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
        
        private System.Collections.Generic.IList<string> _memberNames;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectToSceneMsg, int, int>> _unpackOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectToSceneMsg, int, int>> _unpackOperationTable;
        
        public Stormancer_Dto_ConnectToSceneMsgSerializer(MsgPack.Serialization.SerializationContext context) : 
                base(context, (MsgPack.Serialization.SerializerCapabilities.PackTo | MsgPack.Serialization.SerializerCapabilities.UnpackFrom)) {
            MsgPack.Serialization.PolymorphismSchema schema0 = default(MsgPack.Serialization.PolymorphismSchema);
            schema0 = null;
            this._serializer0 = context.GetSerializer<System.Collections.Generic.Dictionary<string, string>>(schema0);
            MsgPack.Serialization.PolymorphismSchema schema1 = default(MsgPack.Serialization.PolymorphismSchema);
            schema1 = null;
            this._serializer1 = context.GetSerializer<System.Collections.Generic.List<Stormancer.Dto.RouteDto>>(schema1);
            MsgPack.Serialization.PolymorphismSchema schema2 = default(MsgPack.Serialization.PolymorphismSchema);
            schema2 = null;
            this._serializer2 = context.GetSerializer<string>(schema2);
            System.Action<MsgPack.Packer, Stormancer.Dto.ConnectToSceneMsg>[] packOperationList = default(System.Action<MsgPack.Packer, Stormancer.Dto.ConnectToSceneMsg>[]);
            packOperationList = new System.Action<MsgPack.Packer, Stormancer.Dto.ConnectToSceneMsg>[3];
            packOperationList[0] = new System.Action<MsgPack.Packer, Stormancer.Dto.ConnectToSceneMsg>(this.PackValueOfConnectionMetadata);
            packOperationList[1] = new System.Action<MsgPack.Packer, Stormancer.Dto.ConnectToSceneMsg>(this.PackValueOfRoutes);
            packOperationList[2] = new System.Action<MsgPack.Packer, Stormancer.Dto.ConnectToSceneMsg>(this.PackValueOfToken);
            this._packOperationList = packOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Dto.ConnectToSceneMsg>> packOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Dto.ConnectToSceneMsg>>);
            packOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Dto.ConnectToSceneMsg>>(3);
            packOperationTable["ConnectionMetadata"] = new System.Action<MsgPack.Packer, Stormancer.Dto.ConnectToSceneMsg>(this.PackValueOfConnectionMetadata);
            packOperationTable["Routes"] = new System.Action<MsgPack.Packer, Stormancer.Dto.ConnectToSceneMsg>(this.PackValueOfRoutes);
            packOperationTable["Token"] = new System.Action<MsgPack.Packer, Stormancer.Dto.ConnectToSceneMsg>(this.PackValueOfToken);
            this._packOperationTable = packOperationTable;
            System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Dto.ConnectToSceneMsg, bool>> nullCheckerTable = default(System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Dto.ConnectToSceneMsg, bool>>);
            nullCheckerTable = new System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Dto.ConnectToSceneMsg, bool>>(3);
            nullCheckerTable["ConnectionMetadata"] = new System.Func<Stormancer.Dto.ConnectToSceneMsg, bool>(this.IsConnectionMetadataNull);
            nullCheckerTable["Routes"] = new System.Func<Stormancer.Dto.ConnectToSceneMsg, bool>(this.IsRoutesNull);
            nullCheckerTable["Token"] = new System.Func<Stormancer.Dto.ConnectToSceneMsg, bool>(this.IsTokenNull);
            this._nullCheckersTable = nullCheckerTable;
            System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectToSceneMsg, int, int>[] unpackOperationList = default(System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectToSceneMsg, int, int>[]);
            unpackOperationList = new System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectToSceneMsg, int, int>[3];
            unpackOperationList[0] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectToSceneMsg, int, int>(this.UnpackValueOfConnectionMetadata);
            unpackOperationList[1] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectToSceneMsg, int, int>(this.UnpackValueOfRoutes);
            unpackOperationList[2] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectToSceneMsg, int, int>(this.UnpackValueOfToken);
            this._unpackOperationList = unpackOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectToSceneMsg, int, int>> unpackOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectToSceneMsg, int, int>>);
            unpackOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectToSceneMsg, int, int>>(3);
            unpackOperationTable["ConnectionMetadata"] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectToSceneMsg, int, int>(this.UnpackValueOfConnectionMetadata);
            unpackOperationTable["Routes"] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectToSceneMsg, int, int>(this.UnpackValueOfRoutes);
            unpackOperationTable["Token"] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.ConnectToSceneMsg, int, int>(this.UnpackValueOfToken);
            this._unpackOperationTable = unpackOperationTable;
            this._memberNames = new string[] {
                    "ConnectionMetadata",
                    "Routes",
                    "Token"};
            this.this_SetUnpackedValueOfConnectionMetadataDelegate = new System.Action<Stormancer.Dto.ConnectToSceneMsg, System.Collections.Generic.Dictionary<string, string>>(this.SetUnpackedValueOfConnectionMetadata);
            this.this_SetUnpackedValueOfRoutesDelegate = new System.Action<Stormancer.Dto.ConnectToSceneMsg, System.Collections.Generic.List<Stormancer.Dto.RouteDto>>(this.SetUnpackedValueOfRoutes);
            this.this_SetUnpackedValueOfTokenDelegate = new System.Action<Stormancer.Dto.ConnectToSceneMsg, string>(this.SetUnpackedValueOfToken);
            this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, string>(MsgPack.Serialization.UnpackHelpers.UnpackStringValue);
        }
        
        private void PackValueOfConnectionMetadata(MsgPack.Packer packer, Stormancer.Dto.ConnectToSceneMsg objectTree) {
            this._serializer0.PackTo(packer, objectTree.ConnectionMetadata);
        }
        
        private bool IsConnectionMetadataNull(Stormancer.Dto.ConnectToSceneMsg objectTree) {
            return (objectTree.ConnectionMetadata == null);
        }
        
        private void PackValueOfRoutes(MsgPack.Packer packer, Stormancer.Dto.ConnectToSceneMsg objectTree) {
            this._serializer1.PackTo(packer, objectTree.Routes);
        }
        
        private bool IsRoutesNull(Stormancer.Dto.ConnectToSceneMsg objectTree) {
            return (objectTree.Routes == null);
        }
        
        private void PackValueOfToken(MsgPack.Packer packer, Stormancer.Dto.ConnectToSceneMsg objectTree) {
            this._serializer2.PackTo(packer, objectTree.Token);
        }
        
        private bool IsTokenNull(Stormancer.Dto.ConnectToSceneMsg objectTree) {
            return (objectTree.Token == null);
        }
        
        protected internal override void PackToCore(MsgPack.Packer packer, Stormancer.Dto.ConnectToSceneMsg objectTree) {
            MsgPack.Serialization.PackToArrayParameters<Stormancer.Dto.ConnectToSceneMsg> packHelperParameters = default(MsgPack.Serialization.PackToArrayParameters<Stormancer.Dto.ConnectToSceneMsg>);
            packHelperParameters.Packer = packer;
            packHelperParameters.Target = objectTree;
            packHelperParameters.Operations = this._packOperationList;
            MsgPack.Serialization.PackToMapParameters<Stormancer.Dto.ConnectToSceneMsg> packHelperParameters0 = default(MsgPack.Serialization.PackToMapParameters<Stormancer.Dto.ConnectToSceneMsg>);
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
        
        private void SetUnpackedValueOfConnectionMetadata(Stormancer.Dto.ConnectToSceneMsg unpackingContext, System.Collections.Generic.Dictionary<string, string> unpackedValue) {
            unpackingContext.ConnectionMetadata = unpackedValue;
        }
        
        private void UnpackValueOfConnectionMetadata(MsgPack.Unpacker unpacker, Stormancer.Dto.ConnectToSceneMsg unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.ConnectToSceneMsg, System.Collections.Generic.Dictionary<string, string>> unpackHelperParameters = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.ConnectToSceneMsg, System.Collections.Generic.Dictionary<string, string>>);
            unpackHelperParameters.Unpacker = unpacker;
            unpackHelperParameters.UnpackingContext = unpackingContext;
            unpackHelperParameters.Serializer = this._serializer0;
            unpackHelperParameters.ItemsCount = itemsCount;
            unpackHelperParameters.Unpacked = indexOfItem;
            unpackHelperParameters.TargetObjectType = typeof(System.Collections.Generic.Dictionary<string, string>);
            unpackHelperParameters.MemberName = "ConnectionMetadata";
            unpackHelperParameters.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters.DirectRead = null;
            unpackHelperParameters.Setter = this.this_SetUnpackedValueOfConnectionMetadataDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters);
        }
        
        private void SetUnpackedValueOfRoutes(Stormancer.Dto.ConnectToSceneMsg unpackingContext, System.Collections.Generic.List<Stormancer.Dto.RouteDto> unpackedValue) {
            unpackingContext.Routes = unpackedValue;
        }
        
        private void UnpackValueOfRoutes(MsgPack.Unpacker unpacker, Stormancer.Dto.ConnectToSceneMsg unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.ConnectToSceneMsg, System.Collections.Generic.List<Stormancer.Dto.RouteDto>> unpackHelperParameters0 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.ConnectToSceneMsg, System.Collections.Generic.List<Stormancer.Dto.RouteDto>>);
            unpackHelperParameters0.Unpacker = unpacker;
            unpackHelperParameters0.UnpackingContext = unpackingContext;
            unpackHelperParameters0.Serializer = this._serializer1;
            unpackHelperParameters0.ItemsCount = itemsCount;
            unpackHelperParameters0.Unpacked = indexOfItem;
            unpackHelperParameters0.TargetObjectType = typeof(System.Collections.Generic.List<Stormancer.Dto.RouteDto>);
            unpackHelperParameters0.MemberName = "Routes";
            unpackHelperParameters0.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters0.DirectRead = null;
            unpackHelperParameters0.Setter = this.this_SetUnpackedValueOfRoutesDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters0);
        }
        
        private void SetUnpackedValueOfToken(Stormancer.Dto.ConnectToSceneMsg unpackingContext, string unpackedValue) {
            unpackingContext.Token = unpackedValue;
        }
        
        private void UnpackValueOfToken(MsgPack.Unpacker unpacker, Stormancer.Dto.ConnectToSceneMsg unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.ConnectToSceneMsg, string> unpackHelperParameters1 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.ConnectToSceneMsg, string>);
            unpackHelperParameters1.Unpacker = unpacker;
            unpackHelperParameters1.UnpackingContext = unpackingContext;
            unpackHelperParameters1.Serializer = this._serializer2;
            unpackHelperParameters1.ItemsCount = itemsCount;
            unpackHelperParameters1.Unpacked = indexOfItem;
            unpackHelperParameters1.TargetObjectType = typeof(string);
            unpackHelperParameters1.MemberName = "Token";
            unpackHelperParameters1.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters1.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
            unpackHelperParameters1.Setter = this.this_SetUnpackedValueOfTokenDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters1);
        }
        
        protected internal override Stormancer.Dto.ConnectToSceneMsg UnpackFromCore(MsgPack.Unpacker unpacker) {
            Stormancer.Dto.ConnectToSceneMsg result = default(Stormancer.Dto.ConnectToSceneMsg);
            result = new Stormancer.Dto.ConnectToSceneMsg();
            if (unpacker.IsArrayHeader) {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromArray(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Dto.ConnectToSceneMsg>(), this._memberNames, this._unpackOperationList);
            }
            else {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromMap(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Dto.ConnectToSceneMsg>(), this._unpackOperationTable);
            }
        }
    }
}
