﻿using System.ComponentModel.DataAnnotations;

public class EditUserViewModel
{
    public string Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
