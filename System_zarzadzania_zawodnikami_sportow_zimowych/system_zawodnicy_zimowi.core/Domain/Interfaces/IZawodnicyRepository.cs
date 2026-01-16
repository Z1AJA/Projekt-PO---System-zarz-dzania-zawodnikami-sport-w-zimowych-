using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Entities;

namespace system_zawodnicy_zimowi.core.Domain.Interfaces
{
    public interface IZawodnicyRepository
    {
        List<Zawodnik> GetAll();
        Zawodnik? GetById(Guid id);
        void Add(Zawodnik zawodnik);
        void Update(Zawodnik zawodnik);
        void Delete(Guid id);
    }
}