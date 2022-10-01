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
        IEnumerable<Reader> GetAllReaders(bool trackChanges);
        Reader GetReader(Guid readerId, bool trackChanges);
    }
}
