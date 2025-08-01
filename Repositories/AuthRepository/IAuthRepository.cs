using MongoDB.Bson;
using Peyghoom.Core.Results;
using Peyghoom.Entities;

namespace Peyghoom.Repositories.AuthRepository;

public interface IAuthRepository
{
    Task<Result<RefreshToken>> CreateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<Result<RefreshToken>> FindRefreshByTokenAsync( string token, CancellationToken cancellationToken = default);
    Task<Result> UpdateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
}