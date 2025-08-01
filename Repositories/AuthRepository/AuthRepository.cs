using MongoDB.Bson;
using MongoDB.Driver;
using Peyghoom.Core.Results;
using Peyghoom.Entities;

namespace Peyghoom.Repositories.AuthRepository;

public class AuthRepository: IAuthRepository
{
    private readonly IMongoCollection<RefreshToken> _refreshTokens;

    public AuthRepository(IMongoDatabase database)
    {
        _refreshTokens = database.GetCollection<RefreshToken>("refresh_tokens");
    }


    public async Task<Result<RefreshToken>> CreateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await _refreshTokens.InsertOneAsync(refreshToken, cancellationToken: cancellationToken);
        return refreshToken;
    }

    public async Task<Result<RefreshToken>> FindRefreshByTokenAsync( string token, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _refreshTokens.Find(refresh => refresh.Token == token)
            .FirstOrDefaultAsync(cancellationToken);
        return refreshToken;
    }

    public async Task<Result> UpdateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        var filter = Builders<RefreshToken>.Filter.Eq(r => r.Id, refreshToken.Id);
        var result = await _refreshTokens.ReplaceOneAsync(filter, refreshToken, cancellationToken: cancellationToken);

        if (result.IsAcknowledged && result.ModifiedCount > 0) return Result.Success();
        return Result.Failure(Error.ServerError("update refresh token failed"));
    }
}