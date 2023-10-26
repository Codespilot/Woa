using AutoMapper;
using Woa.Webapi.Domain;
using Woa.Transit;

namespace Woa.Webapi.Application;

public class SensitiveWordMappingProfile : Profile
{
	public SensitiveWordMappingProfile()
	{
		CreateMap<SensitiveWordEntity, SensitiveWordItemDto>();
	}
}