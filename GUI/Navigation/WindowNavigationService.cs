using BUS.DTOs;
using GUI.State;
using GUI.Views.Admin;
using GUI.Views.Authentication;
using GUI.Views.Lecturer;
using GUI.Views.Student;
using System.Windows;

namespace GUI.Navigation;

public static class WindowNavigationService
{
    public static void OpenLogin(Window currentWindow)
    {
        SessionContext.Clear();
        OpenAndClose(currentWindow, new LoginWindow());
    }

    public static void OpenRegister(Window currentWindow)
    {
        OpenAndClose(currentWindow, new RegisterWindow());
    }

    public static void OpenHome(Window currentWindow, AuthenticatedUserDto user)
    {
        SessionContext.SetCurrentUser(user);

        Window homeWindow = user.Role switch
        {
            "Admin" => new DashboardWindow(user),
            "Lecturer" => new LecturerHomeWindow(user),
            "Student" => new StudentHomeWindow(user),
            _ => throw new InvalidOperationException($"Unsupported role: {user.Role}")
        };

        OpenAndClose(currentWindow, homeWindow);
    }

    private static void OpenAndClose(Window currentWindow, Window nextWindow)
    {
        nextWindow.Show();
        currentWindow.Close();
    }
}
