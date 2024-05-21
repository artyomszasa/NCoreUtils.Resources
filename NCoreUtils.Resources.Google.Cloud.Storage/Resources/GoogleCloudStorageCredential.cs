using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NCoreUtils.Google;

namespace NCoreUtils.Resources;

public readonly struct GoogleCloudStorageCredential : IEquatable<GoogleCloudStorageCredential>
{
    internal static string[] ReadOnlyScopes { get; } = [GoogleCloudStorageUtils.ReadOnlyScope];

    internal static string[] ReadWriteScopes { get; } = [GoogleCloudStorageUtils.ReadWriteScope];

    #region factory

    public static GoogleCloudStorageCredential ViaAccessToken(string accessToken)
        => new(accessToken, default);

    public static GoogleCloudStorageCredential ViaProvider(IGoogleAccessTokenProvider accessTokenProvider)
        => new(default, accessTokenProvider);

    #endregion

    private string? AccessToken { get; }

    private IGoogleAccessTokenProvider? AccessTokenProvider { get; }

    internal GoogleCloudStorageCredential(string? accessToken, IGoogleAccessTokenProvider? accessTokenProvider)
    {
        if (accessToken is null)
        {
            if (accessTokenProvider is null)
            {
                throw new InvalidOperationException("Either access token or access token provider must be supplied.");
            }
            AccessToken = null;
            AccessTokenProvider = accessTokenProvider;
        }
        else
        {
            if (accessTokenProvider is not null)
            {
                throw new InvalidOperationException("Only one of the parameters can be supplied.");
            }
            AccessToken = accessToken;
            AccessTokenProvider = null;
        }
    }

    #region equality

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator==(GoogleCloudStorageCredential a, GoogleCloudStorageCredential b)
        => a.Equals(b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator!=(GoogleCloudStorageCredential a, GoogleCloudStorageCredential b)
        => !a.Equals(b);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(GoogleCloudStorageCredential other) => AccessToken switch
    {
        null => other.AccessToken is null && ReferenceEquals(AccessTokenProvider, other.AccessTokenProvider),
        var accessToken => accessToken == other.AccessToken
    };

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is GoogleCloudStorageCredential other && Equals(other);

    public override int GetHashCode() => AccessToken switch
    {
        null => AccessTokenProvider switch
        {
            null => throw new InvalidOperationException("Invalid Google Storage credential."),
            var accessTokenProvider => HashCode.Combine(1, RuntimeHelpers.GetHashCode(accessTokenProvider))
        },
        var accessToken => HashCode.Combine(0, accessToken)
    };

    #endregion

    public ValueTask<string> GetAccessTokenAsync(string[] scope, CancellationToken cancellationToken = default)
        => AccessToken switch
        {
            null => AccessTokenProvider switch
            {
                null => throw new InvalidOperationException("Invalid Google Storage credential."),
                var accessTokenProvider => accessTokenProvider.GetAccessTokenAsync(scope, cancellationToken)
            },
            var accessToken => new(accessToken)
        };

    private static string ToStringAccessToken(string accessToken)
    {
        Span<char> buffer = stackalloc char[32];
        const int PrefixLength = 6; //"token:".Length;
#if NET6_0_OR_GREATER
        "token:".CopyTo(buffer);
#else
        "token:".AsSpan().CopyTo(buffer);
#endif
        const int TokenLength = 32 - PrefixLength;
        if (accessToken.Length <= TokenLength)
        {
#if NET6_0_OR_GREATER
            accessToken.CopyTo(buffer[PrefixLength..]);
#else
            accessToken.AsSpan().CopyTo(buffer[PrefixLength..]);
#endif
        }
        else
        {
            accessToken.AsSpan(0, TokenLength - 3).CopyTo(buffer[PrefixLength..]);
            buffer[^3] = '.';
            buffer[^2] = '.';
            buffer[^1] = '.';
        }
        return new(buffer);
    }

    public override string ToString() => AccessToken switch
    {
        null => AccessTokenProvider switch
        {
            null => "<<invalid>>",
            _ => "dynamic"
        },
        var accessToken => ToStringAccessToken(accessToken)
    };
}
