using BUS.DTOs;
using GUI.Navigation;
using System.Windows;

namespace GUI.Views.Lecturer;

public partial class LecturerHomeWindow : Window
{
    public LecturerHomeWindow(AuthenticatedUserDto user)
    {
        InitializeComponent();
        WelcomeTextBlock.Text = $"Welcome, {user.FullName}";
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        WindowNavigationService.OpenLogin(this);
    }
}

