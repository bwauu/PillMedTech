using System;
using System.Collections.Generic;
using System.Linq;

namespace PillMedTech.Models
{
  public interface IPillMedTechRepository
  {
    //Tabeller i databasen
    IQueryable<Employee> Employees { get; }
    IQueryable<SickErrand> SickErrands { get; }
    IQueryable<Logger> Logging { get; }
    IQueryable<Children> Childrens { get; }


    IQueryable<Children> GetChildrenList();

    //Sökning av anställds sjukskrivning
    List<SickErrand> SortedErrands(string employeeId);


    //Sjukskrivningar
    void ReportVAB(SickErrand errand);
    void ReportSickDay();
    void ReportSick(SickErrand errand);
    
    void Log(DateTime createdAt, string IPAdress, string user, string action);
    IQueryable<Logger> ViewLog();
    
  }
}
