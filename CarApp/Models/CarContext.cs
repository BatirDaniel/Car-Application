using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarApp.Models
{
    public class CarContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = CarsDB.db").EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }

        public List<Car> GetCars()
        {
            return Cars.ToList();
        }
        public Car? GetCarById(int id)
        {
            return Cars.FirstOrDefault(p => p.Id == id);
        }
        public void AddCar(Car car)
        {
            Cars.Add(car);
            SaveChanges();
        }

        public void UpdateCar(Car car)
        {
            Cars.Update(car);
            SaveChanges();
        }

        public void DeleteCar(Car car)
        {
            Cars.Remove(car);
            SaveChanges();
        }
    }
}
