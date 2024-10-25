using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Training_Quest3.View
{
    /// <summary>
    /// Interaction logic for UserInfoCodingView.xaml
    /// </summary>
    public partial class UserInfoCodingView : UserControl
    {
        public UserInfoCodingView()
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
