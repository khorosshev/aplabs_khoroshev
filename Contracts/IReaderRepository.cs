using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IReaderRepository
    {
        Task<IEnumerable<Reader>> GetAllReadersAsync(bool trackChanges);
        Task<Reader> GetReaderAsync(Guid readerId, bool trackChanges);
        void CreateReader(Reader reader);
        Task<IEnumerable<Reader>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
        void DeleteReader(Reader reader);
    }
}
