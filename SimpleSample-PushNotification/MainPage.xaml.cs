using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;

//QuickBlox namespaces
using QuickBloxSDK_Silverlight.Core;
using QuickBloxSDK_Silverlight.PushNotification;
using QuickBloxSDK_Silverlight.users;

namespace SimpleSample_PushNotification
{
    public partial class MainPage : PhoneApplicationPage
    {

        /// <summary>
        /// Service Context
        /// </summary>
        public QuickBloxSDK_Silverlight.QuickBlox QBlox;

        /// <summary>
        /// Current Application Context
        /// </summary>
        public App AppContext;

        /// <summary>
        /// Simple user
        /// </summary>
        const string SimpleUser = @"";

        /// <summary>
        /// The passwod of this simple user
        /// </summary>
        const string SimpleUserPassword = "";

        /// <summary>
        /// Simple User id
        /// </summary>
        const int SimpleUserId = 0;

        /// <summary>
        /// Url address of Push
        /// </summary>
        private string URLPush;

        /// <summary>
        /// Current Channel
        /// </summary>
        private HttpNotificationChannel Channel = null;

        /// <summary>
        /// Connaection Status
        /// </summary>
        bool Connection = false;

        /// <summary>
        /// Token Id of Push
        /// </summary>
        private uint PushTokenId = 0;

        public MainPage()
        {
            //Create context
            var MainContext = App.Current as App;
            this.QBlox = MainContext.QBlox;

            //Add event handler for Background events
            this.QBlox.BackgroundEvent += new QuickBloxSDK_Silverlight.QuickBlox.BGR(QBlox_BackgroundEvent);

            //Add event handler for User Service events
            this.QBlox.userService.UserServiceEvent += new UserService.UserServiceHandler(userService_UserServiceEvent);

            //Add event handler for Push Notification events
            this.QBlox.pushNotificationService.PushNotificationServiceEvent += new PushNotificationService.PushNotificationServiceHandler(pushNotificationService_PushNotificationServiceEvent);

            InitializeComponent();
        }

        /// <summary>
        /// Methods to start when application main page is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            bool isNewChannel = false;

            string ChannelName = "QBTestChannel";

            this.Channel = HttpNotificationChannel.Find(ChannelName);

            //Set the new channel
            if (this.Channel == null)
            {
                this.Channel = new HttpNotificationChannel(ChannelName);
                isNewChannel = true;
            }

            //Set the event handler when channel uri has been updated
            this.Channel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(delegate(object senderObj, NotificationChannelUriEventArgs arg)
            {
                Dispatcher.BeginInvoke(() => 
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Chanel Uri was updated {0}", arg.ChannelUri));
                });                
            });
             
