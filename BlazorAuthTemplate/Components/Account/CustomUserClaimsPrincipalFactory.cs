using BlazorAuthTemplate.Client;
using BlazorAuthTemplate.Data;
using BlazorAuthTemplate.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BlazorAuthTemplate.Components.Account
{
    public class CustomUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, 
															RoleManager<IdentityRole> roleManager,
															IOptions<IdentityOptions> options)
		: UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(userManager, roleManager, options)
	{
		protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
		{
			ClaimsIdentity identity = await base.GenerateClaimsAsync(user);

			string profilePicture = user.ImageId.HasValue ? $"/api/uploads/{user.ImageId}" : UploadHelper.DefaultProfilePicture;

			List<Claim> customClaims = [
				new Claim(nameof(UserInfo.ProfilePictureUrl), profilePicture),
				new Claim(nameof(UserInfo.FirstName), user.FirstName!),
				new Claim(nameof(UserInfo.LastName), user.LastName!),
				];
			identity.AddClaims(customClaims);

			return identity;
		}
	}
}
