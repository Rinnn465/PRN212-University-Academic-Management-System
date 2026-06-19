using BUS.DTOs;
using System.Windows;

namespace GUI.Views.Student;

public partial class StudentHomeWindow : Window
{
    public StudentHomeWindow(AuthenticatedUserDto user)
    {
        InitializeComponent();
        WelcomeTextBlock.Text = $"Welcome, {user.FullName}";
    }
}

