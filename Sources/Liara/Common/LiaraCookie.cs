// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 5:33 AM 13-02-2014

using System;

namespace Liara.Common
{
    public class LiaraCookie
    {
        public LiaraCookie()
        {
            HttpOnly = true;
            Path = "/";
        }

        public LiaraCookie(string name, string value, DateTime? expires = null, string domain = null, string path = "/",
            bool httpOnly = true, bool secureOnly = false)
        {
            Name = name;
            Value = value;
            Expires = expires;
            HttpOnly = httpOnly;
            SecureOnly = secureOnly;
            Domain = domain;
            Path = path;
        }

        /// <summary>
        ///     Name of the cookie.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Value of the cookie.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     Gets or sets the domain to associate the cookie with.
        /// </summary>
        /// <returns>The domain to associate the cookie with.</returns>
        public string Domain { get; set; }

        /// <summary>
        ///     Gets or sets the cookie path.
        /// </summary>
        /// <returns>The cookie path.</returns>
        public string Path { get; set; }

        /// <summary>
        ///     Gets or sets the expiration date and time for the cookie.
        /// </summary>
        /// <returns>The expiration date and time for the cookie.</returns>
        public DateTime? Expires { get; set; }

        /// <summary>
        ///     Gets or sets a value that indicates whether to transmit the cookie using Secure Sockets Layer (SSL)—that is, over
        ///     HTTPS only.
        /// </summary>
        /// <returns>True to transmit the cookie only over an SSL connection (HTTPS); otherwise, false.</returns>
        public bool SecureOnly { get; set; }

        /// <summary>
        ///     Gets or sets a value that indicates whether a cookie is accessible by client-side script.
        /// </summary>
        /// <returns>True if a cookie is accessible by client-side script; otherwise, false.</returns>
        public bool HttpOnly { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Domain != null ? Domain.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Path != null ? Path.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Expires.GetHashCode();
                hashCode = (hashCode*397) ^ SecureOnly.GetHashCode();
                hashCode = (hashCode*397) ^ HttpOnly.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        ///     Checks if it represents the same cookie, regardless of the value.
        ///     <para>Note: Use Equals to check value.</para>
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public bool IsSameCookie(LiaraCookie cookie)
        {
            if (cookie == null) return false;
            return cookie.Name == Name && cookie.Domain == Domain && cookie.Path == Path;
        }

        public bool IsSameCookieAndValue(LiaraCookie cookie)
        {
            if (cookie == null) return false;
            return IsSameCookie(cookie) && Value == cookie.Value;
        }

        public bool IsInSameDomain(LiaraCookie cookie)
        {
            if (cookie == null) return false;
            return cookie.Domain == Domain;
        }

        public bool IsInSamePath(LiaraCookie cookie)
        {
            if (cookie == null) return false;
            return cookie.Domain == Domain && cookie.Path == Path;
        }

        public void SetAsExpired()
        {
            Expires = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var cookie = obj as LiaraCookie;
            if (cookie == null) return false;
            return Equals(cookie);
        }

        protected bool Equals(LiaraCookie cookie)
        {
            return string.Equals(Name, cookie.Name) && string.Equals(Value, cookie.Value) &&
                   string.Equals(Domain, cookie.Domain) && string.Equals(Path, cookie.Path) &&
                   Expires.Equals(cookie.Expires) && SecureOnly.Equals(cookie.SecureOnly) &&
                   HttpOnly.Equals(cookie.HttpOnly);
        }

        public static bool operator ==(LiaraCookie a, LiaraCookie b)
        {
            if ((a == null) || (b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(LiaraCookie a, LiaraCookie b)
        {
            return !(a == b);
        }
    }
}