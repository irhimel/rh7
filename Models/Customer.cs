using SQLite;

namespace AccountingApp.Models;

[Table("customers")]
public class Customer
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [MaxLength(200)]
    public string Name { get; set; }
    
    [MaxLength(100)]
    public string Email { get; set; }
    
    [MaxLength(20)]
    public string Phone { get; set; }
    
    public string Address { get; set; }
    
    public decimal Balance { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
