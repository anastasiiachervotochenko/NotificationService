using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Domain.Contracts.Models.DomainModels;
using NotificationService.Domain.Contracts.Models.RequestModel;
using NotificationService.Domain.Exceptions;
using NotificationService.Manager;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppController : ControllerBase
    {
        private readonly IAppManager _manager;

        public AppController(IAppManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        [Route("AllNotifications")]
        public async Task<IActionResult> GetAllNotifications()
        {
            var result = await _manager.GetNotifications();

            return new JsonResult(result);
        }

        [HttpGet]
        [Route("Notification/{id}")]
        public async Task<IActionResult> GetNotificationById([FromRoute] string id)
        {
            try
            {
                var result = await _manager.GetNotificationById(id);

                return new JsonResult(result);
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("AllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _manager.GetAllUsers();

            return new JsonResult(result);
        }

        [HttpGet]
        [Route("User/{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            try
            {
                var result = await _manager.GetUserById(id);

                return new JsonResult(result);
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("Create/Notification")]
        public async Task<IActionResult> CreateNotification(CreateNotificationRequestModel notificationModel)
        {
            try
            {
                await _manager.CreateNotification(notificationModel);

                return Ok();
            }
            catch (ValidationException ex)
            {
                return StatusCode(422, ex.Message);
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Create/User")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestModel userModel)
        {
            try
            {
                await _manager.CreateUser(userModel);

                return Ok();
            }
            catch (ValidationException ex)
            {
                return StatusCode(422, ex.Message);
            }
            catch (Exception ex) when (ex is NotFoundException ||
                                       ex is DuplicateEmailException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Add/UserConfig")]
        public async Task<IActionResult> AddUserConfig([FromBody] AddUserConfigRequestModel userConfigRequestModel)
        {
            try
            {
                await _manager.AddUserConfig(userConfigRequestModel);

                return Ok();
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("SendNotification/{id}")]
        public async Task<IActionResult> SendNotification([FromRoute] string id)
        {
            try
            {
                await _manager.SendNotification(id);

                return Ok();
            }
            catch (Exception ex) when (ex is NotificationException ||
                                       ex is SendNotificationException)
            {
                return StatusCode(422, ex.Message);
            }

            catch (SendEmailException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("Delete/Notification/{id}")]
        public async Task<IActionResult> DeleteNotification([FromRoute] string id)
        {
            try
            {
                await _manager.DeleteNotification(id);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Update/Notification")]
        public async Task<IActionResult> UpdateNotification([FromBody] NotificationModel notification)
        {
            try
            {
                await _manager.UpdateNotification(notification);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Cancel/Notification")]
        public async Task<IActionResult> CancelNotification([FromBody] CancelNotificationRequestModel model)
        {
            try
            {
                await _manager.CancelNotification(model);

                return Ok();
            }
            catch (Exception ex) when (ex is NotFoundException || ex is NotificationException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("Update/UserConfig")]
        public async Task<IActionResult> UpdateUserConfig([FromBody] UserConfigModel userConfig)
        {
            try
            {
                await _manager.UpdateUserConfig(userConfig);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("Delete/UserConfig/{id}")]
        public async Task<IActionResult> DeleteUserConfig([FromRoute] string id)
        {
            try
            {
                await _manager.DeleteUserConfig(id);
                return Ok();
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}