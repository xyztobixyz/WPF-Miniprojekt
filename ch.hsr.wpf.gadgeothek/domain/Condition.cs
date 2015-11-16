using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ch.hsr.wpf.gadgeothek.domain
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Condition
    {
        [EnumMember(Value = "NEW")]
        New,
        [EnumMember(Value = "GOOD")]
        Good,
        [EnumMember(Value = "DAMAGED")]
        Damaged,
        [EnumMember(Value = "WASTE")]
        Waste,
        [EnumMember(Value = "LOST")]
        Lost
    }
}