using ÄstrandTestFietsClient.BikeCommunication;
using Networking.ÄstrandTest;
using Networking.ÄstrandTest.Net.ClientSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using UIControls.Charts;

namespace ÄstrandTestFietsClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IÄstrandClientConnector, IClientMessageReceiver, IAvansAstrandTestConnector
    {
        private ÄstrandClient astrandClient;

        private DataManager dataManager;
        private RealBike bike;

        private LiveChartControl liveChartControl;

        private Thread testDataThread;

        private AvansAstrandTest astrandTest;

        private bool bikeIsConnected;
        private bool testInProgress;

        private int testHeartrate = 60;
        private int testDistance = 0;

        public MainWindow(ÄstrandClient astrandClient)
        {
            InitializeComponent();

            this.astrandClient = astrandClient;
            this.astrandClient.SetConnector(this);

            this.dataManager = new DataManager(this.astrandClient, this);

            this.liveChartControl = new LiveChartControl("Hartslag", "", "sl/pm", 40, 250, 180, 20, LiveChart.BlueGreenDarkTheme, true, true, true, true, false, true, true);
            Grid.SetColumn(this.liveChartControl, 1);
            grd_DataGrid.Children.Add(this.liveChartControl);

            this.bikeIsConnected = false;
            this.testInProgress = false;

            this.Closed += MainWindow_Closed;
            this.KeyUp += MainWindow_KeyUp;
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
                this.testHeartrate += 1;
            if(e.Key == Key.Down)
                this.testHeartrate -= 1;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            this.astrandClient.Disconnect();
            Environment.Exit(0);
        }

        private async void ConnectToBike_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txf_BikeId.Value))
            {
                this.bike = new RealBike(txf_BikeId.Value, this.dataManager);
                if (await this.bike.ConnectToBike())
                {
                    txf_BikeId.IsEnabled = false;
                    btn_ConnectToBike.IsEnabled = false;
                    btn_ConnectToBike.Foreground = Brushes.Gray;

                    this.bikeIsConnected = true;
                }
                else
                {
                    txf_BikeId.IsEnabled = true;
                    btn_ConnectToBike.IsEnabled = true;
                    btn_ConnectToBike.Foreground = Brushes.White;
                    MessageBox.Show("Kon geen verbinding maken met de fiets!");
                }
            }
            else
                MessageBox.Show("FietsId veld mag niet leeg zijn!");
        }

        public void OnMessageReceived(Message message)
        {
            
        }

        public void OnConnectionLost()
        {
            
        }

        private void SendTestBikeData()
        {
            this.testDataThread = new Thread(() =>
            {
                Random random = new Random();

                while (true)
                {
                    Thread.Sleep(1000);
                    this.testDistance += 2;

                    ClientMessage clientMessage = new ClientMessage();
                    clientMessage.HasHeartbeat = true;
                    clientMessage.HasPage16 = true;
                    clientMessage.HasPage25 = true;
                    clientMessage.Heartbeat = (byte)this.testHeartrate;
                    clientMessage.Distance = this.testDistance;
                    clientMessage.Speed = (byte)random.Next(2, 4);
                    clientMessage.Cadence = (byte)random.Next(40, 70);
                    HandleClientMessage(clientMessage);
                }
            });
            this.testDataThread.Start();
        }

        public void HandleClientMessage(ClientMessage clientMessage)
        {
            if(this.testInProgress)
            {
                if (this.astrandTest != null && this.astrandTest.IsRunning)
                    this.astrandClient.Transmit(new Message(Message.ID.BIKEDATA, Message.State.NONE, clientMessage.GetData()));

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    if (clientMessage.HasHeartbeat)
                    {
                        lbl_Heartrate.Content = clientMessage.Heartbeat;
                        this.liveChartControl.GetLiveChart().Update(clientMessage.Heartbeat);
                        this.astrandTest?.OnHeartrateReceived(clientMessage.Heartbeat);
                    }
                    if (clientMessage.HasPage16)
                    {
                        lbl_Distance.Content = clientMessage.Distance;
                        lbl_Speed.Content = clientMessage.Speed;
                        this.astrandTest?.OnDistanceRecieved(clientMessage.Distance);
                    }
                    if (clientMessage.HasPage25)
                    {
                        lbl_CycleRyhthm.Content = clientMessage.Cadence;
                        this.astrandTest?.OnCycleRyhthmReceived(clientMessage.Cadence);
                    }
                }));
            }
        }

        private void StartStopTest_Click(object sender, RoutedEventArgs e)
        {
            if((sender as Button).Content.ToString() == "Start test")
            {
                if(!this.bikeIsConnected && cbx_AllowTestData.IsChecked == true)
                    SendTestBikeData();

                if(this.bikeIsConnected || cbx_AllowTestData.IsChecked == true)
                {
                    if(!this.bikeIsConnected)
                    {
                        txf_BikeId.IsEnabled = false;
                        btn_ConnectToBike.IsEnabled = false;
                    }

                    lbl_NoConnection.Visibility = Visibility.Collapsed;

                    (sender as Button).Content = "Stop test";

                    this.liveChartControl.GetLiveChart().Clear();
                    lbl_LastHeartrate.Content = "Nog geen meeting gedaan";
                    lbl_Message.Content = "";
                    lbl_SteadyState.Content = "NEE";
                    lbl_SteadyState.Foreground = Brushes.Orange;
                    lbl_VO2.Content = "Nog niet berekend";
                    lbl_VO2.Foreground = Brushes.Orange;

                    this.testInProgress = true;

                    this.astrandTest = new AvansAstrandTest(this.bike, UserLogin.isMan, DateTime.Now.Year - UserLogin.BirthYear, this);
                    this.astrandTest.Start();
                }
                else
                {
                    lbl_NoConnection.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (!this.bikeIsConnected)
                {
                    txf_BikeId.IsEnabled = true;
                    btn_ConnectToBike.IsEnabled = true;
                }

                lbl_NoConnection.Visibility = Visibility.Visible;

                (sender as Button).Content = "Start test";

                if (this.testDataThread != null)
                    this.testDataThread.Abort();

                this.testInProgress = false;
                this.testHeartrate = 60;
                this.testDistance = 0;

                this.astrandTest.Stop("De test is handmatig afebroken!");
            }
        }

        public void OnAstrandTestStart()
        {
            this.astrandClient.Transmit(new Message(Message.ID.START_TEST, Message.State.NONE, null));
        }

        public void OnAstrandTestEnd(bool hasSteadyState, double vo2)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                btn_StartStopTest.Content = "Start test";
                if (this.testDataThread != null)
                    this.testDataThread.Abort();

                lbl_VO2.Content = Math.Round(vo2, 2).ToString() + ((hasSteadyState) ? "" : " (Steady state is niet behaald waarde is niet betrouwbaar!)");
                lbl_VO2.Foreground = Brushes.Green;
            }));

            List<byte> bytes = new List<byte>();
            bytes.Add((hasSteadyState) ? (byte)1 : (byte)0);
            bytes.AddRange(Encoding.UTF8.GetBytes(vo2.ToString()));
            this.astrandClient.Transmit(new Message(Message.ID.END_TEST, Message.State.NONE, bytes.ToArray()));
        }

        public void OnAstrandTestAbort(string message)
        {
            this.astrandClient.Transmit(new Message(Message.ID.END_TEST, Message.State.NONE, null));
            MessageBox.Show(message);
        }

        public void OnAstrandTestToFast()
        {
            lbl_Message.Content = "Je fietst te snel!";
            lbl_Message.Foreground = Brushes.Orange;
        }

        public void OnAstrandTestToSlow()
        {
            lbl_Message.Content = "Je fietst niet snel genoeg!";
            lbl_Message.Foreground = Brushes.Orange;
        }

        public void OnAstrandTestGoodSpeed()
        {
            lbl_Message.Content = "Je fietst op een goed tempo!";
            lbl_Message.Foreground = Brushes.Green;
        }

        public void OnAstrandTestSetResistance(int resistance)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                lbl_CurrentResitance.Content = resistance + "%";
            }));
        }

        public void OnAstrandTestSteadyStateReached()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                lbl_SteadyState.Content = "JA";
                lbl_SteadyState.Foreground = Brushes.Green;
            }));
        }

        public void OnAstrandTestLastHeartrateMeasured(int heartrate)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                lbl_LastHeartrate.Content = heartrate;
            }));
        }

        public void OnAstrandTestLogStateAndCountdown(AvansAstrandTest.State state, string timestring)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                lbl_Time.Content = timestring;

                switch (state)
                {
                    case AvansAstrandTest.State.NONE:
                        lbl_Fase.Content = "NIET BEZIG";
                        break;
                    case AvansAstrandTest.State.WARMUP:
                        lbl_Fase.Content = "WARMUP";
                        break;
                    case AvansAstrandTest.State.TESTFASE1:
                        lbl_Fase.Content = "TESTFASE 1";
                        break;
                    case AvansAstrandTest.State.TESTFASE2:
                        lbl_Fase.Content = "TESTFASE 2";
                        break;
                    case AvansAstrandTest.State.COOLDOWN:
                        lbl_Message.Content = "";
                        lbl_Fase.Content = "COOLDOWN";
                        break;
                    default:
                        break;
                }
            }));
        }
    }
}
