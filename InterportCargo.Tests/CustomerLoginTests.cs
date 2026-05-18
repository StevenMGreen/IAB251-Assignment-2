using System.Security.Claims;
using InterportCargo.Web.Data;
using InterportCargo.Web.Models;
using InterportCargo.Web.Pages.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace InterportCargo.Tests;

public class CustomerLoginTests
{
    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates a fresh in-memory database for each test to ensure isolation.
    /// </summary>
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    /// <summary>
    /// Builds a CustomerLoginModel wired to the given database.
    /// When mockAuth is true, a mock IAuthenticationService is injected so
    /// SignInAsync can be called without a real HTTP pipeline.
    /// </summary>
    private static (CustomerLoginModel Model, Mock<IAuthenticationService> AuthMock)
        CreateModel(AppDbContext db, bool mockAuth = false)
    {
        var authMock = new Mock<IAuthenticationService>();
        authMock
            .Setup(a => a.SignInAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        var httpContext = new DefaultHttpContext();

        if (mockAuth)
        {
            httpContext.RequestServices = new ServiceCollection()
                .AddSingleton(authMock.Object)
                .BuildServiceProvider();
        }

        var model = new CustomerLoginModel(db)
        {
            PageContext = new PageContext { HttpContext = httpContext }
        };

        return (model, authMock);
    }

    /// <summary>
    /// Seeds a single customer with a known hashed password.
    /// </summary>
    private static async Task<Customer> SeedCustomerAsync(AppDbContext db,
        string email = "test@example.com",
        string plainPassword = "Password123!")
    {
        var customer = new Customer
        {
            FirstName = "Test",
            FamilyName = "User",
            Email = email,
            Phone = "0400000000",
            Address = "123 Test Street",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword)
        };
        db.Customers.Add(customer);
        await db.SaveChangesAsync();
        return customer;
    }

    // -------------------------------------------------------------------------
    // Typical scenarios
    // -------------------------------------------------------------------------

    [Fact]
    public async Task OnPostAsync_ValidCredentials_RedirectsToCustomerDashboard()
    {
        // Arrange
        var db = CreateDb();
        await SeedCustomerAsync(db);
        var (model, _) = CreateModel(db, mockAuth: true);
        model.Input = new LoginInput { Email = "test@example.com", Password = "Password123!" };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Customers/Dashboard", redirect.PageName);
    }

    [Fact]
    public async Task OnPostAsync_UnknownEmail_ReturnsPageWithAccountNotFoundError()
    {
        // Arrange
        var db = CreateDb(); // empty — no customers seeded
        var (model, _) = CreateModel(db);
        model.Input = new LoginInput { Email = "nobody@example.com", Password = "Password123!" };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.Contains(
            model.ModelState[string.Empty]!.Errors,
            e => e.ErrorMessage.Contains("No account found"));
    }

    [Fact]
    public async Task OnPostAsync_WrongPassword_ReturnsPageWithIncorrectPasswordError()
    {
        // Arrange
        var db = CreateDb();
        await SeedCustomerAsync(db, plainPassword: "CorrectPassword1!");
        var (model, _) = CreateModel(db);
        model.Input = new LoginInput { Email = "test@example.com", Password = "WrongPassword1!" };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
        Assert.Contains(
            model.ModelState[string.Empty]!.Errors,
            e => e.ErrorMessage.Contains("Incorrect password"));
    }

    // -------------------------------------------------------------------------
    // Boundary / validation conditions
    // -------------------------------------------------------------------------

    [Fact]
    public async Task OnPostAsync_EmptyEmail_ReturnsPageWithInvalidModelState()
    {
        // Arrange
        var db = CreateDb();
        var (model, _) = CreateModel(db);
        model.Input = new LoginInput { Email = "", Password = "Password123!" };
        model.ModelState.AddModelError("Input.Email", "The Email field is required.");

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
    }

    [Fact]
    public async Task OnPostAsync_EmptyPassword_ReturnsPageWithInvalidModelState()
    {
        // Arrange
        var db = CreateDb();
        var (model, _) = CreateModel(db);
        model.Input = new LoginInput { Email = "test@example.com", Password = "" };
        model.ModelState.AddModelError("Input.Password", "The Password field is required.");

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
    }

    [Fact]
    public async Task OnPostAsync_InvalidEmailFormat_ReturnsPageWithInvalidModelState()
    {
        // Arrange
        var db = CreateDb();
        var (model, _) = CreateModel(db);
        model.Input = new LoginInput { Email = "not-an-email", Password = "Password123!" };
        model.ModelState.AddModelError("Input.Email", "The Email field is not a valid e-mail address.");

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
    }

    // -------------------------------------------------------------------------
    // Claims verification
    // -------------------------------------------------------------------------

    [Fact]
    public async Task OnPostAsync_ValidCredentials_IssuesCustomerRoleAndCorrectNameIdentifier()
    {
        // Arrange
        var db = CreateDb();
        var customer = await SeedCustomerAsync(db);

        ClaimsPrincipal? capturedPrincipal = null;
        var authMock = new Mock<IAuthenticationService>();
        authMock
            .Setup(a => a.SignInAsync(
                It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()))
            .Callback<HttpContext, string, ClaimsPrincipal, AuthenticationProperties>(
                (_, _, principal, _) => capturedPrincipal = principal)
            .Returns(Task.CompletedTask);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddSingleton(authMock.Object)
                .BuildServiceProvider()
        };

        var model = new CustomerLoginModel(db)
        {
            PageContext = new PageContext { HttpContext = httpContext }
        };
        model.Input = new LoginInput { Email = "test@example.com", Password = "Password123!" };

        // Act
        await model.OnPostAsync();

        // Assert — correct role and customer ID are in the issued claims
        Assert.NotNull(capturedPrincipal);
        Assert.True(capturedPrincipal!.IsInRole("Customer"));
        Assert.Equal(
            customer.Id.ToString(),
            capturedPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}
