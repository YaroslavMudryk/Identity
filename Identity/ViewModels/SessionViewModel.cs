﻿using Extensions.DeviceDetector.Models;
using Identity.Models;

namespace Identity.ViewModels
{
    public class SessionViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool Current { get; set; }
        public SessionStatus Status { get; set; }
        public string Language { get; set; }
        public DateTime LastAccess { get; set; }
        public AppInfo App { get; set; }
        public LocationInfo Location { get; set; }
        public ClientInfo Device { get; set; }
        public DateTime? DeactivatedAt { get; set; }
    }
}
