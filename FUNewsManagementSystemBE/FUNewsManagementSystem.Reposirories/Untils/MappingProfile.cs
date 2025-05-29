using AutoMapper;
using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;

namespace FUNewsManagementSystem.Reposirories.Untils
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<NewsArticle, CreateNewsArticleViewModels>();
            CreateMap<CreateNewsArticleViewModels, NewsArticle>();

            CreateMap<NewsArticle, UpdateNewsArticleViewModels>();
            CreateMap<UpdateNewsArticleViewModels, NewsArticle>();
        }
    }
}
