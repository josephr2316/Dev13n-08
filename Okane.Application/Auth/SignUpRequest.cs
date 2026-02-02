namespace Okane.Application.Auth;

public record SignUpRequest(string Username, string Password, string PasswordConfirmation);
