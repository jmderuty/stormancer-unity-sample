
namespace Stormancer
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct
        | System.AttributeTargets.Enum)]
    public class MsgPackDtoAttribute : System.Attribute
    {
        public string Filename;

        public MsgPackDtoAttribute()
        {
            Filename = "emptyName";
        }

        public MsgPackDtoAttribute(string filename)
        {
            Filename = filename;
        }

    }
}
