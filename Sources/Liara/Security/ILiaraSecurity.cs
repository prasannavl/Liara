using Liara.Common;

namespace Liara.Security
{
    public interface ILiaraSecurity
    {
        object Id { get; set; }
        bool IsAuthenticated { get; set; }
        ILiaraHashTable<string> Claims { get; set; }
    }
}