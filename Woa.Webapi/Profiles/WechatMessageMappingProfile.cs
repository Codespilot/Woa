﻿using AutoMapper;
using Woa.Common;
using Woa.Sdk.Tencent;
using Woa.Transit;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Profiles;

public class WechatMessageMappingProfile : Profile
{
	public WechatMessageMappingProfile()
	{
		CreateMap<WechatMessageEntity, WechatMessageItemDto>()
			.ForMember(dest => dest.HasReply, options => options.MapFrom(src => string.IsNullOrWhiteSpace(src.Reply)))
			.ForMember(dest => dest.TypeName, options => options.MapFrom(src => Enum.Parse<WechatMessageType>(src.Type, true).GetDescription()));
		CreateMap<WechatMessageEntity, WechatMessageDetailDto>();
		//.ForMember(dest => dest.Detail, options => options.MapFrom(GetMessageDetail));
	}

	private static string GetMessageTypeName(WechatMessageEntity entity, WechatMessageItemDto dto, string obj, ResolutionContext context)
	{
		var type = Enum.Parse<WechatMessageType>(entity.Type, true);
		return type.GetDescription();
	}

	private static object GetMessageDetail(WechatMessageEntity entity, WechatMessageDetailDto dto, object obj, ResolutionContext context)
	{
		var payload = entity.Payload;
		if (string.IsNullOrWhiteSpace(payload))
		{
			return null;
		}

		var message = WechatMessage.Parse(payload);
		var type = Enum.Parse<WechatMessageType>(entity.Type, true);
		return type switch
		{
			WechatMessageType.Text => message.GetValue<string>(WechatMessageKey.Standard.Content),
			WechatMessageType.Image => message.GetValue<string>(WechatMessageKey.Standard.PictureUrl),
			WechatMessageType.Voice => message.GetValue<string>(WechatMessageKey.Standard.MediaId),
			WechatMessageType.Video => message.GetValue<string>(WechatMessageKey.Standard.MediaId),
			WechatMessageType.ShortVideo => message.GetValue<string>(WechatMessageKey.Standard.MediaId),
			WechatMessageType.Location => $"{message.GetValue<string>(WechatMessageKey.Standard.Longitude)},{message.GetValue<string>(WechatMessageKey.Standard.Latitude)}",
			WechatMessageType.Link => new
			{
				Title = message.GetValue<string>(WechatMessageKey.Standard.Title),
				Description = message.GetValue<string>(WechatMessageKey.Standard.Description),
				Url = message.GetValue<string>(WechatMessageKey.Standard.Url)
			},
			_ => null
		};
	}
}