using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeManagement.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Žao nam je, nemožemo naći stranicu na toj adresi";
                    logger.LogWarning($"User: {User.Identity.Name}, 404 Error Occured Path = {statusCodeResult.OriginalPath}" + 
                        $" and Query String = {statusCodeResult.OriginalQueryString}");
                    break;
                case 401:
                    ViewBag.ErrorMessage = "Žao nam je, nemate prava za ovu akciju";
                    logger.LogWarning($"User: {User.Identity.Name}, 401 Error Occured Path = {statusCodeResult.OriginalPath}" +
                        $" and Query String = {statusCodeResult.OriginalQueryString}");
                    break;
                case 403:
                    ViewBag.ErrorMessage = "Žao nam je, nemate prava za ovu akciju";
                    logger.LogWarning($"User: {User.Identity.Name}, 403 Error Occured Path = {statusCodeResult.OriginalPath}" +
                        $" and Query String = {statusCodeResult.OriginalQueryString}");
                    break;
            }

            return View("Greska");
        }

        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            logger.LogError($"User: {User.Identity.Name}, The path {exceptionDetails.Path} threw an exception " +
                $"{exceptionDetails.Error}");

            return View("Error");
        }
    }
}
    