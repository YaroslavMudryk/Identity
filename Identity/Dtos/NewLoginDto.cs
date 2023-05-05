using System.ComponentModel.DataAnnotations;

namespace Identity.Dtos
{
    public class NewLoginDto
    {
        [Required, EmailAddress]
        public string NewLogin { get; set; }
        public string CodeMFA { get; set; }
    }
}
