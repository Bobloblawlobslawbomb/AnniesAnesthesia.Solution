using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnniesAnesthesia.Models;
using System.Collections.Generic;
using System.Linq;

namespace AnniesAnesthesia.Controllers
{
  public class DoctorsController : Controller
  {
    private readonly ClinicContext _db;
    public DoctorsController(ClinicContext db)
    {
      _db = db;
    }


    //working here to get a list of each patient associated with a particular doctor
    public ActionResult Index()
    {
      List<Doctor> model = _db.Doctors.ToList();
      var patientCount = _db.DoctorPatient.FromSqlRaw("SELECT COUNT(*) FROM _db.DoctorPatient").ToString();
      ViewBag.PatientCount = patientCount;
      // .Include(doctor => doctor.DoctorId == id);


      //   "WHILE (SELECT DoctorId FROM DoctorPatient) > 0 BEGIN COUNT(*) FROM DoctorPatient WHERE DoctorId != 0"
      // );
      // SQL query: WHILE (SELECT DoctorId FROM DoctorPatients) > 0
      // BEGIN
      // SELECT COUNT(*) FROM DoctorPatients WHERE DoctorId > 0
      return View(model);
    }

    public ActionResult Create()
    {
      ViewBag.SpecialtyId = new SelectList(_db.Specialties, "SpecialtyId", "SpecialtyName");
      return View();
    }

    [HttpPost]
    public ActionResult Create(Doctor doctor, Specialty specialty, int SpecialtyId)
    {
      _db.Doctors.Add(doctor);
      _db.SaveChanges();
      if (SpecialtyId != 0)
      {
        _db.DoctorSpecialties.Add(new DoctorSpecialty() { DoctorId = doctor.DoctorId, SpecialtyId = specialty.SpecialtyId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisDoctor = _db.Doctors
        .Include(doctor => doctor.JoinEntities)
        .ThenInclude(join => join.Patient)
        .FirstOrDefault(doctor => doctor.DoctorId == id);
      return View(thisDoctor);
    }

    public ActionResult Edit(int id)
    {
      var thisDoctor = _db.Doctors
        .Include(doctor => doctor.JoinEntitiesSpecialty)
        .ThenInclude(join => join.Specialty)
        .FirstOrDefault(doctor => doctor.DoctorId == id);
      ViewBag.SpecialtyId = new SelectList(_db.Specialties, "SpecialtyId", "SpecialtyName");
      return View(thisDoctor);
    }

    [HttpPost]
    public ActionResult Edit(Doctor doctor, Specialty specialty, int SpecialtyId)
    {
      if (SpecialtyId != 0)
      {
        _db.DoctorSpecialties.Add(new DoctorSpecialty() { DoctorId = doctor.DoctorId, SpecialtyId = specialty.SpecialtyId });
      }
      _db.Entry(doctor).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    //  public ActionResult Edit(int id)
    // {
    //   var thisDoctor = _db.Doctors.FirstOrDefault(doctor => doctor.DoctorId == id);
    //   return View(thisDoctor);
    // }

    // [HttpPost]
    // public ActionResult Edit(Doctor doctor)
    // {
    //   _db.Entry(doctor).State = EntityState.Modified;
    //   _db.SaveChanges();
    //   return RedirectToAction("Index");
    // }

    public ActionResult Delete(int id)
    {
      var thisDoctor = _db.Doctors.FirstOrDefault(doctor => doctor.DoctorId == id);
      return View(thisDoctor);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisDoctor = _db.Doctors.FirstOrDefault(doctor => doctor.DoctorId == id);
      _db.Doctors.Remove(thisDoctor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}