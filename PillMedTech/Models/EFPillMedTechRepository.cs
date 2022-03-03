using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PillMedTech.Models
{
  public class EFPillMedTechRepository : IPillMedTechRepository
  {
    private ApplicationDbContext appContext;
    private IHttpContextAccessor contextAcc;
    private LoggDbContext loggContext;
    private IDataProtector protector;

    public EFPillMedTechRepository(ApplicationDbContext ctx, IHttpContextAccessor cont, LoggDbContext logg, IDataProtectionProvider protect)
    {
      appContext = ctx;
      contextAcc = cont;
      loggContext = logg;
      protector = protect.CreateProtector("CodeSec");
    }

    public IQueryable<Employee> Employees => appContext.Employees.Include(e => e.Childrens);
    public IQueryable<SickErrand> SickErrands => appContext.SickErrands;
    public IQueryable<Logger> Logging => loggContext.Loggers;
    public IQueryable<Children> Childrens => appContext.Childrens;
    
    //Hämtar en lista med barn tillhörande en specifik anställd
    public IQueryable<Children> GetChildrenList()
    {
      var userName = contextAcc.HttpContext.User.Identity.Name;

      var childrenList = Childrens.Where(ch => ch.EmployeeId == userName);
      return childrenList;
    }

    //Går igenom listan för att hitta ärenden gällande en specifik anställd
    public List<SickErrand> SortedErrands(string employeeId)
    {
      var currentEmp = SickErrands.Where(emp => emp.EmployeeID.Equals(employeeId)).FirstOrDefault();

      List<SickErrand> errands = new List<SickErrand>();

      foreach (SickErrand err in SickErrands)
      {
        if (err.EmployeeID.Equals(employeeId))
        {
          errands.Add(err);
        }
      }
      return errands;
    }

    public void ReportVAB(SickErrand errand)
    {
      if (!errand.Equals(null))
      {
        if (errand.SickErrandID.Equals(0))
        {
          DateTime endDate = errand.HomeFrom.AddDays(1);
          errand.HomeUntil = endDate;
          errand.TypeOfAbsence = "VAB";
          appContext.SickErrands.Add(errand);
        }
      }
      appContext.SaveChanges();

    }

    public void ReportSickDay()
    {
      var user = contextAcc.HttpContext.User.Identity.Name;
      SickErrand errand = new SickErrand { EmployeeID = user, ChildName = "ej aktuellt", HomeFrom = DateTime.Today, TypeOfAbsence = "Sjuk utan intyg" };
      DateTime endDate = errand.HomeFrom.AddDays(1);
      errand.HomeUntil = endDate;
      appContext.SickErrands.Add(errand);

      appContext.SaveChanges();
    }

    public void ReportSick(SickErrand errand)
    {
      if (!errand.Equals(null))
      {
        if (errand.SickErrandID.Equals(0))
        {
          errand.ChildName = "ej aktuellt";
          errand.TypeOfAbsence = "Sjuk med intyg";
          appContext.SickErrands.Add(errand);
        }
      }
      appContext.SaveChanges();
    }
 
        public void Log(DateTime createdAt, string IPAdress, string user, string action)
        {
            string stringedTime = createdAt.ToString();
            // disconnected existing entity 
            var logger = new Logger() { Time = (stringedTime), Ip = (IPAdress), EmployeeId = (user), Action = (action) };
            loggContext.Add(logger);
            loggContext.SaveChanges();

        }

        //Här tar jag bort så att datan på databasen ej är krypterad och skickar den till ITStaff view
        public List<Logger> ViewLog()
        {
            List<Logger> UnEncryptedList = new List<Logger>();
            var logList = from log in Logging
                          orderby log.Time
                          select log;
            foreach(var log in logList)
            {
                UnEncryptedList.Add(new Logger
                {
                    Time = protector.Unprotect(log.Time),
                    Ip = protector.Unprotect(log.Ip),
                    EmployeeId = protector.Unprotect(log.EmployeeId),
                    Action = protector.Unprotect(log.Action)
                });
            }

            
            return UnEncryptedList;
        }

    }
}

/*
  // 1 och 2
        //Lägger till Loggar till databasen och krypterar all infromation
        // Checklista 1.1, 1.2, 7.1, 7.2, 7.3, 7.4
        // checklista 6.1, 6.2, 6.3
    public void Log(DateTime createdAt, string IPAdress, string user, string action)
    {
            string createdAtString = createdAt.ToString();
            Logger logger = new Logger
            {
                Time = protector.Protect(createdAtString),
                Ip = protector.Protect(IPAdress),
                EmployeeId = protector.Protect(user),
                Action = protector.Protect(action)
            };
            loggContext.Loggers.Add(logger);
            loggContext.SaveChanges();
    }
        //Här tar jag bort så att datan på databasen ej är krypterad och skickar den till ITStaff view
        public List<Logger> ViewLog()
        {

            List<Logger> UnEncryptedList = new List<Logger>();

            var list = from s in Logging
                       orderby s.Time
                       select s;

            try {
                foreach (var item in list)
                {
                    try
                    {
                        UnEncryptedList.Add(new Logger
                        {
                            Time = protector.Unprotect(item.Time),
                            Ip = protector.Unprotect(item.Ip),
                            EmployeeId = protector.Unprotect(item.EmployeeId),
                            Action = protector.Unprotect(item.Action)
                        });
                    }
                    catch
                    {
                        UnEncryptedList.Add(new Logger
                        {
                            Time = "Något gick fel",
                            Ip = "Något gick fel",
                            EmployeeId = "Något gick fel",
                            Action = "Något gick fel"
                        });
                    }
                }
            }
            catch {
                UnEncryptedList.Add(new Logger
                {
                    Time = "Något gick fel",
                    Ip = "Något gick fel",
                    EmployeeId = "Något gick fel",
                    Action = "Något gick fel"
                });
            }
            //Sorterar listan så att de senaste händelserna händer högst upp
            List<Logger> UnEncryptedList2 = UnEncryptedList.OrderByDescending(er => er.Time).ToList();
            return UnEncryptedList2;
        } 
 */