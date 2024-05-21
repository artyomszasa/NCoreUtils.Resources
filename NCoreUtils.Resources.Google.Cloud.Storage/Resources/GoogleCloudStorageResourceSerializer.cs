using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using NCoreUtils.Google;

namespace NCoreUtils.Resources;

public static class GoogleCloudStorageResourceSerializer
{
    public static class UriParameters
    {
        public const string ContentType = "ctype";

        public const string CacheControl = "cache";

        public const string Public = "public";

        public const string AccessToken = "access_token";
    }

    static readonly HashSet<string> _truthy = new(StringComparer.OrdinalIgnoreCase)
    {
        "true",
        "t",
        "on",
        "1"
    };

    private static ref SpanBuilder AppendQ(this ref SpanBuilder builder, scoped ref bool first, string name, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return ref builder;
        }
        if (first)
        {
            first = false;
            builder.Append('?');
        }
        else
        {
            builder.Append('&');
        }
        builder.Append(name);
        builder.Append('=');
#if NET6_0_OR_GREATER
        builder.AppendUriEscaped(value);
#else
        builder.Append(Uri.EscapeDataString(value));
#endif
        return ref builder;
    }

    private static ref SpanBuilder AppendQ(this ref SpanBuilder builder, scoped ref bool first, string name, int? value)
    {
        if (value is not int intValue)
        {
            return ref builder;
        }
        if (first)
        {
            first = false;
            builder.Append('?');
        }
        else
        {
            builder.Append('&');
        }
        builder.Append(name);
        builder.Append('=');
        builder.Append(intValue);
        return ref builder;
    }

    private static ref SpanBuilder AppendQ(this ref SpanBuilder builder, scoped ref bool first, string name, bool? value)
    {
        if (value is not bool boolValue)
        {
            return ref builder;
        }
        if (first)
        {
            first = false;
            builder.Append('?');
        }
        else
        {
            builder.Append('&');
        }
        builder.Append(name);
        builder.Append('=');
        builder.Append(boolValue ? "true" : "false");
        return ref builder;
    }

    public static async ValueTask<Uri> SerializeAsync(
        ILogger? logger,
        GoogleCloudStorageResource resource,
        CancellationToken cancellationToken)
    {
        var accessToken = await resource.Credential.GetAccessTokenAsync(GoogleCloudStorageCredential.ReadWriteScopes, cancellationToken);
        var isPublic = resource.IsPublic ? "true" : string.Empty;
        var builder = new UriBuilder
        {
            Scheme = "gs",
            Host = resource.BucketName,
            Path = resource.ObjectName,
            Query = BuildQuery(resource.ContentType, resource.CacheControl, isPublic, accessToken)
        };
        var result = builder.Uri;
        logger?.LogDebug("Successfully serialized GCS resource ({Resource}) as \"{Uri}\"", resource, result);
        return result;

        static string BuildQuery(string? contentType, string? cacheControl, string isPublic, string? accessToken)
        {
            Span<char> buffer = stackalloc char[16 * 1024];
            var qbuilder = new SpanBuilder(buffer);
            var first = true;
            return qbuilder
                .AppendQ(ref first, UriParameters.ContentType, contentType)
                .AppendQ(ref first, UriParameters.CacheControl, cacheControl)
                .AppendQ(ref first, UriParameters.Public, isPublic)
                .AppendQ(ref first, UriParameters.AccessToken, accessToken)
                .ToString();
        }
    }

    public static bool TryDeserialize(
        Uri uri,
        ILogger logger,
        GoogleCloudStorageUtils utils,
        IGoogleAccessTokenProvider? accessTokenProvider,
        bool passthrough,
        bool publicByDefault,
        [NotNullWhen(true)] out GoogleCloudStorageResource? resource)
    {
        if (null != uri && uri.Scheme == "gs")
        {
            var q = HttpUtility.ParseQueryString(uri.Query);
            var accessToken = q.Get(UriParameters.AccessToken);
            var cacheControl = q.Get(UriParameters.CacheControl);
            var contentType = q.Get(UriParameters.ContentType);
            var rawPublic = q.Get(UriParameters.Public);
            var isPublic = string.IsNullOrEmpty(rawPublic)
                ? publicByDefault
                : _truthy.Contains(rawPublic);
            if (passthrough)
            {
                if (string.IsNullOrEmpty(accessToken))
                {
                    logger.LogWarning("Unable to deserialize GCS resource without access token in passthrough mode.");
                    resource = default;
                    return false;
                }
                resource = new GoogleCloudStorageResource(
                    utils: utils,
                    accessToken: accessToken,
                    bucketName: uri.Host,
                    objectName: uri.LocalPath.Trim('/'),
                    contentType: contentType,
                    cacheControl: cacheControl,
                    isPublic: isPublic
                );
            }
            else
            {
                if (string.IsNullOrEmpty(accessToken))
                {
                    if (accessTokenProvider is null)
                    {
                        logger.LogWarning("Unable to deserialize GCS resource without access token and access token provider.");
                        resource = default;
                        return false;
                    }
                    resource = new GoogleCloudStorageResource(
                        utils: utils,
                        accessTokenProvider: accessTokenProvider,
                        bucketName: uri.Host,
                        objectName: uri.LocalPath.Trim('/'),
                        contentType: contentType,
                        cacheControl: cacheControl,
                        isPublic: isPublic
                    );
                }
                else
                {
                    resource = new GoogleCloudStorageResource(
                        utils: utils,
                        accessToken: accessToken,
                        bucketName: uri.Host,
                        objectName: uri.LocalPath.Trim('/'),
                        contentType: contentType,
                        cacheControl: cacheControl,
                        isPublic: isPublic
                    );
                }
            }
            logger.LogDebug("Successfully deserialized \"{Uri}\" as GCS resource: {Resource}", uri, resource);
            return true;
        }
        resource = default;
        return false;
    }
}