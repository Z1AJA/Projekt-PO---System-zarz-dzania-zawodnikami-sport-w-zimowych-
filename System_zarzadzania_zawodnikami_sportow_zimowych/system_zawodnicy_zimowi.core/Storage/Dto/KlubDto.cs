using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Enums;

namespace system_zawodnicy_zimowi.core.Storage.Dto
{
    public class KlubDto
    {
        public Guid Id { get; set; }
        public string Nazwa { get; set; } = "";
        public int MinimalnePunkty { get; set; }
        public int? MaksWiek { get; set; }
        public int? LimitMiejsc { get; set; }
        public List<Dyscyplina> Dyscypliny { get; set; } = new();



    }
}
