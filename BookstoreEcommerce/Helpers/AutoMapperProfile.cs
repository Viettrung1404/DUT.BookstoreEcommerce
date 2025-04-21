using AutoMapper;
using BookstoreEcommerce.Data;
using BookstoreEcommerce.ViewModels;

namespace BookstoreEcommerce.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() { 
            CreateMap<RegisterViewModel, KhachHang>();
            
        }
    }
}
