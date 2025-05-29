using AutoMapper;
using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;

namespace FUNewsManagementSystem.Reposirories.Untils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateNewsArticleViewModels, NewsArticle>();

            CreateMap<UpdateNewsArticleViewModels, NewsArticle>();

            CreateMap<CreateCategoryViewModels, Category>();

            CreateMap<UpdateCategoryViewModels, Category>();

            CreateMap<UpdateSystemAccountViewModel, SystemAccount>();

            CreateMap<CreateSystemAccountViewModel, SystemAccount>();
        }
    }
}
