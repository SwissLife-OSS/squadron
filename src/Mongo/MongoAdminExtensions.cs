using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Squadron;

/// <summary>
/// MongoDB administration extensions for test resources.
/// </summary>
public static class MongoAdminExtensions
{
    private const int MaximumIndexBuildMinAvailableDiskSpaceMb = 8 * 1024 * 1024;

    /// <summary>
    /// Sets the minimum available disk space required for index builds.
    /// </summary>
    /// <param name="client">The MongoDB client.</param>
    /// <param name="megabytes">The minimum available disk space in megabytes.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task SetIndexBuildMinAvailableDiskSpaceAsync(
        this IMongoClient client,
        int megabytes,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        if (megabytes is < 0 or > MaximumIndexBuildMinAvailableDiskSpaceMb)
        {
            throw new ArgumentOutOfRangeException(
                nameof(megabytes),
                megabytes,
                $"The value must be between 0 and {MaximumIndexBuildMinAvailableDiskSpaceMb} megabytes.");
        }

        var command = new BsonDocument
        {
            { "setParameter", 1 },
            { "indexBuildMinAvailableDiskSpaceMB", megabytes }
        };

        await client
            .GetDatabase("admin")
            .RunCommandAsync<BsonDocument>(
                command,
                readPreference: null,
                cancellationToken);
    }
}
