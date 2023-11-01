using AutoMapper;
using Woa.Common;
using Woa.Transit;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Profiles;

public class UserMappingProfile : Profile
{
	public UserMappingProfile()
	{
		CreateMap<UserCreateDto, UserCreateCommand>()
			.ForMember(dest => dest.Username, opt => opt.MapFrom(dto => dto.Username.Trim(TextTrimType.All).Normalize(TextCaseType.Lower)))
			.ForMember(dest => dest.Email, opt => opt.MapFrom(dto => dto.Email.Trim(TextTrimType.All).Normalize(TextCaseType.Lower)))
			.ForMember(dest => dest.Phone, opt => opt.MapFrom(dto => dto.Phone.Trim(TextTrimType.All).Normalize(TextCaseType.Lower)));

		CreateMap<UserEntity, UserProfileDto>();
		CreateMap<UserEntity, UserDetailDto>();

		CreateMap<RoleEntity, RoleInfoDto>()
		.ForMember(dest => dest.CreateBy, options => options.MapFrom<UserNameResolver<RoleEntity, RoleInfoDto, int>, int>(src => src.CreateBy))
		.ForMember(dest => dest.UpdateBy, options => options.MapFrom<UserNameResolver<RoleEntity, RoleInfoDto, int?>, int?>(src => src.UpdateBy));
		CreateMap<RoleEditDto, RoleCreateCommand>();
		CreateMap<RoleEditDto, RoleUpdateCommand>();
	}
}