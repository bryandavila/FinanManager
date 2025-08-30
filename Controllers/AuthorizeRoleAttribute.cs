using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;

public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
{
  private readonly int[] _allowedRoles;

  public AuthorizeRoleAttribute(params int[] allowedRoles)
  {
    _allowedRoles = allowedRoles;
  }

  public void OnAuthorization(AuthorizationFilterContext context)
  {
    var roleClaim = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "RoleID");

    if (roleClaim == null || !int.TryParse(roleClaim.Value, out int roleId) || !_allowedRoles.Contains(roleId))
    {
      context.Result = new RedirectToActionResult("Index", "AccesoDenegado", null);
    }
  }
}
