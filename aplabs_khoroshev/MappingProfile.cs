using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Company, CompanyDto>()
        .ForMember(c => c.FullAddress,
        opt => opt.MapFrom(x => string.Join(' ', x.Address, x.Country)));
        CreateMap<Employee, EmployeeDto>();
        CreateMap<Book, BookDto>();
        CreateMap<Reader, ReaderDto>();
        CreateMap<CompanyForCreationDto, Company>();
        CreateMap<EmployeeForCreationDto, Employee>();
        CreateMap<BookForCreationDto, Book>();
        CreateMap<ReaderForCreationDto, Reader>();
    }
}
