namespace Usersapi;
public interface IUsersService
{
    List<User> GetUsers();
    User AddUser(User user);
}