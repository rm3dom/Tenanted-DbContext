using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Example.Hello.Model
{
    [Table("hello")]
    public class Hello
    {
        [Key]
        [Column("hello_id")]
        public long HelloId { get; set; }

        [Column("say")]
        public string Say { get; set; } = "";
    }
}