using BUS.DTOs;
using BUS.Services;
using DAL;
using DAL.Repositories;
using GUI.Configuration;
using GUI.Navigation;
using GUI.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace GUI.Views.Authentication;

public partial class LoginWindow : Window
{
    private readonly UnitOfWork _unitOfWork;
    private readonly LoginViewModel _viewModel;

    public LoginWindow()
    {
        InitializeComponent();

        var connectionString = AppConfiguration.GetConnectionString();
        var dbContext = AppDbContextFactory.Create(connectionString);
        _unitOfWork = new UnitOfWork(dbContext);
        _viewModel = new LoginViewModel(new AuthService(_unitOfWork), OpenHomeWindow);

        DataContext = _viewModel;
        UsernameTextBox.Focus();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.LoginCommand.CanExecute(PasswordBox.Password))
        {
            _viewModel.LoginCommand.Execute(PasswordBox.Password);
        }
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        WindowNavigationService.OpenRegister(this);
    }

    private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            LoginButton_Click(sender, e);
        }
    }

    private void OpenHomeWindow(AuthenticatedUserDto user)
    {
        WindowNavigationService.OpenHome(this, user);
    }

    protected override void OnClosed(EventArgs e)
    {
        _unitOfWork.Dispose();
        base.OnClosed(e);
    }
}

