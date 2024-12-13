using System;
using System.Collections.Generic;
using System.IO;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public List<Message> Inbox { get; set; } = new List<Message>();
    public List<Message> SentMessages { get; set; } = new List<Message>();

    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }
}

public class Message
{
    public string From { get; set; }
    public string To { get; set; }
    public string Content { get; set; }
    public DateTime DateSent { get; set; }

    public Message(string from, string to, string content)
    {
        From = from;
        To = to;
        Content = content;
        DateSent = DateTime.Now;
    }
}

public class MailBox
{
    private Dictionary<string, User> users = new Dictionary<string, User>();

    public bool RegisterUser(string username, string password)
    {
        if (users.ContainsKey(username))
        {
            Console.WriteLine("User already exists.");
            return false;
        }

        users.Add(username, new User(username, password));
        Console.WriteLine("Registration successful!");
        return true;
    }

    public User Authenticate(string username, string password)
    {
        if (users.ContainsKey(username) && users[username].Password == password)
        {
            return users[username];
        }
        return null;
    }

    public void SendMessage(User fromUser, string toUsername, string content)
    {
        if (users.ContainsKey(toUsername))
        {
            var message = new Message(fromUser.Username, toUsername, content);
            users[toUsername].Inbox.Add(message);
            fromUser.SentMessages.Add(message);
            Console.WriteLine("Message sent successfully!");
        }
        else
        {
            Console.WriteLine("Recipient not found.");
        }
    }

    public void ReceiveMessages(User user)
    {
        if (user.Inbox.Count == 0)
        {
            Console.WriteLine("No messages.");
            return;
        }

        foreach (var message in user.Inbox)
        {
            Console.WriteLine($"From: {message.From}, Date: {message.DateSent}, Message: {message.Content}");
        }
    }

    public void ExportMessagesToTxt(User user)
    {
        string fileName = $"{user.Username}_messages.txt";
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            foreach (var message in user.Inbox)
            {
                writer.WriteLine($"From: {message.From}, Date: {message.DateSent}");
                writer.WriteLine($"Message: {message.Content}");
                writer.WriteLine(new string('-', 40));
            }
        }
        Console.WriteLine($"Messages exported to {fileName}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        MailBox mailBox = new MailBox();
        User currentUser = null;

        while (true)
        {
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Send Message");
            Console.WriteLine("4. View Inbox");
            Console.WriteLine("5. Export Messages to TXT");
            Console.WriteLine("6. Exit");
            Console.Write("Choose an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter username: ");
                    string username = Console.ReadLine();
                    Console.Write("Enter password: ");
                    string password = Console.ReadLine();
                    mailBox.RegisterUser(username, password);
                    break;

                case "2":
                    Console.Write("Enter username: ");
                    username = Console.ReadLine();
                    Console.Write("Enter password: ");
                    password = Console.ReadLine();
                    currentUser = mailBox.Authenticate(username, password);

                    if (currentUser == null)
                    {
                        Console.WriteLine("Authentication failed!");
                    }
                    else
                    {
                        Console.WriteLine($"Welcome, {currentUser.Username}!");
                    }
                    break;

                case "3":
                    if (currentUser == null)
                    {
                        Console.WriteLine("You must log in first.");
                        break;
                    }

                    Console.Write("Enter recipient username: ");
                    string recipient = Console.ReadLine();
                    Console.Write("Enter message: ");
                    string messageContent = Console.ReadLine();
                    mailBox.SendMessage(currentUser, recipient, messageContent);
                    break;

                case "4":
                    if (currentUser == null)
                    {
                        Console.WriteLine("You must log in first.");
                        break;
                    }
                    mailBox.ReceiveMessages(currentUser);
                    break;

                case "5":
                    if (currentUser == null)
                    {
                        Console.WriteLine("You must log in first.");
                        break;
                    }
                    mailBox.ExportMessagesToTxt(currentUser);
                    break;

                case "6":
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}
