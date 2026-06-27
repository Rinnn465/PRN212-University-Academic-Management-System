using BUS.DTOs;
using BUS.Interfaces;
using GUI.Commands;
using System.Windows.Input;

namespace GUI.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly Action<AuthenticatedUserDto> _onLoginSuccess;
    private string _username = string.Empty;
    private string _message = string.Empty;
    private bool _isBusy;

    public LoginViewModel(IAuthService authService, Action<AuthenticatedUserDto> onLoginSuccess)
    {
        _authService = authService;
        _onLoginSuccess = onLoginSuccess;
        LoginCommand = new RelayCommand(async password => await LoginAsync(password as string), _ => !IsBusy);
    }

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
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
            if (SetProperty(ref _isBusy, value) && LoginCommand is RelayCommand command)
            {
                command.RaiseCanExecuteChanged();
            }
        }
    }

    public ICommand LoginCommand { get; }

    private async Task LoginAsync(string? password)
    {
        IsBusy = true;
        Message = string.Empty;

        try
        {
            var result = await _authService.LoginAsync(Username, password ?? string.Empty);

            if (!result.IsSuccess || result.User is null)
            {
                Message = result.Message;
                return;
            }

            _onLoginSuccess(result.User);
        }
        catch (Exception ex)
        {
            Message = $"Cannot login: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}

