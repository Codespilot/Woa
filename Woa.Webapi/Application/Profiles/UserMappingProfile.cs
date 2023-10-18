using System.Globalization;
using System.Text.RegularExpressions;
using AutoMapper;
using Woa.Transit;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Application;

public class UserMappingProfile : Profile
{
	public UserMappingProfile()
	{
		CreateMap<UserRegisterDto, UserCreateCommand>()
			.ForMember(dest => dest.Username, opt => opt.MapFrom(dto => TrimString(dto.Username)))
			.ForMember(dest => dest.Email, opt => opt.MapFrom(dto => TrimString(dto.Email)))
			.ForMember(dest => dest.Phone, opt => opt.MapFrom(dto => TrimString(dto.Phone)));

		CreateMap<UserEntity, UserProfileDto>();
	}

	private static string TrimString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}

		value = Regex.Replace(value, @"\s+", string.Empty);
		return CultureInfo.CurrentCulture.TextInfo.ToLower(value);
	}
}