using CRMPro.Dao;
using CRMPro.Data;
using CRMPro.Models;
using CRMPro.Services; // Mail atmak için servis sınıfımızı ekledik
using System;
using System.Text.RegularExpressions; // Doğrulama kütüphanesi
using System.Windows.Forms;

namespace CRMPro
{
    public partial class FormKayit : Form
    {
        
        private string uretilenDogrulamaKodu = "";

        public FormKayit()
        {
            InitializeComponent();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FormGiris form = new FormGiris();
            form.ShowDialog();
        }

        
        private void BtnKayitOl_Click(object sender, EventArgs e)
        {
            
            string adSoyad = txtAdSoyad.Text.Trim();
            string email = txtEmail.Text.Trim();
            string sifre = txtSifre.Text.Trim();
            string telefon = txtTelefon.Text.Trim();
            string adres = txtAdres.Text.Trim();
            string firmaAdi = txtFirmaAdi.Text.Trim();

            
            if (string.IsNullOrEmpty(adSoyad) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sifre) || string.IsNullOrEmpty(telefon) || string.IsNullOrEmpty(adres) || string.IsNullOrEmpty(firmaAdi))
            {
                MessageBox.Show("Lütfen Ad Soyad, E-posta, Şifre ve Telefon alanlarını boş bırakmayınız!", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            if (!Regex.IsMatch(adSoyad, @"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]{2,50}$"))
            {
                MessageBox.Show("Lütfen geçerli bir Ad Soyad giriniz. (Rakam veya özel karakter içeremez)", "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Lütfen geçerli bir e-posta adresi giriniz. (Örn: adiniz@sirket.com)", "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            if (!Regex.IsMatch(telefon, @"^[0-9]{10,11}$"))
            {
                MessageBox.Show("Lütfen geçerli bir telefon numarası giriniz. Sadece rakam kullanın. (Örn: 05551234567)", "Doğrulama Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            string sifreKurallari = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";
            if (!Regex.IsMatch(sifre, sifreKurallari))
            {
                MessageBox.Show("Şifreniz çok zayıf!\n\nŞartlar:\n- En az 8 karakter uzunluğunda olmalı.\n- En az 1 Büyük harf içermeli.\n- En az 1 Küçük harf içermeli.\n- En az 1 Rakam içermeli.\n- En az 1 Özel Karakter (!@#$%^&* vb.) içermeli.", "Güvenlik Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            SqlBaglanti baglanti = new SqlBaglanti();
            KullaniciDao kDao = new KullaniciDao(baglanti);

            if (kDao.EmailKayitliMi(email))
            {
                MessageBox.Show("Bu e-posta adresi zaten sistemimizde kayıtlı! Lütfen farklı bir e-posta girin veya Şifremi Unuttum ekranını kullanın.", "Kayıtlı E-Posta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }

            Random rnd = new Random();
            uretilenDogrulamaKodu = rnd.Next(100000, 999999).ToString();

            EmailService mailServisi = new EmailService();
            bool mailGittiMi = mailServisi.DogrulamaKoduGonder(email, uretilenDogrulamaKodu);

            if (mailGittiMi)
            {
                MessageBox.Show("Bilgileriniz geçerli. E-Posta adresinize 6 haneli bir doğrulama kodu gönderdik!", "Mail Gönderildi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                pnlDogrulama.Visible = true;          

                btnKayitOl.Enabled = false;
            }
            else
            {
                MessageBox.Show("Güvenlik kodu gönderilirken bir hata oluştu. Lütfen geçerli bir mail adresi girdiğinizden veya internet bağlantınızdan emin olun.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnKoduOnayla_Click_1(object sender, EventArgs e)
        {
            if (txtOnayKodu.Text.Trim() == uretilenDogrulamaKodu)
            {
                SqlBaglanti baglanti = new SqlBaglanti();
                KullaniciDao kDao = new KullaniciDao(baglanti);
                MusteriDao mDao = new MusteriDao(baglanti);

                string email = txtEmail.Text.Trim();
                string sifre = txtSifre.Text.Trim();

                try
                {
                    
                    
                    bool kullaniciEklendi = kDao.KullaniciEkle(email, sifre, 3);

                    if (kullaniciEklendi)
                    {
                        
                        Musteri yeniMusteri = new Musteri();
                        yeniMusteri.AdSoyad = txtAdSoyad.Text.Trim();
                        yeniMusteri.Eposta = email;
                        yeniMusteri.Telefon = txtTelefon.Text.Trim();
                        yeniMusteri.Adres = txtAdres.Text.Trim();
                        yeniMusteri.FirmaAdi = txtFirmaAdi.Text.Trim();
                        yeniMusteri.Sifre = sifre;

                        
                        yeniMusteri.RolId = 3;
                        yeniMusteri.MusteriSkoru = 50; 

                        
                        if (mDao.MusteriEkle(yeniMusteri))
                        {
                            MessageBox.Show("E-Posta doğrulandı ve Kayıt Başarılı! Giriş yapabilirsiniz.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Kullanıcı oluşturuldu ancak müşteri detayları kaydedilemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Kayıt sırasında bir hata oluştu. Bu mail adresi zaten kullanılıyor olabilir.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                  
                    MessageBox.Show("Veritabanı Hatası: " + ex.Message, "Sistem Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Girdiğiniz doğrulama kodu hatalı!", "Hatalı Kod", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}