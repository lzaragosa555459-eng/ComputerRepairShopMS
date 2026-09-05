using ComputerRepairSystem.domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerRepairSystem.domain.entities
{
public class Company
    {
        public int CompanyId { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<DeviceSample> Devices { get; set; } = new List<DeviceSample>();



    }
}
