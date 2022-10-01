using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ReaderRepository : RepositoryBase<Reader>, IReaderRepository
    {
        public ReaderRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
        {
        }

        public IEnumerable<Reader> GetAllReaders(bool trackChanges) =>
    FindAll(trackChanges)
            .OrderBy(c => c.Name)
            .ToList();

        public IEnumerable<Reader> GetByIds(IEnumerable<Guid> ids, bool trackChanges) =>
FindByCondition(x => ids.Contains(x.Id), trackChanges).ToList();

        public Reader GetReader(Guid readerId, bool trackChanges) => FindByCondition(c
=> c.Id.Equals(readerId), trackChanges).SingleOrDefault();

        public void CreateReader(Reader reader) => Create(reader);
    }
}
