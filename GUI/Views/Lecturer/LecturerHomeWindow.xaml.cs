using BUS.DTOs;
using System.Windows;

namespace GUI.Views.Lecturer;

public partial class LecturerHomeWindow : Window
{
    public LecturerHomeWindow(AuthenticatedUserDto user)
    {
        InitializeComponent();
        WelcomeTextBlock.Text = $"Welcome, {user.FullName}";
    }
}

