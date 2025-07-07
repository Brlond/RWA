using AutoMapper;
using Lib.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MVC.Controllers;
using MVC.ViewModels;

namespace WebApp.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryVM>()
                .ForMember(dst => dst.TopicCount, opt => opt.MapFrom(src => src.Topics.Count))
                .ReverseMap();

            CreateMap<Post, PostVM>()
                .ForMember(dst => dst.CreatorUsername, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dst => dst.Published_Date, opt => opt.MapFrom(src => src.PostedAt))
                .ForMember(dst => dst.Score, opt => opt.MapFrom(src => src.Ratings.Sum(r => r.Score ?? 0)))
                .ForMember(dst => dst.TopicTitle, opt => opt.MapFrom(src => src.Topic.Title))
                .ReverseMap();

            CreateMap<Post, PostDTO>()
                .ForMember(dst => dst.UserId, opt => opt.MapFrom(src => src.UserId ?? 0))
                .ForMember(dst => dst.TopicId, opt => opt.MapFrom(src => src.TopicId ?? 0))
                .ForMember(dst => dst.Scores, opt => opt.MapFrom(src => src.Ratings.Select(r => r.Score ?? 0)))
                .ReverseMap()
                .ForMember(dst => dst.Ratings, opt => opt.Ignore());

            CreateMap<Tag, TagVM>()
                .ForMember(dst => dst.TopicCount, opt => opt.MapFrom(src => src.Topics.Count))
                .ReverseMap();

            CreateMap<Topic, TopicVM>()
                .ForMember(dst => dst.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dst => dst.TagIds, opt => opt.MapFrom(src => src.Tags.Select(t => t.Id).ToList()))
                .ForMember(dst => dst.TagNames, opt => opt.MapFrom(src => src.Tags.Select(t => t.Name).ToList()))
                .ForMember(dst => dst.PostsCount, opt => opt.MapFrom(src => src.Posts.Count))
                .ForMember(dst => dst.Publish_Date, opt => opt.MapFrom(src => src.CreatedAt))
                .ReverseMap()
                .ForMember(dst => dst.Category, opt => opt.Ignore()) 
                .ForMember(dst => dst.Tags, opt => opt.Ignore())    
                .ForMember(dst => dst.Posts, opt => opt.Ignore())   
                .ForMember(dst => dst.CreatedAt, opt => opt.MapFrom(src => src.Publish_Date));
        }
    }
}
