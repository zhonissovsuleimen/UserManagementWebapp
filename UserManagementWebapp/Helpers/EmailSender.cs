using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

namespace UserManagementWebapp.Helpers
{
    public class EmailSender
    {
        public async static Task SendVerificationEmail(string name, string email, string verificationLink)
        {
            string api = Environment.GetEnvironmentVariable("MAILJET_API_KEY") ?? "";
            string secret = Environment.GetEnvironmentVariable("MAILJET_SECRET") ?? "";


            MailjetClient client = new(api, secret);

            string senderEmail = Environment.GetEnvironmentVariable("MAILJET_SENDER_EMAIL") ?? "";
            string senderName = Environment.GetEnvironmentVariable("MAILJET_SENDER_NAME") ?? "";
            JObject from = new JObject
            {
                { "Email", senderEmail },
                { "Name", senderName }
            };
            JArray to = new JArray
            {
                new JObject
                {
                    { "Email", email },
                    { "Name", name }
                }
            };
            string subject = "Please verify your email address";
            string text = $"""
                To complete your registration, please click the link below to verify your email address:
                
                {verificationLink}

                If you did not request this verification, please ignore this email. Your account will not be affected.
                """;
            string html = $"""
                <h2>To complete your registration, please press the link below to verify your email address:</h2>
                
                <a href="{verificationLink}">Verify Email</a>

                If you did not request this verification, please ignore this email. Your account will not be affected.
                """;

            MailjetRequest request = new MailjetRequest
            {
                Resource = SendV31.Resource,
            }.Property(Send.Messages, new JArray
            {
                new JObject
                {
                    { "From", from },
                    { "To", to },
                    { "Subject", subject },
                    { "TextPart", text },
                    { "HTMLPart", html },
                }
            });

            MailjetResponse response = await client.PostAsync(request);
        }
    }
}
