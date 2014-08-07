using Android.App;
using Android.OS;
using AuthDemo.Core.ViewModels;
using Cirrious.MvvmCross.Droid.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace AuthDemo.Droid.Views
{
	[Activity(Label = "View for FirstViewModel")]
	public class FirstView : MvxActivity
	{
		private FirstViewModel model;
		private bool isloggingin = false;
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.FirstView);
			model = (FirstViewModel)this.DataContext;

			if(!isloggingin)
				LoginToFacebook();
		}

		private void LoginToFacebook()
		{
			isloggingin = true;
			var auth = new OAuth2Authenticator(
				clientId: "548850318570514",
				scope: "email",
				authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
				redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));

			// If authorization succeeds or is canceled, .Completed will be fired.
			auth.Completed += LoginComplete;

			StartActivity(auth.GetUI(this));
		}
		public void LoginComplete(object sender, AuthenticatorCompletedEventArgs e)
		{
			if (!e.IsAuthenticated)
			{
				Console.WriteLine("Not Authorised");
				return;
			}

			var accessToken = e.Account.Properties["access_token"].ToString();
			var expiresIn = Convert.ToDouble(e.Account.Properties["expires_in"]);
			var expiryDate = DateTime.Now + TimeSpan.FromSeconds(expiresIn);

			// Now that we're logged in, make a OAuth2 request to get the user's id.
			var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me"), null, e.Account);
						
			request.GetResponseAsync().ContinueWith(t =>
			{
				if (t.IsFaulted)
					model.Hello = "Error: " + t.Exception.InnerException.Message;
				else
				{
					string mail = t.Result.GetResponseText();
					
					Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(mail);
					string email;
					bool s = values.TryGetValue("email",out email);
					if (s)
					{
						model.Hello = email;
						model.UpdateProperties();
					}
					else
						model.Hello = "Error encountered at the absolutely last second";
				}
			});

		}
	}
}