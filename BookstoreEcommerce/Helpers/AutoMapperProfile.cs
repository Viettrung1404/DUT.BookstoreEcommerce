using AutoMapper;
using BookstoreEcommerce.Models;
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
