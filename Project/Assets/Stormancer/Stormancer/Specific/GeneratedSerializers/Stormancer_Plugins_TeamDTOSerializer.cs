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
    public class Stormancer_Plugins_TeamDTOSerializer : MsgPack.Serialization.MessagePackSerializer<Stormancer.Plugins.TeamDTO> {
        
        private MsgPack.Serialization.MessagePackSerializer<System.Collections.Generic.List<Stormancer.Plugins.PlayerDTO>> _serializer0;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Packer, Stormancer.Plugins.TeamDTO>> _packOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.TeamDTO>> _packOperationTable;
        
        private System.Collections.Generic.IDictionary<string, System.Func<Stormancer.Plugins.TeamDTO, bool>> _nullCheckersTable;
        
        private System.Action<Stormancer.Plugins.TeamDTO, System.Collections.Generic.List<Stormancer.Plugins.PlayerDTO>> this_SetUnpackedValueOfTeamDelegate;
        
        private System.Collections.Generic.IList<string> _memberNames;
        
        private System.Collections.Generic.IList<System.Action<MsgPack.Unpacker, Stormancer.Plugins.TeamDTO, int, int>> _unpackOperationList;
        
        private System.Collections.Generic.IDictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.TeamDTO, int, int>> _unpackOperationTable;
        
        public Stormancer_Plugins_TeamDTOSerializer(MsgPack.Serialization.SerializationContext context) : 
                base(context, (MsgPack.Serialization.SerializerCapabilities.PackTo | MsgPack.Serialization.SerializerCapabilities.UnpackFrom)) {
            MsgPack.Serialization.PolymorphismSchema schema0 = default(MsgPack.Serialization.PolymorphismSchema);
            schema0 = null;
            this._serializer0 = context.GetSerializer<System.Collections.Generic.List<Stormancer.Plugins.PlayerDTO>>(schema0);
            System.Action<MsgPack.Packer, Stormancer.Plugins.TeamDTO>[] packOperationList = default(System.Action<MsgPack.Packer, Stormancer.Plugins.TeamDTO>[]);
            packOperationList = new System.Action<MsgPack.Packer, Stormancer.Plugins.TeamDTO>[1];
            packOperationList[0] = new System.Action<MsgPack.Packer, Stormancer.Plugins.TeamDTO>(this.PackValueOfTeam);
            this._packOperationList = packOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.TeamDTO>> packOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.TeamDTO>>);
            packOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Packer, Stormancer.Plugins.TeamDTO>>(1);
            packOperationTable["Team"] = new System.Action<MsgPack.Packer, Stormancer.Plugins.TeamDTO>(this.PackValueOfTeam);
            this._packOperationTable = packOperationTable;
            System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.TeamDTO, bool>> nullCheckerTable = default(System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.TeamDTO, bool>>);
            nullCheckerTable = new System.Collections.Generic.Dictionary<string, System.Func<Stormancer.Plugins.TeamDTO, bool>>(1);
            nullCheckerTable["Team"] = new System.Func<Stormancer.Plugins.TeamDTO, bool>(this.IsTeamNull);
            this._nullCheckersTable = nullCheckerTable;
            System.Action<MsgPack.Unpacker, Stormancer.Plugins.TeamDTO, int, int>[] unpackOperationList = default(System.Action<MsgPack.Unpacker, Stormancer.Plugins.TeamDTO, int, int>[]);
            unpackOperationList = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.TeamDTO, int, int>[1];
            unpackOperationList[0] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.TeamDTO, int, int>(this.UnpackValueOfTeam);
            this._unpackOperationList = unpackOperationList;
            System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.TeamDTO, int, int>> unpackOperationTable = default(System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.TeamDTO, int, int>>);
            unpackOperationTable = new System.Collections.Generic.Dictionary<string, System.Action<MsgPack.Unpacker, Stormancer.Plugins.TeamDTO, int, int>>(1);
            unpackOperationTable["Team"] = new System.Action<MsgPack.Unpacker, Stormancer.Plugins.TeamDTO, int, int>(this.UnpackValueOfTeam);
            this._unpackOperationTable = unpackOperationTable;
            this._memberNames = new string[] {
                    "Team"};
            this.this_SetUnpackedValueOfTeamDelegate = new System.Action<Stormancer.Plugins.TeamDTO, System.Collections.Generic.List<Stormancer.Plugins.PlayerDTO>>(this.SetUnpackedValueOfTeam);
        }
        
        private void PackValueOfTeam(MsgPack.Packer packer, Stormancer.Plugins.TeamDTO objectTree) {
            this._serializer0.PackTo(packer, objectTree.Team);
        }
        
        private bool IsTeamNull(Stormancer.Plugins.TeamDTO objectTree) {
            return (objectTree.Team == null);
        }
        
        protected internal override void PackToCore(MsgPack.Packer packer, Stormancer.Plugins.TeamDTO objectTree) {
            MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.TeamDTO> packHelperParameters = default(MsgPack.Serialization.PackToArrayParameters<Stormancer.Plugins.TeamDTO>);
            packHelperParameters.Packer = packer;
            packHelperParameters.Target = objectTree;
            packHelperParameters.Operations = this._packOperationList;
            MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.TeamDTO> packHelperParameters0 = default(MsgPack.Serialization.PackToMapParameters<Stormancer.Plugins.TeamDTO>);
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
        
        private void SetUnpackedValueOfTeam(Stormancer.Plugins.TeamDTO unpackingContext, System.Collections.Generic.List<Stormancer.Plugins.PlayerDTO> unpackedValue) {
            unpackingContext.Team = unpackedValue;
        }
        
        private void UnpackValueOfTeam(MsgPack.Unpacker unpacker, Stormancer.Plugins.TeamDTO unpackingContext, int indexOfItem, int itemsCount) {
            MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.TeamDTO, System.Collections.Generic.List<Stormancer.Plugins.PlayerDTO>> unpackHelperParameters = default(MsgPack.Serialization.UnpackReferenceTypeValueParameters<Stormancer.Plugins.TeamDTO, System.Collections.Generic.List<Stormancer.Plugins.PlayerDTO>>);
            unpackHelperParameters.Unpacker = unpacker;
            unpackHelperParameters.UnpackingContext = unpackingContext;
            unpackHelperParameters.Serializer = this._serializer0;
            unpackHelperParameters.ItemsCount = itemsCount;
            unpackHelperParameters.Unpacked = indexOfItem;
            unpackHelperParameters.TargetObjectType = typeof(System.Collections.Generic.List<Stormancer.Plugins.PlayerDTO>);
            unpackHelperParameters.MemberName = "Team";
            unpackHelperParameters.NilImplication = MsgPack.Serialization.NilImplication.MemberDefault;
            unpackHelperParameters.DirectRead = null;
            unpackHelperParameters.Setter = this.this_SetUnpackedValueOfTeamDelegate;
            MsgPack.Serialization.UnpackHelpers.UnpackReferenceTypeValue(ref unpackHelperParameters);
        }
        
        protected internal override Stormancer.Plugins.TeamDTO UnpackFromCore(MsgPack.Unpacker unpacker) {
            Stormancer.Plugins.TeamDTO result = default(Stormancer.Plugins.TeamDTO);
            result = new Stormancer.Plugins.TeamDTO();
            if (unpacker.IsArrayHeader) {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromArray(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.TeamDTO>(), this._memberNames, this._unpackOperationList);
            }
            else {
                return MsgPack.Serialization.UnpackHelpers.UnpackFromMap(unpacker, result, MsgPack.Serialization.UnpackHelpers.GetIdentity<Stormancer.Plugins.TeamDTO>(), this._unpackOperationTable);
            }
        }
    }
}

