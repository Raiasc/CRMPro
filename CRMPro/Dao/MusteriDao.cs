using CRMPro.Data;
using CRMPro.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CRMPro.Dao
{
    public class MusteriDao
    {
        private readonly SqlBaglanti _sqlBaglanti;

        public MusteriDao(SqlBaglanti sqlBaglanti)
        {
            _sqlBaglanti = sqlBaglanti;
        }

        public bool MusteriEkle(Musteri yeniMusteri)
        {
            using (SqlConnection baglanti = _sqlBaglanti.BaglantiAl())
            {
                string sorgu = @"INSERT INTO Musteriler 
                (AdSoyad, Eposta, Sifre, FirmaAdi, Telefon, Adres, MusteriSkoru, RolId) 
                VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8)";

                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@p1", yeniMusteri.AdSoyad ?? "");
                komut.Parameters.AddWithValue("@p2", yeniMusteri.Eposta ?? "");
                komut.Parameters.AddWithValue("@p3", yeniMusteri.Sifre ?? "12345");
                komut.Parameters.AddWithValue("@p4", yeniMusteri.FirmaAdi ?? (object)DBNull.Value);
                komut.Parameters.AddWithValue("@p5", yeniMusteri.Telefon ?? "");
                komut.Parameters.AddWithValue("@p6", yeniMusteri.Adres ?? (object)DBNull.Value);
                komut.Parameters.AddWithValue("@p7", yeniMusteri.MusteriSkoru);
                komut.Parameters.AddWithValue("@p8", yeniMusteri.RolId);

                baglanti.Open();
                return komut.ExecuteNonQuery() > 0;
            }
        }

        public List<Musteri> MusteriAra(string kelime)
        {
            List<Musteri> liste = new List<Musteri>();
            using (SqlConnection conn = _sqlBaglanti.BaglantiAl())
            {
                
                string query = "SELECT * FROM Musteriler WHERE AdSoyad LIKE @p OR Telefon LIKE @p OR Eposta LIKE @p";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@p", "%" + kelime + "%");

                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    liste.Add(new Musteri
                    {
                        Id = (int)dr["Id"],
                        AdSoyad = dr["AdSoyad"].ToString(),
                        Telefon = dr["Telefon"].ToString(),
                        Eposta = dr["Eposta"].ToString(),
                        FirmaAdi = dr["FirmaAdi"].ToString(),
                        Adres = dr["Adres"].ToString(),
                    });
                }
            }
            return liste;
        }

        public List<Musteri> TumMusterileriGetir()
        {
            List<Musteri> liste = new List<Musteri>();
            using (SqlConnection conn = _sqlBaglanti.BaglantiAl())
            {
                string query = "SELECT * FROM Musteriler";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    liste.Add(new Musteri
                    {
                        Id = (int)dr["Id"],
                        AdSoyad = dr["AdSoyad"].ToString(),
                        Eposta = dr["Eposta"].ToString(),
                        FirmaAdi = dr["FirmaAdi"].ToString(),
                        Telefon = dr["Telefon"].ToString(),
                        Adres = dr["Adres"].ToString(),
                        MusteriSkoru = dr["MusteriSkoru"] != DBNull.Value ? Convert.ToInt32(dr["MusteriSkoru"]) : 0,
                        RolId = dr["RolId"] != DBNull.Value ? Convert.ToInt32(dr["RolId"]) : 0,
                        Sifre = dr["Sifre"].ToString()
                    });
                }
            }
            return liste;
        }
        public Musteri MusteriBilgileriniGetir(string email)
        {
            Musteri m = null;
            using (SqlConnection conn = _sqlBaglanti.BaglantiAl())
            {
                string query = "SELECT AdSoyad, Eposta, FirmaAdi, Telefon, Adres, ProfilResmi FROM Musteriler WHERE Eposta = @email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@email", email);
                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        m = new Musteri
                        {
                            AdSoyad = dr["AdSoyad"].ToString(),
                            Eposta = dr["Eposta"].ToString(),
                            FirmaAdi = dr["FirmaAdi"].ToString(),
                            Telefon = dr["Telefon"].ToString(),
                            Adres = dr["Adres"].ToString(),
                           
                            ProfilResmi = dr["ProfilResmi"] != DBNull.Value ? (byte[])dr["ProfilResmi"] : null
                        };
                    }
                }
            }
            return m;
        }

        public Musteri GetirEpostaIle(string eposta)
        {
            using (SqlConnection conn = _sqlBaglanti.BaglantiAl())
            {
                string query = "SELECT * FROM Musteriler WHERE Eposta = @p1";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", eposta);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new Musteri
                    {
                        Id = (int)dr["Id"],
                        AdSoyad = dr["AdSoyad"].ToString(),
                        Eposta = dr["Eposta"].ToString(),
                        FirmaAdi = dr["FirmaAdi"].ToString(),
                        Telefon = dr["Telefon"].ToString(),
                        Adres = dr["Adres"].ToString(),
                        MusteriSkoru = Convert.ToInt32(dr["MusteriSkoru"]),
                        
                        ProfilResmi = dr["ProfilResmi"] != DBNull.Value ? (byte[])dr["ProfilResmi"] : null
                    };
                }
            }
            return null;
        }

        public bool MusteriGuncelle(Musteri m)
        {
            using (SqlConnection conn = _sqlBaglanti.BaglantiAl())
            {
                string query = "UPDATE Musteriler SET AdSoyad=@p1, FirmaAdi=@p2, Telefon=@p3, Adres=@p4, ProfilResmi=@p5 WHERE Id=@pId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", m.AdSoyad ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@p2", m.FirmaAdi ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@p3", m.Telefon ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@p4", m.Adres ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@pId", m.Id);

                
                SqlParameter imageParam = new SqlParameter("@p5", System.Data.SqlDbType.VarBinary);
                imageParam.Value = (object)m.ProfilResmi ?? DBNull.Value;
                cmd.Parameters.Add(imageParam);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool MusteriSil(int id)
        {
            using (SqlConnection conn = _sqlBaglanti.BaglantiAl())
            {
                string query = "DELETE FROM Musteriler WHERE Id=@p1";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@p1", id);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        
        public bool ProfilGuncelle(Musteri m, string yeniSifre = null)
        {
            using (SqlConnection conn = _sqlBaglanti.BaglantiAl())
            {
                conn.Open();
                SqlTransaction tr = conn.BeginTransaction(); 

                try
                {
                    
                    string mSorgu = "UPDATE Musteriler SET AdSoyad=@p1, FirmaAdi=@p2, Telefon=@p3, Adres=@p4 WHERE Eposta=@pEmail";
                    SqlCommand mCmd = new SqlCommand(mSorgu, conn, tr);
                    mCmd.Parameters.AddWithValue("@p1", m.AdSoyad);
                    mCmd.Parameters.AddWithValue("@p2", m.FirmaAdi);
                    mCmd.Parameters.AddWithValue("@p3", m.Telefon);
                    mCmd.Parameters.AddWithValue("@p4", m.Adres);
                    mCmd.Parameters.AddWithValue("@pEmail", m.Eposta);
                    mCmd.ExecuteNonQuery();

                    
                    if (!string.IsNullOrEmpty(yeniSifre))
                    {
                        string kSorgu = "UPDATE Kullanicilar SET Sifre=@pSifre WHERE Eposta=@pEmail";
                        SqlCommand kCmd = new SqlCommand(kSorgu, conn, tr);
                        kCmd.Parameters.AddWithValue("@pSifre", yeniSifre);
                        kCmd.Parameters.AddWithValue("@pEmail", m.Eposta);
                        kCmd.ExecuteNonQuery();
                    }

                    tr.Commit();
                    return true;
                }
                catch
                {
                    tr.Rollback();
                    return false;
                }
            }
        }
        
        public bool ProfilResmiGuncelle(string email, byte[] resimBaytlari)
        {
            using (SqlConnection conn = _sqlBaglanti.BaglantiAl())
            {
                string query = "UPDATE Musteriler SET ProfilResmi = @resim WHERE Eposta = @email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@resim", resimBaytlari);
                cmd.Parameters.AddWithValue("@email", email);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}