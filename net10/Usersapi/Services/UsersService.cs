namespace Usersapi;

public class UsersService
{
    List<User> _users = new List<User>();

    public List<User> GetUsers()
    {
        return _users;
    }

    public List<User> AddUser(User user)
    {
        _users.Add(user);
        return user;
    }
}