using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Interfaces;

namespace system_zawodnicy_zimowi.core.Infrastructure
{
    public class InMemoryZawodnicyRepository : IZawodnicyRepository
    {
        private readonly List<Zawodnik> _zawodnicy = new();

        public List<Zawodnik> GetAll() => _zawodnicy.ToList();

        public Zawodnik? GetById(Guid id) => _zawodnicy.FirstOrDefault(z => z.Id == id);



        public void Add(Zawodnik zawodnik)
        {
            if (zawodnik is null) throw new ArgumentNullException(nameof(zawodnik));
            _zawodnicy.Add(zawodnik);
        }



        public void Update(Zawodnik zawodnik)
        {
            if (zawodnik is null) throw new ArgumentNullException(nameof(zawodnik));

            var idx = _zawodnicy.FindIndex(z => z.Id == zawodnik.Id);
            if (idx >= 0) _zawodnicy[idx] = zawodnik;
        }



        public void Delete(Guid id)
        {
            var z = GetById(id);
            if (z is not null) _zawodnicy.Remove(z);
        }
    }
}