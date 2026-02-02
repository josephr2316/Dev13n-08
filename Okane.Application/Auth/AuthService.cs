using Okane.Domain;

namespace Okane.Application.Auth;

public class AuthService(
    IUsersRepository users, 
    IPasswordHasher passwordHasher, 
    ITokenGenerator tokenGenerator)
{
    public Result<SignUpResponse> SignUp(SignUpRequest request)
    {
        if (users.ByUsername(request.Username) != null)
            return new ErrorResult<SignUpResponse>("Username is already taken.");
        
        if (string.IsNullOrWhiteSpace(request.Username))
            return new ErrorResult<SignUpResponse>("Username cannot be empty.");

        if (!IsValidPassword(request.Password))
            return new ErrorResult<SignUpResponse>("Password must be longer than 8 characters and contain at least one uppercase letter, one lowercase letter, one number and one special character.");

        if (request.Password != request.PasswordConfirmation)
            return new ErrorResult<SignUpResponse>("Password and password confirmation do not match.");

        users.Add(new User
        {
            Username = request.Username,
            HashedPassword = passwordHasher.Hash(request.Password),
        });
        
        return new OkResult<SignUpResponse>(new SignUpResponse(request.Username));
    }

    public Result<SignInResponse> SignIn(SignInRequest request)
    {
        var user = users.ByUsername(request.Username);
        
        if (user == null)
            return new UnauthorizedResult<SignInResponse>("Invalid username or password.");
        
        if (!passwordHasher.Verify(request.Password, user.HashedPassword))
            return new UnauthorizedResult<SignInResponse>("Invalid username or password.");

        var token = tokenGenerator.Generate(user);
        var response = new SignInResponse(token);
        return new OkResult<SignInResponse>(response);
    }

    private static bool IsValidPassword(string password)
    {
        return password.Length > 8
               && password.Any(char.IsUpper)
               && password.Any(char.IsLower)
               && password.Any(char.IsDigit)
               && password.Any(c => !char.IsLetterOrDigit(c));
    }
}