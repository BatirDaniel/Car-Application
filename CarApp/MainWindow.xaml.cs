using CarApp.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Xml;

namespace CarApp
{
    public partial class MainWindow : Window
    {
        private Car car;
        private CarContext _context;
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new Car(); 
            _context = new CarContext();
            _context.Database.EnsureCreated();

            car = new Car();
            RefreshCars();
        }

        private void RefreshCars()
        {
            dgCars.ItemsSource = _context.GetCars();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                try
                {
                    List<Car> car = (List<Car>)_context.GetCars();
                    string json = JsonConvert.SerializeObject(car, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(fileName, json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving data to file: " + ex.Message);
                }
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                try
                {
                    string json = File.ReadAllText(fileName);
                    List<Car> cars = JsonConvert.DeserializeObject<List<Car>>(json);

                    foreach (var car in cars)
                    {
                        if (CheckForExistingId(car.Id))
                        {
                            MessageBox.Show($"Car with ID {car.Id} already exists in the database.");
                            return;
                        }
                        _context.AddCar(car);
                    }
                    MessageBox.Show("The cars has been successfully added!");

                    RefreshCars();

                    dgCarsInter.ItemsSource = _context.GetCars().ToList();
                    dgCarsInterSearch.ItemsSource = _context.GetCars().ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading data from file: " + ex.Message);
                }
            }
        }
        private bool CheckForExistingId(int id)
        {
                return _context.GetCarById(id) != null;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddCar_Click(object sender, RoutedEventArgs e)
        {
            grbxCars.Visibility = Visibility.Visible;
            grbShowCars.Visibility = Visibility.Collapsed;
            grbCarsInterogations.Visibility = Visibility.Collapsed;
            grbCarsInterogationSearch.Visibility = Visibility.Collapsed;

            tbxMarca.Text = string.Empty;
            tbxCuloare.Text = string.Empty;
            tbxAnulFabricarii.Text = string.Empty;
            tbxPret.Text = string.Empty;

            tbxMarca.Focus();
        }

        private void UpdateCar_Click(object sender, RoutedEventArgs e)
        {
            grbxCars.Visibility = Visibility.Collapsed;
            grbShowCars.Visibility = Visibility.Visible;
            grbCarsInterogations.Visibility = Visibility.Collapsed;
            grbCarsInterogationSearch.Visibility = Visibility.Collapsed;

            button.Content = "Edit";
            button.Background = Brushes.CornflowerBlue;
        }

        private void DeleteCar_Click(object sender, RoutedEventArgs e)
        {
            grbxCars.Visibility = Visibility.Collapsed;
            grbShowCars.Visibility = Visibility.Visible;
            grbCarsInterogations.Visibility = Visibility.Collapsed;
            grbCarsInterogationSearch.Visibility = Visibility.Collapsed;

            button.Content = "Delete";
            button.Background = Brushes.Red;
        }

        private void Author_Click(object sender, RoutedEventArgs e)
        {
            Details details= new Details();
            details.Show();
        }

        private void btnCancelCar_Click(object sender, RoutedEventArgs e)
        {
            tbxMarca.Text = string.Empty;
            tbxCuloare.Text = string.Empty;
            tbxAnulFabricarii.Text = string.Empty;
            tbxPret.Text = string.Empty;

            tbxMarca.Focus();
        }

        private void btnSaveCar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnSaveCar.Content != "Edit")
                {
                    using (var context = new CarContext())
                    {
                        var car = new Car
                        {
                            Marca = tbxMarca.Text,
                            Culoare = tbxCuloare.Text,
                            Pret = double.Parse(tbxPret.Text),
                            AnulProducerii = int.Parse(tbxAnulFabricarii.Text)
                        };
                        _context.AddCar(car);
                        MessageBox.Show("The car has been successfully added!");

                        tbxMarca.Text = "";
                        tbxCuloare.Text = "";
                        tbxPret.Text = "";
                        tbxAnulFabricarii.Text = "";

                        tbxMarca.Focus();

                        RefreshCars();
                    }
                }
                else
                {
                    Car? car = dgCars.SelectedItem as Car;
                    if (car == null)
                    {
                        return;
                    }
                    if (_context.GetCarById(car.Id) != null)
                    {
                        car.Marca = tbxMarca.Text;
                        car.Culoare = tbxCuloare.Text;
                        car.Pret = double.Parse(tbxPret.Text);
                        car.AnulProducerii = int.Parse(tbxAnulFabricarii.Text);

                        _context.UpdateCar(car);
                        RefreshCars();

                        MessageBox.Show("The car has been edited successfully!");

                        grbxCars.Visibility = Visibility.Collapsed;
                        grbShowCars.Visibility = Visibility.Visible;
                    }
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Eroare! Date introduse incorecte!");
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (button.Content == "Delete")
            {
                if (dgCars.SelectedItem == null)
                {
                    return;
                }
                var selectedCar = (Car)dgCars.SelectedItem;
                _context.DeleteCar(selectedCar);

                MessageBox.Show("The car has been deleted successfully!");

                RefreshCars();
            }
            else
            {
                btnSaveCar.Content = "Edit";

                if (dgCars.SelectedItem == null)
                {
                    return;
                }
                var car = (Car)dgCars.SelectedItem;
                tbxMarca.Text = car.Marca;
                tbxCuloare.Text = car.Culoare;
                tbxPret.Text = car.Pret.ToString();
                tbxAnulFabricarii.Text = car.AnulProducerii.ToString();

                grbShowCars.Visibility = Visibility.Collapsed;
                grbxCars.Visibility = Visibility.Visible;
            }
        }

        //Afisarea interogarilor realizate in tabelul Car

        private void CarsOrderedByPrice_Click(object sender, RoutedEventArgs e)
        {
            grbxCars.Visibility = Visibility.Collapsed;
            grbShowCars.Visibility = Visibility.Collapsed;
            grbCarsInterogations.Visibility = Visibility.Visible;
            grbCarsInterogationSearch.Visibility = Visibility.Collapsed;

            var cars = _context.GetCars();
            var carsSortedByPrice = from car in cars orderby car.Pret select car;

            grbCarsInterogations.Header = "The information about the cars sorted by price";
            dgCarsInter.ItemsSource = carsSortedByPrice;
            dgCarsInter.IsReadOnly= true;
        }

        private void CarsOrderedByYearManufactured_Click(object sender, RoutedEventArgs e)
        {
            grbxCars.Visibility = Visibility.Collapsed;
            grbShowCars.Visibility = Visibility.Collapsed;
            grbCarsInterogations.Visibility = Visibility.Visible;
            grbCarsInterogationSearch.Visibility = Visibility.Collapsed;

            var cars = _context.GetCars();
            var carsSortedByYearManufactured = from car in cars orderby car.AnulProducerii select car;

            grbCarsInterogations.Header = "The information about the cars sorted by year manufactured";
            dgCarsInter.ItemsSource = carsSortedByYearManufactured;
            dgCarsInter.IsReadOnly = true;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            grbxCars.Visibility = Visibility.Collapsed;
            grbShowCars.Visibility = Visibility.Collapsed;
            grbCarsInterogations.Visibility = Visibility.Collapsed;
            grbCarsInterogationSearch.Visibility = Visibility.Visible;

            dgCarsInterSearch.ItemsSource = _context.GetCars();
            tbxSearchInCars.Focus();
        }


        private void tbxSearchInCars_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string words = tbxSearchInCars.Text;
            var cars = _context.GetCars();

            var searchCar = cars.Where(c => c.Marca.Contains(words) || c.Culoare.Contains(words) || c.Pret.ToString().Contains(words) || c.AnulProducerii.ToString().Contains(words));

            dgCarsInterSearch.ItemsSource = searchCar;
            dgCarsInterSearch.IsReadOnly = true;
        }
    }
}
