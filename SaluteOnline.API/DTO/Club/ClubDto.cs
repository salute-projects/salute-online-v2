﻿using System;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.DTO.Club
{
    public class ClubDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Registered { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
        public int CreatorId { get; set; }
        public ClubStatus Status { get; set; }
        public string Logo { get; set; }
        public bool CanBeEdited { get; set; }
    }
}
