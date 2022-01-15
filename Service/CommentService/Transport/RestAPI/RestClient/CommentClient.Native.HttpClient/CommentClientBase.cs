using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Client.Base;
using CommentService.Interface;
using CommentService.Interface.Models.DTO;
using Service.Interface.Base;
using ServiceBaseClient.HttpClient.Base;

namespace CommentClient.Native.HttpClient
{
    public abstract class CommentClientBase: ServiceHttpClientBase, ICommentService
    {
        private const string ClientVersion = "1.0";
        private const string UserAgentPrefix = "commentsvc-nativehttpclient";

        private static readonly string ServicePathBase = CommentService.RestDefinitions.Definitions.ServicePathBase.ToUriComponent();
        private static readonly string RouteCommentBase = $"{ServicePathBase}/Comment";
        private static readonly string RouteSubjectComment = $"{RouteCommentBase}/subject";
        private static readonly string RouteComment = $"{RouteCommentBase}/comment";

        protected CommentClientBase(System.Net.Http.HttpClient httpClient, Uri serviceBaseUrl, string apiVersion, ICommunicationDebugListener? communicationListener = null)
            : base(httpClient, serviceBaseUrl, apiVersion, communicationListener)
        {
            UserAgentName = GetUserAgentName(ClientVersion);
        }

        protected CommentClientBase(Uri serviceBaseUrl, string apiVersion, ICommunicationDebugListener? communicationListener = null)
            : this(new System.Net.Http.HttpClient(), serviceBaseUrl, apiVersion, communicationListener)
        {
        }

        public Task<CallResult<IReadOnlyCollection<CommentDto>?>> GetSubjectComments(Guid subjectId, CancellationToken cancellationToken)
        {
            return CallGetServiceSafe<IReadOnlyCollection<CommentDto>?>(
                $"{RouteSubjectComment}/{subjectId}",
                cancellationToken);
        }

        public Task<CallResult<CommentDto?>> AddComment(Guid subjectId, CommentDataDto commentData, CancellationToken cancellationToken)
        {
            return CallPostServiceSafe<CommentDto?, CommentDataDto>(
                $"{RouteSubjectComment}/{subjectId}",
                commentData,
                cancellationToken);
        }

        public Task<CallResult<CommentDto?>> FindComment(Guid commentId, CancellationToken cancellationToken)
        {
            return CallGetServiceSafe<CommentDto?>(
                $"{RouteComment}/{commentId}",
                cancellationToken);
        }

        public Task<CallResult<CommentDto?>> UpdateComment(Guid commentId, CommentDataDto commentData, CancellationToken cancellationToken)
        {
            return CallPutServiceSafe<CommentDto?, CommentDataDto>(
                $"{RouteComment}/{commentId}",
                commentData,
                cancellationToken);
        }

        protected override string UserAgentName { get; }
        protected override string ResponseApiVersion => ServiceInfo.ApiVersion;

        private static string GetUserAgentName(string? clientVersion)
        {
            var clientVersionInfo = string.IsNullOrWhiteSpace(clientVersion) ? null : $"/{clientVersion}";
            return $"{UserAgentPrefix}{clientVersionInfo}";
        }
    }
}
