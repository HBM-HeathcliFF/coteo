﻿using coteo.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace coteo.Domain.Entities
{
    public class Department : EntityBase
    {
        public string LeaderId { get; set; }
        [ForeignKey("LeaderId")]
        public ApplicationUser Leader { get; set; }

        public string OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }
    }
}