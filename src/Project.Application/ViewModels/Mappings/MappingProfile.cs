using AutoMapper;
using Project.Application.ViewModels.Auth;
using Project.Application.ViewModels.Tasks;
using Project.Domain.Entities;
using Project.Domain.Entities.Enums;
using TaskStatus = Project.Domain.Entities.Enums.TaskStatus;

namespace Project.Application.ViewModels.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User Mappings
        CreateMap<RegisterViewModel, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
            .ForMember(dest => dest.Tasks, opt => opt.Ignore());

        CreateMap<User, AuthResponseViewModel>()
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore())
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        // Task Mappings
        CreateMap<CreateTaskViewModel, TaskItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TaskStatus.Todo))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => (TaskPriority)src.Priority))
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<UpdateTaskViewModel, TaskItem>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (TaskStatus)src.Status))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => (TaskPriority)src.Priority))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedAt, opt => opt.Ignore());

        CreateMap<TaskItem, TaskResponseViewModel>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src =>
                src.DueDate.HasValue &&
                src.DueDate.Value < DateTime.UtcNow &&
                src.Status != TaskStatus.Completed));
    }
}
