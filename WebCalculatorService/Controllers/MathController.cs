using System.Web.Http;

namespace WebCalculatorService.Controllers
{
    public class MathController : ApiController
    {
        // Sample Call: http://localhost:8999/webcalcapp/api/add?a=1&b=2
        [HttpGet]
        public int Add(int a, int b)
        {
            return a + b;
        }

        // Sample Call: http://localhost:8999/webcalcapp/api/subtract?a=10&b=5
        [HttpGet]
        public int Subtract(int a, int b)
        {
            return a - b;
        }
    }
}