﻿using Microsoft.AspNetCore.DataProtection;
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


            // disconnected existing entity 
            var logger = new Logger() { Time = createdAt, Ip = IPAdress, EmployeeId = user, Action = action};

            using (loggContext)
            {
                loggContext.Entry(logger).State = logger.Id == 0 ? EntityState.Added : EntityState.Modified;

                loggContext.SaveChanges();
            }

        }

        public IQueryable<Logger> ViewLog()
        {

         return (IQueryable<Logger>)loggContext.Loggers;
        }
   
    }
}
