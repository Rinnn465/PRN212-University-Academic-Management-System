using BUS.DTOs;

namespace GUI.State;

public static class SessionContext
{
    public static AuthenticatedUserDto? CurrentUser { get; private set; }

    public static bool IsAuthenticated => CurrentUser is not null;

    public static void SetCurrentUser(AuthenticatedUserDto user)
    {
        CurrentUser = user;
    }

    public static void Clear()
    {
        CurrentUser = null;
    }
}
