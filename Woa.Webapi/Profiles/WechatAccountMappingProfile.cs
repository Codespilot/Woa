using AutoMapper;
using Woa.Common;
using Woa.Transit;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Profiles;

public class WechatAccountMappingProfile : Profile
{
	public WechatAccountMappingProfile()
	{
		CreateMap<WechatAccountEntity, WechatAccountItemDto>()
			.ForMember(dest => dest.TypeName, options => options.MapFrom(src => ConvertAccountType(src.Type)))
			.ForMember(dest => dest.CreateBy, options => options.MapFrom<UserNameResolver<WechatAccountEntity, WechatAccountItemDto, int>, int>(src => src.CreateBy))
			.ForMember(dest => dest.UpdateBy, options => options.MapFrom<UserNameResolver<WechatAccountEntity, WechatAccountItemDto, int?>, int?>(src => src.UpdateBy));

		CreateMap<WechatAccountEntity, WechatAccountDetailDto>()
			.ForMember(dest => dest.TypeName, options => options.MapFrom(src => ConvertAccountType(src.Type)))
			.ForMember(dest => dest.CreateBy, options => options.MapFrom<UserNameResolver<WechatAccountEntity, WechatAccountDetailDto, int>, int>(src => src.CreateBy))
			.ForMember(dest => dest.UpdateBy, options => options.MapFrom<UserNameResolver<WechatAccountEntity, WechatAccountDetailDto, int?>, int?>(src => src.UpdateBy));

		CreateMap<WechatAccountCreateDto, WechatAccountCreateCommand>()
			.ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id.Trim(TextTrimType.All).Normalize(TextCaseType.Lower)))
			.ForMember(dest => dest.Account, options => options.MapFrom(src => src.Account.Trim(TextTrimType.All).Normalize(TextCaseType.Lower)))
			.ForMember(dest => dest.AppId, options => options.MapFrom(src => src.AppId.Trim(TextTrimType.All).Normalize(TextCaseType.Lower)))
			.ForMember(dest => dest.AppSecret, options => options.MapFrom(src => src.AppSecret.Trim(TextTrimType.All)))
			.ForMember(dest => dest.AppToken, options => options.MapFrom(src => src.AppToken.Trim(TextTrimType.All)))
			.ForMember(dest => dest.EncryptKey, options => options.MapFrom(src => src.EncryptKey.Trim(TextTrimType.All)));

		CreateMap<WechatAccountCreateCommand, WechatAccountEntity>();

		CreateMap<WechatAccountUpdateCommand, WechatAccountEntity>();
	}

	private static string ConvertAccountType(string type)
	{
		return type switch
		{
			"subscription" => "订阅号",
			"service" => "服务号",
			"enterprise" => "企业号",
			_ => "未知"
		};
	}
}