using CarDiary.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CarDiary.Data
{
    public class CarDiaryContext : DbContext
    {
        public CarDiaryContext(DbContextOptions<CarDiaryContext> options) : base(options)
        {
        }

        public DbSet<CarDetails> CarDetails { get; set; }
    }
}
