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
    public partial class MainWindow : Window, IÄstrandClientConnector, IClientMessageReceiver
    {
        private ÄstrandClient astrandClient;

        private DataManager dataManager;
        private RealBike bike;

        private LiveChartControl liveChartControl;

        private Thread testDataThread;

        public MainWindow(ÄstrandClient astrandClient)
        {
            InitializeComponent();

            this.astrandClient = astrandClient;
            this.astrandClient.SetConnector(this);

            this.dataManager = new DataManager(this.astrandClient, this);

            this.liveChartControl = new LiveChartControl("Hartslag", "", "", 40, 250, 180, 20, LiveChart.BlueGreenDarkTheme, true, true, true, true, false, false, true);
            Grid.SetColumn(this.liveChartControl, 1);
            grd_DataGrid.Children.Add(this.liveChartControl);

            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            this.astrandClient.Disconnect();
            Environment.Exit(0);
        }

        private bool ConnectToBike()
        {
            if (!String.IsNullOrEmpty(txf_BikeId.Value))
            {
                this.bike = new RealBike(txf_BikeId.Value, this.dataManager);
                return true;
            }

            MessageBox.Show("FietsId veld mag niet leeg zijn!");
            return false;
        }

        private void ConnectToBike_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectToBike())
            {
                txf_BikeId.IsEnabled = false;
                btn_ConnectToBike.IsEnabled = false;
                btn_ConnectToBike.Foreground = Brushes.Gray;
            }
        }

        private void SendTestData_Click(object sender, RoutedEventArgs e)
        {
            this.astrandClient.Transmit(new Message(Message.ID.START_TEST, Message.State.NONE, null));
            SendTestBikeData();
            btn_SendTestData.IsEnabled = false;
            btn_SendTestData.Foreground = Brushes.Gray;
            btn_StopSendTestData.IsEnabled = true;
            btn_StopSendTestData.Foreground = Brushes.White;
        }

        private void StopSendTestData_Click(object sender, RoutedEventArgs e)
        {
            this.testDataThread.Abort();
            this.astrandClient.Transmit(new Message(Message.ID.END_TEST, Message.State.NONE, null));
            btn_SendTestData.IsEnabled = true;
            btn_SendTestData.Foreground = Brushes.White;
            btn_StopSendTestData.IsEnabled = false;
            btn_StopSendTestData.Foreground = Brushes.Gray;
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

                    ClientMessage clientMessage = new ClientMessage();
                    clientMessage.HasHeartbeat = true;
                    clientMessage.HasPage16 = true;
                    clientMessage.HasPage25 = true;
                    clientMessage.Heartbeat = (byte)random.Next(10, 100);
                    clientMessage.Distance = (byte)random.Next(10, 100);
                    clientMessage.Speed = (byte)random.Next(10, 100);
                    clientMessage.Cadence = (byte)random.Next(10, 100);
                    HandleClientMessage(clientMessage);

                    List<byte> bytes = new List<byte>();
                    //bytes.Add((byte)Message.ValueId.HEARTRATE);
                    //bytes.Add((byte)random.Next(10, 100));
                    //bytes.Add((byte)Message.ValueId.DISTANCE);
                    //bytes.Add((byte)random.Next(5, 20));
                    //bytes.Add((byte)Message.ValueId.SPEED);
                    //bytes.Add((byte)random.Next(0, 50));
                    //bytes.Add((byte)Message.ValueId.CYCLE_RHYTHM);
                    //bytes.Add((byte)random.Next(20, 60));
                    bytes.Add((byte)Message.ValueId.HEARTRATE);
                    bytes.Add(clientMessage.Heartbeat);
                    bytes.Add((byte)Message.ValueId.DISTANCE);
                    bytes.Add(clientMessage.Distance);
                    bytes.Add((byte)Message.ValueId.SPEED);
                    bytes.Add(clientMessage.Speed);
                    bytes.Add((byte)Message.ValueId.CYCLE_RHYTHM);
                    bytes.Add(clientMessage.Cadence);

                    this.astrandClient.Transmit(new Message(Message.ID.BIKEDATA, Message.State.NONE, bytes.ToArray()));
                }
            });
            this.testDataThread.Start();
        }

        public void HandleClientMessage(ClientMessage clientMessage)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (clientMessage.HasHeartbeat)
                {
                    lbl_Heartrate.Content = clientMessage.Heartbeat;
                    this.liveChartControl.GetLiveChart().Update(clientMessage.Heartbeat);
                }
                if (clientMessage.HasPage16)
                {
                    lbl_Distance.Content = clientMessage.Distance;
                    lbl_Speed.Content = clientMessage.Speed;
                }
                if (clientMessage.HasPage25)
                {
                    lbl_CycleRyhthm.Content = clientMessage.Cadence;
                }
            }));
        }
    }
}
