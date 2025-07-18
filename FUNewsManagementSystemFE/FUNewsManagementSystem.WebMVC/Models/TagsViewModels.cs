﻿using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystem.WebMVC.Models
{
    public class TagsViewModels
    {
        public int TagId { get; set; }
        [MaxLength(50)]
        public string? TagName { get; set; }
        [MaxLength(400)]
        public string? Note { get; set; }
    }
}
