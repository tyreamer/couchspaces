using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;

namespace couchspacesBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FirebaseController : ControllerBase
    {
        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken([FromBody] string idToken)
        {
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                return Ok(decodedToken);
            }
            catch (FirebaseAuthException)
            {
                return Unauthorized();
            }
        }
    }
}