namespace Woa.Webapi.Domain;

public record WechatMessageDeleteCommand(long Id) : ICommand;