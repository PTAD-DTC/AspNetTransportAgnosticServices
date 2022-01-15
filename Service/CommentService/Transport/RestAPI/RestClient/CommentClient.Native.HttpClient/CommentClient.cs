using System;
using CommentService.Interface;
using JetBrains.Annotations;

namespace CommentClient.Native.HttpClient
{
    [PublicAPI]
    public sealed class CommentClient: CommentClientBase
    {
        public CommentClient(Uri serviceUrl)
            : base(serviceUrl, ServiceInfo.ApiVersion)
        {
        }

        public CommentClient(System.Net.Http.HttpClient httpClient, Uri serviceUrl)
            : base(httpClient, serviceUrl, ServiceInfo.ApiVersion)
        {
        }
    }
}
