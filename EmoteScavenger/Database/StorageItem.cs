using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmoteScavenger.Database
{
    [Table("ItemTable")]
    public class StorageItem
    {
        [Column("key"), Required, Key]
        public string Key { get; set; }

        [Column("value"), Required]
        public string Value { get; set; }
    }
}
