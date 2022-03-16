using FlexiPlace.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace FlexiPlace.Services
{
    public interface IMailer
    {
        Task SendEmailAsync(string email, string subject, string body);
    }

    public class Mailer : IMailer
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly IHostingEnvironment _env;

        public Mailer(IOptions<SmtpSettings> smtpSettings, IHostingEnvironment env)
        {
            _smtpSettings = smtpSettings.Value;
            _env = env;
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
                message.To.Add(new MailboxAddress(email, email));
                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = body
                };

                using (var client = new SmtpClient())
                {

                    await client.ConnectAsync(
                        _smtpSettings.Server,
                        _smtpSettings.Port,
                        _smtpSettings.UseTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    await client.SendAsync(message).ConfigureAwait(false);

                    await client.DisconnectAsync(true).ConfigureAwait(false);

                    // Note: only needed if the SMTP server requires authentication
                    //if (smtpConfig.RequiresAuthentication)
                    //{
                    //    await client.AuthenticateAsync(smtpConfig.SmtpUser, smtpConfig.SmtpPassword)
                    //        .ConfigureAwait(false);
                    //}



                    //client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    //if (_env.IsDevelopment())
                    //{
                    //    await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, true);
                    //}
                    //else
                    //{
                    //    await client.ConnectAsync(_smtpSettings.Server);
                    //}

                    //await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, true);

                    // vidi ovo dolje da li treba kemijati !!!!
                    //await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                    //await client.SendAsync(message);
                    //await client.DisconnectAsync(true);

                    //client.AuthenticationMechanisms.Remove("XOAUTH2");
                    //client.SslProtocols = System.Security.Authentication.SslProtocols.None;
                    //client.Connect(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.StartTls);
                    //client.Send(message);
                    //client.Disconnect(true);
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
    }
}