// Author: Prasanna V. Loganathar
// Project: Liara.Hosting.Owin
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

namespace Liara.Constants
{
    public static class OwinConstants
    {
        public static string Version = "owin.Version";
        public static string CallCancelled = "owin.CallCancelled";

        public static string RequestBody = "owin.RequestBody";
        public static string RequestHeaders = "owin.RequestHeaders";
        public static string RequestScheme = "owin.RequestScheme";
        public static string RequestMethod = "owin.RequestMethod";
        public static string RequestPathBase = "owin.RequestPathBase";
        public static string RequestPath = "owin.RequestPath";
        public static string RequestQueryString = "owin.RequestQueryString";
        public static string RequestProtocol = "owin.RequestProtocol";

        public static string ResponseStatusCode = "owin.ResponseStatusCode";
        public static string ResponseReasonPhrase = "owin.ResponseReasonPhrase";
        public static string ResponseHeaders = "owin.ResponseHeaders";
        public static string ResponseBody = "owin.ResponseBody";
        public static string ResponseProtocol = "owin.ResponseProtocol";

        public static string ServerCapabilities = "server.Capabilities";
        public static string ServerName = "server.Name";
        public static string ServerRemoteIpAddress = "server.RemoteIpAddress";
        public static string ServerRemotePort = "server.RemotePort";
        public static string ServerLocalIpAddress = "server.LocalIpAddress";
        public static string ServerLocalPort = "server.LocalPort";
        public static string ServerIsLocal = "server.IsLocal";

        public static string HostAddresses = "host.Addresses";
        public static string HostAppName = "host.AppName";
        public static string HostOnAppDisposing = "host.OnAppDisposing";
        public static string HostTraceOutput = "host.TraceOutput";

        public static string SslClientCertifiate = "ssl.ClientCertificate";
        public static string SslLoadClientCertAsync = "ssl.LoadClientCertAsync";

        public static string ServerOnSendingHeaders = "server.OnSendingHeaders";
        public static string ServerLoggerFactory = "server.LoggerFactory";
        public static string ServerUser = "server.User";

        public static class OpaqueConstants
        {
            public static string Version = "opaque.Version";
            public static string Upgrade = "opaque.Upgrade";
            public static string Stream = "opaque.Stream";
            public static string CallCancelled = "opaque.CallCancelled";
        }

        public static class Security
        {
            public static string Authenticate = "security.Authenticate";
            public static string SignIn = "security.SignIn";
            public static string SignOut = "security.SignOut";
            public static string Challenge = "security.Challenge";
        }

        public static class SendFiles
        {
            public static string Version = "sendfile.Version";
            public static string Support = "sendfile.Support";
            public static string Concurrency = "sendfile.Concurrency";
            public static string SendAsync = "sendfile.SendAsync";
        }

        public static class WebSockets
        {
            public static string WebSocketAccept = "websocket.Accept";
            public static string WebSocketSubProtocol = "websocket.SubProtocol";
            public static string WebSocketSendAsync = "websocket.SendAsync";
            public static string WebSocketReceiveAync = "websocket.ReceiveAsync";
            public static string WebSocketCloseAsync = "websocket.CloseAsync";
            public static string WebSocketCallCancelled = "websocket.CallCancelled";
            public static string WebSocketVersion = "websocket.Version";
            public static string WebSocketCloseStatus = "websocket.ClientCloseStatus";
            public static string WebSocketCloseDescription = "websocket.ClientCloseDescription";
        }
    }
}