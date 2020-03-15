using System.ComponentModel.DataAnnotations;

namespace GainBargain.DAL.Entities
{
    public class Market
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
