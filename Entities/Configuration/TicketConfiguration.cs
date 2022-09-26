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
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasData
            (
            new Ticket
            {
                Id = new Guid("80abbca8-124a-1194-36fa-873476882d6a"),
                Cinema = "Madagaskar, City-Park",
                Film = "Apocalypse Now",
                Year = 1979,
                Sit = 22
            },
            new Ticket
            {
                Id = new Guid("29243053-4916-410c-ca78-2a54b999c870"),
                Cinema = "Cinema-Park, RIO",
                Film = "Drive",
                Year = 2007,
                Sit = 12
            }); ;
        }
    }
}
