using CRMPro.Data;
using CRMPro.Models;
using CRMPro.Dao; // TalepDao'nun olduğu namespace'i eklediğinden emin ol!
using System;
using System.Windows.Forms;

namespace CRMPro.UI
{
    public partial class FormTalep : Form
    {
        private string _gelenMusteriEmail;
        private SqlBaglanti _baglanti;
        private TalepDao _tDao;

        public FormTalep(string email)
        {
            InitializeComponent(); 

            this._gelenMusteriEmail = email;

            
            _baglanti = new SqlBaglanti();
            _tDao = new TalepDao(_baglanti);
        }

        private void btnGonder_Click_1(object sender, EventArgs e)
        {
            // 1. Girdileri al ve temizle
            string cihazAdi = txtBaslik.Text.Trim(); // Eski 'baslik' artık cihaz adı
            string arizaAciklamasi = txtAciklama.Text.Trim(); // Eski 'aciklama' artık arıza detayı

            // Tasarıma txtSeriNo eklediğini varsayıyorum, yoksa bu satırı null yapabilirsin
            string seriNo = txtSeriNo.Text.Trim();

            // 2. Zorunlu alan kontrolü
            if (string.IsNullOrWhiteSpace(cihazAdi) || string.IsNullOrWhiteSpace(arizaAciklamasi))
            {
                MessageBox.Show("Lütfen Cihaz Adı ve Arıza Açıklaması alanlarını doldurun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 3. Yeni servis talebi (Talep) nesnesini oluştur
                Talep yeniTalep = new Talep
                {
                    MusteriEmail = _gelenMusteriEmail,
                    CihazAdi = cihazAdi, // Veritabanındaki CihazAdi kolonuna gider
                    ArizaAciklamasi = arizaAciklamasi, // Veritabanındaki ArizaAciklamasi kolonuna gider
                    SeriNo = string.IsNullOrEmpty(seriNo) ? "Belirtilmedi" : seriNo,
                    Durum = "Bekliyor", // Yeni kayıt her zaman 'Bekliyor' başlar
                    OlusturmaTarihi = DateTime.Now
                };

                // 4. Veritabanına gönder
                if (_tDao.TalepEkle(yeniTalep))
                {
                    MessageBox.Show("Teknik servis talebiniz başarıyla alınmıştır. Teknikerlerimiz en kısa sürede inceleyecektir.", "Kayıt Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Talep kaydedilemedi, lütfen tekrar deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Talep gönderilirken bir sistem hatası oluştu: " + ex.Message, "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CikisButonu_Click(object sender, EventArgs e) { this.Close(); new FormGiris().Show(); }

    }
}