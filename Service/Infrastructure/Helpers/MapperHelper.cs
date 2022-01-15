using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Helpers
{
    public static class MapperHelper
    {
        public static IEnumerable<TResult> MapTo<TData, TResult>(
            this IEnumerable<TData> data, 
            Func<TData, TResult> converter)
        {
            Debug.Assert(converter != null, $"{nameof(converter)} is null");

            if (data is null)
            {
                yield break;
            }

            foreach (var d in data)
            {
                var mapped = converter(d);
                if (mapped is { })
                {
                    yield return mapped;
                }
            }
        }
    }
}
