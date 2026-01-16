using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using system_zawodnicy_zimowi.core.Domain.Entities;
using system_zawodnicy_zimowi.core.Domain.Interfaces;

namespace system_zawodnicy_zimowi.core.Infrastructure
{
    public class InMemoryKlubyRepository : IKlubyRepository
    {
        private readonly List<KlubSportowy> _kluby = new();

        public List<KlubSportowy> GetAll() => _kluby.ToList();

        public KlubSportowy? GetById(Guid id) => _kluby.FirstOrDefault(k => k.Id == id);

        public void Add(KlubSportowy klub)
        {
            if (klub is null) throw new ArgumentNullException(nameof(klub));
            _kluby.Add(klub);
        }

        public void Update(KlubSportowy klub)
        {
            if (klub is null) throw new ArgumentNullException(nameof(klub));

            var idx = _kluby.FindIndex(k => k.Id == klub.Id);
            if (idx >= 0) _kluby[idx] = klub;
        }

        public void Delete(Guid id)
        {
            var k = GetById(id);
            if (k is not null) _kluby.Remove(k);
        }
    }
}