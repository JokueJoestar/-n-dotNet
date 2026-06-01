using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace DOANdotNET.ViewModels.Helpers
{
    public static class PasswordHelper
    {
  
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordHelper),
                new PropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordHelper),
                new PropertyMetadata(false, Attach));

        public static string GetPassword(DependencyObject dp) => (string)dp.GetValue(PasswordProperty);
        public static void SetPassword(DependencyObject dp, string value) => dp.SetValue(PasswordProperty, value);
        public static bool GetAttach(DependencyObject dp) => (bool)dp.GetValue(AttachProperty);
        public static void SetAttach(DependencyObject dp, bool value) => dp.SetValue(AttachProperty, value);

        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                
                passwordBox.PasswordChanged -= PasswordChanged;

                string newValue = (string)e.NewValue ?? string.Empty;

                if (passwordBox.Password != newValue)
                {
                    passwordBox.Password = newValue;
                }

                
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                if ((bool)e.OldValue) passwordBox.PasswordChanged -= PasswordChanged;
                if ((bool)e.NewValue) passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                
                SetPassword(passwordBox, passwordBox.Password);
            }
        }
    }
}
