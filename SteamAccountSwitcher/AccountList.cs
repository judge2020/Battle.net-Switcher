using System.Collections.Generic;

namespace BattlenetAccountSwitcher
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
