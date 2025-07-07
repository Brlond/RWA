using AutoMapper;
using Lib.Models;
using Lib.ViewModels;
using WebAPI.DTO;
using WebApp.DTO;

namespace WebAPI.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Log, LogDTO>().ReverseMap();

            CreateMap<Category, CategoryView>()
                .ForMember(dst => dst.TopicTitles, opt => opt.MapFrom(src => src.Topics.Select(x => x.Title)))
                .ReverseMap()
                .ForMember(dst => dst.Topics, opt => opt.MapFrom(src => src.TopicTitles.Select(title => new Topic { Title = title })));
            CreateMap<Category, CategoryDTO>().ReverseMap();

            CreateMap<Topic, TopicView>()
                .ForMember(dst => dst.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dst => dst.TagNames, opt => opt.MapFrom(src => src.Tags.Select(tt => tt.Name)))
                .ForMember(dst => dst.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt ?? DateTime.MinValue))
                .ForMember(dst => dst.Posts, opt => opt.MapFrom(src => src.Posts));

            CreateMap<Topic, TopicDTO>()
                .ForMember(dst => dst.TagIds, opt => opt.MapFrom(src => src.Tags.Select(tt => tt.Id)))
                .ReverseMap()
                .ForMember(dst => dst.Tags, opt => opt.MapFrom(src => src.TagIds.Select(id => new Tag{ Id = id })));

            CreateMap<Post, PostPreview>()
                .ForMember(dst => dst.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dst => dst.PostedAt, opt => opt.MapFrom(src => src.PostedAt ?? DateTime.MinValue));

            CreateMap<Post, PostDTO>()
                .ForMember(dst => dst.UserId, opt => opt.MapFrom(src => src.UserId ?? 0))
                .ForMember(dst => dst.TopicId, opt => opt.MapFrom(src => src.TopicId ?? 0))
                .ForMember(dst => dst.Scores, opt => opt.MapFrom(src => src.Ratings.Select(r => r.Score ?? 0)))
                .ReverseMap()
                .ForMember(dst => dst.Ratings, opt => opt.Ignore()); 

            CreateMap<Post, PostView>()
                .ForMember(dst => dst.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dst => dst.TopicTitle, opt => opt.MapFrom(src => src.Topic.Title))
                .ForMember(dst => dst.PostedAt, opt => opt.MapFrom(src => src.PostedAt ?? DateTime.MinValue))
                .ForMember(dst => dst.Approved, opt => opt.MapFrom(src => src.Approved ?? false))
                .ForMember(dst => dst.Scores, opt => opt.MapFrom(src => src.Ratings.Select(r => r.Score)));

            CreateMap<Tag, TagView>()
                .ForMember(dst => dst.TopicTitles, opt => opt.MapFrom(src => src.Topics.Select(t => t.Title)));

            CreateMap<Tag, TagDTO>()
                .ReverseMap();

        }
    }
}
