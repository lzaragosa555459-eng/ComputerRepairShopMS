using System;

namespace ComputerRepairSystem.company.Entities;

public class Attendance
{
    public int AttendanceId { get; set; }

    // Employee being recorded
    public int EmployeeId { get; set; }

    // Date of attendance
    public DateTime AttendanceDate { get; set; }

    // Time employee arrived
    public TimeSpan? TimeIn { get; set; }

    // Time employee left
    public TimeSpan? TimeOut { get; set; }

    // Present, Absent, Late, Leave
    public string Status { get; set; } = "Present";

    public string? Remarks { get; set; }

    // Navigation property
    public Employee? Employee { get; set; }
}