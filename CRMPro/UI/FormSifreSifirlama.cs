using CRMPro.Dao;
using CRMPro.Data;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CRMPro
{
    public partial class FormSifreSifirla : Form
    {
        
        private string hedefEmail = "";
        private string dogruKod = "";

        
        public FormSifreSifirla(string gelenEmail, string gelenKod)
        {
            InitializeComponent();

            
            hedefEmail = gelenEmail;
            dogruKod = gelenKod;
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            string girilenKod = txtGelenKod.Text.Trim();
            string yeniSifre = txtYeniSifre.Text.Trim();
            string yeniSifreTekrar = txtYeniSifreTekrar.Text.Trim();

            
            if (girilenKod != dogruKod)
            {
                MessageBox.Show("Girdiğiniz doğrulama kodu hatalı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            if (yeniSifre != yeniSifreTekrar)
            {
                MessageBox.Show("Girdiğiniz şifreler birbiriyle uyuşmuyor!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            string sifreKurallari = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";
            if (!Regex.IsMatch(yeniSifre, sifreKurallari))
            {
                MessageBox.Show("Şifreniz çok zayıf! En az 8 karakter, büyük/küçük harf, rakam ve özel karakter içermelidir.", "Güvenlik Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            SqlBaglanti baglanti = new SqlBaglanti();
            KullaniciDao kDao = new KullaniciDao(baglanti);

            bool guncellendiMi = kDao.SifreGuncelle(hedefEmail, yeniSifre);

            if (guncellendiMi)
            {
                MessageBox.Show("Şifreniz başarıyla değiştirildi! Yeni şifrenizle giriş yapabilirsiniz.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); 
            }
            else
            {
                MessageBox.Show("Şifre güncellenirken bir hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}