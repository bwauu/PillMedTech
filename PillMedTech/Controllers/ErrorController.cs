using Microsoft.AspNetCore.Mvc;

namespace PillMedTech.Controllers
{

    /* 

    1. Datalagring.
        1.1. Olika databaser som hanterar olika typer av data (ex. inloggningsuppgifter, loggar, datahanteringen etc).

    4. Auktorisering.
        4.1. Begränsa behörighet (inte mer än man behöver tillgång till).

    7. Loggning.
        7.1. Logga viktiga saker (inloggningar, vad som gör, felmeddelanden).

 */
    public class ErrorController : Controller
    {
        public IActionResult CustomerError()
        {
            return View();
        }
    }
}
