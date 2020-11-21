using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMinister.Models.Sync
{
    public class LocalShareInfo
    {
        public int ShareId { get; set; }
        public string ShareName { get; set; }
        public string SharePathUNC { get; set; }
        public string SharePathLocal { get; set; }
        public string WindowsUser { get; set; }
        public string Password { get; set; }
        public Guid TenantId { get; set; }
    }
}
