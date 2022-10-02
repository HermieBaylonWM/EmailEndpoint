using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

using Azure;
using Azure.Communication.Email;
using Azure.Communication.Email.Models;

namespace SendEmail
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Read connection string
            var connectionString = ConfigurationManager.AppSettings["ConnectionString"];

            // Authenticate the client
            EmailClient emailClient = new EmailClient(connectionString);

            // Send an email 
            EmailContent emailContent = new EmailContent("Subject Title");
            emailContent.PlainText = "Edit 6 This email message is sent from Azure Communication Service Email using .NET SDK.";
            
            List<EmailAddress> emailAddresses = new List<EmailAddress> { new EmailAddress("hermib100@gmail.com"), new EmailAddress("hbaylonb@gmail.com") };
            EmailRecipients emailRecipients = new EmailRecipients(emailAddresses);
            EmailMessage emailMessage = new EmailMessage("donotreply@6b9d1514-73bf-47e7-8046-dd28473e7598.azurecomm.net", emailContent, emailRecipients);
            
            SendEmailResult emailResult = emailClient.Send(emailMessage, CancellationToken.None);

            // Getting MessageId to track email delivery
            Console.WriteLine($"MessageId = {emailResult.MessageId}");

            // Getting status on email delivery
            Response<SendStatusResult> messageStatus = null;
            messageStatus = emailClient.GetSendStatus(emailResult.MessageId);
            Console.WriteLine($"MessageStatus = {messageStatus.Value.Status}");
            TimeSpan duration = TimeSpan.FromMinutes(3);
            long start = DateTime.Now.Ticks;
            do
            {
                messageStatus = emailClient.GetSendStatus(emailResult.MessageId);
                if (messageStatus.Value.Status != SendStatus.Queued)
                {
                    Console.WriteLine($"MessageStatus = {messageStatus.Value.Status}");
                    break;
                }
                Thread.Sleep(10000);
                Console.WriteLine($"...");

            } while (DateTime.Now.Ticks - start < duration.Ticks);
        }
    }
}