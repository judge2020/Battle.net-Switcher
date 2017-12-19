namespace BattlenetAccountSwitcher
{
    public class SteamAccount
    {
	    public string Name { get; set; }

	    public string Username { get; set; }

	    public string Password { get; set; }

	    public AccountType Type { get; set; }

	    public AutoStart AutoStart { get; set; } = AutoStart.None;

	    public override string ToString()
        {
            return Name + "~ (user: " + Username + ")";
        }
    }
}
