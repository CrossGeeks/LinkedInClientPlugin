using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkedInClientSample.ViewModels;
using Xamarin.Forms;

namespace LinkedInClientSample
{
	public partial class LoginPage : ContentPage
    {
		public LoginPage()
        {
            InitializeComponent();

			BindingContext = new LoginPageViewModel();
        }
    }
}
