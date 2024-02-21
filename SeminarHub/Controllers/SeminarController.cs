using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;
using SeminarHub.Models;

namespace SeminarHub.Controllers
{
    [Authorize]
    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext _context;

        public SeminarController(SeminarHubDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {

            var s = await _context.Seminars
                .AsNoTracking()
                .Select(s => new SeminarAllViewModel(
                s.Id,
                s.Topic,
                s.Lecturer,
                s.Category.Name,
                s.DateAndTime,
                s.Organizer.UserName))
                .ToListAsync();

            return View(s);

        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var seminars = await _context.Seminars
                .Where(s => s.Id == id)
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (seminars == null)
            {
                return BadRequest();
            }

            if (seminars.SeminarsParticipants.Any(s => s.ParticipantId == GetUserId()))
            {
                return RedirectToAction(nameof(Joined));
            }

            seminars.SeminarsParticipants.Add(new SeminarParticipant()
            {
                ParticipantId = GetUserId(),
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            var userId = GetUserId();

            var model = await _context.SeminarsParticipants
                .Where(se => se.ParticipantId == userId)
                .AsNoTracking()
                .Select(se => new SeminarAllViewModel(
               se.SeminarId,
               se.Seminar.Topic,
               se.Seminar.Lecturer,
               se.Seminar.Category.Name,
               se.Seminar.DateAndTime,
               se.Seminar.Organizer.UserName
               ))
                .ToListAsync();

            return View(model);
        }


        public async Task<IActionResult> Leave(int id)
        {
            var seminars = await _context.Seminars
                .Where(s => s.Id == id)
                .Include(s => s.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (seminars == null)
            {
                return BadRequest();
            }

            var se = seminars.SeminarsParticipants
                .FirstOrDefault(s => s.ParticipantId == GetUserId());

            if (se == null)
            {
                return BadRequest();
            }

            seminars.SeminarsParticipants.Remove(se);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new SeminarEditViewModel
            {
                Categories = await GetCategories()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SeminarEditViewModel model)
        {
            

            

            var ent = new Seminar()
            {
                Topic = model.Topic,
                Lecturer = model.Lecturer,
                Details = model.Details,
                DateAndTime = model.DateAndTime,
                Duration = model.Duration,
                CategoryId = model.CategoryId,
                OrganizerId = GetUserId()


            };
            await _context.Seminars.AddAsync(ent);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(All));

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var sem = await _context.Seminars
                .FindAsync(id);

            if (sem == null)
            {
                return BadRequest();
            }

            if (sem.OrganizerId != GetUserId())
            {
                return BadRequest();
            }

            var model = new SeminarEditViewModel
            {
                Topic = sem.Topic,
                Lecturer = sem.Lecturer,
                Details = sem.Details,
                DateAndTime = sem.DateAndTime,
                Duration = sem.Duration,
                CategoryId = sem.CategoryId,
                Categories = await GetCategories()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SeminarEditViewModel model, int id)
        {
            var sem = await _context.Seminars
                .FindAsync(id);

            if (sem == null)
            {
                return BadRequest();
            }

            if (sem.OrganizerId != GetUserId())
            {
                return BadRequest();
            }

            sem.DateAndTime = model.DateAndTime;
            sem.Details = model.Details;
            sem.Topic = model.Topic;
            sem.Lecturer = model.Lecturer;
            sem.Duration = model.Duration;
            sem.CategoryId = model.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }
        
        public async Task<IActionResult> Details(int id)
        {
            var model = await _context.Seminars
                .Where(se => se.Id == id)
                .AsNoTracking()
                .Select(se => new SeminarDetailsViewModel()
                {
                    
                    DateAndTime = se.DateAndTime.ToString(DataConstants.DateTimeFormat),
                    Duration = se.Duration,
                    Lecturer = se.Lecturer,
                    Details = se.Details,
                    Organizer = se.Organizer.UserName,
                    Category = se.Category.Name,
                    Topic = se.Topic,
                    Id=se.Id
                })
                .FirstOrDefaultAsync();

            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            

            var se = _context.Seminars.Find(id);
            
            if (se == null)
            {
                return BadRequest();
            }

            return View(se);
        }

        [HttpPost]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            var seminar = _context.Seminars.Include(s => s.SeminarsParticipants).FirstOrDefault(s => s.Id == id);

            if (seminar != null)
            {
                
                _context.SeminarsParticipants.RemoveRange(seminar.SeminarsParticipants);

              
                _context.Seminars.Remove(seminar);

                
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(All));
        }
        
        
        
        
        
        
        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? String.Empty;
        }

        private async Task<IEnumerable<CategoriesViewModel>> GetCategories()
        {
            return await _context.Categories
                .AsNoTracking()
                .Select(c => new CategoriesViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToListAsync();
        }
    }
}
