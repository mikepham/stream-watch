namespace StreamWatchService.Api.Controllers
{
    using System.Web.Http;

    using NLog;

    [RoutePrefix("users")]
    public class UserController : ApiController
    {
        private readonly Logger logger;

        public UserController(Logger logger)
        {
            this.logger = logger;
        }

        [AllowAnonymous]
        [Route("authenticate")]
        [HttpGet]
        public object Post(string token)
        {
            return new object();
        }
    }
}