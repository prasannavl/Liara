// Author: Prasanna V. Loganathar
// Project: Liara
// Copyright (c) Launchark Technologies. All rights reserved.
// See License.txt in the project root for license information.
// 
// Created: 8:31 AM 15-02-2014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Liara.Helpers
{
    public static class ReflectionHelpers
    {
        public static Assembly LiaraAssembly = Assembly.GetExecutingAssembly();

        public static IEnumerable<Assembly> GetAllAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public static IEnumerable<Type> GetSubClassesOfType(Type type, bool includeParentIfPresent = false,
            bool includeAbstract = false)
        {
            return from assem in GetAllAssemblies()
                from t in assem.GetTypes()
                where
                    (includeParentIfPresent && t == type) ||
                    t.IsClass && t.IsSubclassOf(type) && (includeAbstract || !t.IsAbstract)
                select t;
        }

        public static IEnumerable<Type> GetSubClassesOfType(Type type, Assembly fromAssembly,
            bool includeParentIfPresent = false, bool includeAbstract = false)
        {
            return
                fromAssembly.GetTypes()
                    .Where(
                        t =>
                            (includeParentIfPresent && t == type) ||
                            t.IsClass && t.IsSubclassOf(type) && (includeAbstract || !t.IsAbstract));
        }

        public static IEnumerable<Type> GetSubClassesOfType(Type type, Type fromOuterType, bool includeOuterType = false,
            bool includeAbstract = false)
        {
            string innerClassNamePrefix = null;
            if (includeOuterType)
            {
                innerClassNamePrefix = fromOuterType.FullName;
            }
            else
            {
                innerClassNamePrefix = fromOuterType.FullName + "+";
            }
            return fromOuterType.Assembly.GetTypes()
                .Where(
                    t =>
                        (t.FullName.StartsWith(innerClassNamePrefix) && (t.IsClass && t.IsSubclassOf(type)) &&
                         (includeAbstract || !t.IsAbstract)));
        }

        public static IEnumerable<Type> GetClassesInsideType(Type outerType, bool includeOuterType = false,
            bool includeAbstract = false)
        {
            string innerClassNamePrefix = null;
            if (includeOuterType)
            {
                innerClassNamePrefix = outerType.FullName;
            }
            else
            {
                innerClassNamePrefix = outerType.FullName + "+";
            }
            return outerType.Assembly.GetTypes()
                .Where(
                    t =>
                        (t.FullName.StartsWith(innerClassNamePrefix) && (t.IsClass) &&
                         (includeAbstract || !t.IsAbstract)));
        }

        public static bool IsTypeAnonymous(Type type)
        {
            var hasCompilerGeneratedAttribute =
                type.GetCustomAttributes(typeof (CompilerGeneratedAttribute), false).Any();
            var nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            var isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return isAnonymousType;
        }

        public static IEnumerable<Type> GetImplementationsOfType(Type interfaceType, bool includeAbstract = false)
        {
            return
                GetAllAssemblies()
                    .SelectMany(
                        assembly =>
                            assembly.GetTypes()
                                .Where(
                                    t =>
                                        !t.IsInterface && interfaceType.IsAssignableFrom(t) &&
                                        (includeAbstract || !t.IsAbstract)));
        }

        public static IEnumerable<Type> GetImplementationsOfType(Type interfaceType, Type fromOuterType,
            bool includeOuterType = true, bool includeAbstract = false)
        {
            string innerClassNamePrefix = null;
            if (includeOuterType)
            {
                innerClassNamePrefix = fromOuterType.FullName;
            }
            else
            {
                innerClassNamePrefix = fromOuterType.FullName + "+";
            }
            return fromOuterType.Assembly.GetTypes()
                .Where(
                    t =>
                        (!t.IsInterface && t.FullName.StartsWith(innerClassNamePrefix) &&
                         (interfaceType.IsAssignableFrom(t)) && (includeAbstract || !t.IsAbstract)));
        }

        public static IEnumerable<Type> GetImplementationsOfType(Type interfaceType, Assembly fromAssembly,
            bool includeAbstract = false)
        {
            return
                fromAssembly.GetTypes()
                    .Where(
                        t => !t.IsInterface && interfaceType.IsAssignableFrom(t) && (includeAbstract || !t.IsAbstract));
        }

        public static IEnumerable<MethodInfo> GetMethods(Type fromType)
        {
            return fromType.GetMethods();
        }

        public static IEnumerable<MethodInfo> GetMethods(Type fromType, Type fromOuterType, bool includeOuterType = true)
        {
            return GetSubClassesOfType(fromType, fromOuterType, includeOuterType)
                .Select(t => t.GetMethods())
                .SelectMany(method => method);
        }

        public static IEnumerable<MethodInfo> GetMethods(Type fromType, Assembly fromAssembly,
            bool includeParentIfPresent = true)
        {
            return
                GetSubClassesOfType(fromType, fromAssembly, includeParentIfPresent)
                    .Select(t => t.GetMethods())
                    .SelectMany(method => method);
        }
    }
}