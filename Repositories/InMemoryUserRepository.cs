using UserManagementAPI.Models;
using System.Collections.Concurrent;

namespace UserManagementAPI.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<int, User> _users = new();
        private int _nextId = 1;
        private readonly object _idLock = new();

        public InMemoryUserRepository()
        {
            var user1 = new User
            {
                Id = GetNextId(),
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice.smith@example.com",
                Department = "HR",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var user2 = new User
            {
                Id = GetNextId(),
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob.johnson@example.com",
                Department = "IT",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _users[user1.Id] = user1;
            _users[user2.Id] = user2;
        }

        private int GetNextId()
        {
            lock (_idLock)
            {
                return _nextId++;
            }
        }


        public Task<IEnumerable<User>> GetAllAsync()
        {
            return Task.FromResult(_users.Values?.AsEnumerable() ?? Enumerable.Empty<User>());
        }

        public Task<User?> GetByIdAsync(int id)
        {
            _users.TryGetValue(id, out var user);
            return Task.FromResult(user);
        }

        public Task<User> CreateAsync(User user)
        {
            user.Id = GetNextId();
            user.CreatedAt = DateTime.UtcNow;
            _users[user.Id] = user;
            return Task.FromResult(user);
        }

        public Task<bool> UpdateAsync(User user)
        {
            if (!_users.ContainsKey(user.Id))
                return Task.FromResult(false);
            _users[user.Id] = user;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(int id)
        {
            return Task.FromResult(_users.TryRemove(id, out _));
        }

        public bool EmailExists(string email, int? excludeId = null)
        {
            return _users.Values.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && (!excludeId.HasValue || u.Id != excludeId.Value));
        }
    }
}
