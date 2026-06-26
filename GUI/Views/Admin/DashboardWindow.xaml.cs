using BUS.DTOs;
using BUS.Services;
using DAL;
using DAL.Repositories;
using GUI.Configuration;
using GUI.Navigation;
using System.Windows;

namespace GUI.Views.Admin;

public partial class DashboardWindow : Window
{
    private readonly UnitOfWork _unitOfWork;

    public DashboardWindow(AuthenticatedUserDto user)
    {
        InitializeComponent();
        WelcomeTextBlock.Text = $"Welcome, {user.FullName}";

        var connectionString = AppConfiguration.GetConnectionString();
        var dbContext = AppDbContextFactory.Create(connectionString);
        _unitOfWork = new UnitOfWork(dbContext);

        Loaded += async (_, _) => await LoadDashboardAsync();
    }

    private async Task LoadDashboardAsync()
    {
        try
        {
            var dashboardService = new DashboardService(_unitOfWork);
            var summary = await dashboardService.GetSummaryAsync();

            TotalStudentsTextBlock.Text = summary.TotalStudents.ToString();
            TotalLecturersTextBlock.Text = summary.TotalLecturers.ToString();
            TotalClassesTextBlock.Text = summary.TotalClasses.ToString();
            TotalSubjectsTextBlock.Text = summary.TotalSubjects.ToString();
            AverageGpaTextBlock.Text = summary.AverageGPA?.ToString("0.00") ?? "N/A";
        }
        catch (Exception ex)
        {
            AverageGpaTextBlock.Text = "Error";
            MessageBox.Show($"Cannot load dashboard: {ex.Message}", "Dashboard", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        WindowNavigationService.OpenLogin(this);
    }

    protected override void OnClosed(EventArgs e)
    {
        _unitOfWork.Dispose();
        base.OnClosed(e);
    }
}

