using Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Configuration
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasData
            (
            new Book
            {
                Id = new Guid("20babca8-124a-1194-36fa-873476882d6a"),
                Name = "Master and Margarita",
                Author = "Mikhail Bulgakov",
                Year = 1966,
                Address = "Polezhaeva, 88"
            },
            new Book
            {
                Id = new Guid("29893053-4916-410c-ca78-2a54b999c870"),
                Name = "Idiot",
                Author = "Fyodor Dostoevsky",
                Year = 1868,
                Address = "Polezhaeva, 88"
            }); ;
        }
    }
}
