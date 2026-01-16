using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Entities;

namespace system_zawodnicy_zimowi.core.Services
{
    public class PrzydzialKlubuService
    {
        public KlubSportowy? ZnajdzNajlepszyKlub(Zawodnik zawodnik, IEnumerable<KlubSportowy> kluby)
        {
            if (zawodnik is null) throw new ArgumentNullException(nameof(zawodnik));
            if (kluby is null) throw new ArgumentNullException(nameof(kluby));

            //najwyzsze min punkty jakie spelnia zawonik by dolaczyc
            return kluby.Where(k => k.PasujeDo(zawodnik)).OrderByDescending(k => k.MinimalnePunkty).FirstOrDefault();
        }
    }
}
