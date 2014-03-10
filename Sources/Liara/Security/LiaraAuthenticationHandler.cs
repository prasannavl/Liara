namespace Liara.Security
{
    public class LiaraAuthenticationHandler : ILiaraAuthenticationHandler
    {
        private int priority = Constants.LiaraServiceConstants.PriorityLow;

        public virtual bool Authenticate(ILiaraContext context)
        {
            return false;
        }

        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }
    }
}