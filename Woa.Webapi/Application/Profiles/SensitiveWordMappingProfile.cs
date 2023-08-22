using AutoMapper;
using Woa.Webapi.Domain;
using Woa.Webapi.Dtos;

namespace Woa.Webapi.Application;

public class SensitiveWordMappingProfile : Profile
{
	public SensitiveWordMappingProfile()
	{
		CreateMap<SensitiveWordEntity, SensitiveWordItemDto>();
	}
}