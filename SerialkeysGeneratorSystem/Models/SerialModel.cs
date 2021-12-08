using System;
using System.Collections.Generic;
using System.Text;

namespace SerialkeysGeneratorSystem.Models
{
    public class SerialModel
    {
        public DateTime CreateDate { get; set; }
        public string PublicKey { get; set; }
        public double Expiration { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
