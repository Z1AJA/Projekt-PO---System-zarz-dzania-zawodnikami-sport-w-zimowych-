using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using system_zawodnicy_zimowi.core.Domain.Entities;

namespace system_zawodnicy_zimowi.core.Domain.Interfaces
{
    public interface IKlubyRepository
    {
        List<KlubSportowy> GetAll();
        KlubSportowy? GetById(Guid id);
        void Add(KlubSportowy klub);
        void Update(KlubSportowy klub);
        void Delete(Guid id);
    }
}