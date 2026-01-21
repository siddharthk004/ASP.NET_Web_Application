using System.ComponentModel.DataAnnotations;

namespace Login_Project_Simulation.loginDTO
{
    public class LoginDTOc
    {
        [Required(ErrorMessage ="Username is Required")]
        [MinLength(2,ErrorMessage ="Username must be at least 2 character")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Username is Required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 character")]
        public string Password { get; set; }
    }
}
