using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerRepairSystem.domain.Entities
{
    public class DeviceSample
    {
        public Guid DeviceID { get; set; }
        public int CompanyID { get; set; } 
        public string DeviceCode { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public bool isActive { get; set; }
        public DateTime CreatedAt { get; set; }
        //Navigation property
        public Company Company { get; set; } = null!;

    }
}
