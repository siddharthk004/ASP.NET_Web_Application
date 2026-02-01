using EmployeeCRUD.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Remoting.Lifetime;
using System.Security.Cryptography.Xml;

public class EmployeeVM
{
    // MAIN EMPLOYEE TABLE
    public Employee Employee { get; set; }

    // PROFILE RELATED TABLES
    public BasicInfo BasicInfo { get; set; }
    public List<EmployeeEmploymentHistory> EmploymentHistory { get; set; }
    public List<EmployeeEducationHistory> EducationHistory { get; set; }
    public List<Assessment> Assessments { get; set; }
    public List<License> Licenses { get; set; }
    public List<EmployeeTraining> Trainings { get; set; }

    // LEAVE & PAY
    public List<EmployeeLeaveBalance> Leaves { get; set; }
    public List<EmployeeLeaveBalance> LeaveBalances { get; set; }
    public List<EmployeePaySlip> PaySlips { get; set; }

    // FINANCE
    public List<EmployeeBankDetail> Banks { get; set; }
    public List<EmployeeLoan> Loans { get; set; }
    public List<EmployeeInsurance> Insurances { get; set; }
    public List<EmployeePension> Pensions { get; set; }

    // PEOPLE & RELATIONS
    public List<EmployeeFamily> Family { get; set; }
    public List<EmployeeEmergencyContact> EmergencyContacts { get; set; }
    public List<Reference> References { get; set; }

    // OTHER
    public List<EmployeeDisciplinaryLog> DisciplinaryLogs { get; set; }
    public List<EmployeeConcessionPass> ConcessionPasses { get; set; }
    public List<EmployeeRemuneration> Remunerations { get; set; }
    public List<EmployeeTimeZone> TimeZones { get; set; }
    public int? EmploymentType{ get; set; }
}
