using AutoMapper;
using Woa.Common;
using Woa.Transit;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Application;

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

		CreateMap<RoleEntity, RoleInfoDto>();
		CreateMap<RoleEditDto, RoleCreateCommand>();
		CreateMap<RoleEditDto, RoleUpdateCommand>();
	}
}