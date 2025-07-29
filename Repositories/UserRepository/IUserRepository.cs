using Peyghoom.Entities;

namespace Peyghoom.Repositories.UserRepository;

public interface IUserRepository
{
   public Task<User?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default); 
   public Task<User?> GetUserByPhoneNumberAsync(long phoneNumber, CancellationToken cancellationToken = default); 
   
   public Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default);
}