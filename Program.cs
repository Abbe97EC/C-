using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ContactManager
{
    public class Program
    {
        private static readonly string FilePath = "contacts.json";
        private static ContactService _contactService;

        static void Main(string[] args)
        {
            _contactService = new ContactService(FilePath);
            _contactService.LoadContacts();

            while (true)
            {
                Console.WriteLine("\nVälj ett alternativ:");
                Console.WriteLine("1. Lista alla kontakter");
                Console.WriteLine("2. Skapa en kontakt");
                Console.WriteLine("3. Avsluta");
                Console.Write("Ditt val: ");

                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        ListContacts();
                        break;
                    case "2":
                        CreateContact();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Ogiltigt val, försök igen.");
                        break;
                }
            }
        }

        private static void ListContacts()
        {
            var contacts = _contactService.GetContacts();
            if (contacts.Count == 0)
            {
                Console.WriteLine("Inga kontakter hittades.");
            }
            else
            {
                Console.WriteLine("\nKontakter:");
                foreach (var contact in contacts)
                {
                    Console.WriteLine($"- {contact.Name} ({contact.Phone})");
                }
            }
        }

        private static void CreateContact()
        {
            Console.Write("Ange namn: ");
            var name = Console.ReadLine();

            Console.Write("Ange telefonnummer: ");
            var phone = Console.ReadLine();

            _contactService.AddContact(new Contact { Name = name, Phone = phone });
            Console.WriteLine("Kontakt skapad.");
        }
    }

    public class Contact
    {
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    public class ContactService
    {
        private readonly string _filePath;
        private readonly List<Contact> _contacts;

        public ContactService(string filePath)
        {
            _filePath = filePath;
            _contacts = new List<Contact>();
        }

        public void LoadContacts()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var loadedContacts = JsonSerializer.Deserialize<List<Contact>>(json);
                if (loadedContacts != null)
                {
                    _contacts.AddRange(loadedContacts);
                }
            }
        }

        public void SaveContacts()
        {
            var json = JsonSerializer.Serialize(_contacts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public List<Contact> GetContacts()
        {
            return new List<Contact>(_contacts);
        }

        public void AddContact(Contact contact)
        {
            _contacts.Add(contact);
            SaveContacts();
        }
    }
}

// Enhetstester

using System;
using System.Collections.Generic;
using Xunit;
using System.IO;

namespace ContactManager.Tests
{
    public class ContactServiceTests
    {
        [Fact]
        public void AddContact_ShouldAddContactToList()
        {
            // Arrange
            var filePath = "test_contacts.json";
            if (File.Exists(filePath)) File.Delete(filePath);
            var service = new ContactService(filePath);

            // Act
            var contact = new Contact { Name = "Test", Phone = "12345" };
            service.AddContact(contact);

            // Assert
            var contacts = service.GetContacts();
            Assert.Single(contacts);
            Assert.Equal("Test", contacts[0].Name);
            Assert.Equal("12345", contacts[0].Phone);
        }

        [Fact]
        public void LoadContacts_ShouldLoadContactsFromFile()
        {
            // Arrange
            var filePath = "test_contacts.json";
            File.WriteAllText(filePath, "[ { \"Name\": \"Test\", \"Phone\": \"12345\" } ]");
            var service = new ContactService(filePath);

            // Act
            service.LoadContacts();

            // Assert
            var contacts = service.GetContacts();
            Assert.Single(contacts);
            Assert.Equal("Test", contacts[0].Name);
            Assert.Equal("12345", contacts[0].Phone);
        }
    }
}
