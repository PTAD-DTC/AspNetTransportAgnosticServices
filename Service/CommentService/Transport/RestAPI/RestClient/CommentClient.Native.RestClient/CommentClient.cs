using System;
using CommentService.Interface;
using JetBrains.Annotations;
using RestSharp;

namespace CommentClient.Native.RestClient
{
    [PublicAPI]
    public sealed class CommentClient: CommentClientBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CommentClient(IRestClient restClient)
            : base(restClient, ServiceInfo.ApiVersion)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CommentClient(Uri serviceBaseUrl)
            : base(serviceBaseUrl, ServiceInfo.ApiVersion)
        {
        }
    }
}
