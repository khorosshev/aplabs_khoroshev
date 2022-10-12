using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetAllBooks(bool trackChanges);
        Book GetBook(Guid bookId, bool trackChanges);
        void CreateBook(Book book);
        IEnumerable<Book> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
        void DeleteBook(Book book);
    }
}
