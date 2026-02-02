using Okane.Application.Auth;
using Okane.Storage.InMemory;

namespace Okane.Tests;

public class AuthServiceTests
{
    private readonly AuthService _service;

    public AuthServiceTests() => 
        _service = new AuthService(
            new InMemoryUsersRepository(), 
            new FakePasswordHasher(), 
            new FakeTokenGenerator());

    [Fact]
    public void SignUp()
    {
        var response = _service.SignUp(new("test-user", "Password1!", "Password1!"))
            .AssertOk();
        
        Assert.Equal("test-user", response.Username);
    }
    
    [Fact]
    public void SingIn()
    {
        _service.SignUp(new("test-user", "Password1!", "Password1!"))
            .AssertOk();
        
        var response = _service.SignIn(new("test-user", "Password1!"))
            .AssertOk();
        
        Assert.Equal("token-test-user", response.Token);
    }
    
    [Fact]
    public void SingIn_PasswordDoesNotMatch()
    {
        _service.SignUp(new("test-user", "Password1!", "Password1!"))
            .AssertOk();
        
        var error = _service.SignIn(new("test-user", "WrongPass1!"))
            .AssertUnauthorized();
        
        Assert.Equal("Invalid username or password.", error);
    }
    
    [Fact]
    public void SingIn_UserDoesNotExist()
    {
        var error = _service.SignIn(new("test-user", "Password1!"))
            .AssertUnauthorized();
        
        Assert.Equal("Invalid username or password.", error);
    }

    [Fact]
    public void Signup_PasswordAndPasswordConfirmationDoNotMatch()
    {
        var error = _service.SignUp(new("test-user", "Password1!", "Password2!"))
            .AssertError();
        
        Assert.Equal("Password and password confirmation do not match.", error);
    }

    [Fact]
    public void SingUp_EmptyUsername()
    {
        var error = _service.SignUp(new("", "1234", "1234"))
            .AssertError();
        
        Assert.Equal("Username cannot be empty.", error);
    }

    [Fact]
    public void SingUp_WhiteEpaceUsername()
    {
        var error = _service.SignUp(new(" ", "1234", "1234"))
            .AssertError();
        
        Assert.Equal("Username cannot be empty.", error);
    }

    [Fact]
    public void SingUp_DuplicateUsername()
    { 
        _service.SignUp(new("same-user", "Password1!", "Password1!"))
            .AssertOk();

        var error = _service.SignUp(new("same-user", "OtherPass2!", "OtherPass2!"))
            .AssertError();
        
        Assert.Equal("Username is already taken.", error);
    }
    
    [Fact]
    public void SignUp_PasswordIsTooShort()
    {
        var error = _service.SignUp(new("test-user", "Ab1!", "Ab1!"))
            .AssertError();
        
        Assert.Equal("Password must be longer than 8 characters and contain at least one uppercase letter, one lowercase letter, one number and one special character.", error);
    }
    
    [Fact]
    public void SignUp_PasswordWithoutUppercaseLetter()
    {
        var error = _service.SignUp(new("test-user", "password1!", "password1!"))
            .AssertError();
            
        Assert.Equal("Password must be longer than 8 characters and contain at least one uppercase letter, one lowercase letter, one number and one special character.", error);
    }

    [Fact]
    public void SignUp_PasswordWithoutSpecialCharacter()
    {
        var error = _service.SignUp(new("test-user", "Password12", "Password12"))
            .AssertError();
            
        Assert.Equal("Password must be longer than 8 characters and contain at least one uppercase letter, one lowercase letter, one number and one special character.", error);
    }

    [Fact]
    public void SignUp_PasswordWithoutNumber()
    {
        var error = _service.SignUp(new("test-user", "Password!", "Password!"))
            .AssertError();
            
        Assert.Equal("Password must be longer than 8 characters and contain at least one uppercase letter, one lowercase letter, one number and one special character.", error);
    }

    [Fact]
    public void SignUp_PasswordWithoutLowercaseLetter()
    {
        var error = _service.SignUp(new("test-user", "PASSWORD1!", "PASSWORD1!"))
            .AssertError();
            
        Assert.Equal("Password must be longer than 8 characters and contain at least one uppercase letter, one lowercase letter, one number and one special character.", error);
    }

    [Fact]
    public void SignUp_ValidPassword()
    {
        var response = _service.SignUp(new("test-user", "Password1!", "Password1!"))
            .AssertOk();
            
        Assert.Equal("test-user", response.Username);
    }
}