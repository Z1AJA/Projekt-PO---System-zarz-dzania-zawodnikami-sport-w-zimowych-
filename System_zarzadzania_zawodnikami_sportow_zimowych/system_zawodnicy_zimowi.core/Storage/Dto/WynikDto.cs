using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system_zawodnicy_zimowi.core.Storage.Dto
{
    public class WynikDto
    {
        public Guid Id { get; set; }
        public DateTime Data { get; set; }
        public string NazwaZawodow { get; set; } = "";
        public int Miejsce { get; set; }
        public int TrudnoscTrasy { get; set; }
        public int PunktyBazowe { get; set; }
    }
}
