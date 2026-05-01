using CRMPro.Dao;
using CRMPro.Data;
using CRMPro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CRMPro.UI
{
    public partial class PersonelForm : Form
    {
        private SqlBaglanti _baglanti;
        private TalepDao _tDao;

        // Müşteri yönetimi için seçili müşteri ID'sini tutacak değişken
        int seciliMusteriId = 0;

        public PersonelForm()
        {
            InitializeComponent();
            _baglanti = new SqlBaglanti();
            _tDao = new TalepDao(_baglanti);

            this.Load += PersonelForm_Load;
            dgvTalepler.CellClick += dgvTalepler_CellClick;

            // EKLENEN KISIM: Müşteri gridine tıklama olayını (event) bağlıyoruz
            dataGridViewmMusteri.CellClick += dataGridViewmMusteri_CellClick;

            VerileriListeleHepsi();
        }

        // Form açıldığında listeyi doldur
        private void PersonelForm_Load(object sender, EventArgs e)
        {
            cmbDurum.Items.Clear();
            cmbDurum.Items.Add("Bekliyor");
            cmbDurum.Items.Add("İşlemde");
            cmbDurum.Items.Add("Tamamlandı");

            ListeyiYenile();
        }

        private void ListeyiYenile()
        {
            List<Talep> liste = _tDao.TumTalepleriGetir();
            dgvTalepler.DataSource = null;
            dgvTalepler.DataSource = liste;

            // Grid görsel ayarları
            dgvTalepler.Columns["Id"].HeaderText = "Kayıt No";
            dgvTalepler.Columns["CihazAdi"].HeaderText = "Cihaz";
            dgvTalepler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        // Grid'den bir satıra tıklandığında bilgileri kutulara doldur (Talepler için)
        private void dgvTalepler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvTalepler.Rows[e.RowIndex];

                txtSecilenId.Text = row.Cells["Id"].Value.ToString();
                txtCihazBilgisi.Text = row.Cells["CihazAdi"].Value.ToString() + " - " + row.Cells["ArizaAciklamasi"].Value.ToString();

                txtTeknikerNotu.Text = row.Cells["TeknikerNotu"].Value?.ToString();
                txtFiyat.Text = row.Cells["Fiyat"].Value?.ToString();
                cmbDurum.Text = row.Cells["Durum"].Value?.ToString();

                if (row.Cells["TahminiTeslimTarihi"].Value != DBNull.Value && row.Cells["TahminiTeslimTarihi"].Value != null)
                {
                    dtpTahminiTeslim.Value = Convert.ToDateTime(row.Cells["TahminiTeslimTarihi"].Value);
                }
                else
                {
                    dtpTahminiTeslim.Value = DateTime.Now; // Boşsa bugünü göster
                }
            }
        }

        // EKLENEN METOT: Müşteri gridinden bir satıra tıklandığında bilgileri kutulara doldur
        private void dataGridViewmMusteri_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewmMusteri.Rows[e.RowIndex];

                // Müşteri silme/güncelleme işlemleri için ID'yi yine de hafızada tutalım
                if (row.Cells["Id"].Value != null)
                {
                    seciliMusteriId = Convert.ToInt32(row.Cells["Id"].Value);
                }

                // Tıklanan müşterinin e-postasını alıyoruz
                string seciliEmail = row.Cells["Eposta"].Value?.ToString();

                // Eğer e-posta boş değilse, e-postaya göre filtreleme yap
                if (!string.IsNullOrEmpty(seciliEmail))
                {
                    List<Talep> filtrelenmisTalepler = _tDao.MusterininTalepleriniGetir(seciliEmail);
                    dgvTalepler.DataSource = null;
                    dgvTalepler.DataSource = filtrelenmisTalepler;

                    if (dgvTalepler.Columns["Id"] != null) dgvTalepler.Columns["Id"].HeaderText = "Kayıt No";
                    if (dgvTalepler.Columns["CihazAdi"] != null) dgvTalepler.Columns["CihazAdi"].HeaderText = "Cihaz";
                    dgvTalepler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                }

                // Metin kutularını dolduruyoruz
                txtAdSoyad.Text = row.Cells["AdSoyad"].Value?.ToString();
                txtTelefon.Text = row.Cells["Telefon"].Value?.ToString();
                txtEmail.Text = seciliEmail;

                if (txtEkleEposta != null)
                {
                    txtEkleEposta.Text = seciliEmail;
                }

                txtAdres.Text = row.Cells["Adres"].Value?.ToString();
                txtFirma.Text = row.Cells["FirmaAdi"].Value?.ToString();
                txtSkor.Text = row.Cells["MusteriSkoru"].Value?.ToString();
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void CikisButonu_Click(object sender, EventArgs e) { this.Close(); new FormGiris().Show(); }


        private void btnGuncelle_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSecilenId.Text))
            {
                MessageBox.Show("Lütfen tablodan güncellenecek bir talep seçin!");
                return;
            }

            int id = Convert.ToInt32(txtSecilenId.Text);
            string not = txtTeknikerNotu.Text.Trim();
            string durum = cmbDurum.SelectedItem?.ToString() ?? "İşlemde";

            decimal? fiyat = null;
            if (decimal.TryParse(txtFiyat.Text, out decimal parseFiyat))
            {
                fiyat = parseFiyat;
            }

            // Bir önceki mesajda verdiğim AdminTalepCevapla metodunu Dao içinde oluşturduğundan emin ol.
            bool basarili = _tDao.AdminTalepCevapla(id, not, dtpTahminiTeslim.Value, fiyat, durum);

            if (basarili)
            {
                MessageBox.Show("Talep başarıyla güncellendi ve müşteriye yansıtıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ListeyiYenile(); // Güncel halini ekrana bas

                // Formu temizle
                txtSecilenId.Clear();
                txtCihazBilgisi.Clear();
                txtTeknikerNotu.Clear();
                txtFiyat.Clear();
            }
            else
            {
                MessageBox.Show("Güncelleme başarısız oldu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMusteriGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliMusteriId == 0) return;
            Musteri m = new Musteri
            {
                Id = seciliMusteriId,
                AdSoyad = txtAdSoyad.Text,
                Eposta = txtEmail.Text,
                FirmaAdi = txtFirma.Text,
                Telefon = txtTelefon.Text,
                Adres = txtAdres.Text,
                MusteriSkoru = string.IsNullOrEmpty(txtSkor.Text) ? 0 : Convert.ToInt32(txtSkor.Text),
                RolId = 3
            };
            if (new Dao.MusteriDao(_baglanti).MusteriGuncelle(m))
            {
                MessageBox.Show("Müşteri güncellendi.");
                VerileriListeleHepsi(); // Güncelledikten sonra listeyi yenile
            }
        }

        private void btnMusteriSil_Click(object sender, EventArgs e)
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
                    string silinecekEmail = txtEmail.Text;

                    Dao.MusteriDao mDao = new Dao.MusteriDao(_baglanti);
                    Dao.KullaniciDao kDao = new Dao.KullaniciDao(_baglanti);

                    if (mDao.MusteriSil(seciliMusteriId))
                    {
                        if (!string.IsNullOrEmpty(silinecekEmail))
                        {
                            kDao.KullaniciSil(silinecekEmail);
                        }

                        seciliMusteriId = 0;
                        txtEmail.Clear(); txtAdSoyad.Clear();

                        MessageBox.Show("Müşteri ve ona ait kullanıcı hesabı başarıyla silindi.");
                        VerileriListeleHepsi(); // Sildikten sonra listeyi yenile
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Silme işlemi sırasında hata oluştu: " + ex.Message);
                }
            }
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
                AdSoyad = txtAdSoyad.Text,
                Eposta = txtEkleEposta.Text,
                FirmaAdi = txtFirma.Text,
                Telefon = txtTelefon.Text,
                Adres = txtAdres.Text,
                RolId = 3,
                MusteriSkoru = string.IsNullOrEmpty(txtSkor.Text) ? 0 : Convert.ToInt32(txtSkor.Text),
                Sifre = "12345"
            };

            Dao.MusteriDao mDao = new Dao.MusteriDao(_baglanti);
            Dao.KullaniciDao kDao = new Dao.KullaniciDao(_baglanti);

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

        private void VerileriListeleHepsi()
        {
            try
            {
                Dao.MusteriDao mDao = new Dao.MusteriDao(_baglanti);
                dataGridViewmMusteri.DataSource = null;
                dataGridViewmMusteri.DataSource = mDao.TumMusterileriGetir();

                if (dataGridViewmMusteri.Columns["Sifre"] != null) dataGridViewmMusteri.Columns["Sifre"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Liste Hatası: " + ex.Message); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Talepler gridini veritabanından çekerek tüm verilerle yeniden doldurur
            ListeyiYenile();

            // 2. Filtreleme bittiği için hafızadaki seçili müşteri ID'sini sıfırla
            seciliMusteriId = 0;

            // 3. Talep bilgilerinin bulunduğu metin kutularını temizle
            txtSecilenId.Clear();
            txtCihazBilgisi.Clear();
            txtTeknikerNotu.Clear();
            txtFiyat.Clear();
            cmbDurum.SelectedIndex = -1; // Durum kutusundaki seçimi kaldırır
            dtpTahminiTeslim.Value = DateTime.Now; // Tarihi bugüne çeker

            // 4. Müşteri bilgilerinin bulunduğu metin kutularını temizle
            txtAdSoyad.Clear();
            txtTelefon.Clear();
            txtEmail.Clear();

            // Varsa e-posta ekleme kutusunu da temizle
            if (txtEkleEposta != null)
            {
                txtEkleEposta.Clear();
            }

            txtAdres.Clear();
            txtFirma.Clear();
            txtSkor.Clear();
        }

        private void txtMusteriAra_TextChanged(object sender, EventArgs e)
        {
            string aranan = txtMusteriAra.Text.Trim();
            Dao.MusteriDao mDao = new Dao.MusteriDao(_baglanti);

            if (aranan.Length >= 2) // En az 2 harf yazınca aramaya başlasın (performans için)
            {
                dataGridViewmMusteri.DataSource = mDao.MusteriAra(aranan);
            }
            else if (aranan.Length == 0)
            {
                VerileriListeleHepsi(); // Boşsa tüm listeyi getir
            }
        }
    }
}