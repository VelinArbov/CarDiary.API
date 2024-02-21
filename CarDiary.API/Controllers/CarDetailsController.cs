using CarDiary.Data;
using CarDiary.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace CarDiary.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarDetailsController : ControllerBase
    {
        private readonly CarDiaryContext _carDetailsContext;

        public CarDetailsController(CarDiaryContext carDiaryContext)
        {
            _carDetailsContext = carDiaryContext;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAsync()
        {
           var cars = await _carDetailsContext.CarDetails!
               .Where(cd => RequestCanAccessToDo(cd.Id))
               .ToListAsync();

            return Ok(cars);
        }

        [HttpPost]
        [RequiredScopeOrAppPermission(
                                RequiredScopesConfigurationKey = "AzureAD:Scopes:Write",
                                RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:Write"
                            )]
        public async Task<IActionResult> PostAsync([FromBody] CarDetails toDo)
        {
            // Only let applications with global to-do access set the user ID or to-do's
            var ownerIdOfTodo = IsAppMakingRequest() ? toDo.Id : GetUserId();

            var newToDo = new CarDetails()
            {
                Id = Guid.NewGuid(),
                Number = "EH9873KX"
            };

            await _carDetailsContext.CarDetails!.AddAsync(newToDo);
            await _carDetailsContext.SaveChangesAsync();

            return Created($"/todo/{newToDo!.Id}", newToDo);
        }


        private bool IsAppMakingRequest()
        {
            if (HttpContext.User.Claims.Any(c => c.Type == "idtyp"))
            {
                return HttpContext.User.Claims.Any(c => c.Type == "idtyp" && c.Value == "app");
            }
            else
            {
                return HttpContext.User.Claims.Any(c => c.Type == "roles") && !HttpContext.User.Claims.Any(c => c.Type == "scp");
            }
        }

        private bool RequestCanAccessToDo(Guid userId)
        {
            return IsAppMakingRequest() || (userId == GetUserId());
        }

        private Guid GetUserId()
        {
            Guid userId;
            if (!Guid.TryParse(HttpContext.User.GetObjectId(), out userId))
            {
                throw new Exception("User ID is not valid.");
            }
            return userId;
        }

    }
}
