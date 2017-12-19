using System;
using System.Windows;

namespace BattlenetAccountSwitcher
{
    /// <summary>
    /// Interaction logic for AddAccount.xaml
    /// </summary>
    
    public partial class AddAccount
    {
	    public SteamAccount Account { get; private set; }

	    public AddAccount()
        {
            Account = new SteamAccount();
            InitializeComponent();
            comboBoxType.ItemsSource = Enum.GetValues(typeof(AccountType));
            ComboBoxAutoStart.ItemsSource = Enum.GetValues(typeof(AutoStart));
        }

        public AddAccount(SteamAccount editAccount)
        {
            InitializeComponent();
            Account = editAccount;
            comboBoxType.ItemsSource = Enum.GetValues(typeof(AccountType));
            ComboBoxAutoStart.ItemsSource = Enum.GetValues(typeof(AutoStart));

            comboBoxType.SelectedItem = editAccount.Type;
            ComboBoxAutoStart.SelectedItem = editAccount.AutoStart;
            textBoxProfilename.Text = editAccount.Name;
            textBoxUsername.Text = editAccount.Username;
            textBoxPassword.Password = editAccount.Password;
            
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Account.Type = (AccountType)comboBoxType.SelectedItem;
                Account.AutoStart = (AutoStart) ComboBoxAutoStart.SelectedItem;
                Account.Name = textBoxProfilename.Text;
                Account.Username = textBoxUsername.Text;
                Account.Password = textBoxPassword.Password;
            }
            catch
            {
                Account = null;
            }
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Account != null)
            {
                if (string.IsNullOrEmpty(Account.Username))
                {
                    Account = null;
                }
            }
        }
    }
}
