using BCrypt.Net;
using MongoDB.Driver;
using UserManagement.Models;

namespace UserManagement.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService()
        {
            // ConnectionString (Hardcoded)
            var connectionString = "mongodb://localhost:27017";
            var databaseName = "UserManagementDB";
            var collectionName = "Users";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _users = database.GetCollection<User>(collectionName);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _users.Find(user => true).ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            //  Salt = random key
            // user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash, BCrypt.Net.BCrypt.GenerateSalt());

            await _users.InsertOneAsync(user);
        }

        public async Task<bool> UpdateUserAsync(string id, User updatedUser)
        {
            var result = await _users.ReplaceOneAsync(user => user.Id == id, updatedUser);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var result = await _users.DeleteOneAsync(user => user.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task DeleteUsersByEmailPatternAsync(string emailPattern)
        {
            var filter = Builders<User>.Filter.Regex(u => u.Email, new MongoDB.Bson.BsonRegularExpression(emailPattern));
            await _users.DeleteManyAsync(filter);
        }
    }
}
