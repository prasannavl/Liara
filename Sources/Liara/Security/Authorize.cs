// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 3:06 AM 07-03-2014

using System;
using System.Linq;
using System.Threading.Tasks;
using Liara.MessageHandlers;

namespace Liara.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class Authorize : LiaraMessageHandler
    {
        public Authorize()
        {
        }

        public Authorize(string claim)
        {
            Claims = new[] {new[] {claim}};
        }

        public Authorize(string claim, string value)
        {
            Claims = new[] {new[] {claim, value}};
        }

        public Authorize(string[][] claims)
        {
            Claims = claims;
        }

        private string[][] Claims { get; set; }

        public void Verify()
        {
            if (Claims != null)
            {
                if (Claims.Any(claim => claim.Length > 2 || claim.Length < 1))
                {
                    throw new ArgumentException(
                        "Invalid claim values. Format: [ { claim, (value) }, { claim, (value) } ]");
                }
            }
        }

        public override Task ProcessAsync(ILiaraContext context)
        {
            var res = context.Engine.Configuration.AuthenticationHandler.Authenticate(context);

            if (res)
            {
                if (Claims != null)
                {
                    foreach (var claim in Claims)
                    {
                        var exists = context.Security.Claims.ContainsKey(claim[0]);
                        if (exists)
                        {
                            if (claim.Length > 1)
                            {
                                if (!context.Security.Claims[claim[0]].Contains(claim[1]))
                                {
                                    res = false;
                                    break;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }

                        res = false;
                        break;
                    }
                }
            }

            if (res)
            {
                context.Security.IsAuthenticated = true;
                return base.ProcessAsync(context);
            }

            context.Response.Status = LiaraHttpStatus.Unauthorized;
            return TaskHelpers.Completed();
        }
    }
}