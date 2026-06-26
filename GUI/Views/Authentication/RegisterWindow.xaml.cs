using BUS.Services;
using DAL;
using DAL.Repositories;
using GUI.Configuration;
using GUI.Navigation;
using GUI.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace GUI.Views.Authentication;

public partial class RegisterWindow : Window
{
    private readonly UnitOfWork _unitOfWork;
    private readonly RegisterViewModel _viewModel;

    public RegisterWindow()
    {
        InitializeComponent();

        var connectionString = AppConfiguration.GetConnectionString();
        var dbContext = AppDbContextFactory.Create(connectionString);
        _unitOfWork = new UnitOfWork(dbContext);
        _viewModel = new RegisterViewModel(new AuthService(_unitOfWork), BackToLogin);

        DataContext = _viewModel;
        StudentCodeTextBox.Focus();
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        var passwordInput = new RegisterPasswordInput
        {
            Password = PasswordBox.Password,
            ConfirmPassword = ConfirmPasswordBox.Password
        };

        if (_viewModel.RegisterCommand.CanExecute(passwordInput))
        {
            _viewModel.RegisterCommand.Execute(passwordInput);
        }
    }

    private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
    {
        BackToLogin();
    }

    private void ConfirmPasswordBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            RegisterButton_Click(sender, e);
        }
    }

    private void BackToLogin()
    {
        WindowNavigationService.OpenLogin(this);
    }

    protected override void OnClosed(EventArgs e)
    {
        _unitOfWork.Dispose();
        base.OnClosed(e);
    }
}