            //Set the event handler when status of connection has been changed
            this.Channel.ConnectionStatusChanged +=new EventHandler<NotificationChannelConnectionEventArgs>(delegate(object senderObj, NotificationChannelConnectionEventArgs arg)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("Connection status changed: {0}", arg.ConnectionStatus));
                    });                
                });

            //Set the event handler when error happened
            this.Channel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(delegate(object senderObj, NotificationChannelErrorEventArgs arg)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("Error: {0}", arg.Message));
                    });
                });               

            //Set the event handler when received http notifications
            this.Channel.HttpNotificationReceived += new EventHandler<HttpNotificationEventArgs>(delegate(object senderObj, HttpNotificationEventArgs arg)
                {                    
                    Dispatcher.BeginInvoke(() =>
                    {
                        //you get the raw message that you can parse as you like
                        System.Diagnostics.Debug.WriteLine("Raw message received.");                        
                    });
                });

            //Set the event handler when toast notification received
            this.Channel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(delegate(object senderObj, NotificationEventArgs arg)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string key in arg.Collection.Keys)
                        sb.AppendFormat("{0}:{1}\n", key, arg.Collection[key]);
                    string result = sb.ToString();
                    Dispatcher.BeginInvoke(() =>
                    {
                        System.Diagnostics.Debug.WriteLine(result);
                        MessageBox.Show(result);
                    });
                });

            //Work with channels

            if (isNewChannel)
                this.Channel.Open();

            if (!this.Channel.IsShellToastBound)
                this.Channel.BindToShellToast();

            if (!this.Channel.IsShellTileBound)
                this.Channel.BindToShellTile();
        }

        /// <summary>
        /// Event handler for events of  Push Notification Service
        /// </summary>
        /// <param name="Args"></param>
        void pushNotificationService_PushNotificationServiceEvent(QuickBloxSDK_Silverlight.PushNotification.PushNotificationEventArgs Args)
        {            
            this.Dispatcher.BeginInvoke(new Action(() => 
            { 
                switch (Args.currentCommand)
                {
                        #region CreateEvent
                case QuickBloxSDK_Silverlight.PushNotification.PushNotificationCommand.CreateEvent:
                    {
                        if (Args.status == Status.OK)
                        {                            
                            System.Diagnostics.Debug.WriteLine("CreateEvent - YES");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("CreateEvent - NO\n ErrorMessage:" + Args.errorMessage);
                        }
                        break;
                    }
                #endregion
                        #region CreatePushToken
                case QuickBloxSDK_Silverlight.PushNotification.PushNotificationCommand.CreatePushToken:
                    {
                        if (Args.status == Status.OK)
                        {
                            this.URLPush = ((PushToken)Args.result).ClientIdentificationSequence;
                            this.PushTokenId = ((PushToken)Args.result).Id;
                            System.Diagnostics.Debug.WriteLine("CreatePushToken - YES");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("CreatePushToken - NO\n ErrorMessage: " + Args.errorMessage);
                        }
                        break;
                    }
                #endregion
                        #region CreateSubscriptions
                case QuickBloxSDK_Silverlight.PushNotification.PushNotificationCommand.CreateSubscriptions:
                    {
                        if (Args.status == Status.OK)
                        {
                           // this.SubscribeId = ((Subscription)Args.result).Id;
                            System.Diagnostics.Debug.WriteLine("CreateSubscriptions - YES");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("CreateSubscriptions - NO\n ErrorMessage:" + Args.errorMessage);
                        }
                        break;
                    }
                #endregion
                        #region DeletePushToken
                case QuickBloxSDK_Silverlight.PushNotification.PushNotificationCommand.DeletePushToken:
                    {
                        if (Args.status == Status.OK)
                        {
                            System.Diagnostics.Debug.WriteLine("DeletePushToken - YES");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("DeletePushToken - NO\n ErrorMessage:" + Args.errorMessage);
                        }
                        break;
                    }
                #endregion
                        #region DeleteSubscribe
                case QuickBloxSDK_Silverlight.PushNotification.PushNotificationCommand.DeleteSubscribe:
                    {
                        if (Args.status == Status.OK)
                        {
                            System.Diagnostics.Debug.WriteLine("DeleteSubscribe - YES");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("DeleteSubscribe - NO\n ErrorMessage:" + Args.errorMessage);
                        }
            
                        break;
                    }
                #endregion
                        #region GetSubscriptions
                case QuickBloxSDK_Silverlight.PushNotification.PushNotificationCommand.GetSubscriptions:
                    {
                        if (Args.status == Status.OK)
                        {
                            System.Diagnostics.Debug.WriteLine("GetSubscriptions - YES");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("GetSubscriptions - NO\n ErrorMessage:" + Args.errorMessage);
                        }
                        break;
                    }
                #endregion
                }
            }));
        }

        /// <summary>
        /// Event handler for User service events
        /// </summary>
        /// <param name="Args"></param>
        void userService_UserServiceEvent(QuickBloxSDK_Silverlight.users.UserServiceEventArgs Args)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if (Args.currentCommand == QuickBloxSDK_Silverlight.users.UserServiceCommand.Authenticate)
                    {
                        if (Args.status == Status.OK)
                        {
                            //If User has been connected then create token for push
                            this.QBlox.pushNotificationService.CreatePushToken(this.Channel.ChannelUri.ToString());

                            System.Diagnostics.Debug.WriteLine("User online");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("User ofline");
                        }
                    }
                }
                catch (Exception ex) 
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                };

            }));
        }

        /// <summary>
        /// Event handler for background events
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Result"></param>
        void QBlox_BackgroundEvent(string Command, object Result)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (Command == "Online")
                {
                    //If application is online - auth User (this is for simple user)
                    this.QBlox.userService.Authenticate(SimpleUser, SimpleUserPassword);
                }
                if (!Connection)
                    if (Command == "Connection")
                    {
                        Connection = true;
                        System.Diagnostics.Debug.WriteLine("App connetion");
                    }
                if (Command == "Offline")
                    System.Diagnostics.Debug.WriteLine("Connect error");
            }));
        }                

        /// <summary>
        /// Send Simple message to yourself by using pushnotifications
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appBarSendMessage_Click(object sender, System.EventArgs e)
        {
            if (!CheckConnect())
                return;

            string notificationHeader = "Simple Push Notification";

            string textMesssage = "This is test notification. Please try it. Thanks to Chiba!";

            //Page that will be opened when you tap on the toast.
            string pagePath = "/Main.xaml";

            //Create new Notifications with Title and Text
            string notificationMessage = new PushNotification(PushNotificationType.Toast, notificationHeader,textMesssage, pagePath).ToBase64String();

            //Create notification
            //In order to make it work you need to put your message betweeen  "mpns=" and "26headers=Q29udGV..."
            this.QBlox.pushNotificationService.CreateEvent(new int[] { SimpleUserId }, 
                                                           "mpns=" + notificationMessage + "%26headers=Q29udGVudC1UeXBlLHRleHQveG1sLENvbnRlbnQtTGVuZ3RoLDIxOCxYLU5vdGlmaWNhdGlvbkNsYXNzLDIsWC1XaW5kb3dzUGhvbmUtVGFyZ2V0LHRvYXN0");
        }

        /// <summary>
        /// Get Subcriptions for device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appBarGetSubScription_Click(object sender, System.EventArgs e)
        {
            if (!CheckConnect())
                return;
            
            //Get Subscription
            this.QBlox.pushNotificationService.GetSubscriptions();
        }

        /// <summary>
        /// Create Subscription
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appBarCreateSubScription_Click(object sender, System.EventArgs e)
        {
        	if (!CheckConnect())
                return;

            //Create subscription
            this.QBlox.pushNotificationService.CreateSubscription(this.URLPush, QuickBloxSDK_Silverlight.PushNotification.NotificationType.mpns);
        }

        /// <summary>
        /// Remove subscription
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appBarRemoveSubscibe_Click(object sender, System.EventArgs e)
        {
            if (!CheckConnect())
                return;

            //Get id from "get subscription" method
            uint subscriptionId = 13;

            //Delete this subscription
            this.QBlox.pushNotificationService.DeleteSubscribe(subscriptionId);

            //Delete push notifications
            this.QBlox.pushNotificationService.DeletePushToken(this.PushTokenId);
        }


        /// <summary>
        /// Method to check connection
        /// </summary>
        /// <returns></returns>
        private bool CheckConnect()
        {
            if (!QBlox.IsOnline)
            {
                System.Diagnostics.Debug.WriteLine("Application is offline. Please wait untill application connect to server and try again.");
                return false;
            }
            else
                return true;
        }        
    }
}