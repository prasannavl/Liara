// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

namespace Liara.Constants
{
    // ReSharper disable InconsistentNaming
    public static class RequestHeaderConstants
    {
        public static string Accept = "Accept";
        public static string AcceptCharset = "Accept-Charset";
        public static string AcceptEncoding = "Accept-Encoding";
        public static string AcceptLanguage = "Accept-Language";
        public static string AcceptDatetime = "Accept-Datetime";
        public static string Authorization = "Authorization";
        public static string CacheControl = "Cache-Control";
        public static string Connection = "Connection";
        public static string Cookie = "Cookie";
        public static string ContentLength = "Content-Length";
        public static string ContentMD5 = "Content-MD5";
        public static string ContentType = "Content-Type";
        public static string Date = "Date";
        public static string Expect = "Expect";
        public static string From = "From";
        public static string Host = "Host";
        public static string IfMatch = "If-Match";
        public static string IfModifiedSince = "If-Modified-Since";
        public static string IfNoneMatch = "If-None-Match";
        public static string IfRange = "If-Range";
        public static string IfUnmodifiedSince = "If-Unmodified-Since";
        public static string MaxForwards = "Max-Forwards";
        public static string Origin = "Origin";
        public static string Pragma = "Pragma";
        public static string ProxyAuthorization = "Proxy-Authorization";
        public static string Range = "Range";
        public static string Referrer = "Referer";
        public static string TE = "TE";
        public static string Upgrade = "Upgrade";
        public static string UserAgent = "User-Agent";
        public static string Via = "Via";
        public static string Warning = "Warning";

        #region Non Standard Headers

        public static string XRequestedWith = "X-Requested-With";
        public static string DNT = "DNT";
        public static string XForwardedFor = "X-Forwarded-For";
        public static string XForwardedProto = "X-Forwarded-Proto";
        public static string FrontEndHttps = "Front-End-Https";
        public static string XATTDeviceId = "X-ATT-DeviceId";
        public static string XWapProfile = "X-Wap-Profile";
        public static string ProxyConnection = "Proxy-Connection";

        #endregion
    }
}