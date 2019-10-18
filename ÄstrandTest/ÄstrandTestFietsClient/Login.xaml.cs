using Networking;
using Networking.ÄstrandTest;
using Networking.ÄstrandTest.Net.ClientSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ÄstrandTestFietsClient
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window, IÄstrandClientConnector
    {
        private ÄstrandClient astrandClient;

        private ThicknessAnimation slideAnimation;
        private Storyboard storyBoard;

        private bool isRegistering;

        public Login()
        {
            InitializeComponent();

            this.slideAnimation = new ThicknessAnimation();
            this.slideAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath(Grid.MarginProperty));
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath(Grid.MarginProperty));
            this.storyBoard = new Storyboard();
            this.storyBoard.Children.Add(this.slideAnimation);
            this.isRegistering = false;

            cmb_Gender.ItemsSource = new string[2] { "Man", "Vrouw" };
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txb_Ip.Text) || !String.IsNullOrEmpty(txb_Port.Text))
            {
                this.astrandClient = new ÄstrandClient(txb_Ip.Text, int.Parse(txb_Port.Text), this, null);
                if (!this.astrandClient.Connect())
                {
                    lbl_Error.Content = "Kon geen connectie leggen, host is niet gevonden!";
                    lbl_Error.Visibility = Visibility.Visible;
                }
                else
                {
                    stk_Connect.Visibility = Visibility.Collapsed;
                    stk_Content.Visibility = Visibility.Visible;
                }
            }
            else
            {
                lbl_Error.Content = "Ip en Poort mogen niet leeg zijn!";
                lbl_Error.Visibility = Visibility.Visible;
            }
        }

        private void ShowRegister_Click(object sender, RoutedEventArgs e)
        {
            ScrollDown();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txb_LoginUsername.Text) && !String.IsNullOrEmpty(txb_LoginPassword.Password))
            {
                btn_Login.IsEnabled = false;
                btn_ShowRegister.IsEnabled = false;

                string usernameAndPasword = HashUtil.HashSha256(txb_LoginPassword.Password) + txb_LoginUsername.Text;
                this.astrandClient.Transmit(new Message(Message.ID.CLIENT_LOGIN, Message.State.NONE, Encoding.UTF8.GetBytes(usernameAndPasword)));
            }
            else
            {
                lbl_LoginError.Content = "Velden mogen niet leeg zijn!";
                lbl_LoginError.Visibility = Visibility.Visible;
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txb_RegisterUsername.Text) && !String.IsNullOrEmpty(txb_RegisterPassword.Password) && !String.IsNullOrEmpty(txb_RegisterConfirmPassword.Password)
                && !String.IsNullOrEmpty(txb_BirthYear.Text) && !String.IsNullOrEmpty(txb_Weight.Text))
            {
                if (txb_RegisterPassword.Password == txb_RegisterConfirmPassword.Password)
                {
                    btn_Register.IsEnabled = false;
                    btn_Back.IsEnabled = false;

                    List<byte> bytes = new List<byte>();
                    bytes.AddRange(Encoding.UTF8.GetBytes(HashUtil.HashSha256(txb_RegisterPassword.Password)));
                    bytes.Add((byte)txb_RegisterUsername.Text.Length);
                    bytes.AddRange(Encoding.UTF8.GetBytes(txb_RegisterUsername.Text));
                    bytes.Add((byte)txb_BirthYear.Text.Length);
                    bytes.AddRange(Encoding.UTF8.GetBytes(txb_BirthYear.Text));
                    bytes.Add((byte)int.Parse(txb_Weight.Text));
                    bytes.Add((cmb_Gender.SelectedItem.ToString().ToLower() == "man") ? (byte)1 : (byte)0);

                    this.astrandClient.Transmit(new Message(Message.ID.CLIENT_REGISTER, Message.State.NONE, bytes.ToArray()));
                }
                else
                {
                    lbl_RegisterError.Content = "Wachtwoorden komen niet overeen!";
                    lbl_RegisterError.Visibility = Visibility.Visible;
                }
            }
            else
            {
                lbl_RegisterError.Content = "Velden mogen niet leeg zijn!";
                lbl_RegisterError.Visibility = Visibility.Visible;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ScrollUp();
        }

        private void ScrollUp()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (this.isRegistering)
                {
                    this.slideAnimation.From = new Thickness(0, -600, 0, 0);
                    this.slideAnimation.To = new Thickness(0, 0, 0, 0);

                    storyBoard.Begin(stk_Content);
                    this.isRegistering = false;
                }
            }));
        }

        private void ScrollDown()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (!this.isRegistering)
                {
                    this.slideAnimation.From = new Thickness(0, 0, 0, 0);
                    this.slideAnimation.To = new Thickness(0, -600, 0, 0);

                    storyBoard.Begin(stk_Content);
                    this.isRegistering = true;
                }
            }));
        }

        public void OnMessageReceived(Message message)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                List<byte> content = new List<byte>(message.GetContent());

                switch (message.GetId())
                {
                    case Message.ID.CLIENT_REGISTER:
                        {
                            btn_Register.IsEnabled = true;
                            btn_Back.IsEnabled = true;

                            if (message.GetState() == Message.State.OK)
                            {
                                lbl_RegisterError.Visibility = Visibility.Hidden;
                                ScrollUp();
                            }
                            else if (message.GetState() == Message.State.ERROR)
                            {
                                lbl_RegisterError.Content = Encoding.UTF8.GetString(content.ToArray());
                                lbl_RegisterError.Visibility = Visibility.Visible;
                            }
                            break;
                        }
                    case Message.ID.CLIENT_LOGIN:
                        {
                            btn_Login.IsEnabled = true;
                            btn_ShowRegister.IsEnabled = true;

                            if (message.GetState() == Message.State.OK)
                            {
                                lbl_LoginError.Visibility = Visibility.Hidden;

                                int birthYear = int.Parse(Encoding.UTF8.GetString(content.GetRange(1, content[0]).ToArray()));
                                int weight = content[content[0] + 1];
                                bool isMan = (content[content[0] + 2] == 1);
                                UserLogin.Username = txb_LoginUsername.Text;
                                UserLogin.BirthYear = birthYear;
                                UserLogin.Weight = weight;
                                UserLogin.isMan = isMan;

                                MainWindow mainWindow = new MainWindow(this.astrandClient);
                                mainWindow.Show();
                                this.Close();
                            }
                            else if (message.GetState() == Message.State.ERROR)
                            {
                                lbl_LoginError.Content = Encoding.UTF8.GetString(content.ToArray());
                                lbl_LoginError.Visibility = Visibility.Visible;
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }));
        }

        public void OnConnectionLost()
        {
            
        }
    }
}
