using Liara.Common;

namespace Liara.Security
{
    public interface ILiaraAuthenticationHandler : ILiaraPrioritizedService
    {
        bool Authenticate(ILiaraContext context);
    }
}