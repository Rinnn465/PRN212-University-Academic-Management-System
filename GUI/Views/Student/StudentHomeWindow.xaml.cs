using BUS.DTOs;
using GUI.Navigation;
using System.Windows;

namespace GUI.Views.Student;

public partial class StudentHomeWindow : Window
{
    public StudentHomeWindow(AuthenticatedUserDto user)
    {
        InitializeComponent();
        WelcomeTextBlock.Text = $"Welcome, {user.FullName}";
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        WindowNavigationService.OpenLogin(this);
    }
}

