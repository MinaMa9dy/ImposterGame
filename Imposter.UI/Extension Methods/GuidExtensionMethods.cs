using System.Text.Json;

namespace Imposter.UI.Extension_Methods
{
    public static class GuidExtensions
    {
        public static Guid? TryParses(this string? str)
        {
            if (Guid.TryParse(str, out Guid guid))
            {
                return guid;
            }
            return null;
        }
        
    }
}
