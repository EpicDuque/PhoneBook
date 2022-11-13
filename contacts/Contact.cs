using System;
using System.Collections.Generic;

namespace Contacts
{
    public class Contact
    {
        public string Name;
        public string LastName;
        public string Email;
        public long PhoneNumber;

        private const string formatPattern = "{0,-11}{1,-18}{2,-20}{3,-10}";
        
        public override bool Equals(object obj)
        {
            if(obj == null) return false;
            if(obj == this) return true;
            
            if(obj.GetType() != typeof(Contact)) return false;

            var contact = (Contact)obj;

            if (contact.Name == Name && contact.LastName == LastName) return true;
            if (contact.PhoneNumber == PhoneNumber) return true;

            return false;
        }

        public override string ToString()
        {
            return string.Format(formatPattern, Name, LastName, Email, PhoneNumber.ToString());
        }

        public static void DrawContactsHeader()
        {
            Console.WriteLine(formatPattern, "NAME", "LAST NAME", "PHONE NUMBER", "EMAIL");
        }

        public class NameComparer : IComparer<Contact>
        {
            public int Compare(Contact x, Contact y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }
        
        public class LastNameComparer : IComparer<Contact>
        {
            public int Compare(Contact x, Contact y)
            {
                return x.LastName.CompareTo(y.LastName);
            }
        }
        
        public class PhoneComparer : IComparer<Contact>
        {
            public int Compare(Contact x, Contact y)
            {
                return x.PhoneNumber.CompareTo(y.PhoneNumber);
            }
        }
        
        public class EmailComparer : IComparer<Contact>
        {
            public int Compare(Contact x, Contact y)
            {
                return x.Email.CompareTo(y.Email);
            }
        }
    }
}