using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace system_zawodnicy_zimowi.core.Storage.Dto
{
    public class AppStateDto
    {
        public List<ZawodnikDto> Zawodnicy { get; set; } = new();
        public List<KlubDto> Kluby { get; set; } = new();
    }
}
