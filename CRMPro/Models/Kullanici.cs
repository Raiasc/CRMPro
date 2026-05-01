using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMPro.Models
{
    public class Kullanici
    {
        public int Id { get; set; }
        public int RolId { get; set; } // 1: Admin, 2: Müşteri ,3: Personel
        public string Email { get; set; }
        public string Sifre { get; set; }
        public DateTime? SonGirisTarihi { get; set; }
        public bool AktifMi { get; set; }
    }
}
