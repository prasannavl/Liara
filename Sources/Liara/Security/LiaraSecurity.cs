using Liara.Common;

namespace Liara.Security
{
    public class LiaraSecurity : ILiaraSecurity
    {
        private ILiaraHashTable<string> claims;
        public object Id { get; set; }
        public bool IsAuthenticated { get; set; }

        public ILiaraHashTable<string> Claims
        {
            get { return claims ?? (claims = new LiaraHashTable<string>()); }
            set { claims = value; }
        }
    }
}