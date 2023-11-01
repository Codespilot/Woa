using AutoMapper;
using Woa.Transit;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Profiles;

public sealed class WechatMenuMappingProfile : Profile
{
	public WechatMenuMappingProfile()
	{
		CreateMap<WechatMenuEntity, WechatMenuItemDto>();
		CreateMap<WechatMenuEntity, WechatMenuDetailDto>();
		CreateMap<WechatMenuEditDto, WechatMenuCreateCommand>();
		CreateMap<WechatMenuEditDto, WechatMenuUpdateCommand>();
	}
}