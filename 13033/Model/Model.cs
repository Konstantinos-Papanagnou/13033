using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace _13033.Model
{
    public class Model
    {
        public int Code { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public bool ConfirmMessage()
        {
            if (Code == 0)
                return false;
            if (string.IsNullOrEmpty(Surname))
                return false;
            if (string.IsNullOrEmpty(Name))
                return false;
            if (string.IsNullOrEmpty(Address))
                return false;
            return true;
        }

        public string GetMessage()
        {
            return Code.ToString() + " " + Surname + " " + Name + " " + Address;
        }

        public Model(int Code, string Surname, string Name, string Address)
        {
            this.Code = Code;
            this.Surname = Surname;
            this.Name = Name;
            this.Address = Address;
        }
    }
}