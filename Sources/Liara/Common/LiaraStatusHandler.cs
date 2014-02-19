// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 12:49 PM 16-02-2014

namespace Liara.Common
{
    public class LiaraStatusHandler : ILiaraStatusHandler
    {
        private int priority = Constants.LiaraServiceConstants.PriorityLowest;

        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        public virtual bool HandleStatus(ILiaraContext context)
        {
            switch (context.Response.Status.Code)
            {
                case 200:
                {
                    return true;
                }
                default:
                {
                    SetContentAsErrorMessage(context);
                    return true;
                }
            }
        }

        public virtual void SetContentAsErrorMessage(ILiaraContext context)
        {
            if (context.Response.Content == null || context.Response.Content.GetType() != typeof(ErrorMessage))
            context.Response.Content = new ErrorMessage
            {
                ErrorCode = context.Response.Status.Code,
                Description = context.Response.Status.Description,
                Message = context.Response.Content
            };
        }
    }
}