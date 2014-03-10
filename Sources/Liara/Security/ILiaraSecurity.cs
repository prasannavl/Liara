using Liara.Common;

namespace Liara.Security
{
    public interface ILiaraSecurity
    {
        object Id { get; set; }
        bool IsAuthenticated { get; set; }
        ILiaraHashTable<object> Claims { get; set; }
    }
}