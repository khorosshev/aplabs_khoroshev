using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ReaderRepository : RepositoryBase<Reader>, IReaderRepository
    {
        public ReaderRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Reader>> GetAllReadersAsync(bool trackChanges) =>
            await FindAll(trackChanges)
            .OrderBy(c => c.Name)
            .ToListAsync();

        public async Task<IEnumerable<Reader>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges) =>
        await FindByCondition(x => ids.Contains(x.Id), trackChanges).ToListAsync();

        public async Task<Reader> GetReaderAsync(Guid readerId, bool trackChanges) => await FindByCondition(c
=> c.Id.Equals(readerId), trackChanges).SingleOrDefaultAsync();

        public void CreateReader(Reader reader) => Create(reader);

        public void DeleteReader(Reader reader)
        {
            Delete(reader);
        }
    }
}
