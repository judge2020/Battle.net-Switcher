using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;
using Microsoft.Win32;
using System.Reflection;
namespace SteamAccountSwitcher
{
    /// ****
    /// SteamAccountSwitcher
    /// Copyright by Christoph Wedenig
    /// ****
    
    public partial class MainWindow
    {
	    private AccountList _accountList;
	    private readonly Steam _steam;

	    public MainWindow()
        {
            InitializeComponent();

			UpdateManager.Updater();
            Top = Properties.Settings.Default.Top;
            Left = Properties.Settings.Default.Left;
            Height = Properties.Settings.Default.Height;
            Width = Properties.Settings.Default.Width;

            if (Properties.Settings.Default.Maximized)
            {
                WindowState = WindowState.Maximized;
            }

            _accountList = new AccountList();
            
            //Get directory of Executable
            //GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)?.TrimStart(@"file:\\".ToCharArray());

            buttonInfo.ToolTip = "Build Version: " + Assembly.GetEntryAssembly().GetName().Version;

            try
            {
                ReadAccountsFromFile();
            }
            catch
            {
                //Maybe create file?
            }

            

            listBoxAccounts.ItemsSource = _accountList.Accounts;
            listBoxAccounts.Items.Refresh();

            if (_accountList.InstallDir == "" || (_accountList.InstallDir == null))
            {
                _accountList.InstallDir = SelectSteamFile(@"C:\Program Files (x86)\Battle.net");
                if(_accountList.InstallDir == null)
                {
                    MessageBox.Show("You cannot use Battle.net switcher without selecting your Battle.net.exe. Program will close now.", "BNET missing", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }

            _steam = new Steam(_accountList.InstallDir);
            
        }

        private string SelectSteamFile(string initialDirectory)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter =
               "Bnet |Battle.net.exe";
            dialog.InitialDirectory = initialDirectory;
            dialog.Title = "Select your Battle.net Installation";
            return (dialog.ShowDialog() == true)
               ? dialog.FileName : null;
        }

        private void buttonLogout_Click(object sender, RoutedEventArgs e)
        {
            _steam.LogoutSteam();
        }

        private void buttonAddAccount_Click(object sender, RoutedEventArgs e)
        {
            AddAccount newAccWindow = new AddAccount();
            newAccWindow.Owner = this;
            newAccWindow.ShowDialog();

            if (newAccWindow.Account != null)
            {
                _accountList.Accounts.Add(newAccWindow.Account);

                listBoxAccounts.Items.Refresh();
            }
        }

	    private void WriteAccountsToFile()
        {
            var xmlAccounts = ToXML(_accountList);
            
            var file = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\accounts.ini");
            file.Write(Crypto.Encrypt(xmlAccounts));
            
            file.Close();
        }

        public void ReadAccountsFromFile()
        {
            var text = Crypto.Decrypt(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\accounts.ini"));
            _accountList = FromXml<AccountList>(text);
        }

        public static T FromXml<T>(string xml)
        {
            using (StringReader stringReader = new StringReader(xml))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
        }

        public string ToXML<T>(T obj)
        {
            using (StringWriter stringWriter = new StringWriter(new StringBuilder()))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }

        private void listBoxAccounts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SteamAccount selectedAcc = (SteamAccount)listBoxAccounts.SelectedItem;
            _steam.StartSteamAccount(selectedAcc);
        }


        private void buttonEditAccount_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxAccounts.SelectedItem != null)
            {
                AddAccount newAccWindow = new AddAccount((SteamAccount)listBoxAccounts.SelectedItem);
                newAccWindow.Owner = this;
                newAccWindow.ShowDialog();

                if (newAccWindow.Account.Username != "" && newAccWindow.Account.Password != "")
                {
                    _accountList.Accounts[listBoxAccounts.SelectedIndex] = newAccWindow.Account;

                    listBoxAccounts.Items.Refresh();
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            WriteAccountsToFile();

            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.Maximized = true;
            }
            else
            {
                Properties.Settings.Default.Top = Top;
                Properties.Settings.Default.Left = Left;
                Properties.Settings.Default.Height = Height;
                Properties.Settings.Default.Width = Width;
                Properties.Settings.Default.Maximized = false;
            }

            Properties.Settings.Default.Save();
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image itemClicked = (Image)e.Source;

            SteamAccount selectedAcc = (SteamAccount)itemClicked.DataContext;
            MessageBoxResult dialogResult = MessageBox.Show("Are you sure you want to delete the '" + selectedAcc.Name + "' account?", "Delete Account", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                _accountList.Accounts.Remove((SteamAccount)listBoxAccounts.SelectedItem);
                listBoxAccounts.Items.Refresh();
            }
            else if (dialogResult == MessageBoxResult.No)
            {
                //do something else
            }
        }

        private void buttonInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Battle.net account switcher. \n \n Origionally steam account switcher by W3D3: https://github.com/W3D3/SteamAccountSwitcher \n This is an adaptation of the original.");
        }
    }
}
