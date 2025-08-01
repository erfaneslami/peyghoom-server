using MongoDB.Driver;
using Peyghoom.Entities;

namespace Peyghoom.Repositories.UserRepository;

public class UserRepository: IUserRepository
{
    private readonly IMongoCollection<User> _users;
    

    public UserRepository(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("users");
    }

    public async Task<User?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _users.Find(user => user.Id.ToString() == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetUserByPhoneNumberAsync(long phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _users.Find(user => user.PhoneNumber == phoneNumber).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await _users.InsertOneAsync(user, cancellationToken: cancellationToken);
        return user;
    }
}