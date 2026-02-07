using Xunit;
using Moq;

using Usersapi;
using Microsoft.AspNetCore.Mvc;

public class UsersControllerTests
{
    [Fact]
    public void GetUser_ReturnsOk_WhenUserExists()
    {
        var mockUserService = new Mock<IUsersService>();
        User user1 = new() { Name ="punith", Id=1, Address="blr, sada siva nagar"};
        mockUserService.Setup(s => s.AddUser(It.IsAny<User>())).Returns(user1);
        mockUserService.Setup(s => s.GetUsers()).Returns(new List<User> { user1 });

        var controller = new UsersController(mockUserService.Object);
        //Act
        var user = controller.PostUser(user1);
        var users = controller.GetUsers();

        //Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(user.Result);
        var returnedUser = Assert.IsType<User>(createdAtActionResult.Value);
        Assert.Equal(nameof(UsersController.PostUser), createdAtActionResult.ActionName);
        Assert.Equal(user1.Id, returnedUser.Id);
        Assert.Equal(201, createdAtActionResult.StatusCode);
        // Assert.Equal(okRes.StatusCode, 200);
        
    }

    [Fact]
    public void GetUsers_ReturnsOk_WithListOfUsers()
    {
        // Arrange
        var mockUserService = new Mock<IUsersService>();
        var usersList = new List<User>
        {
            new User { Id = 1, Name = "punith", Address = "blr, sada siva nagar" },
            new User { Id = 2, Name = "kumar", Address = "bangalore" }
        };
        mockUserService.Setup(s => s.GetUsers()).Returns(usersList);
        var controller = new UsersController(mockUserService.Object);

        // Act
        var result = controller.GetUsers();

        // Assert
        var actionResult = Assert.IsType<ActionResult<List<User>>>(result);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedUsers = Assert.IsType<List<User>>(okObjectResult.Value);
        Assert.Equal(2, returnedUsers.Count);
        Assert.Equal(usersList, returnedUsers);
    }
}
