using ShopQaWPF.DTO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShopQaWPF.Customer
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private UserDto _user;
        public UserDto User
        {
            get => _user;
            set { _user = value; OnPropertyChanged(); }
        }

        private string _address;
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }
        private string _city;
        public string City { get => _city; set { _city = value; OnPropertyChanged(); } }
        private string _country;
        public string Country { get => _country; set { _country = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
