using System;
using System.ComponentModel;

namespace LinkedInClientSample.Models
{
	public class LinkedInUser : INotifyPropertyChanged
    {
        public string AccessToken { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Uri Picture { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
