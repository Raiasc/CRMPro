using CRMPro.Dao;
using CRMPro.Data;
using CRMPro.Models;
using CRMPro.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection.Emit;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CRMPro
{
    public partial class MusteriForm : Form
    {
        SqlBaglanti baglanti = new SqlBaglanti();
        string girisYapanEmail;
        int seciliMusteriId = 0;
        List<Panel> panelListesi = new List<Panel>();

        // --- YENİ EKLENEN DEĞİŞKENLER ---
        private MesajDao _mesajDao;
        private int _seciliTalepId = 0;

        public MusteriForm(string email)
        {
            InitializeComponent();
            this.girisYapanEmail = email;
            _mesajDao = new MesajDao(baglanti); // Mesajlaşma işlemleri için Dao başlatıldı
        }

        private void MusteriForm_Load(object sender, EventArgs e)
        {

            panelListesi.Add(pnlDashboard);
            panelListesi.Add(pnlProfil);
            panelListesi.Add(pnlTalepler);

            dgvTalepler.CellClick += dgvTalepler_CellClick_1;

            PanelGoster(pnlDashboard);

            MesajlariIkinciTabloyaYukle();
            MusteriVerileriniDoldur();
        }


        private void PanelGoster(Panel aktifPanel)
        {
            foreach (var pnl in panelListesi)
            {
                pnl.Visible = (pnl == aktifPanel);
            }
            aktifPanel.BringToFront();
        }



        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
            new FormGiris().Show();
        }



        private void MusteriVerileriniDoldur()
        {
            MusteriDao mDao = new MusteriDao(baglanti);

            Musteri m = mDao.GetirEpostaIle(girisYapanEmail);

            if (m != null)
            {

                seciliMusteriId = m.Id;
                lblHosgeldin.Text = $"Hoş geldin, {m.AdSoyad}!";
                txtProfilAdSoyad.Text = m.AdSoyad;
                txtProfilEmail.Text = m.Eposta;
                txtProfilFirma.Text = m.FirmaAdi;
                txtProfilTelefon.Text = m.Telefon;
                txtProfilAdres.Text = m.Adres;


                if (m.ProfilResmi != null && m.ProfilResmi.Length > 0)
                {
                    try
                    {

                        using (MemoryStream ms = new MemoryStream(m.ProfilResmi))
                        {
                            Image yeniResim = Image.FromStream(ms);
                            if (pbProfil.Image != null) pbProfil.Image.Dispose();
                            pbProfil.Image = new Bitmap(yeniResim);
                        }


                        System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
                        gp.AddEllipse(0, 0, pbProfil.Width - 1, pbProfil.Height - 1);
                        pbProfil.Region = new Region(gp);
                        pbProfil.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    catch (Exception)
                    {
                        pbProfil.Image = null;
                    }
                }
            }

            // --- TALEP İSTATİSTİKLERİ ---
            // --- TALEP İSTATİSTİKLERİ ---
            TalepDao tDao = new TalepDao(baglanti);
            var stats = tDao.TalepIstatistikleriniGetir(girisYapanEmail);

            int bekleyen = stats["Bekliyor"];
            int islemde = stats["İşlemde"];       // DEĞİŞTİ
            int tamamlanan = stats["Tamamlandı"]; // DEĞİŞTİ
            int toplam = bekleyen + islemde + tamamlanan;

            lblBekleyenSayi.Text = bekleyen.ToString();
            lblIslemdeSayi.Text = islemde.ToString();
            lblTamamlananSayi.Text = tamamlanan.ToString();

            if (toplam > 0)
            {
                int yuzde = (tamamlanan * 100) / toplam;
                prgSürec.Value = yuzde;
                lblYuzde.Text = $"%{yuzde} Tamamlandı";
            }
            else
            {
                prgSürec.Value = 0;
                lblYuzde.Text = "Henüz talebiniz bulunmuyor.";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (seciliMusteriId == 0)
            {
                MessageBox.Show("Müşteri bilgileri yüklenemediği için güncelleme yapılamaz.");
                return;
            }


            byte[] resimBaytlari = null;
            if (pbProfil.Image != null)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {

                        Bitmap bmp = new Bitmap(pbProfil.Image);
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Resim işlenirken bir hata oluştu: " + ex.Message);
                }
            }


            Musteri m = new Musteri
            {
                Id = seciliMusteriId,
                AdSoyad = txtProfilAdSoyad.Text,
                Eposta = txtProfilEmail.Text,
                FirmaAdi = txtProfilFirma.Text,
                Telefon = txtProfilTelefon.Text,
                Adres = txtProfilAdres.Text,
                ProfilResmi = resimBaytlari
            };


            MusteriDao mDao = new MusteriDao(baglanti);
            if (mDao.MusteriGuncelle(m))
            {
                MessageBox.Show("Profil bilgileriniz ve fotoğrafınız başarıyla güncellendi!");


                MusteriVerileriniDoldur();
            }
        }



        private void TalepleriListeleVeGoster(string durum)
        {
            TalepDao tDao = new TalepDao(baglanti);
            List<Talep> liste = tDao.TalepleriGetirDurumaGore(girisYapanEmail, durum);

            dgvTalepler.DataSource = liste;

            dgvTalepler.Columns["MusteriEmail"].Visible = false;
            dgvTalepler.Columns["CihazAdi"].HeaderText = "İş Başlığı";
            dgvTalepler.Columns["ArizaAciklamasi"].HeaderText = "Detaylar";
            dgvTalepler.Columns["SeriNo"].HeaderText = "Seri Numarası";
            dgvTalepler.Columns["TeknikerNotu"].HeaderText = "Tekniker Notu";
            dgvTalepler.Columns["TahminiTeslimTarihi"].HeaderText = "Tahmini Teslim";
            if (dgvTalepler.Columns.Contains("Fiyat"))
                dgvTalepler.Columns["Fiyat"].HeaderText = "Ücret (TL)";

            PanelGoster(pnlTalepler);

            lblTalepBaslik.Text = durum + " Talepleriniz";
        }



        private void btnDashboard_Click_1(object sender, EventArgs e)
        {
            DashboardGuncelle();
            PanelGoster(pnlDashboard);
        }

        private void btnProfil_Click_1(object sender, EventArgs e)
        {
            PanelGoster(pnlProfil);
            MusteriVerileriniDoldur();
        }

        private void btnTalepler_Click_1(object sender, EventArgs e) => PanelGoster(pnlTalepler);

        private void CikisButonu_Click(object sender, EventArgs e) { this.Close(); new FormGiris().Show(); }

        private void btnTalepOlustur_Click(object sender, EventArgs e)
        {

            FormTalep frm = new FormTalep(girisYapanEmail);
            DialogResult sonuc = frm.ShowDialog();

            if (sonuc == DialogResult.OK)
            {

                DashboardGuncelle();
            }
        }
        private void DashboardGuncelle()
        {
            TalepDao tDao = new TalepDao(baglanti);
            var veriler = tDao.IstatistikleriGetir(girisYapanEmail);

            // Formdaki etiketleri (Label) güncelle
            lblBekleyenSayi.Text = veriler["Bekliyor"].ToString();
            lblIslemdeSayi.Text = veriler["İşlemde"].ToString();       // DEĞİŞTİ
            lblTamamlananSayi.Text = veriler["Tamamlandı"].ToString(); // DEĞİŞTİ

            //(Biten işlerin tüm işlere oranı)
            int toplam = veriler["Bekliyor"] + veriler["İşlemde"] + veriler["Tamamlandı"]; // DEĞİŞTİ
            if (toplam > 0)
            {
                int yuzde = (veriler["Tamamlandı"] * 100) / toplam;    // DEĞİŞTİ
                prgSürec.Value = yuzde;
                lblYuzde.Text = $"%{yuzde} Tamamlandı";
            }
        }

        private void lnkBekleyenDetay_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TalepleriListeleVeGoster("Bekliyor");
        }

        private void lnkIslemdeDetay_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TalepleriListeleVeGoster("işlemde");
        }

        private void lnkTamamlandiDetay_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TalepleriListeleVeGoster("Tamamlandi");
        }

        private void btnResimSec_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {

                pbProfil.Image = Image.FromFile(ofd.FileName);

                // 2. Resmi byte dizisine çevir (Veritabanı için)
                byte[] resimBaytlari = File.ReadAllBytes(ofd.FileName);


                MusteriDao mDao = new MusteriDao(new SqlBaglanti());
                if (mDao.ProfilResmiGuncelle(girisYapanEmail, resimBaytlari))
                {
                    MessageBox.Show("Profil fotoğrafınız güncellendi!", "Harika");
                }
            }
        }

        private void dgvTalepler_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hti = dgvTalepler.HitTest(e.X, e.Y);
                if (hti.RowIndex != -1)
                {
                    dgvTalepler.ClearSelection();
                    dgvTalepler.Rows[hti.RowIndex].Selected = true;
                }
            }
        }

        private void talebiSilToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            // 1. DataGridView'de seçili satır var mı kontrol et
            if (dgvTalepler.SelectedRows.Count > 0)
            {
                // 2. Seçili satırdaki ID değerini al
                int talepId = Convert.ToInt32(dgvTalepler.SelectedRows[0].Cells["Id"].Value);

                // 3. Kullanıcıdan onay al
                DialogResult onay = MessageBox.Show("Bu talebi silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (onay == DialogResult.Yes)
                {
                    TalepDao tDao = new TalepDao(baglanti);
                    if (tDao.TalepSil(talepId))
                    {
                        MessageBox.Show("Talep başarıyla silindi.");

                        // 4. Listeyi yenile (Hangi durumdaysa ona göre tekrar çağır)
                        // lblTalepBaslik.Text içindeki "Bekliyor", "Islemde" gibi metne göre listeyi tazeler
                        string suAnkiDurum = lblTalepBaslik.Text.Replace(" Talepleriniz", "");
                        TalepleriListeleVeGoster(suAnkiDurum);

                        // 5. Dashboard'daki sayıları da güncelle
                        DashboardGuncelle();
                    }
                    else
                    {
                        MessageBox.Show("Silme işlemi başarısız oldu.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz talebin satırını (en sol başından) seçin.");
            }
        }

       

        //private void dgvTalepler_CellClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    // Başlık (Header) satırına tıklandıysa işlem yapma
        //    if (e.RowIndex == -1) return;

        //    try
        //    {
        //        DataGridViewRow row = dgvTalepler.Rows[e.RowIndex];

        //        // 1. Seçilen ID'yi alıyoruz (Eğer "Id" kolonun yoksa hata fırlatacak ve catch bloğuna düşecek)
        //        _seciliTalepId = Convert.ToInt32(row.Cells["Id"].Value);

        //        // 2. Senin istediğin gibi Label'a yazdırıyoruz
        //        lblSeciliTalep.Text = $"Seçilen Talep ID: {_seciliTalepId}";
        //        lblSeciliTalep.ForeColor = Color.Green;

        //        // 3. TIKLAMANIN ÇALIŞTIĞINI KANITLAMAK İÇİN TEST MESAJI (Çalıştığını görünce bu satırı silebilirsin)
        //        MessageBox.Show($"Tabloya tıklandı ve ID alındı: {_seciliTalepId}", "Bağlantı Başarılı");

        //        // 4. İkinci tabloya mesajları yükle
        //        MesajlariIkinciTabloyaYukle();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Eğer tablonda "Id" adında bir sütun yoksa veya başka bir hata varsa programı çökertmeden bize sebebini söyler
        //        MessageBox.Show("Tablodan veri çekerken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        // Seçilen talebin geçmiş mesajlarını 2. bir tabloya yükleyen metot
        private void MesajlariIkinciTabloyaYukle()
        {
            if (_seciliTalepId == 0) return;

            var mesajlar = _mesajDao.TalebeAitMesajlariGetir(_seciliTalepId);

            // Form tasarımındaki 2. DataGridView'i güncelliyoruz. (Not: Tasarıma 'dgvMesajGecmisi' adında bir tablo eklemelisin)
            dgvMesajGecmisi.DataSource = null;
            dgvMesajGecmisi.DataSource = mesajlar;

            // Görsel ayarlamalar: Sadece Rol, Tarih ve Mesaj İçeriği görünsün
            if (dgvMesajGecmisi.Columns["Id"] != null) dgvMesajGecmisi.Columns["Id"].Visible = false;
            if (dgvMesajGecmisi.Columns["TalepId"] != null) dgvMesajGecmisi.Columns["TalepId"].Visible = false;

            if (dgvMesajGecmisi.Columns["GonderenRol"] != null) dgvMesajGecmisi.Columns["GonderenRol"].HeaderText = "Kimden";
            if (dgvMesajGecmisi.Columns["Tarih"] != null) dgvMesajGecmisi.Columns["Tarih"].HeaderText = "Tarih";
            if (dgvMesajGecmisi.Columns["MesajMetni"] != null)
            {
                dgvMesajGecmisi.Columns["MesajMetni"].HeaderText = "Mesaj İçeriği";
                dgvMesajGecmisi.Columns["MesajMetni"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

       
     

        private void btnGonderM_Click(object sender, EventArgs e)
        {
            if (_seciliTalepId == 0)
            {
                MessageBox.Show("Mesaj göndermek için alttaki 'Talepleriniz' listesinden bir satır (talep) seçmelisiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Form tasarımındaki yazı alanından mesajı alıyoruz. (Not: Tasarıma 'txtMesajYaz' adında bir Textbox veya RichTextBox eklemelisin)
            if (string.IsNullOrWhiteSpace(RtxtMesajYaz.Text))
            {
                MessageBox.Show("Boş mesaj gönderilemez!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Yeni mesajı veritabanına hazırla
            TalepMesaj yeniMesaj = new TalepMesaj
            {
                TalepId = _seciliTalepId,
                GonderenRol = "Müşteri", // Bu form müşteri formu olduğu için rolü sabit veriyoruz
                MesajMetni = RtxtMesajYaz.Text.Trim()
            };

            // Veritabanına ekle
            bool basarili = _mesajDao.MesajEkle(yeniMesaj);

            if (basarili)
            {
                RtxtMesajYaz.Clear(); // Yazılanları temizle
                MesajlariIkinciTabloyaYukle(); // Tabloyu anında yenile ki mesaj ekranda belirtsin
            }
            else
            {
                MessageBox.Show("Mesaj kaydedilirken veritabanı bağlantısında bir hata oluştu!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvTalepler_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            // Başlığa tıklandıysa iptal et
            if (e.RowIndex == -1) return;

            try
            {
                DataGridViewRow row = dgvTalepler.Rows[e.RowIndex];

                // 1. ID'yi al
                _seciliTalepId = Convert.ToInt32(row.Cells["Id"].Value);

                // 2. Seninkinin adı "label6" olduğu için onu kullanıyoruz!
                lblSeciliTalep.Text = $"Seçilen Talep ID: {_seciliTalepId}";
                lblSeciliTalep.ForeColor = Color.Green;

                // 3. Mesaj geçmişini yan tabloya yükle
                MesajlariIkinciTabloyaYukle();
            }
            catch (Exception ex)
            {
                // Eğer veritabanı ID sütun adı farklıysa (Örn: KayitNo) bize bunu söyleyecek
                MessageBox.Show("Sütun bulunamadı veya veri çekilemedi: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}