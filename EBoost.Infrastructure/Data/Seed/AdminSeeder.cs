using EBoost.Domain.Entities;
using EBoost.Application.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace EBoost.Infrastructure.Data.Seed;

public static class AdminSeeder
{
    public static async Task SeedAsync(
        EBoostDbContext context,
        IPasswordHasher hasher)
    {
        // 1️⃣ Ensure Roles exist
        if (!await context.Roles.AnyAsync())
        {
            context.Roles.AddRange(
                new Role { Name = "User" },
                new Role { Name = "Admin" }
            );

            await context.SaveChangesAsync();
        }

        var roles = await context.Roles
            .ToDictionaryAsync(r => r.Name, r => r.Id);

        var userRoleId = roles["User"];
        var adminRoleId = roles["Admin"];

        // 2️⃣ Backfill users without role
        var usersWithoutRole = await context.Users
            .Where(u => u.RoleId == 0)
            .ToListAsync();

        if (usersWithoutRole.Any())
        {
            foreach (var user in usersWithoutRole)
                user.RoleId = userRoleId;

            await context.SaveChangesAsync();
        }

        // 3️⃣ Ensure admin user
        var adminEmail = Environment.GetEnvironmentVariable("ADMIN_SEED_EMAIL") ?? "admin@eboost.com";
        var adminPassword = Environment.GetEnvironmentVariable("ADMIN_SEED_PASSWORD") ?? "Admin@123";

        var admin = await context.Users
            .FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (admin == null)
        {
            context.Users.Add(new User
            {
                FullName = "System Administrator",
                Email = adminEmail,
                PasswordHash = hasher.HashPassword(adminPassword),
                RoleId = adminRoleId,
                CreatedAt = DateTime.UtcNow
            });
        }
        else if (admin.RoleId != adminRoleId)
        {
            admin.RoleId = adminRoleId;
        }

        await context.SaveChangesAsync();
    }
}


