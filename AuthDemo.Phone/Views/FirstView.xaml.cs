using Cirrious.MvvmCross.WindowsPhone.Views;
using Facebook;
using Facebook.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace AuthDemo.Phone.Views
{
	public partial class FirstView : MvxPhonePage
	{
		public FirstView()
		{
			 InitializeComponent();
            this.Loaded += FacebookLoginPage_Loaded;
		}

		async void FacebookLoginPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (!App.isAuthenticated)
			{
				App.isAuthenticated = true;
				await Authenticate();
			}
		}

		private FacebookSession session;
		private async Task Authenticate()
		{
            string message = String.Empty;
            try
            {
                session = await App.FacebookSessionClient.LoginAsync("email,user_about_me,read_stream");
                App.AccessToken = session.AccessToken;
                App.FacebookId = session.FacebookId;

                LoadUserInfo();
            }
            catch (InvalidOperationException e)
            {
                message = "Login failed! Exception details: " + e.Message;
                MessageBox.Show(message);
            }
		}

        private void LoadUserInfo()
        {
            var fb = new FacebookClient(App.AccessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();
                Console.WriteLine(result);
            
            };

            fb.GetTaskAsync("me");
        }


	}
}