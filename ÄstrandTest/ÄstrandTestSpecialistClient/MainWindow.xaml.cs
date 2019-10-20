using ÄstrandTestSpecialistClient.Net;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ÄstrandTestSpecialistClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IÄstrandClientConnector
    {
        private ÄstrandClient astrandClient;
        private List<TestData> runningTests;
        private List<string> testNames;

        private TestData selectedTest;
        private TestDataWindow testDataWindow;

        public MainWindow(ÄstrandClient astrandClient)
        {
            InitializeComponent();

            this.astrandClient = astrandClient;
            this.astrandClient.SetConnector(this);

            this.runningTests = new List<TestData>();
            this.testNames = new List<string>();

            this.astrandClient.Transmit(new Message(Message.ID.GET_TESTS, Message.State.OK, null));

            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            this.astrandClient.Disconnect();
            Environment.Exit(0);
        }

        private void HandleAddClient(string username)
        {
            TestData testdata = new TestData(username);
            this.runningTests.Add(testdata);

            StackPanel stackpanel = new StackPanel();
            stackpanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
            stackpanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            stackpanel.VerticalAlignment = VerticalAlignment.Top;
            stackpanel.Margin = new Thickness(5, 5, 5, 0);

            Label nameLabel = new Label();
            nameLabel.Foreground = Brushes.White;
            nameLabel.Margin = new Thickness(5, 5, 5, 5);
            nameLabel.Content = username;

            stackpanel.Children.Add(nameLabel);

            stackpanel.MouseDown += new MouseButtonEventHandler((object sender, MouseButtonEventArgs e) =>
            {
                this.selectedTest = testdata;
                con_TestData.Header = this.selectedTest.Username;
                con_TestData.Children.Clear();
                con_TestData.Children.Add(testdata.TestDataControl);
            });

            stackpanel.MouseEnter += new MouseEventHandler((object sender, MouseEventArgs e) =>
            {
                stackpanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF007ACC"));
            });

            stackpanel.MouseLeave += new MouseEventHandler((object sender, MouseEventArgs e) =>
            {
                stackpanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2D2D30"));
            });

            con_RunningTests.Children.Add(stackpanel);
        }

        private void HandleRemoveClient(string username)
        {
            if (this.runningTests.Where(c => c.Username == username).Count() != 0)
            {
                TestData testdata = this.runningTests.Where(c => c.Username == username).First();
                this.runningTests.Remove(testdata);

                if (this.selectedTest == testdata)
                    con_TestData.Children.Clear();

                StackPanel removeStackPanel = null;
                foreach (StackPanel stackPanel in con_RunningTests.Children)
                {
                    if ((stackPanel.Children[0] as Label).Content.ToString() == testdata.Username)
                    {
                        removeStackPanel = stackPanel;
                        break;
                    }
                }

                if (removeStackPanel != null)
                    con_RunningTests.Children.Remove(removeStackPanel);
            }
        }

        public void OnMessageReceived(Message message)
        {
            lock (message)
            {
                List<byte> content = new List<byte>(message.GetContent());

                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    switch (message.GetId())
                    {
                        case Message.ID.TEST_DATA_BEGIN:
                            {
                                if (message.GetState() == Message.State.OK)
                                {
                                    string username = Encoding.UTF8.GetString(content.GetRange(1, content[0]).ToArray());
                                    int birthYear = int.Parse(Encoding.UTF8.GetString(content.GetRange(username.Length + 2, content[username.Length + 1]).ToArray()));
                                    int weight = content[username.Length + content[username.Length + 1] + 2];
                                    bool isMan = (content[username.Length + content[username.Length + 1] + 3] == 1);

                                    bool hasSteadyState = (content[username.Length + content[username.Length + 1] + 4] == 1);
                                    int index = username.Length + content[username.Length + 1] + 5;
                                    double vo2 = double.Parse(Encoding.UTF8.GetString(content.GetRange(index + 1, content[index]).ToArray()));

                                    this.testDataWindow = new TestDataWindow(username, birthYear, weight, isMan, 20, hasSteadyState, vo2);
                                }
                                break;
                            }
                        case Message.ID.TEST_DATA_END:
                            {
                                if (message.GetState() == Message.State.OK)
                                {
                                    if (this.testDataWindow != null)
                                    {
                                        this.testDataWindow.ProcessHistoryData();
                                        this.testDataWindow.Show();
                                        this.testDataWindow = null;
                                        btn_GetTests.IsEnabled = true;
                                    }
                                }
                                break;
                            }
                        case Message.ID.TEST_DATA:
                            {
                                if (message.GetState() == Message.State.OK)
                                {
                                    string username = Encoding.UTF8.GetString(content.GetRange(1, content[0]).ToArray());
                                    HandleTestData(username, content.GetRange(content[0] + 1, content.Count() - (content[0] + 1)));
                                }
                                break;
                            }
                        case Message.ID.START_TEST:
                            {
                                if (message.GetState() == Message.State.OK)
                                {
                                    string username = Encoding.UTF8.GetString(message.GetContent());

                                    if (this.runningTests.Where(c => c.Username == username).Count() == 0)
                                        HandleAddClient(username);
                                }
                                break;
                            }
                        case Message.ID.END_TEST:
                            {
                                if (message.GetState() == Message.State.OK)
                                {
                                    HandleRemoveClient(Encoding.UTF8.GetString(message.GetContent()));
                                    this.astrandClient.Transmit(new Message(Message.ID.GET_TESTS, Message.State.OK, null));
                                }
                                break;
                            }
                        case Message.ID.BIKEDATA:
                            {
                                if(message.GetState() == Message.State.OK)
                                {
                                    string username = Encoding.UTF8.GetString(content.GetRange(1, content[0]).ToArray());

                                    if (this.runningTests.Where(c => c.Username == username).Count() == 0)
                                        HandleAddClient(username);

                                    TestData testdata = this.runningTests.Where(c => c.Username == username).First();
                                    testdata.HandleBikeData(content.GetRange(username.Length + 1, content.Count - (username.Length + 1)));
                                }
                                break;
                            }
                        case Message.ID.TEST_NAME:
                            {
                                if (message.GetState() == Message.State.OK)
                                {
                                    string filename = Encoding.UTF8.GetString(message.GetContent());

                                    if (!this.testNames.Contains(filename))
                                    {
                                        filename = filename.Replace("#", ":");
                                        this.testNames.Add(filename);
                                        cmf_TestNames.Value = this.testNames.ToArray();
                                    }
                                }
                                break;
                            }
                        case Message.ID.GET_TESTS:
                            {
                                if (message.GetState() == Message.State.ERROR)
                                    MessageBox.Show(Encoding.UTF8.GetString(message.GetContent()));
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }));
            }
        }

        private void HandleTestData(string username, List<byte> bytes)
        {
            if (this.testDataWindow != null)
            {
                //for (int i = 0; i < bytes.Count(); i += 21)
                //{
                //    int value = bytes[i + 1];
                //    DateTime time = DateTime.Parse(Encoding.UTF8.GetString(bytes.GetRange(i + 2, 19).ToArray()));

                //    switch ((Message.ValueId)bytes[i])
                //    {
                //        case Message.ValueId.HEARTRATE:
                //            {
                //                this.testDataWindow.AddHeartRate((value, time));
                //                break;
                //            }
                //        case Message.ValueId.DISTANCE:
                //            {
                //                this.testDataWindow.AddDistance((value, time));
                //                break;
                //            }
                //        case Message.ValueId.SPEED:
                //            {
                //                this.testDataWindow.AddSpeed((value, time));
                //                break;
                //            }
                //        case Message.ValueId.CYCLE_RHYTHM:
                //            {
                //                this.testDataWindow.AddCycleRyhthm((value, time));
                //                break;
                //            }
                //    }
                //}

                int skip = 21;
                for (int i = 0; i < bytes.Count; i += skip)
                {
                    Message.ValueId valueType = (Message.ValueId)bytes[i];

                    switch (valueType)
                    {
                        case Message.ValueId.HEARTRATE:
                            {
                                skip = 21;
                                int value = bytes[i + 1];
                                DateTime time = DateTime.Parse(Encoding.UTF8.GetString(bytes.GetRange(i + 2, 19).ToArray()));
                                this.testDataWindow.AddHeartRate((heartRate: value, time: time));
                                break;
                            }
                        case Message.ValueId.DISTANCE:
                            {
                                skip = bytes[i + 1] + 21;
                                int value = int.Parse(Encoding.UTF8.GetString(bytes.GetRange(i + 2, bytes[i + 1]).ToArray()));    
                                DateTime time = DateTime.Parse(Encoding.UTF8.GetString(bytes.GetRange(i + bytes[i + 1] + 2, 19).ToArray()));
                                this.testDataWindow.AddDistance((distance: value, time: time));
                                break;
                            }
                        case Message.ValueId.SPEED:
                            {
                                skip = 21;
                                int value = bytes[i + 1];
                                DateTime time = DateTime.Parse(Encoding.UTF8.GetString(bytes.GetRange(i + 2, 19).ToArray()));
                                this.testDataWindow.AddSpeed((speed: value, time: time));
                                break;
                            }
                        case Message.ValueId.CYCLE_RHYTHM:
                            {
                                skip = 21;
                                int value = bytes[i + 1];
                                DateTime time = DateTime.Parse(Encoding.UTF8.GetString(bytes.GetRange(i + 2, 19).ToArray()));
                                this.testDataWindow.AddCycleRyhthm((cycleRhythm: value, time: time));
                                break;
                            }
                    }
                }
            }
        }

        public void OnConnectionLost()
        {
            
        }

        private void GetTests_Click(object sender, RoutedEventArgs e)
        {
            if (cmf_TestNames.SelectedValue != null)
            {
                string filename = cmf_TestNames.SelectedValue.ToString();
                filename = filename.Replace(":", "#");
                btn_GetTests.IsEnabled = false;
                this.astrandClient.Transmit(new Message(Message.ID.GET_TEST_DATA, Message.State.NONE, Encoding.UTF8.GetBytes(filename)));
            }
            else
                MessageBox.Show("Er is geen test geselecteerd!");
        }
    }
}
