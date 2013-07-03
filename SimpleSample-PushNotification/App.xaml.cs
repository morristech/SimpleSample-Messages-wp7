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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Info;

namespace SimpleSample_PushNotification
{
    public partial class App : Application
    {
    
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Simple application ID
        /// </summary>
        public int AppID = 2495;
        /// <summary>
        /// Simple owner ID
        /// </summary>
        public int OwnerID = 0;
        /// <summary>
        /// Simple Authorization Key
        /// </summary>
        public string AuthKey = "BLCe37wsP7NWvPf";
        /// <summary>
        /// Simple Authorization Secret
        /// </summary>
        public string AuthSecret = "sLmB-XapnD8tRgO";

        public QuickBloxSDK_Silverlight.QuickBlox QBlox
        { get; set; }


        public string GetDeviceUniqueID()
        {
            try
            {
                byte[] result = null;
                object uniqueId;
                if (DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out uniqueId))
                    result = (byte[])uniqueId;

                return Convert.ToBase64String(result);
            }
            catch
            {
                return string.Empty;
            }
        }  
        public App()
        {

            UnhandledException += Application_UnhandledException;
            InitializeComponent();
            InitializePhoneApplication();

            //Start to use QuickBlox Service
            this.QBlox = new QuickBloxSDK_Silverlight.QuickBlox(AppID, OwnerID, this.AuthKey, this.AuthSecret, null, true, this.GetDeviceUniqueID());
            //Set the background events update rate - 3 sec
            this.QBlox.PingInterval = 3;
            //Start to work in background mode
            this.QBlox.BackgroundUpdateStart();
        }

       
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

      
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Инициализация приложения телефона

        // Избегайте двойной инициализации
        private bool phoneApplicationInitialized = false;

        // Не добавляйте в этот метод дополнительный код
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Создайте кадр, но не задавайте для него значение RootVisual; это позволит
            // экрану-заставке оставаться активным, пока приложение не будет готово для визуализации.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Обработка сбоев навигации
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Убедитесь, что инициализация не выполняется повторно
            phoneApplicationInitialized = true;
        }

        // Не добавляйте в этот метод дополнительный код
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Задайте корневой визуальный элемент для визуализации приложения
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Удалите этот обработчик, т.к. он больше не нужен
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}