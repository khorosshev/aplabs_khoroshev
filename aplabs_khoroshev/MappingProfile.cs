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
        CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap();
        CreateMap<CompanyForUpdateDto, Company>().ReverseMap();
        CreateMap<BookForUpdateDto, Book>().ReverseMap();
        CreateMap<ReaderForUpdateDto, Reader>().ReverseMap();
        CreateMap<BookForCreationDto, Book>();
        CreateMap<ReaderForCreationDto, Reader>();
        CreateMap<UserForRegistrationDto, User>();
    }
}
