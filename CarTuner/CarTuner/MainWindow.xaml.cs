using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace CarTuner
{
    public partial class MainWindow : Window
    {
        private const int BaseTopSpeed = 0;

        public ObservableCollection<CarPart> AvailableParts { get; } =
            new ObservableCollection<CarPart>();

        public ObservableCollection<CarPart> SelectedParts { get; } =
            new ObservableCollection<CarPart>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            SeedParts();
            UpdateStats();
        }

        private void SeedParts()
        {
            AvailableParts.Add(new Engine
            {
                Name = "Street Engine",
                SpeedBonus = 0,
                Cost = 0m,
                CylinderCount = 0
            });

            AvailableParts.Add(new Engine
            {
                Name = "Track Engine",
                SpeedBonus = 0,
                Cost = 0m,
                CylinderCount = 0
            });

            AvailableParts.Add(new Exhaust
            {
                Name = "Sport Exhaust",
                SpeedBonus = 0,
                Cost = 0m,
                IsSport = true
            });

            AvailableParts.Add(new Exhaust
            {
                Name = "Racing Exhaust",
                SpeedBonus = 0,
                Cost = 0m,
                IsSport = true
            });

            AvailableParts.Add(new Wheel
            {
                Name = "All-Season Wheels",
                SpeedBonus = 0,
                Cost = 0m,
                SizeInInches = 0
            });

            AvailableParts.Add(new Wheel
            {
                Name = "Performance Wheels",
                SpeedBonus = 0,
                Cost = 0m,
                SizeInInches = 0
            });
        }

        // Navigation

        private void GoToWorkshop_Click(object sender, RoutedEventArgs e)
        {
            MainTabs.SelectedIndex = 1;
        }

        private void GoToGarage_Click(object sender, RoutedEventArgs e)
        {
            MainTabs.SelectedIndex = 2;
        }

        // Workshop actions

        private void AddPartButton_Click(object sender, RoutedEventArgs e)
        {
            if (AvailablePartsListBox.SelectedItem is CarPart part)
            {
                SelectedParts.Add(part);
                UpdateStats();
                UpdatePreviewImage(part);
            }
            else
            {
                MessageBox.Show("Please select a part from the list first.",
                                "No part selected",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        private void RemovePartButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPartsListBox.SelectedItem is CarPart part)
            {
                SelectedParts.Remove(part);
                UpdateStats();
                UpdatePreviewImage(part);
            }
            else
            {
                MessageBox.Show("Please select a part from the current build to remove.",
                                "No part selected",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        private void ClearBuild_Click(object sender, RoutedEventArgs e)
        {
            SelectedParts.Clear();
            UpdateStats();
            BuildImage.Source = null;
            ImageInfoTextBlock.Text = "Pick parts to see a preview here.";
        }

        // Stats (LINQ)

        private void UpdateStats()
        {
            int totalSpeedBonus = SelectedParts.Sum(p => p.SpeedBonus);
            decimal totalCost = SelectedParts.Sum(p => p.Cost);

            int finalTopSpeed = BaseTopSpeed + totalSpeedBonus;

            CurrentSpeedTextBlock.Text = $"{finalTopSpeed} km/h";
            TotalCostTextBlock.Text = $"{totalCost:C}";
        }

        // Image preview

        private void UpdatePreviewImage(CarPart part)
        {
            //string imagePath = "Images/car_base.png";


        }

        // JSON SAVE (manual JSON string, with try/catch)

        private void SaveToGarage_Click(object sender, RoutedEventArgs e)
        {
            if (!SelectedParts.Any())
            {
                MessageBox.Show("Add at least one part before saving your build.",
                                "Nothing to save",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            string buildName = string.IsNullOrWhiteSpace(BuildNameTextBox.Text)
                ? "My Build"
                : BuildNameTextBox.Text.Trim();

            int totalSpeedBonus = SelectedParts.Sum(p => p.SpeedBonus);
            decimal totalCost = SelectedParts.Sum(p => p.Cost);
            int finalTopSpeed = BaseTopSpeed + totalSpeedBonus;

            var dialog = new SaveFileDialog
            {
                Title = "Save car build to JSON",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                FileName = buildName + ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string json = BuildJson(buildName, totalSpeedBonus, totalCost, finalTopSpeed);
                    File.WriteAllText(dialog.FileName, json, Encoding.UTF8);

                    MessageBox.Show("Car build saved to your garage!",
                                    "Saved",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong while saving:\n" + ex.Message,
                                    "Save Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }

        private string BuildJson(string buildName, int totalSpeedBonus,
                                 decimal totalCost, int finalTopSpeed)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  \"BuildName\": \"{EscapeJson(buildName)}\",");
            sb.AppendLine($"  \"BaseTopSpeed\": {BaseTopSpeed},");
            sb.AppendLine($"  \"TotalSpeedBonus\": {totalSpeedBonus},");
            sb.AppendLine($"  \"FinalTopSpeed\": {finalTopSpeed},");
            sb.AppendLine($"  \"TotalCost\": {totalCost.ToString(CultureInfo.InvariantCulture)},");
            sb.AppendLine("  \"Parts\": [");

            for (int i = 0; i < SelectedParts.Count; i++)
            {
                CarPart p = SelectedParts[i];
                string typeName = p is Engine ? "Engine" :
                                  p is Exhaust ? "Exhaust" :
                                  p is Wheel ? "Wheel" : "Unknown";

                sb.Append("    { ");
                sb.AppendFormat("\"Name\": \"{0}\", ", EscapeJson(p.Name));
                sb.AppendFormat("\"SpeedBonus\": {0}, ", p.SpeedBonus);
                sb.AppendFormat("\"Cost\": {0}, ", p.Cost.ToString(CultureInfo.InvariantCulture));
                sb.AppendFormat("\"Type\": \"{0}\"", typeName);
                sb.Append(" }");

                if (i < SelectedParts.Count - 1)
                    sb.Append(",");

                sb.AppendLine();
            }

            sb.AppendLine("  ]");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string EscapeJson(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text.Replace("\\", "\\\\")
                       .Replace("\"", "\\\"");
        }

        // JSON LOAD (just shows raw JSON text safely)

        private void LoadFromGarage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Load car build JSON",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string json = File.ReadAllText(dialog.FileName, Encoding.UTF8);
                    GarageJsonTextBox.Text = json;
                    MainTabs.SelectedIndex = 2;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong while loading:\n" + ex.Message,
                                    "Load Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }
    }
}