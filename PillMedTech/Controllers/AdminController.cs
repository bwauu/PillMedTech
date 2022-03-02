using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PillMedTech.Models;
using System;


namespace PillMedTech.Controllers
{
  public class AdminController : Controller
  {
    private IPillMedTechRepository repository;
    private IHttpContextAccessor contextAcc;

    public AdminController(IPillMedTechRepository repo, IHttpContextAccessor ctxAcc)
    {
      repository = repo;
      contextAcc = ctxAcc;
    }

    //Visa upp söksidan för HR-personal
    public ViewResult HRStaff()
    {
      return View();
    }
    //Resultatet av sökningen
    public ViewResult EmployeeInfo(SickErrand errand)
    {
      var errandsList = repository.SortedErrands(errand.EmployeeID);
      repository.Log(DateTime.Now,
                             HttpContext.Connection.RemoteIpAddress.ToString(),
                             contextAcc.HttpContext.User.Identity.Name,
                             $"Searched for {errand.EmployeeID}");
      return View(errandsList);
    }



    public ViewResult ITStaff()
    {
      var loggList = repository.ViewLog();
      repository.Log(DateTime.Now,
                             HttpContext.Connection.RemoteIpAddress.ToString(),
                             contextAcc.HttpContext.User.Identity.Name,
                             "Looked at logglist");

      return View(loggList);
    }
  }
}
