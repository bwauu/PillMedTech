using System.ComponentModel.DataAnnotations;

namespace PillMedTech.Models
{
    /*
     
     2. Datainhämtning.
        2.1. Använda formulärkontroller där användaren väljer istället för att skriva (så långt det är möjligt).
        2.2. Validera input med data annotations (som läggs på dataklasserna s.k poco-klasser) – vilket tvingar användaren att ge oss rätt data. 

     */
    public class LoginModel
    {
        [Required(ErrorMessage = "Vänligen fyll i användarnamn")]
        [Display(Name = "Användarnamn:")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vänligen fyll i lösenord")]
        [UIHint("password")]
        [Display(Name = "Lösenord:")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
