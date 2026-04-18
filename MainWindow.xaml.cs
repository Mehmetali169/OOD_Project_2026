using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace CarTuner
{
    public partial class MainWindow : Window
    {
        private const int BaseTopSpeed = 230;

        private SoundPlayer upgradeSound;
        private SoundPlayer errorSound;
        private bool soundEnabled = true;

        public ObservableCollection<CarPart> AvailableParts { get; } =
            new ObservableCollection<CarPart>();

        public ObservableCollection<CarPart> SelectedParts { get; } =
            new ObservableCollection<CarPart>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadSounds();
            SeedParts();
            UpdateStats();
        }

        private void LoadSounds()
        {
            try
            {
                var upgradeRes = Application.GetResourceStream(new Uri("Sounds/upgrade.wav", UriKind.Relative));
                if (upgradeRes != null)
                {
                    upgradeSound = new SoundPlayer(upgradeRes.Stream);
                    upgradeSound.Load();
                }

                var errorRes = Application.GetResourceStream(new Uri("Sounds/error.wav", UriKind.Relative));
                if (errorRes != null)
                {
                    errorSound = new SoundPlayer(errorRes.Stream);
                    errorSound.Load();
                }
            }
            catch { }
        }

        private void PlaySound(SoundPlayer sound)
        {
            if (!soundEnabled || sound == null) return;
            try { sound.Play(); } catch { }
        }

        private void SoundToggle_Changed(object sender, RoutedEventArgs e)
        {
            if (SoundToggle == null) return;
            soundEnabled = SoundToggle.IsChecked == true;
            SoundToggle.Content = soundEnabled ? "Sound On" : "Sound Off";
        }

        private void SeedParts()
        {
            AvailableParts.Add(new Engine
            {
                Name = "Tuned 4-Cylinder Engine",
                SpeedBonus = 8, Cost = 2800m,
                CylinderCount = 4, Category = "Engine",
                ImagePath = "Images/engine_4cyl.png"
            });
            AvailableParts.Add(new Engine
            {
                Name = "V8 Performance Engine",
                SpeedBonus = 55, Cost = 14500m,
                CylinderCount = 8, Category = "Engine",
                ImagePath = "Images/engine_v8.png"
            });

            AvailableParts.Add(new Exhaust
            {
                Name = "Sport Exhaust System",
                SpeedBonus = 3, Cost = 650m,
                IsSport = true, Category = "Exhaust",
                ImagePath = "Images/exhaust_sport.png"
            });
            AvailableParts.Add(new Exhaust
            {
                Name = "Titanium Race Exhaust",
                SpeedBonus = 10, Cost = 3200m,
                IsSport = true, Category = "Exhaust",
                ImagePath = "Images/exhaust_titanium.png"
            });

            AvailableParts.Add(new Wheel
            {
                Name = "Sport Alloy Wheels",
                SpeedBonus = 3, Cost = 1400m,
                SizeInInches = 18, Category = "Wheels",
                ImagePath = "Images/wheels_sport.png"
            });
            AvailableParts.Add(new Wheel
            {
                Name = "Lightweight Forged Track Wheels",
                SpeedBonus = 7, Cost = 4500m,
                SizeInInches = 19, Category = "Wheels",
                ImagePath = "Images/wheels_forged.png"
            });

            AvailableParts.Add(new Turbo
            {
                Name = "Standard Turbo Kit",
                SpeedBonus = 22, Cost = 4500m,
                BoostType = "Single Turbo", Category = "Turbo",
                ImagePath = "Images/turbo_standard.png"
            });
            AvailableParts.Add(new Turbo
            {
                Name = "High-Power Turbo Kit",
                SpeedBonus = 45, Cost = 8500m,
                BoostType = "Single Turbo", Category = "Turbo",
                ImagePath = "Images/turbo_big.png"
            });

            AvailableParts.Add(new Intake
            {
                Name = "Cold Air Intake System",
                SpeedBonus = 2, Cost = 280m,
                IntakeType = "Cold Air", Category = "Intake",
                ImagePath = "Images/intake_cold.png"
            });
            AvailableParts.Add(new Intake
            {
                Name = "Carbon Fibre Air Intake",
                SpeedBonus = 3, Cost = 650m,
                IntakeType = "Carbon Fibre", Category = "Intake",
                ImagePath = "Images/intake_carbon.png"
            });

            AvailableParts.Add(new ECUTune
            {
                Name = "Stage 1 ECU Tune",
                SpeedBonus = 10, Cost = 400m,
                Stage = 1, Category = "Stage",
                ImagePath = "Images/ecu_stage1.png"
            });
            AvailableParts.Add(new ECUTune
            {
                Name = "Stage 3 Custom Dyno Tune",
                SpeedBonus = 22, Cost = 1200m,
                Stage = 3, Category = "Stage",
                ImagePath = "Images/ecu_stage3.png"
            });

            AvailableParts.Add(new Intercooler
            {
                Name = "Upgraded Front-Mount Intercooler",
                SpeedBonus = 3, Cost = 800m,
                CoolerType = "Front-Mount", Category = "Intercooler",
                ImagePath = "Images/intercooler_upgraded.png"
            });
            AvailableParts.Add(new Intercooler
            {
                Name = "Competition Intercooler",
                SpeedBonus = 5, Cost = 1400m,
                CoolerType = "Front-Mount", Category = "Intercooler",
                ImagePath = "Images/intercooler_competition.png"
            });

            AvailableParts.Add(new Transmission
            {
                Name = "Performance Clutch Kit",
                SpeedBonus = 0, Cost = 650m,
                TransType = "Clutch", Category = "Transmission",
                ImagePath = "Images/transmission_clutch.png"
            });
            AvailableParts.Add(new Transmission
            {
                Name = "Limited-Slip Differential",
                SpeedBonus = 3, Cost = 1800m,
                TransType = "Differential", Category = "Transmission",
                ImagePath = "Images/transmission_lsd.png"
            });

            AvailableParts.Add(new Brake
            {
                Name = "Drilled Brake Rotors",
                SpeedBonus = 0, Cost = 350m,
                PistonCount = 0, Category = "Brakes",
                ImagePath = "Images/brake_rotors.png"
            });
            AvailableParts.Add(new Brake
            {
                Name = "Performance Brake Kit",
                SpeedBonus = 0, Cost = 2200m,
                PistonCount = 4, Category = "Brakes",
                ImagePath = "Images/brake_kit.png"
            });
        }

        private void GoToWorkshop_Click(object sender, RoutedEventArgs e)
        {
            MainTabs.SelectedIndex = 1;
        }

        private void GoToGarage_Click(object sender, RoutedEventArgs e)
        {
            MainTabs.SelectedIndex = 2;
        }

        private void AvailablePartsListBox_SelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (AvailablePartsListBox.SelectedItem is CarPart part)
                ShowSinglePreview(part);
        }

        private void SelectedPartsListBox_SelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (SelectedPartsListBox.SelectedItem is CarPart)
                ShowBuildPreview();
        }

        private void ShowSinglePreview(CarPart part)
        {
            BuildPreviewScroll.Visibility = Visibility.Collapsed;
            ImageInfoTextBlock.Visibility = Visibility.Collapsed;
            try
            {
                PreviewImage.Source = new BitmapImage(new Uri(part.ImagePath, UriKind.Relative));
                PreviewImage.Visibility = Visibility.Visible;
            }
            catch
            {
                PreviewImage.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowBuildPreview()
        {
            PreviewImage.Visibility = Visibility.Collapsed;

            if (SelectedParts.Count == 0)
            {
                BuildPreviewScroll.Visibility = Visibility.Collapsed;
                ImageInfoTextBlock.Visibility = Visibility.Visible;
                return;
            }

            ImageInfoTextBlock.Visibility = Visibility.Collapsed;
            BuildPreviewScroll.Visibility = Visibility.Visible;
            BuildPartsPanel.Children.Clear();

            foreach (var part in SelectedParts)
            {
                try
                {
                    var img = new System.Windows.Controls.Image();
                    img.Source = new BitmapImage(new Uri(part.ImagePath, UriKind.Relative));
                    img.Width = 120;
                    img.Height = 120;
                    img.Margin = new Thickness(5);
                    img.Stretch = System.Windows.Media.Stretch.Uniform;
                    BuildPartsPanel.Children.Add(img);
                }
                catch { }
            }
        }

        private void AddPartButton_Click(object sender, RoutedEventArgs e)
        {
            if (AvailablePartsListBox.SelectedItem is CarPart part)
            {
                if (SelectedParts.Contains(part))
                {
                    PlaySound(errorSound);
                    MessageBox.Show("You already added this part.",
                        "Duplicate part", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool oneOnly = part is Engine || part is Exhaust || part is Wheel
                    || part is Turbo || part is Intake || part is ECUTune || part is Intercooler;

                if (oneOnly)
                {
                    CarPart existing = null;
                    foreach (var p in SelectedParts)
                    {
                        if (p.GetType() == part.GetType())
                        {
                            existing = p;
                            break;
                        }
                    }
                    if (existing != null)
                        SelectedParts.Remove(existing);
                }

                SelectedParts.Add(part);
                UpdateStats();
                ShowBuildPreview();
                PlaySound(upgradeSound);
            }
            else
            {
                PlaySound(errorSound);
                MessageBox.Show("Please select a part first.",
                    "No part selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RemovePartButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPartsListBox.SelectedItem is CarPart part)
            {
                SelectedParts.Remove(part);
                UpdateStats();
                ShowBuildPreview();
            }
            else
            {
                PlaySound(errorSound);
                MessageBox.Show("Select a part from your build to remove it.",
                    "No part selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ClearBuild_Click(object sender, RoutedEventArgs e)
        {
            SelectedParts.Clear();
            UpdateStats();
            ShowBuildPreview();
        }

        private void UpdateStats()
        {
            int speedBonus = SelectedParts.Sum(p => p.SpeedBonus);
            decimal totalCost = SelectedParts.Sum(p => p.Cost);

            CurrentSpeedTextBlock.Text = (BaseTopSpeed + speedBonus) + " km/h";
            TotalCostTextBlock.Text = totalCost.ToString("C");
        }

        private void SaveToGarage_Click(object sender, RoutedEventArgs e)
        {
            if (!SelectedParts.Any())
            {
                MessageBox.Show("Add at least one part before saving.",
                    "Nothing to save", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string buildName = string.IsNullOrWhiteSpace(BuildNameTextBox.Text)
                ? "My Build" : BuildNameTextBox.Text.Trim();

            int speedBonus = SelectedParts.Sum(p => p.SpeedBonus);
            decimal totalCost = SelectedParts.Sum(p => p.Cost);
            int topSpeed = BaseTopSpeed + speedBonus;

            var dialog = new SaveFileDialog
            {
                Title = "Save car build",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                FileName = buildName + ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string json = BuildJson(buildName, speedBonus, totalCost, topSpeed);
                    File.WriteAllText(dialog.FileName, json, Encoding.UTF8);
                    MessageBox.Show("Build saved!", "Saved",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving:\n" + ex.Message,
                        "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string GetPartType(CarPart p)
        {
            if (p is Engine) return "Engine";
            if (p is Exhaust) return "Exhaust";
            if (p is Wheel) return "Wheel";
            if (p is Turbo) return "Turbo";
            if (p is Intake) return "Intake";
            if (p is ECUTune) return "Stage";
            if (p is Intercooler) return "Intercooler";
            if (p is Transmission) return "Transmission";
            if (p is Brake) return "Brake";
            return "Unknown";
        }

        private string BuildJson(string buildName, int speedBonus,
            decimal totalCost, int topSpeed)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"BuildName\": \"" + EscapeJson(buildName) + "\",");
            sb.AppendLine("  \"BaseTopSpeed\": " + BaseTopSpeed + ",");
            sb.AppendLine("  \"TotalSpeedBonus\": " + speedBonus + ",");
            sb.AppendLine("  \"FinalTopSpeed\": " + topSpeed + ",");
            sb.AppendLine("  \"TotalCost\": " + totalCost.ToString(CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("  \"Parts\": [");

            for (int i = 0; i < SelectedParts.Count; i++)
            {
                var p = SelectedParts[i];

                sb.Append("    { ");
                sb.Append("\"Name\": \"" + EscapeJson(p.Name) + "\", ");
                sb.Append("\"SpeedBonus\": " + p.SpeedBonus + ", ");
                sb.Append("\"Cost\": " + p.Cost.ToString(CultureInfo.InvariantCulture) + ", ");
                sb.Append("\"Category\": \"" + EscapeJson(p.Category) + "\", ");
                sb.Append("\"Type\": \"" + GetPartType(p) + "\"");
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
            if (string.IsNullOrEmpty(text)) return "";
            return text.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private void LoadFromGarage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Load car build",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    GarageJsonTextBox.Text = File.ReadAllText(dialog.FileName, Encoding.UTF8);
                    MainTabs.SelectedIndex = 2;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading:\n" + ex.Message,
                        "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}