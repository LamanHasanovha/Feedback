using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Contexts;

public class FeedbackContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Student> Students { get; set; }
}
