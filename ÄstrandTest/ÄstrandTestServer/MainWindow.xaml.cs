using ÄstrandTestServer.Files;
using ÄstrandTestServer.Net;
using Networking;
using Networking.ÄstrandTest;
using Networking.ÄstrandTest.Net.ServerSide;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ÄstrandTestServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IÄstrandServerConnector, ILogger
    {
        private ÄstrandServer astrandServer;

        private List<UserAccount> clients;
        private List<UserAccount> specialists;
        private Dictionary<UserAccount, ÄstrandTest> currentTests;

        public MainWindow()
        {
            InitializeComponent();

            this.clients = new List<UserAccount>();
            this.specialists = new List<UserAccount>();
            this.currentTests = new Dictionary<UserAccount, ÄstrandTest>();

            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            this.astrandServer?.Stop();
            Environment.Exit(0);
        }

        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button.Content.ToString() == "Start")
            {
                if (!String.IsNullOrEmpty(txb_Ip.Text) || !String.IsNullOrEmpty(txb_Port.Text))
                {
                    this.astrandServer = new ÄstrandServer(txb_Ip.Text, int.Parse(txb_Port.Text), this, this);
                    this.astrandServer.Start();

                    txb_Ip.IsEnabled = false;
                    txb_Port.IsEnabled = false;
                    button.Content = "Stop";
                    lbl_Error.Visibility = Visibility.Hidden;
                }
                else
                {
                    lbl_Error.Content = "De velden Ip en Poort mogen niet leeg zijn!";
                    lbl_Error.Visibility = Visibility.Visible;
                }
            }
            else
            {
                this.astrandServer.Stop();
                this.astrandServer = null;
                txb_Ip.IsEnabled = true;
                txb_Port.IsEnabled = true;
                button.Content = "Start";
            }
        }

        public void OnMessageReceived(Message message, UserAccount user)
        {
            List<byte> content = new List<byte>(message.GetContent());

            switch (message.GetId())
            {
                case Message.ID.CLIENT_REGISTER:
                    {
                        string username = Encoding.UTF8.GetString(content.GetRange(65, content[64]).ToArray());
                        string password = Encoding.UTF8.GetString(content.GetRange(0, 64).ToArray());
                        int birthYear = int.Parse(Encoding.UTF8.GetString(content.GetRange(username.Length + password.Length + 2, content[username.Length + password.Length + 1]).ToArray()));
                        int weight = content[username.Length + password.Length + content[(username.Length + password.Length + 1)] + 2];
                        bool isMan = (content[username.Length + password.Length + content[(username.Length + password.Length + 1)] + 3] == 1);

                        HandleClientRegister(username, password, birthYear, weight, isMan, user);
                        break;
                    }
                case Message.ID.SPECIALIST_REGISTER:
                    {
                        string username = Encoding.UTF8.GetString(content.GetRange(64, content.Count - 64).ToArray());
                        string password = Encoding.UTF8.GetString(content.GetRange(0, 64).ToArray());

                        HandleSpecialistRegister(username, password, user);
                        break;
                    }
                case Message.ID.CLIENT_LOGIN:
                    {
                        string username = Encoding.UTF8.GetString(content.GetRange(64, content.Count - 64).ToArray());
                        string password = Encoding.UTF8.GetString(content.GetRange(0, 64).ToArray());

                        HandleClientLogin(username, password, user);
                        break;
                    }
                case Message.ID.SPECIALIST_LOGIN:
                    {
                        string username = Encoding.UTF8.GetString(content.GetRange(64, content.Count - 64).ToArray());
                        string password = Encoding.UTF8.GetString(content.GetRange(0, 64).ToArray());

                        HandleSpecialistLogin(username, password, user);
                        break;
                    }
                case Message.ID.LOGOUT:
                    {
                        HandleLogout(user);
                        break;
                    }
                case Message.ID.START_TEST:
                    {
                        if(user.IsAuthorized && this.clients.Contains(user))
                        {
                            if(!this.currentTests.Keys.Contains(user))
                            {
                                (int birthYear, int weight, bool isMan) personalData = FileHandler.GetPersonalData(user.Username);
                                this.currentTests.Add(user, new ÄstrandTest(user.Username, personalData.birthYear, personalData.weight, personalData.isMan));
                                BroadcastToSpecialists(new Message(Message.ID.START_TEST, Message.State.OK, Encoding.UTF8.GetBytes(user.Username)));
                            }              
                        }
                        break;
                    }
                case Message.ID.END_TEST:
                    {
                        if (user.IsAuthorized && this.clients.Contains(user))
                        {
                            if (this.currentTests.Keys.Contains(user))
                            {
                                if(message.GetContent().Length != 0)
                                {
                                    bool hasSteadyState = (content[0] == 1);
                                    double vo2 = double.Parse(Encoding.UTF8.GetString(content.GetRange(1, content.Count() - 1).ToArray()));

                                    ÄstrandTest astrandTest = this.currentTests[user];
                                    astrandTest.HasSteadyState = hasSteadyState;
                                    astrandTest.VO2 = vo2;

                                    FileHandler.SaveAstrandTestData(astrandTest);
                                    this.currentTests.Remove(user);
                                    BroadcastToSpecialists(new Message(Message.ID.END_TEST, Message.State.OK, Encoding.UTF8.GetBytes(user.Username)));
                                }
                            }
                        }
                        break;
                    }
                case Message.ID.BIKEDATA:
                    {
                        if (user.IsAuthorized && this.clients.Contains(user) && this.currentTests.Keys.Contains(user))
                            HandleBikeData(message.GetContent(), user);
                        break;
                    }
                case Message.ID.GET_TESTS:
                    {
                        if (user.IsAuthorized)
                            HandleGetTests(user);
                        break;
                    }
                case Message.ID.GET_TEST_DATA:
                    {
                        if (user.IsAuthorized)
                        {
                            string filename = Encoding.UTF8.GetString(message.GetContent());
                            HandleGetTestData(filename, user);
                        }
                        break;
                    }
            }
        }

        private void HandleClientRegister(string username, string password, int birthYear, int weight, bool isMan, UserAccount user)
        {
            if(Authorizer.AddNewClientAuthorization(username, password, birthYear, weight, isMan, DataEncryptor.FileKey))
                this.astrandServer.Transmit(new Message(Message.ID.CLIENT_REGISTER, Message.State.OK, null), user);
            else
                this.astrandServer.Transmit(new Message(Message.ID.CLIENT_REGISTER, Message.State.ERROR, Encoding.UTF8.GetBytes("Username is already in use!")), user);
        }

        private void HandleSpecialistRegister(string username, string password, UserAccount user)
        {
            if (Authorizer.AddNewSpecialistAuthorization(username, password, DataEncryptor.FileKey))
                this.astrandServer.Transmit(new Message(Message.ID.SPECIALIST_REGISTER, Message.State.OK, null), user);
            else
                this.astrandServer.Transmit(new Message(Message.ID.SPECIALIST_REGISTER, Message.State.ERROR, Encoding.UTF8.GetBytes("Username is already in use!")), user);
        }

        private void HandleClientLogin(string username, string password, UserAccount user)
        {
            if (Authorizer.CheckAuthorization(username, password, false, DataEncryptor.FileKey))
            {
                user.Username = username;
                user.IsAuthorized = true;
                this.clients.Add(user);

                (int birthYear, int weight, bool isMan) personalData = FileHandler.GetPersonalData(user.Username);
                List<byte> bytes = new List<byte>();
                bytes.Add((byte)personalData.birthYear.ToString().Length);
                bytes.AddRange(Encoding.UTF8.GetBytes(personalData.birthYear.ToString()));
                bytes.Add((byte)personalData.weight);
                bytes.Add((personalData.isMan) ? (byte)1 : (byte)0);

                this.astrandServer.Transmit(new Message(Message.ID.CLIENT_LOGIN, Message.State.OK, bytes.ToArray()), user);
            }
            else
                this.astrandServer.Transmit(new Message(Message.ID.CLIENT_LOGIN, Message.State.ERROR, Encoding.UTF8.GetBytes("Username or password is incorrect!")), user);
        }

        private void HandleSpecialistLogin(string username, string password, UserAccount user)
        {
            if (Authorizer.CheckAuthorization(username, password, true, DataEncryptor.FileKey))
            {
                user.Username = username;
                user.IsAuthorized = true;
                this.specialists.Add(user);
                this.astrandServer.Transmit(new Message(Message.ID.SPECIALIST_LOGIN, Message.State.OK, null), user);
            }
            else
                this.astrandServer.Transmit(new Message(Message.ID.SPECIALIST_LOGIN, Message.State.ERROR, Encoding.UTF8.GetBytes("Username or password is incorrect!")), user);
        }

        private void HandleLogout(UserAccount user)
        {
            if (user.IsAuthorized)
                user.IsAuthorized = false;

            if (this.clients.Contains(user))
            {
                this.clients.Remove(user);
                if (this.currentTests.Keys.Contains(user))
                    this.currentTests.Remove(user);
            }
            else if (this.specialists.Contains(user))
                this.specialists.Remove(user);
        }

        private void HandleBikeData(byte[] bikeData, UserAccount user)
        {
            List<byte> bikeDataBytes = new List<byte>();
            bikeDataBytes.Add((byte)user.Username.Length);
            bikeDataBytes.AddRange(Encoding.UTF8.GetBytes(user.Username));
            bikeDataBytes.AddRange(bikeData);
            BroadcastToSpecialists(new Message(Message.ID.BIKEDATA, Message.State.OK, bikeDataBytes.ToArray()));

            ÄstrandTest astrandTest = this.currentTests[user];

            List<byte> bytes = new List<byte>(bikeData);

            for (int i = 0; i < bytes.Count; i += 2)
            {
                Message.ValueId valueType = (Message.ValueId)bytes[i];
                int value = bytes[i + 1];
                DateTime dateTime = DateTime.Now;

                switch (valueType)
                {
                    case Message.ValueId.HEARTRATE:
                        {
                            astrandTest.HeartrateValues.Add((heartRate: value, time: dateTime));
                            break;
                        }
                    case Message.ValueId.DISTANCE:
                        {
                            astrandTest.DistanceValues.Add((distance: value, time: dateTime));
                            break;
                        }
                    case Message.ValueId.SPEED:
                        {
                            astrandTest.SpeedValues.Add((speed: value, time: dateTime));
                            break;
                        }
                    case Message.ValueId.CYCLE_RHYTHM:
                        {
                            astrandTest.CycleRhythmValues.Add((cycleRhythm: value, time: dateTime));
                            break;
                        }
                }
            }
        }

        private void HandleGetTests(UserAccount user)
        {
            List<string> tests = FileHandler.GetAllTests();

            if (tests.Count() != 0)
            {
                foreach (string test in tests)
                    this.astrandServer.Transmit(new Message(Message.ID.TEST_NAME, Message.State.OK, Encoding.UTF8.GetBytes(test)), user);
            }
            else
                this.astrandServer.Transmit(new Message(Message.ID.GET_TESTS, Message.State.ERROR, Encoding.UTF8.GetBytes("Kon geen tests vinden!")), user);
        }

        private void HandleGetTestData(string filename, UserAccount user)
        {
            if (FileHandler.TestExists(filename))
            {
                ÄstrandTest astrandTest = FileHandler.GetAstrandTestData(filename);

                if (astrandTest != null)
                    astrandTest.Transmit(user);
                else
                    this.astrandServer.Transmit(new Message(Message.ID.GET_TEST_DATA, Message.State.ERROR, Encoding.UTF8.GetBytes("Kon test niet openen!")), user);
            }
            else
                this.astrandServer.Transmit(new Message(Message.ID.GET_TEST_DATA, Message.State.ERROR, Encoding.UTF8.GetBytes("Kon test niet vinden!")), user);
        }

        public void OnUserConnected(UserAccount user)
        {
            
        }

        public void OnUserDisconnected(UserAccount user)
        {
            HandleLogout(user);
        }

        private void BroadcastToSpecialists(Message message)
        {
            foreach (UserAccount userAccount in this.specialists)
                this.astrandServer.Transmit(message, userAccount);
        }

        public void Log(string text)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                txb_Log.Text += text;
                txb_Log.ScrollToEnd();
            }));
        }
    }
}
