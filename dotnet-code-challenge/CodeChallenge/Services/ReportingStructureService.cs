using CodeChallenge.Models;
using CodeChallenge.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CodeChallenge.Services
{
    public class ReportingStructureService : IReportingStructureService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public ReportingStructureService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public ReportingStructure GetByEmployeeId(string id)
        {
            ReportingStructure reportingStructure = new ReportingStructure();
            if (!String.IsNullOrEmpty(id))
            {
                Employee emp = _employeeRepository.GetById(id);
                if (emp != null)
                {
                    reportingStructure.Employee = emp;
                    reportingStructure.NumberOfReports = GetNumberOfReports(emp);
                }
            }
            else
            {
                _logger.LogError("Id null or empty.");
            }

            return reportingStructure;
        }

        private int GetNumberOfReports(Employee emp)
        {
            Stack<Employee> empStack = new Stack<Employee>();
            int count = 0;

            empStack.Push(emp); //Push the initial employee onto the stack
            while (empStack.Count != 0)
            {
                Employee node = empStack.Pop(); //Pop top employee and count direct reports on that employee
                if (node.DirectReports != null)
                {
                    foreach (Employee e in node.DirectReports)
                    {
                        empStack.Push(_employeeRepository.GetById(e.EmployeeId)); //Push the subordinate employee to the stack
                        count++;
                    }
                }
            }

            return count;
        }
    }
}
