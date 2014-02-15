// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

namespace Liara.Common
{
    public class LiaraStatusHandler : ILiaraStatusHandler
    {
        private int priority = -1;

        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        // TODO: Handle all common status messages, with reasonable defaults.
        public bool HandleStatus(ILiaraContext context)
        {
            switch (context.Response.Status.Code)
            {
                case 200:
                {
                    return false;
                }
                case 400:
                {
                    HandleBadRequest(context);
                    return true;
                }
                case 404:
                {
                    HandleNotFound(context);
                    return true;
                }
                case 405:
                {
                    HandleMethodNotAllowed(context);
                    return true;
                }
                case 500:
                {
                    HandleInternalServerError(context);
                    return true;
                }
            }
            return false;
        }

        //TODO : Add error-data structures.

        private void HandleBadRequest(ILiaraContext context)
        {
            context.Response.Content = "Bad request.";
        }

        private void HandleMethodNotAllowed(ILiaraContext context)
        {
            context.Response.Content = "Method not allowed.";
        }

        private void HandleInternalServerError(ILiaraContext context)
        {
            context.Response.Content = "Internal server error.";
        }

        private void HandleNotFound(ILiaraContext context)
        {
            context.Response.Content = "Not found.";
        }
    }
}