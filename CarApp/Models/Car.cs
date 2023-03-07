using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarApp.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string? Marca { get; set; }
        public string? Culoare { get; set; }
        public double Pret { get; set; }
        public int AnulProducerii { get; set; }
        public Car() { }
    }
}
