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
    public class BookRepository : RepositoryBase<Book>, IBookRepository
    {
        public BookRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
        {
        }

        public IEnumerable<Book> GetAllBooks(bool trackChanges) =>
    FindAll(trackChanges)
            .OrderBy(c => c.Name)
            .ToList();
        public Book GetBook(Guid bookId, bool trackChanges) => FindByCondition(c
=> c.Id.Equals(bookId), trackChanges).SingleOrDefault();

        public void CreateBook(Book book) => Create(book);

        public IEnumerable<Book> GetByIds(IEnumerable<Guid> ids, bool trackChanges) =>
FindByCondition(x => ids.Contains(x.Id), trackChanges).ToList();

        public void DeleteBook(Book book)
        {
            Delete(book);
        }
    }
}
