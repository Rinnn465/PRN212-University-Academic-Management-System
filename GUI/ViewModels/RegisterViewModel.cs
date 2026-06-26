using BUS.DTOs;
using BUS.Interfaces;
using GUI.Commands;
using System.Windows.Input;

namespace GUI.ViewModels;

public class RegisterViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly Action _onBackToLogin;
    private string _studentCode = string.Empty;
    private string _fullName = string.Empty;
    private string _gender = string.Empty;
    private string? _email;
    private string? _phone;
    private string _message = string.Empty;
    private bool _isBusy;

    public RegisterViewModel(IAuthService authService, Action onBackToLogin)
    {
        _authService = authService;
        _onBackToLogin = onBackToLogin;
        RegisterCommand = new RelayCommand(async parameter => await RegisterAsync(parameter as RegisterPasswordInput), _ => !IsBusy);
        BackToLoginCommand = new RelayCommand(_ => _onBackToLogin(), _ => !IsBusy);
    }

    public string StudentCode
    {
        get => _studentCode;
        set => SetProperty(ref _studentCode, value);
    }

    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    public string Gender
    {
        get => _gender;
        set => SetProperty(ref _gender, value);
    }

    public string? Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string? Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                if (RegisterCommand is RelayCommand registerCommand)
                {
                    registerCommand.RaiseCanExecuteChanged();
                }

                if (BackToLoginCommand is RelayCommand backCommand)
                {
                    backCommand.RaiseCanExecuteChanged();
                }
            }
        }
    }

    public ICommand RegisterCommand { get; }
    public ICommand BackToLoginCommand { get; }

    private async Task RegisterAsync(RegisterPasswordInput? passwordInput)
    {
        Message = string.Empty;

        if (passwordInput is null)
        {
            Message = "Password is required.";
            return;
        }

        if (passwordInput.Password != passwordInput.ConfirmPassword)
        {
            Message = "Password confirmation does not match.";
            return;
        }

        IsBusy = true;

        try
        {
            var result = await _authService.RegisterStudentAsync(new StudentRegistrationDto
            {
                StudentCode = StudentCode,
                FullName = FullName,
                Password = passwordInput.Password,
                Gender = Gender,
                Email = Email,
                Phone = Phone
            });

            Message = result.Message;

            if (result.IsSuccess)
            {
                _onBackToLogin();
            }
        }
        catch (Exception ex)
        {
            Message = $"Cannot register: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public class RegisterPasswordInput
{
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
