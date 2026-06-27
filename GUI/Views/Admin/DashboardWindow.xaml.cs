using BUS.DTOs;
using System.Windows;

namespace GUI.Views.Admin;

public partial class DashboardWindow : Window
{
    public DashboardWindow(AuthenticatedUserDto user)
    {
        InitializeComponent();
        WelcomeTextBlock.Text = $"Welcome, {user.FullName}";
    }
}

