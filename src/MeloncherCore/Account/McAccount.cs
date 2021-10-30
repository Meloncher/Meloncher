using System;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using Newtonsoft.Json;
using XboxAuthNet.OAuth;

namespace MeloncherCore.Account
{
	public class McAccount
	{
		public McAccount()
		{
		}

		public McAccount(MSession gameSession)
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

		public McAccount(MSession gameSession, MicrosoftOAuthResponse microsoftOAuthSession, AuthenticationResponse xboxSession)
		{
			AccountType = AccountType.Microsoft;
			GameSession = gameSession;
			MicrosoftOAuthSession = microsoftOAuthSession;
			XboxSession = xboxSession;
		}
		
		public McAccount(string username)
		{
			AccountType = AccountType.Offline;
			GameSession = new MSession(username, null, null);
		}

		[JsonProperty("AccountType")] public AccountType AccountType { get; set; }
		[JsonProperty("GameSession")] public MSession GameSession { get; set; }

		[JsonProperty("MicrosoftOAuthSession")]
		public MicrosoftOAuthResponse? MicrosoftOAuthSession { get; set; }

		[JsonProperty("XboxSession")] public AuthenticationResponse? XboxSession { get; set; }

		public bool Validate()
		{
			if (AccountType == AccountType.Microsoft) return new MicrosoftLoginHandler().Validate(this);

			if (AccountType == AccountType.Mojang)
				try
				{
					return new MLogin().Validate(GameSession).Result == MLoginResult.Success;
				}
				catch (Exception)
				{
					return false;
				}

			return true;
		}

		public bool Refresh()
		{
			if (AccountType == AccountType.Microsoft) return new MicrosoftLoginHandler().Refresh(this);

			if (AccountType == AccountType.Mojang)
				try
				{
					return new MLogin().Refresh(GameSession).Result == MLoginResult.Success;
				}
				catch (Exception)
				{
					return false;
				}

			return false;
		}
	}

	public enum AccountType
	{
		Offline,
		Mojang,
		Microsoft
	}
}