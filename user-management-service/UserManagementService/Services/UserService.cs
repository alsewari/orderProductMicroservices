using MongoDB.Driver;
using UserManagementService.Data;
using UserManagementService.Models;

namespace UserManagementService.Services;

public class UserService
{
    private readonly MongoDbContext _context;

    public UserService(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.Find(_ => true).ToListAsync();
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _context.Users.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<(bool Success, string Message, User? User)> CreateAsync(User user)
    {
        var existingUser = await _context.Users
            .Find(x => x.Email == user.Email)
            .FirstOrDefaultAsync();

        if (existingUser != null)
        {
            return (false, "User with this email already exists.", null);
        }

        await _context.Users.InsertOneAsync(user);

        return (true, "User created successfully.", user);
    }

    public async Task<bool> UpdateRoleAsync(string id, string role)
    {
        var result = await _context.Users.UpdateOneAsync(
            x => x.Id == id,
            Builders<User>.Update.Set(x => x.Role, role)
        );

        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _context.Users.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }
}