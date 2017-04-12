using System.Collections.Generic;

namespace SteamAccountSwitcher
{
    public class AccountList
    {
	    public AccountList()
        {
            Accounts = new List<SteamAccount>();
        }

        public string InstallDir { get; set; }

	    public List<SteamAccount> Accounts { get; }
    }
}
