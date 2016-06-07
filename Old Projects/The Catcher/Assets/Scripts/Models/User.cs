
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string Token { get; set; }
    public bool Remember { get; set; }

    public User()
    {
        Login = string.Empty;
        Password = string.Empty;
        Token = string.Empty;
        Remember = false;
    }
}
