using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TalentPlus.Shared
{
	public class ReAuthHandler : DelegatingHandler
	{

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var response = await base.SendAsync(request, cancellationToken);

			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				// Oh noes, user is not logged in - we got a 401
				// Log them in, this time hardcoded with Facebook but you would
				// trigger the login presentation in your application
				try
				{
					var user = await TalentDb.SetUserOrLogin(true);
					// we're now logged in again.

					// Clone the request
					//clonedRequest = await CloneRequest(request);

					request.Headers.Remove("X-ZUMO-AUTH");
					// Set the authentication header
					request.Headers.Add("X-ZUMO-AUTH", user.MobileServiceAuthenticationToken);

					// Resend the request
					response = await base.SendAsync(request, cancellationToken);
				}
				catch (InvalidOperationException)
				{
					// user cancelled auth, so lets return the original response
					return response;
				}
			}

			return response;
		}
	}
}
