using System;
using System.Net;
using System.Net.Mail;

namespace CRMPro.Services
{
    public class EmailService
    {
        
        private string gonderenEmail = "deneme1mail1.1@gmail.com";
        private string uygulamaSifresi = "sqel blus nxym goqa"; 
        public bool DogrulamaKoduGonder(string aliciEmail, string dogrulamaKodu)
        {
            try
            {
                MailMessage mesaj = new MailMessage();
                mesaj.From = new MailAddress(gonderenEmail, "CRM Otomasyon Sistemi");
                mesaj.To.Add(aliciEmail); // Kodun gideceği müşteri
                mesaj.Subject = "Güvenlik Doğrulama Kodu";
                mesaj.Body = $"Merhaba,\n\nSistemimize kayıt olmak için doğrulama kodunuz: {dogrulamaKodu}\n\nLütfen bu kodu ekrandaki ilgili alana giriniz.\nİyi çalışmalar dileriz.";

                SmtpClient istemci = new SmtpClient("smtp.gmail.com", 587);
                istemci.Credentials = new NetworkCredential(gonderenEmail, uygulamaSifresi);
                istemci.EnableSsl = true;

                istemci.Send(mesaj); // Maili gönder!
                return true;
            }
            catch (Exception )
            {
                
                return false;
            }
        }
    }
}