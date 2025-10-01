using DotNetCoreCalendar.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreCalendar.Data
{
    public interface IDAL
    {
        List<Event> GetEvents();
        List<Event> GetMyEvents(string userid);
        Event GetEvent(int id);
        void CreateEvent(IFormCollection form);
        void UpdateEvent(IFormCollection form);
        void DeleteEvent(int id);

        List<Location> GetLocations();
        Location GetLocation(int id);
        void CreateLocation(Location location);

        void UpdateLocation(Location location);
        void DeleteLocation(int id);
    }

    public class DAL : IDAL
    {
        private readonly ApplicationDbContext db;   // <-- injected

        public DAL(ApplicationDbContext db)         // <-- ctor injection
        {
            this.db = db;
        }

        public List<Event> GetEvents() => db.Events.ToList();

        public List<Event> GetMyEvents(string userid) =>
            db.Events.Where(x => x.User.Id == userid).ToList();

        public Event GetEvent(int id) =>
            db.Events.FirstOrDefault(x => x.Id == id);

        public void CreateEvent(IFormCollection form)
        {
            var locname = form["Location"].ToString();
            var user = db.Users.FirstOrDefault(x => x.Id == form["UserId"].ToString());
            var newevent = new Event(form, db.Locations.FirstOrDefault(x => x.Name == locname), user);
            db.Events.Add(newevent);
            db.SaveChanges();
        }

        public void UpdateEvent(IFormCollection form)
        {
            var locname = form["Location"].ToString();
            var eventid = int.Parse(form["Event.Id"]);
            var myevent = db.Events.FirstOrDefault(x => x.Id == eventid);
            var location = db.Locations.FirstOrDefault(x => x.Name == locname);
            var user = db.Users.FirstOrDefault(x => x.Id == form["UserId"].ToString());
            myevent.UpdateEvent(form, location, user);
            db.Entry(myevent).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void DeleteEvent(int id)
        {
            var myevent = db.Events.Find(id);
            db.Events.Remove(myevent);
            db.SaveChanges();
        }

        public List<Location> GetLocations() => db.Locations.ToList();

        public Location GetLocation(int id) => db.Locations.Find(id);

        public void CreateLocation(Location location)
        {
            db.Locations.Add(location);
            db.SaveChanges();
        }

        public void UpdateLocation(Location location)
        {
            var existing = db.Locations.Find(location.Id);
            if (existing == null) return;
            existing.Name = location.Name;
            db.Entry(existing).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void DeleteLocation(int id)
        {
            var relatedEvents = db.Events
                .Where(e => EF.Property<int?>(e, "LocationId") == id)
                .ToList();

            foreach (var ev in relatedEvents)
            {
                ev.Location = null;
                db.Entry(ev).Property("LocationId").CurrentValue = null;
                db.Entry(ev).State = EntityState.Modified;
            }

            var loc = db.Locations.Find(id);
            if (loc == null) return;

            db.Locations.Remove(loc);
            db.SaveChanges();
        }
    }
}



