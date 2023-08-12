// This file was generated by a tool; you should avoid making direct changes.
// Consider using 'partial classes' to extend these types
// Input: Battle.proto

#pragma warning disable CS1591, CS0612, CS3021, IDE1006
namespace Proto.Battle
{

    [global::ProtoBuf.ProtoContract(Name = @"whole")]
    public partial class Whole : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"id")]
        [global::System.ComponentModel.DefaultValue("")]
        public string Id { get; set; } = "";

        [global::ProtoBuf.ProtoMember(2, Name = @"type")]
        public int Type { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"subtype")]
        public int Subtype { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public int rightFormation { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public int leftFormation { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public bool isPVP { get; set; }

        [global::ProtoBuf.ProtoMember(7)]
        public bool isReplay { get; set; }

        [global::ProtoBuf.ProtoMember(8, Name = @"fighters")]
        public global::System.Collections.Generic.List<Fighter> Fighters { get; } = new global::System.Collections.Generic.List<Fighter>();

    }

    [global::ProtoBuf.ProtoContract(Name = @"fighter")]
    public partial class Fighter : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"id")]
        public int Id { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"type")]
        public int Type { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"side")]
        public int Side { get; set; }

        [global::ProtoBuf.ProtoMember(4, Name = @"name")]
        [global::System.ComponentModel.DefaultValue("")]
        public string Name { get; set; } = "";

        [global::ProtoBuf.ProtoMember(5, Name = @"shape")]
        public int Shape { get; set; }

        [global::ProtoBuf.ProtoMember(6, Name = @"grade")]
        public int Grade { get; set; }

        [global::ProtoBuf.ProtoMember(7, Name = @"prof")]
        public int Prof { get; set; }

        [global::ProtoBuf.ProtoMember(8, Name = @"position")]
        public int Position { get; set; }

        [global::ProtoBuf.ProtoMember(9, Name = @"status")]
        public int Status { get; set; }

        [global::ProtoBuf.ProtoMember(10)]
        public int playerID { get; set; }

        [global::ProtoBuf.ProtoMember(11)]
        public int operatorID { get; set; }

        [global::ProtoBuf.ProtoMember(12)]
        public int masterID { get; set; }

        [global::ProtoBuf.ProtoMember(13)]
        public global::System.Collections.Generic.List<Skill> activeSkills { get; } = new global::System.Collections.Generic.List<Skill>();

    }

    [global::ProtoBuf.ProtoContract(Name = @"skill")]
    public partial class Skill : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"id")]
        public int Id { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"level")]
        public int Level { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfBattleStart : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"data")]
        public Whole Data { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfEnterBattle : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"type")]
        public int Type { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"side")]
        public int Side { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfBattleEnd : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfRoundStart : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int roundNum { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfRoundCommand : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"millisecond")]
        public int Millisecond { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfRoundFight : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfRoundEnd : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"millisecond")]
        public int Millisecond { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfTurnStart : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int fighterID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfTurnEnd : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfCommandSkillStart : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int skillID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int targetID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfCommandSkillEnd : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfCommandAttackStart : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int targetID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfCommandAttackEnd : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfCommandUseItemStart : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int itemID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int targetID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfCommandUseItemEnd : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfCommandSummonStart : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int petID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfCommandSummonEnd : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfCommandEscapeStart : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfCommandEscapeEnd : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfActionSkillStart : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"id")]
        public int Id { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int fighterID { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public bool isCounter { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfActionSkillEnd : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfActionAttackStart : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"id")]
        public int Id { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int fighterID { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public bool isCounter { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfActionAttackEnd : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfActionUseItemStart : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"id")]
        public int Id { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int fighterID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfActionUseItemEnd : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfActionSummonStart : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"id")]
        public int Id { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int fighterID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfActionSummonEnd : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfActionEscapeStart : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"id")]
        public int Id { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int fighterID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfActionEscapeEnd : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ReqSetCommandSkill : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int fighterID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int skillID { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public int targetID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ReqSetCommandAttack : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int fighterID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int targetID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ReqSetCommandAI : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int fighterID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ReqSetCommandUseItem : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int fighterID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int itemID { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public int targetID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ReqSetCommandSummon : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int fighterID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int petID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ReqSetCommandDefend : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int fighterID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ReqSetCommandEscape : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int fighterID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ReqSetCommandProtect : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int fighterID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int targetID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfAssaultStart : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"id")]
        public int Id { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"type")]
        public int Type { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public int actionID { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfAssaultEnd : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfPerformInfo : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int skillID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int skillLevel { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public int swingIndex { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfPerformShout : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int skillID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int skillLevel { get; set; }

    }

    [global::ProtoBuf.ProtoContract(Name = @"victim")]
    public partial class Victim : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int fighterID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public bool isDodging { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public bool isCritical { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public bool isDefending { get; set; }

        [global::ProtoBuf.ProtoMember(5, Name = @"damage")]
        public int Damage { get; set; }

        [global::ProtoBuf.ProtoMember(6, Name = @"status")]
        public int Status { get; set; }

    }

    [global::ProtoBuf.ProtoContract(Name = @"protector")]
    public partial class Protector : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int fighterID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int protectedID { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"damage")]
        public int Damage { get; set; }

        [global::ProtoBuf.ProtoMember(4, Name = @"status")]
        public int Status { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfPerformRun : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int skillID { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int fighterID { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"victimList")]
        public global::System.Collections.Generic.List<Victim> victimLists { get; } = new global::System.Collections.Generic.List<Victim>();

        [global::ProtoBuf.ProtoMember(4, Name = @"protectorList")]
        public global::System.Collections.Generic.List<Protector> protectorLists { get; } = new global::System.Collections.Generic.List<Protector>();

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfAttackInfo : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfAttackRun : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int fighterID { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"target")]
        public Victim Target { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"protector_field")]
        public Protector ProtectorField { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class NtfPopUpNumber : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public int fighterID { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"type")]
        public int Type { get; set; }

        [global::ProtoBuf.ProtoMember(3, Name = @"value")]
        public int Value { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public bool isDown { get; set; }

    }

}

#pragma warning restore CS1591, CS0612, CS3021, IDE1006
