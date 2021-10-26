using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using Newtonsoft.Json;
using XboxAuthNet.OAuth;

namespace MeloncherCore.Account
{
	public class MinecraftAccount
	{
		[JsonProperty("AccountType")] public AccountType AccountType { get; set; }
		[JsonProperty("GameSession")] public MSession GameSession { get; set; }

		[JsonProperty("MicrosoftOAuthSession")]
		public MicrosoftOAuthResponse? MicrosoftOAuthSession { get; set; }

		[JsonProperty("XboxSession")] public AuthenticationResponse? XboxSession { get; set; }

		public MinecraftAccount()
		{
		}

		public MinecraftAccount(MSession gameSession)
		{
			AccountType = gameSession.ClientToken == null ? AccountType.Offline : AccountType.Mojang;
			GameSession = gameSession;
		}

		// public MinecraftAccount(SessionCache sessionCache)
		// {
		// 	AccountType = AccountType.Microsoft;
		// 	GameSession = sessionCache.GameSession;
		// 	MicrosoftOAuthSession = sessionCache.MicrosoftOAuthSession;
		// 	XboxSession = sessionCache.XboxSession;
		// }

		public MinecraftAccount(MSession gameSession, MicrosoftOAuthResponse microsoftOAuthSession, AuthenticationResponse xboxSession)
		{
			AccountType = AccountType.Microsoft;
			GameSession = gameSession;
			MicrosoftOAuthSession = microsoftOAuthSession;
			XboxSession = xboxSession;
		}

		public bool Validate()
		{
			return AccountType switch
			{
				AccountType.Microsoft => new MicrosoftLoginHandler().Validate(this),
				AccountType.Mojang => new MLogin().Validate(GameSession).Result == MLoginResult.Success,
				_ => true
			};
		}

		public bool Refresh()
		{
			return AccountType switch
			{
				AccountType.Microsoft => new MicrosoftLoginHandler().Refresh(this),
				AccountType.Mojang => new MLogin().Refresh(GameSession).Result == MLoginResult.Success,
				_ => false
			};
		}
	}

	public enum AccountType
	{
		Offline,
		Mojang,
		Microsoft
	}
}