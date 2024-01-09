using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Extension
{
    public static class ClaimsExtension
    {
        public static string GetName(this ClaimsPrincipal user){
             return  user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}