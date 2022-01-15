using System;
using System.Collections.Generic;
using System.Linq;
using CommentService.Interface.Models.DTO;
using ForecastService.Services.BusinessLogic.Model;
using Helpers;

namespace ForecastService.CommentService
{
    internal static class Extensions
    {
        public static CommentDataDto Map(this CommentData commentData) =>
            new CommentDataDto(commentData.Comment);

        public static CommentData Map(this CommentDataDto commentData) =>
            new CommentData(commentData.Comment ?? string.Empty);

        public static CommentData? MapSafe(this CommentDataDto? commentData) =>
            commentData?.Map();

        public static ForecastComment Map(this CommentDto comment) =>
            new ForecastComment(
                comment.Id,
                comment.CommentSubjectId,
                comment.CommentData.MapSafe() ?? throw new InvalidOperationException($"Unable to map {nameof(comment.CommentData)}"));

        public static ForecastComment? MapSafe(this CommentDto? comment) =>
            comment is null || comment.CommentData is null ? null : comment.Map();

        public static IReadOnlyCollection<ForecastComment>? MapSafe(this IReadOnlyCollection<CommentDto>? data) =>
            data?.MapTo(Map).ToArray();

    }
}
