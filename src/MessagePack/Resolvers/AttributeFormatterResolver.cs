﻿// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Reflection;
using MessagePack.Formatters;
using MessagePack.Internal;

namespace MessagePack.Resolvers
{
    /// <summary>
    /// Get formatter from <see cref="MessagePackFormatterAttribute"/>.
    /// </summary>
    public sealed class AttributeFormatterResolver : IFormatterResolver
    {
        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly AttributeFormatterResolver Instance = new AttributeFormatterResolver();

        private AttributeFormatterResolver()
        {
        }

        public IMessagePackFormatter<T>? GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            internal static readonly IMessagePackFormatter<T>? Formatter;

            static FormatterCache()
            {
                MessagePackFormatterAttribute? attr = typeof(T).GetTypeInfo().GetCustomAttribute<MessagePackFormatterAttribute>();
                if (attr == null)
                {
                    return;
                }

                var formatterType = attr.FormatterType;
                if (formatterType.IsGenericType && !formatterType.IsConstructedGenericType)
                {
                    formatterType = formatterType.MakeGenericType(typeof(T).GetGenericArguments());
                }

                Formatter = (IMessagePackFormatter<T>)ResolverUtilities.ActivateFormatter(formatterType, attr.Arguments);
            }
        }
    }
}
