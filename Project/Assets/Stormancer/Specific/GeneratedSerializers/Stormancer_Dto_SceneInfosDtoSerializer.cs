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
    public class Stormancer_Dto_SceneInfosDtoSerializer : MsgPack.Serialization.MessagePackSerializer<Stormancer.Dto.SceneInfosDto> {
        
        private MsgPack.Serialization.MessagePackSerializer<string> _serializer0;
        
        private MsgPack.Serialization.MessagePackSerializer<System.Collections.Generic.Dictionary<string, string>> _serializer1;
        
        private MsgPack.Serialization.MessagePackSerializer<System.Collections.Generic.List<Stormancer.Dto.RouteDto>> _serializer2;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>> _packOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>> _packOperationTable;
        
        private System.Collections.Generic.IDictionary<string, System.Func<Stormancer.Dto.SceneInfosDto, bool>> _nullCheckersTable;
        
        private System.Action<Stormancer.Dto.SceneInfosDto, string> this_SetUnpackedValueOfSceneIdDelegate;
        
        private System.Func<MsgPack.Unpacker, System.Type, string, string> MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
        
        private System.Action<Stormancer.Dto.SceneInfosDto, System.Collections.Generic.Dictionary<string, string>> this_SetUnpackedValueOfMetadataDelegate;
        
        private System.Action<Stormancer.Dto.SceneInfosDto, System.Collections.Generic.List<Stormancer.Dto.RouteDto>> this_SetUnpackedValueOfRoutesDelegate;
        
        private System.Action<Stormancer.Dto.SceneInfosDto, string> this_SetUnpackedValueOfSelectedSerializerDelegate;
        
        private System.Collections.Generic.IList<string> _memberNames;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>> _unpackOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>> _unpackOperationTable;
        
        public Stormancer_Dto_SceneInfosDtoSerializer(MsgPack.Serialization.SerializationContext context) : 
                base(context, (MsgPack.Serialization.SerializerCapabilities.PackTo | MsgPack.Serialization.SerializerCapabilities.UnpackFrom)) {
            MsgPack.Serialization.PolymorphismSchema schema0 = default(MsgPack.Serialization.PolymorphismSchema);
            schema0 = null;
            this._serializer0 = context.GetSerializer<string>(schema0);
            MsgPack.Serialization.PolymorphismSchema schema1 = default(MsgPack.Serialization.PolymorphismSchema);
            schema1 = null;
            this._serializer1 = context.GetSerializer<System.Collections.Generic.Dictionary<string, string>>(schema1);
            MsgPack.Serialization.PolymorphismSchema schema2 = default(MsgPack.Serialization.PolymorphismSchema);
            schema2 = null;
            this._serializer2 = context.GetSerializer<System.Collections.Generic.List<Stormancer.Dto.RouteDto>>(schema2);
            System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>[] packOperationList = default(System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>[]);
            packOperationList = new System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>[4];
            packOperationList[0] = new System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>(this.PackValueOfSceneId);
            packOperationList[1] = new System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>(this.PackValueOfMetadata);
            packOperationList[2] = new System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>(this.PackValueOfRoutes);
            packOperationList[3] = new System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>(this.PackValueOfSelectedSerializer);
            this._packOperationList = packOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>> packOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>>);
            packOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>>(4);
            packOperationTable["SceneId"] = new System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>(this.PackValueOfSceneId);
            packOperationTable["Metadata"] = new System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>(this.PackValueOfMetadata);
            packOperationTable["Routes"] = new System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>(this.PackValueOfRoutes);
            packOperationTable["SelectedSerializer"] = new System.Action<MsgPack.Packer, Stormancer.Dto.SceneInfosDto>(this.PackValueOfSelectedSerializer);
            this._packOperationTable = packOperationTable;
            System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Dto.SceneInfosDto, bool>> nullCheckerTable = default(System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Dto.SceneInfosDto, bool>>);
            nullCheckerTable = new System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Dto.SceneInfosDto, bool>>(4);
            nullCheckerTable["SceneId"] = new System.Func<Stormancer.Dto.SceneInfosDto, bool>(this.IsSceneIdNull);
            nullCheckerTable["Metadata"] = new System.Func<Stormancer.Dto.SceneInfosDto, bool>(this.IsMetadataNull);
            nullCheckerTable["Routes"] = new System.Func<Stormancer.Dto.SceneInfosDto, bool>(this.IsRoutesNull);
            nullCheckerTable["SelectedSerializer"] = new System.Func<Stormancer.Dto.SceneInfosDto, bool>(this.IsSelectedSerializerNull);
            this._nullCheckersTable = nullCheckerTable;
            System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>[] unpackOperationList = default(System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>[]);
            unpackOperationList = new System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>[4];
            unpackOperationList[0] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>(this.UnpackValueOfSceneId);
            unpackOperationList[1] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>(this.UnpackValueOfMetadata);
            unpackOperationList[2] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>(this.UnpackValueOfRoutes);
            unpackOperationList[3] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>(this.UnpackValueOfSelectedSerializer);
            this._unpackOperationList = unpackOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>> unpackOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>>);
            unpackOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>>(4);
            unpackOperationTable["SceneId"] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>(this.UnpackValueOfSceneId);
            unpackOperationTable["Metadata"] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>(this.UnpackValueOfMetadata);
            unpackOperationTable["Routes"] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>(this.UnpackValueOfRoutes);
            unpackOperationTable["SelectedSerializer"] = new System.Action<MsgPack.Unpacker, Stormancer.Dto.SceneInfosDto, int, int>(this.UnpackValueOfSelectedSerializer);
            this._unpackOperationTable = unpackOperationTable;
            this._memberNames = new string[] {
                    "SceneId",
                    "Metadata",
                    "Routes",
                    "SelectedSerializer"};
            this.this_SetUnpackedValueOfSceneIdDelegate = new System.Action<Stormancer.Dto.SceneInfosDto, string>(this.SetUnpackedValueOfSceneId);
            this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate = new System.Func<MsgPack.Unpacker, System.Type, string, string>(MsgPack.Serialization.UnpackHelpers.UnpackStringValue);
            this.this_SetUnpackedValueOfMetadataDelegate = new System.Action<Stormancer.Dto.SceneInfosDto, System.Collections.Generic.Dictionary<string, string>>(this.SetUnpackedValueOfMetadata);
            this.this_SetUnpackedValueOfRoutesDelegate = new System.Action<Stormancer.Dto.SceneInfosDto, System.Collections.Generic.List<Stormancer.Dto.RouteDto>>(this.SetUnpackedValueOfRoutes);
            this.this_SetUnpackedValueOfSelectedSerializerDelegate = new System.Action<Stormancer.Dto.SceneInfosDto, string>(this.SetUnpackedValueOfSelectedSerializer);
        }
        
        private void PackValueOfSceneId(MsgPack.Packer packer, Stormancer.Dto.SceneInfosDto objectTree) {
            this._serializer0.PackTo(packer, objectTree.SceneId);
        }
        
        private bool IsSceneIdNull(Stormancer.Dto.SceneInfosDto objectTree) {
            return (objectTree.SceneId == null);
        }
        
        private void PackValueOfMetadata(MsgPack.Packer packer, Stormancer.Dto.SceneInfosDto objectTree) {
            this._serializer1.PackTo(packer, objectTree.Metadata);
        }
        
        private bool IsMetadataNull(Stormancer.Dto.SceneInfosDto objectTree) {
            return (objectTree.Metadata == null);
        }
        
        private void PackValueOfRoutes(MsgPack.Packer packer, Stormancer.Dto.SceneInfosDto objectTree) {
            this._serializer2.PackTo(packer, objectTree.Routes);
        }
        
        private bool IsRoutesNull(Stormancer.Dto.SceneInfosDto objectTree) {
            return (objectTree.Routes == null);
        }
        
        private void PackValueOfSelectedSerializer(MsgPack.Packer packer, Stormancer.Dto.SceneInfosDto objectTree) {
            this._serializer0.PackTo(packer, objectTree.SelectedSerializer);
        }
        
        private bool IsSelectedSerializerNull(Stormancer.Dto.SceneInfosDto objectTree) {
            return (objectTree.SelectedSerializer == null);
        }
        
        protected internal override void PackToCore(MsgPack.Packer packer, Stormancer.Dto.SceneInfosDto objectTree) {
            MsgPack.Serialization.PackToArrayParameters<Stormancer.Dto.SceneInfosDto> packHelperParameters = default(MsgPack.Serialization.PackToArrayParameters<Stormancer.Dto.SceneInfosDto>);
            packHelperParameters.Packer = packer;
            packHelperParameters.Target = objectTree;
            packHelperParameters.Operations = this._packOperationList;
            MsgPack.Serialization.PackToMapParameters<Stormancer.Dto.SceneInfosDto> packHelperParameters0 = default(MsgPack.Serialization.PackToMapParameters<Stormancer.Dto.SceneInfosDto>);
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
        
        private void SetUnpackedValueOfSceneId(Stormancer.Dto.SceneInfosDto unpackingContext, string unpackedValue) {
            unpackingContext.SceneId = unpackedValue;
        }
        
        private void UnpackValueOfSceneId(MsgPack.Unpacker unpacker, Stormancer.Dto.SceneInfosDto unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.SceneInfosDto, string> unpackHelperParameters = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.SceneInfosDto, string>);
            unpackHelperParameters.Unpacker = unpacker;
            unpackHelperParameters.UnpackingContext = unpackingContext;
            unpackHelperParameters.Serializer = this._serializer0;
            unpackHelperParameters.ItemsCount = itemsCount;
            unpackHelperParameters.Unpacked = indexOfItem;
            unpackHelperParameters.TargetObjectType = typeof(string);
            unpackHelperParameters.MemberName = "SceneId";
            unpackHelperParameters.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
            unpackHelperParameters.Setter = this.this_SetUnpackedValueOfSceneIdDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters);
        }
        
        private void SetUnpackedValueOfMetadata(Stormancer.Dto.SceneInfosDto unpackingContext, System.Collections.Generic.Dictionary<string, string> unpackedValue) {
            unpackingContext.Metadata = unpackedValue;
        }
        
        private void UnpackValueOfMetadata(MsgPack.Unpacker unpacker, Stormancer.Dto.SceneInfosDto unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.SceneInfosDto, System.Collections.Generic.Dictionary<string, string>> unpackHelperParameters0 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.SceneInfosDto, System.Collections.Generic.Dictionary<string, string>>);
            unpackHelperParameters0.Unpacker = unpacker;
            unpackHelperParameters0.UnpackingContext = unpackingContext;
            unpackHelperParameters0.Serializer = this._serializer1;
            unpackHelperParameters0.ItemsCount = itemsCount;
            unpackHelperParameters0.Unpacked = indexOfItem;
            unpackHelperParameters0.TargetObjectType = typeof(System.Collections.Generic.Dictionary<string, string>);
            unpackHelperParameters0.MemberName = "Metadata";
            unpackHelperParameters0.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters0.DirectRead = null;
            unpackHelperParameters0.Setter = this.this_SetUnpackedValueOfMetadataDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters0);
        }
        
        private void SetUnpackedValueOfRoutes(Stormancer.Dto.SceneInfosDto unpackingContext, System.Collections.Generic.List<Stormancer.Dto.RouteDto> unpackedValue) {
            unpackingContext.Routes = unpackedValue;
        }
        
        private void UnpackValueOfRoutes(MsgPack.Unpacker unpacker, Stormancer.Dto.SceneInfosDto unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.SceneInfosDto, System.Collections.Generic.List<Stormancer.Dto.RouteDto>> unpackHelperParameters1 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.SceneInfosDto, System.Collections.Generic.List<Stormancer.Dto.RouteDto>>);
            unpackHelperParameters1.Unpacker = unpacker;
            unpackHelperParameters1.UnpackingContext = unpackingContext;
            unpackHelperParameters1.Serializer = this._serializer2;
            unpackHelperParameters1.ItemsCount = itemsCount;
            unpackHelperParameters1.Unpacked = indexOfItem;
            unpackHelperParameters1.TargetObjectType = typeof(System.Collections.Generic.List<Stormancer.Dto.RouteDto>);
            unpackHelperParameters1.MemberName = "Routes";
            unpackHelperParameters1.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters1.DirectRead = null;
            unpackHelperParameters1.Setter = this.this_SetUnpackedValueOfRoutesDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters1);
        }
        
        private void SetUnpackedValueOfSelectedSerializer(Stormancer.Dto.SceneInfosDto unpackingContext, string unpackedValue) {
            unpackingContext.SelectedSerializer = unpackedValue;
        }
        
        private void UnpackValueOfSelectedSerializer(MsgPack.Unpacker unpacker, Stormancer.Dto.SceneInfosDto unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.SceneInfosDto, string> unpackHelperParameters2 = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Dto.SceneInfosDto, string>);
            unpackHelperParameters2.Unpacker = unpacker;
            unpackHelperParameters2.UnpackingContext = unpackingContext;
            unpackHelperParameters2.Serializer = this._serializer0;
            unpackHelperParameters2.ItemsCount = itemsCount;
            unpackHelperParameters2.Unpacked = indexOfItem;
            unpackHelperParameters2.TargetObjectType = typeof(string);
            unpackHelperParameters2.MemberName = "SelectedSerializer";
            unpackHelperParameters2.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters2.DirectRead = this.MsgPack_Serialization_UnpackHelpers_UnpackStringValueDelegate;
            unpackHelperParameters2.Setter = this.this_SetUnpackedValueOfSelectedSerializerDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters2);
        }
        
        protected internal override Stormancer.Dto.SceneInfosDto UnpackFromCore(MsgPack.Unpacker unpacker) {
            Stormancer.Dto.SceneInfosDto result = default(Stormancer.Dto.SceneInfosDto);
            result = new Stormancer.Dto.SceneInfosDto();
            if (unpacker.IsArrayHeader) {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromArray(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Dto.SceneInfosDto>(), this._memberNames, this._unpackOperationList);
            }
            else {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromMap(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Dto.SceneInfosDto>(), this._unpackOperationTable);
            }
        }
    }
}

