using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liara.MessageHandlers;

namespace Liara.Security
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AllowAnonymous : LiaraMessageHandler
    {

    }
}
