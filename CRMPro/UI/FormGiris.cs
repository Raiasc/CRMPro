using CRMPro.Dao;
using CRMPro.Data;
using CRMPro.Models;
using CRMPro.Services;
using CRMPro.UI;

//using CRMPro.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRMPro
{
    public partial class FormGiris : Form
    {
        public FormGiris()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string girilenEmail = txtEmail.Text;
            string girilenSifre = txtSifre.Text;

            
            SqlBaglanti baglanti = new SqlBaglanti();

            if (string.IsNullOrWhiteSpace(girilenEmail) || string.IsNullOrWhiteSpace(girilenSifre))
            {
                MessageBox.Show("Lütfen E-posta ve Şifre alanlarını boş bırakmayınız!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            KullaniciDao dao = new KullaniciDao(baglanti);
            Kullanici girisYapanKullanici = dao.GirisYap(girilenEmail, girilenSifre);

            if (girisYapanKullanici != null)
            {
                
                if (girisYapanKullanici.RolId == 1)
                {
                    MessageBox.Show("Admin Paneline Yönlendiriliyorsunuz...", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    adminForm adminFormu = new adminForm();
                    adminFormu.Show();
                    this.Hide();
                }
                else if (girisYapanKullanici.RolId == 2)
                {
                    MessageBox.Show("Personel Paneline Yönlendiriliyorsunuz...", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PersonelForm personelForm = new PersonelForm();
                    personelForm.Show();
                    this.Hide();
                }
                else if (girisYapanKullanici.RolId == 3) 
                {
                    // Giriş başarılı olduktan sonra:
                    string girisYapanEmail = txtEmail.Text; // Email yazdığın textbox adı neyse o
                    MessageBox.Show("Müşteri Portaline Yönlendiriliyorsunuz...", "Hoş Geldiniz", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Hatalı yer burasıydı, içine değişkeni ekledik:
                    MusteriForm musteriForm = new MusteriForm(girisYapanEmail);
                    musteriForm.Show();
                    this.Hide();
                }
            }
            else
            {
                
                MessageBox.Show("E-Posta veya Şifre hatalı!", "Giriş Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

   
        private void MusteriOlLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FormKayit form = new FormKayit();
            form.ShowDialog();
        }

        private void CikisButonu_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Sifremiunuttum_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panelSS.Visible = true;
        }

        private void btnSifreKoduGonder_Click(object sender, EventArgs e)
        {
            string email = SifreSeposta.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Lütfen e-posta adresinizi giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SqlBaglanti baglanti = new SqlBaglanti();
            KullaniciDao kDao = new KullaniciDao(baglanti);

            
            if (!kDao.EmailKayitliMi(email))
            {
                MessageBox.Show("Sistemde böyle bir e-posta adresi bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            }

            
            Random rnd = new Random();
            string uretilenKod = rnd.Next(100000, 999999).ToString();

            EmailService mailServisi = new EmailService();
            bool mailGittiMi = mailServisi.DogrulamaKoduGonder(email, uretilenKod);

            if (mailGittiMi)
            {
                MessageBox.Show("Şifre sıfırlama kodunuz e-posta adresinize gönderildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                
                FormSifreSifirla formSifre = new FormSifreSifirla(email, uretilenKod);
                formSifre.ShowDialog();

                
                panelSS.Visible = false;
            }
            else
            {
                MessageBox.Show("Mail gönderilirken bir hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
