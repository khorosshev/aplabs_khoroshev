using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Entities.Configuration
{
    public class ReaderConfiguration : IEntityTypeConfiguration<Reader>
    {
        public void Configure(EntityTypeBuilder<Reader> builder)
        {
            builder.HasData
            (
            new Reader
            {
                Id = new Guid("80abbca8-124a-1194-36fa-873476882d6a"),
                Surname = "Khoroshev",
                Name = "Matvey",
                Age = 18,
                Phone = "+79506047092"
            },
            new Reader
            {
                Id = new Guid("29243053-4916-410c-ca78-2a54b999c870"),
                Surname = "Burova",
                Name = "Anastasia",
                Age = 19,
                Phone = "+79506439933"
            }); ;
        }
    }
}
