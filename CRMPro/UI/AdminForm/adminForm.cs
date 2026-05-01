using CRMPro.Data;
using CRMPro.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CRMPro
{
    public partial class adminForm : Form
    {
        SqlBaglanti baglanti = new SqlBaglanti();
        int seciliMusteriId = 0;

        public adminForm()
        {
            InitializeComponent();
            VerileriListeleHepsi();
        }

       
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                this.Tag = row.Cells["Id"].Value;
                txtKullaniciAdi.Text = row.Cells["Email"].Value?.ToString();
                txtRolId.Text = row.Cells["RolId"].Value?.ToString();
            }
        }

       
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.Tag == null) { MessageBox.Show("Önce soldan bir kullanıcı seçin!"); return; }

            try
            {
                int seciliId = Convert.ToInt32(this.Tag);
                int yeniRol = Convert.ToInt32(txtRolId.Text);
                string email = txtKullaniciAdi.Text;

                Dao.KullaniciDao kDao = new Dao.KullaniciDao(baglanti);
                Dao.MusteriDao mDao = new Dao.MusteriDao(baglanti);

               
                if (kDao.RolGuncelle(seciliId, yeniRol))
                {
                    
                    if (yeniRol == 3)
                    {
                        var mevcutMusteriler = mDao.TumMusterileriGetir();
                        if (!mevcutMusteriler.Any(m => m.Eposta == email))
                        {
                            mDao.MusteriEkle(new Musteri
                            {
                                AdSoyad = email.Split('@')[0],
                                Eposta = email,
                                RolId = 3,
                                MusteriSkoru = 50,
                                Sifre = "12345"
                            });
                        }
                    }
                    
                    else
                    {
                        var silinecek = mDao.TumMusterileriGetir().FirstOrDefault(m => m.Eposta == email);
                        if (silinecek != null) mDao.MusteriSil(silinecek.Id);
                    }

                    MessageBox.Show("Rol güncellendi ve listeler senkronize edildi.");
                    VerileriListeleHepsi();
                }
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void VerileriListeleHepsi()
        {
            try
            {
                
                Dao.KullaniciDao kDao = new Dao.KullaniciDao(baglanti);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = kDao.TumKullanicilariGetir();

               
                Dao.MusteriDao mDao = new Dao.MusteriDao(baglanti);
                dgvMusteriler.DataSource = null;
                dgvMusteriler.DataSource = mDao.TumMusterileriGetir();

               
                if (dataGridView1.Columns["Sifre"] != null) dataGridView1.Columns["Sifre"].Visible = false;
                if (dgvMusteriler.Columns["Sifre"] != null) dgvMusteriler.Columns["Sifre"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Liste Hatası: " + ex.Message); }
        }

        
        private void btnAra_Click(object sender, EventArgs e)
        {
            var k = new Dao.KullaniciDao(baglanti).KullaniciGetirFiltreIle(txtAra.Text);
            if (k != null) { txtKullaniciAdi.Text = k.Email; txtRolId.Text = k.RolId.ToString(); this.Tag = k.Id; }
        }

        private void btnMusteriGuncelle_Click_1(object sender, EventArgs e)
        {
            if (seciliMusteriId == 0) return;
            Musteri m = new Musteri
            {
                Id = seciliMusteriId,
                AdSoyad = txtMAdSoyad.Text,
                Eposta = txtMEmail.Text,
                FirmaAdi = txtMFirma.Text,
                Telefon = txtMTelefon.Text,
                Adres = txtMAdres.Text,
                MusteriSkoru = string.IsNullOrEmpty(txtMusteriSkor.Text) ? 0 : Convert.ToInt32(txtMusteriSkor.Text),
                RolId = 3
            };
            if (new Dao.MusteriDao(baglanti).MusteriGuncelle(m)) { VerileriListeleHepsi(); MessageBox.Show("Müşteri güncellendi."); }
        }

        
        private void btnMusteriEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEkleEposta.Text))
            {
                MessageBox.Show("E-posta boş olamaz!");
                return;
            }

            Musteri yeni = new Musteri
            {
                AdSoyad = txtMAdSoyad.Text,
                Eposta = txtEkleEposta.Text,
                FirmaAdi = txtMFirma.Text,
                Telefon = txtMTelefon.Text,
                Adres = txtMAdres.Text,
                RolId = 3, 
                MusteriSkoru = string.IsNullOrEmpty(txtMusteriSkor.Text) ? 0 : Convert.ToInt32(txtMusteriSkor.Text),
                Sifre = "12345"
            };

            Dao.MusteriDao mDao = new Dao.MusteriDao(baglanti);
            Dao.KullaniciDao kDao = new Dao.KullaniciDao(baglanti);

            
            if (mDao.MusteriEkle(yeni))
            {
                
                var tumKullanicilar = kDao.TumKullanicilariGetir();
                if (!tumKullanicilar.Any(k => k.Email == yeni.Eposta))
                {
                    
                    kDao.KullaniciEkle(yeni.Eposta, "12345", 3);
                }

                VerileriListeleHepsi();
                //EklemePaneliniTemizle();
                MessageBox.Show("Yeni müşteri eklendi ve sisteme kullanıcı olarak tanımlandı.");
            }
        }

        private void dgvMusteriler_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvMusteriler.Rows[e.RowIndex];
                seciliMusteriId = Convert.ToInt32(row.Cells["Id"].Value);
                txtMAdSoyad.Text = row.Cells["AdSoyad"].Value?.ToString();
                txtMEmail.Text = row.Cells["Eposta"].Value?.ToString();
                txtMFirma.Text = row.Cells["FirmaAdi"].Value?.ToString();
                txtMTelefon.Text = row.Cells["Telefon"].Value?.ToString();
                txtMAdres.Text = row.Cells["Adres"].Value?.ToString();
                txtMusteriSkor.Text = row.Cells["MusteriSkoru"].Value?.ToString();
            }
        }

        
        private void btnMusteriSil_Click_1(object sender, EventArgs e)
        {
            if (seciliMusteriId == 0)
            {
                MessageBox.Show("Lütfen silmek için önce listeden bir müşteri seçin!");
                return;
            }

            // Silme işlemi için uyarı mesajı
            DialogResult secim = MessageBox.Show(
                "Bu müşteriyi ve sisteme kayıtlı kullanıcı girişini tamamen silmek istediğinize emin misiniz?",
                "Kalıcı Silme Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (secim == DialogResult.Yes)
            {
                try
                {
                    
                    string silinecekEmail = txtMEmail.Text;

                    Dao.MusteriDao mDao = new Dao.MusteriDao(baglanti);
                    Dao.KullaniciDao kDao = new Dao.KullaniciDao(baglanti);

                    
                    if (mDao.MusteriSil(seciliMusteriId))
                    {
                        
                        if (!string.IsNullOrEmpty(silinecekEmail))
                        {
                            kDao.KullaniciSil(silinecekEmail);
                        }

                        VerileriListeleHepsi(); 
                        seciliMusteriId = 0; 
                        txtMEmail.Clear(); txtMAdSoyad.Clear(); 

                        MessageBox.Show("Müşteri ve ona ait kullanıcı hesabı başarıyla silindi.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Silme işlemi sırasında hata oluştu: " + ex.Message);
                }
            }
        }

        //private void EklemePaneliniTemizle()
        //{
        //    txtEkleAdSoyad.Clear(); txtEkleEposta.Clear(); txtEkleFirma.Clear();
        //    txtEkleTelefon.Clear(); txtEkleAdres.Clear(); txtEkleSkor.Clear();
        //}

        private void CikisButonu_Click(object sender, EventArgs e) { this.Close(); new FormGiris().Show(); }
    }
}