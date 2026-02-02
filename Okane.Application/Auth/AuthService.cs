using Okane.Domain;

namespace Okane.Application.Auth;

public class AuthService(
    IUsersRepository users, 
    IPasswordHasher passwordHasher, 
    ITokenGenerator tokenGenerator)
{
    public Result<SignUpResponse> SignUp(SignUpRequest request)
    {
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
}