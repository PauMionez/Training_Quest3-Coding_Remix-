using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;

namespace Training_Quest3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public MainWindow()
        {
            InitializeComponent();
        }


        #region Input regex

        private void AlphanumericOnly(object sender, TextCompositionEventArgs e)
        {
            // Check if the input is alphanumeric (letters or digits)
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, $"^[a-za-z0-9]+$"))
                e.Handled = true; // Reject the input
        }
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            // Check if the input is numeric
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[0-9\\s?]+$"))
                e.Handled = true; // Reject the input
        }
        private void AlphabeticOnly(object sender, TextCompositionEventArgs e)
        {
            // Check if the input is alphabetic (letters only)
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, $@"[^\d]+"))
                e.Handled = true; // Reject the input
        }

        #endregion

    }
}
