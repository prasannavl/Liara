using Liara.Common;

namespace Liara.Security
{
    public class LiaraSecurity : ILiaraSecurity
    {
        private ILiaraHashTable<object> claims;
        public object Id { get; set; }
        public bool IsAuthenticated { get; set; }

        public ILiaraHashTable<object> Claims
        {
            get { return claims ?? (claims = new LiaraHashTable<object>()); }
            set { claims = value; }
        }
    }
}