using AutoMapper;
using Woa.Webapi.Domain;
using Woa.Webapi.Dtos;

namespace Woa.Webapi.Application;

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