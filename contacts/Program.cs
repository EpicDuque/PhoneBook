using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Contacts
{
    class Program
    {
        private static List<Contact> PhoneBook = new();

        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome to PhoneBook");
            
            PressAnyKey();

            bool exit = false;
            
            while (!exit)
            {
                ShowMenu();

                Console.Write("\nSelection: ");
                
                char option = Console.ReadKey(true).KeyChar;

                switch (option)
                {
                    case '0':
                        AddNewContact();
                        break;
                    
                    case '1':
                        EditContact();
                        break;
                    
                    case '2':
                        DeleteContact();
                        break;
                    
                    case '3':
                        ViewAllContacts();
                        break;
                    
                    case '4':
                        SaveContacts();
                        break;
                    
                    case '5':
                        LoadContacts();
                        break;
                    
                    case '6':
                        exit = true;
                        break;
                    
                    default:
                        WriteError("Menu option invalid");
                        PressAnyKey();
                        break;
                }
                
                Console.Clear();
            }
            
            Console.WriteLine("Thanks for using my NoteBook program, GoodBye!");
                
            PressAnyKey();
            
        }

        private static void PressAnyKey()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private static void WriteError(string message)
        {
            var oldColor = Console.ForegroundColor;
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: {message}");
            Console.ForegroundColor = oldColor;
        }

        private static void AddNewContact()
        {
            Console.WriteLine("Add New Contact\n");
            
            var contact = PromptContact();

            PhoneBook.Add(contact);
            
            Console.WriteLine("Contact successfully added");
            
            PressAnyKey();
        }
        
        // Ask user for a new contact information
        private static Contact PromptContact()
        {
            var contact = new Contact();
            
            Console.Write("Enter Name: ");
            contact.Name = Console.ReadLine();
            
            Console.Write("Enter Last Name: ");
            contact.LastName = Console.ReadLine();

            while (true)
            {
                try
                {
                    Console.Write("Enter Phone Number: ");
                    
                    // Convert from text to long number
                    contact.PhoneNumber = long.Parse(Console.ReadLine() ?? string.Empty);
                    break;
                }
                catch
                {
                    WriteError("Cannot parse Phone Number into a proper number");
                }
            }
            
            Console.Write("Enter Email: ");
            contact.Email = Console.ReadLine();

            return contact;
        }

        private static void EditContact()
        {
            Console.WriteLine("Edit Contact\n");

            var match = SearchContact();
            var tempContact = PromptContact();

            var index = PhoneBook.IndexOf(match);
            
            PhoneBook.RemoveAt(index);
            PhoneBook.Insert(index, tempContact);
            
            Console.WriteLine("Contact Edited Successfully!");
            
            PressAnyKey();
        }

        // Searches contact by name
        private static Contact SearchContact()
        {
            string name;
            while (true)
            {
                Console.Write("Search By Contact Name: ");
                name = Console.ReadLine();

                if (string.IsNullOrEmpty(name))
                {
                    WriteError("Please enter a Name");
                    continue;
                }

                break;
            }

            var matches = PhoneBook.Where(c => c.Name.Contains(name)).ToArray();
            if (matches.Length == 0)
            {
                WriteError("No Matches found!");
                PressAnyKey();
                return null;
            }

            while (true)
            {
                Console.WriteLine("Found Matches: \n");

                for (int i = 0; i < matches.Length; i++)
                {
                    Console.WriteLine($"[{i}]: {matches[i]})");
                }

                Console.Write("\nSelect your Match Number: ");

                int num;
                try
                {
                    num = Convert.ToInt32(Console.ReadKey().KeyChar.ToString());
                }
                catch
                {
                    Console.Clear();
                    continue;
                }

                if (num >= matches.Length)
                {
                    WriteError("\nMatch number out of bounds");
                    PressAnyKey();
                    continue;
                }

                var chosen = matches[num];
                return chosen;
            }
        }

        private static void DeleteContact()
        {
            Console.WriteLine("Delete Contact\n");
            var match = SearchContact();

            PhoneBook.Remove(match);
            
            Console.WriteLine("Contact deleted successfully");
            PressAnyKey(); 
        }

        private static void ViewAllContacts()
        {
            Console.WriteLine("View All Contacts\n");
            
            Console.WriteLine("[1] Name");
            Console.WriteLine("[2] Last Name");
            Console.WriteLine("[3] Phone Number");
            Console.WriteLine("[4] Email");
            
            Console.Write("\nSelect by which field the contact list would be sorted: ");

            var option = Console.ReadKey().KeyChar;

            Console.WriteLine();

            IComparer<Contact> comparer;

            switch (option)
            {
                case '1':
                    comparer = new Contact.NameComparer();
                    break;
                
                case '2':
                    comparer = new Contact.LastNameComparer();
                    break;
                
                case '3':
                    comparer = new Contact.PhoneComparer();
                    break;
                
                case '4':
                    comparer = new Contact.EmailComparer();
                    break;
                
                default:
                    WriteError("Invalid Selection");
                    PressAnyKey();
                    return;
            }

            var sortedContacts = new List<Contact>(PhoneBook);
            sortedContacts.Sort(comparer);
            
            var contactsPerPage = 10;
            var numPages = MathF.Ceiling(PhoneBook.Count / (float)contactsPerPage);
            
            for (int i = 0; i < numPages; i++)
            {
                Contact.DrawContactsHeader();
                Console.WriteLine();
                
                for (int j = i * contactsPerPage; j < (i+1) * contactsPerPage; j++)
                {
                    if (j >= sortedContacts.Count) break;
                    Console.WriteLine(sortedContacts[j]);
                }
                
                PressAnyKey();
                Console.Clear();
            }
        }

        private static void LoadContacts()
        {
            Console.WriteLine("Load Contacts\n");

            while (true)
            {

                Console.WriteLine("Enter Filename or leave empty to cancel: ");

                var filename = Console.ReadLine();

                if (string.IsNullOrEmpty(filename))
                    break;

                var loadedContacts = LoadContactsFromFile(filename);

                if (loadedContacts == null || loadedContacts.Count == 0)
                {
                    WriteError("An error occurred while loading contacts from file");
                }
                else
                {
                    PhoneBook.Clear();
                    
                    // Add all loaded contacts to the PhoneBook
                    PhoneBook.AddRange(loadedContacts);
                    
                    Console.WriteLine($"Contacts loaded successfully");
                }
                
                PressAnyKey();
                break;
            }
        }

        private static void SaveContacts()
        {
            Console.WriteLine("Save Contacts\n");

            while (true)
            {
                
                Console.WriteLine("Enter Filename: ");

                var filename = Console.ReadLine();

                if(string.IsNullOrEmpty(filename)) continue;
                
                if (File.Exists(filename))
                {
                    Console.Write($"File: {filename} already exists, overwrite? y/n: ");
                    var yesno = Console.ReadKey().KeyChar;

                    if (yesno == 'n') continue;
                }
                
                var writer = new StreamWriter(filename);

                foreach (var conatct in PhoneBook)
                {
                    writer.WriteLine(conatct.Name);
                    writer.WriteLine(conatct.LastName);
                    writer.WriteLine(conatct.PhoneNumber);
                    writer.WriteLine(conatct.Email);
                }
                
                writer.Close();

                Console.WriteLine("Contacts saved sucessfully!");
                
                PressAnyKey();
                break;
            }
        }

        private static List<Contact> LoadContactsFromFile(string filename)
        {
            List<Contact> contacts = new List<Contact>();
            
            if (!File.Exists(filename))
            {
                WriteError($"Could not find {filename}");
                PressAnyKey();
                
                // Returns empty contact list
                return contacts;
            }
                
            var reader = new StreamReader(filename);

            // Keep repeating the loop until we reach the end of the file
            while (!reader.EndOfStream)
            {
                var contact = new Contact();
                    
                contact.Name = reader.ReadLine();
                contact.LastName = reader.ReadLine();
                contact.PhoneNumber = long.Parse(reader.ReadLine());
                contact.Email = reader.ReadLine();
                
                contacts.Add(contact);
            }

            return contacts;
        }
        
        private static void ShowMenu()
        {
            Console.WriteLine("Main Menu");
            Console.WriteLine("--------------------------");
            Console.WriteLine("[0] Add New Contact");
            Console.WriteLine("[1] Edit Contact");
            Console.WriteLine("[2] Delete Contact");
            Console.WriteLine("[3] View all Contacts");
            Console.WriteLine("[4] Save All Contacts to File");
            Console.WriteLine("[5] Load Contacts from File");
            Console.WriteLine("[6] Exit");
        }
    }
}