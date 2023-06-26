using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ship_convenient.Core.CoreModel;

namespace ship_convenient.Controllers
{
    [Route("api/v1.0/[controller]s")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected IActionResult SendResponse<T>(T response) where T : ApiResponse
        {
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        
        protected IActionResult SendResponse<T>(ApiResponse<T> response) where T : class
        {
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        protected IActionResult SendResponse<T>(ApiResponsePaginated<T> response) where T : class
        {
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        protected IActionResult SendResponse(ApiResponseListError response)
        {
            if (response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

    }
}
