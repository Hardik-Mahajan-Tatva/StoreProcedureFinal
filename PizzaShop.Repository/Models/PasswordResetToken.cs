using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class PasswordResetToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiryTime { get; set; }

    public virtual Userslogin User { get; set; } = null!;
}
