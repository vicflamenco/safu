using System.ComponentModel.DataAnnotations;

namespace safuCHARTS.Models
{
    public class TokenHitModel
    {
        [Required, MinLength(1)]
        public string TokenAddress { get; set; }

        [Required, MinLength(1)]
        public string TokenName { get; set; }
    }
}