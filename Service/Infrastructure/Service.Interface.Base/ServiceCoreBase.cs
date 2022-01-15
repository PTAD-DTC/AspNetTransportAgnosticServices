using System;
using System.Collections.Generic;
using System.Linq;

namespace Service.Interface.Base
{
    public abstract class ServiceCoreBase : IServiceBase
    {
        protected ServiceCoreBase(string apiVersion)
        {
            ApiVersion = apiVersion;
        }

        public string ApiVersion { get; }

        protected CallResult<T> Ok<T>(T result) =>
            new(result, ApiVersion);
        protected CallResult<T> NotFound<T>(string description = "Not found") =>
            new(ResultCode.NotFound, ApiVersion, description);
        protected CallResult<T> BadRequest<T>(string description = "Invalid request") =>
            new(ResultCode.BadRequest, ApiVersion, description);
        protected CallResult<T> Error<T>(string description = "Processing error") =>
            new(ResultCode.Error, ApiVersion, description);

        protected CallResult<IReadOnlyCollection<TResult>?> CallResult<TData, TResult>(
            IReadOnlyCollection<TData>? data,
            Func<IEnumerable<TData>,
                IEnumerable<TResult>> convert)
        {
            if (convert is null)
            {
                throw new ArgumentNullException(nameof(convert));
            }

            return data is null
                ? NotFound<IReadOnlyCollection<TResult>?>()
                : Ok<IReadOnlyCollection<TResult>?>(convert(data).ToArray());
        }

        protected CallResult<TResult?> CallResult<TData, TResult>(TData? data, Func<TData?, TResult?> convert)
        where TData : class
        where TResult : class
        {
            if (convert is null)
            {
                throw new ArgumentNullException(nameof(convert));
            }

            return data is null
                ? NotFound<TResult?>()
                : Ok(convert(data));
        }
    }
}
