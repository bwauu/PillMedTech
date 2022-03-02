using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PillMedTech.Models;
using System;

namespace PillMedTech.Controllers
{
  public class EmployeeController : Controller
  {
    private IPillMedTechRepository repository;
    private IHttpContextAccessor contextAcc;

    //Konstruktor
    public EmployeeController(IPillMedTechRepository repo, IHttpContextAccessor ctxAcc)
    {
      repository = repo;
      contextAcc = ctxAcc;
    }

    //VISAR UPP SIDOR DÄR NÅGOT KAN GÖRAS:
    //Val av sjukskrivning 
    public ViewResult StartEmployee()
    {
      return View();
    }

    //Sida för att rapportera in VAB
    public ViewResult ReportSickChild()
    {
      ViewBag.Children = repository.GetChildrenList();
      return View();
    }

    //Sida för att rapportera in sjukskrivning med intyg
    public ViewResult ReportSick()
    {
      return View();
    }

    //Tack-sidan när rapporteringen sparats 
    public ViewResult ThankYou()
    {
      return View();
    }


    //HANTERING AV SJUKSKRIVNING

    //Hantera VAB
    [HttpPost]
    public ViewResult ReportSickChild(SickErrand errand)
    {
      repository.ReportVAB(errand);
      repository.Log(DateTime.Now,
                             HttpContext.Connection.RemoteIpAddress.ToString(),
                             contextAcc.HttpContext.User.Identity.Name,
                             "Reported VAB");
      
      return View("ThankYou");
    }

    //Hantera sjukskrivning en dag
    public IActionResult ReportSickDay()
    {
      repository.ReportSickDay();
      repository.Log(DateTime.Now,
                             HttpContext.Connection.RemoteIpAddress.ToString(),
                             contextAcc.HttpContext.User.Identity.Name,
                             "Reported Sickday");
      
      return View("ThankYou");
    }

    //Hantera sjukskrivning med intyg
    [HttpPost]
    public ViewResult ReportSick(SickErrand errand)
    {
      repository.ReportSick(errand);
      repository.Log(DateTime.Now,
                             HttpContext.Connection.RemoteIpAddress.ToString(),
                             contextAcc.HttpContext.User.Identity.Name,
                             "Reported Sick (doctor)");
     
      return View("ThankYou");
    }

  }
}
