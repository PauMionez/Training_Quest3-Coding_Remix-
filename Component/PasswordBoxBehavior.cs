using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace Training_Quest3.Component
{
    class PasswordBoxBehavior
    {

        public static readonly DependencyProperty PasswordProperty =
       DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxBehavior),
           new FrameworkPropertyMetadata(string.Empty, OnPasswordChanged));

        public static string GetPassword(DependencyObject obj) => (string)obj.GetValue(PasswordProperty);
        public static void SetPassword(DependencyObject obj, string value) => obj.SetValue(PasswordProperty, value);

        private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                if (e.NewValue != null)
                    passwordBox.Password = e.NewValue.ToString();

                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            SetPassword(passwordBox, passwordBox.Password);
        }
    }
}
