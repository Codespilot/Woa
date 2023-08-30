namespace Woa.Sdk.Wechat;

public class WechatMenuType
{
	/// <summary>
	/// 跳转URL
	/// </summary>
	/// <remarks>
	///	用户点击view类型按钮后，微信客户端将会打开开发者在按钮中填写的网页URL，可与网页授权获取用户基本信息接口结合，获得用户基本信息。
	/// </remarks>
	public const string View = "view";

	/// <summary>
	/// 点击事件
	/// </summary>
	/// <remarks>
	///	用户点击click类型按钮后，微信服务器会通过消息接口推送消息类型为event的结构给开发者（参考消息接口指南），并且带上按钮中开发者填写的key值，开发者可以通过自定义的key值与用户进行交互；
	/// </remarks>
	public const string Click = "click";

	/// <summary>
	/// 小程序
	/// </summary>
	public const string MiniApp = "miniprogram";

	/// <summary>
	/// 扫码推送
	/// </summary>
	/// <remarks>
	/// 扫码推事件用户点击按钮后，微信客户端将调起扫一扫工具，完成扫码操作后显示扫描结果（如果是URL，将进入URL），且会将扫码的结果传给开发者，开发者可以下发消息。
	/// </remarks>
	public const string ScanCodePush = "scancode_push";

	/// <summary>
	/// 扫码推送且弹出“消息接收中”提示框
	/// </summary>
	/// <remarks>
	///	码推事件且弹出“消息接收中”提示框用户点击按钮后，微信客户端将调起扫一扫工具，完成扫码操作后，将扫码的结果传给开发者，同时收起扫一扫工具，然后弹出“消息接收中”提示框，随后可能会收到开发者下发的消息。
	/// </remarks>
	public const string ScanCodeWait = "scancode_waitmsg";

	/// <summary>
	/// 弹出系统拍照发图
	/// </summary>
	/// <remarks>
	///	用户点击按钮后，微信客户端将调起系统相机，完成拍照操作后，会将拍摄的相片发送给开发者，并推送事件给开发者，同时收起系统相机，随后可能会收到开发者下发的消息。
	/// </remarks>
	public const string PickPhoto = "pic_sysphoto";

	/// <summary>
	/// 弹出拍照或者相册发图
	/// </summary>
	/// <remarks>
	///	用户点击按钮后，微信客户端将弹出选择器供用户选择“拍照”或者“从手机相册选择”。用户选择后即走其他两种流程。
	/// </remarks>
	public const string PickPhotoOrAlbum = "pic_photo_or_album";

	/// <summary>
	/// 弹出微信相册发图器
	/// </summary>
	/// <remarks>
	///	用户点击按钮后，微信客户端将调起微信相册，完成选择操作后，将选择的相片发送给开发者的服务器，并推送事件给开发者，同时收起相册，随后可能会收到开发者下发的消息。
	/// </remarks>
	public const string PickWeixinPhoto = "pic_weixin";

	/// <summary>
	/// 弹出地理位置选择器
	/// </summary>
	/// <remarks>
	///	用户点击按钮后，微信客户端将调起地理位置选择工具，完成选择操作后，将选择的地理位置发送给开发者的服务器，同时收起位置选择工具，随后可能会收到开发者下发的消息。
	/// </remarks>
	public const string LocationSelect = "location_select";

	public static IEnumerable<string> GetValues()
	{
		yield return View;
		yield return Click;
		yield return MiniApp;
		yield return ScanCodePush;
		yield return ScanCodeWait;
		yield return PickPhoto;
		yield return PickPhotoOrAlbum;
		yield return PickWeixinPhoto;
		yield return LocationSelect;
	}

	public static string GetDescription(string type)
	{
		return type switch
		{
			View => "跳转URL",
			Click => "点击事件",
			MiniApp => "小程序",
			ScanCodePush => "扫码推送",
			ScanCodeWait => "扫码推送且弹出“消息接收中”提示框",
			PickPhoto => "弹出系统拍照发图",
			PickPhotoOrAlbum => "弹出拍照或者相册发图",
			PickWeixinPhoto => "弹出微信相册发图器",
			LocationSelect => "弹出地理位置选择器",
			_ => type
		};
	}
}