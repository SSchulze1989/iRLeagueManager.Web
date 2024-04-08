using System.Collections.Generic;
using System.Data;

namespace iRLeagueApiCore.Server.Filters;

/// <summary>
/// Sets requirement for having an active subscription in order to hit this endpoint -
/// Only works in combination with <see cref="CheckLeagueSubscriptionAttribute"/>
/// <para>Endpoint will return "402 - Payment Required" if no subscription available</para>
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
internal sealed class RequireSubscriptionAttribute : Attribute
{
}
