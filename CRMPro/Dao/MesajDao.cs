using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CRMPro.Models;
using CRMPro.Data; // SqlBaglanti sınıfının olduğu yer

namespace CRMPro.Dao
{
    public class MesajDao
    {
        private SqlBaglanti _baglanti;

        public MesajDao(SqlBaglanti baglanti)
        {
            _baglanti = baglanti;
        }

        
        public bool MesajEkle(TalepMesaj mesaj)
        {
            try
            {
                using (SqlConnection conn = _baglanti.BaglantiAl())
                {
                    // Tarih sütununu SQL kendisi GETDATE() ile dolduracağı için buraya yazmıyoruz
                    string query = "INSERT INTO TalepMesajlari (TalepId, GonderenRol, MesajMetni) VALUES (@pTalepId, @pGonderenRol, @pMesajMetni)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@pTalepId", mesaj.TalepId);
                    cmd.Parameters.AddWithValue("@pGonderenRol", mesaj.GonderenRol);
                    cmd.Parameters.AddWithValue("@pMesajMetni", mesaj.MesajMetni);

                    conn.Open();
                    int sonuc = cmd.ExecuteNonQuery();
                    return sonuc > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        
        public List<TalepMesaj> TalebeAitMesajlariGetir(int talepId)
        {
            List<TalepMesaj> liste = new List<TalepMesaj>();
            try
            {
                using (SqlConnection conn = _baglanti.BaglantiAl())
                {
                    // Eskiden yeniye doğru (tarihe göre) sıralayarak getiriyoruz
                    string query = "SELECT * FROM TalepMesajlari WHERE TalepId = @pTalepId ORDER BY Tarih ASC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@pTalepId", talepId);

                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        liste.Add(new TalepMesaj
                        {
                            Id = (int)dr["Id"],
                            TalepId = (int)dr["TalepId"],
                            GonderenRol = dr["GonderenRol"].ToString(),
                            MesajMetni = dr["MesajMetni"].ToString(),
                            Tarih = (DateTime)dr["Tarih"]
                        });
                    }
                }
            }
            catch (Exception)
            {
                // Hata durumu loglanabilir
            }
            return liste;
        }
    }
}