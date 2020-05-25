using MiApp.Web.Data;
using MiApp.Web.Data.Entities;
using MiApp.Web.Helpers;
using MiApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MiApp.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AppUsersController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;

        public AppUsersController(
            DataContext context,
            IUserHelper userHelper,
            ICombosHelper combosHelper,
            IConverterHelper converterHelper,
            IImageHelper imageHelper,
            IMailHelper mailHelper
)
        {
            _dataContext = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
        }

        // GET: AppUsers
        public IActionResult Index()
        {
            return View(_dataContext.AppUsers
                .Include(o => o.User));
        }


        // GET: AppUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addUser = await _dataContext.AppUsers
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id.Value);
            if (addUser == null)
            {
                return NotFound();
            }

            return View(addUser);
        }



        // GET: AppUsers/Create
        public IActionResult Create()
        {
            var model = new AddUserViewModel
            {
                UserTypes = _combosHelper.GetComboUserTypes(),
            };
            return View(model);
        }

        // POST: AppUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserTypeId = 1;
                var user = await AddUser(model);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este email ya existe.");
                    model.UserTypes = _combosHelper.GetComboUserTypes();
                    return View(model);
                }

                var appUser = new AppUserEntity
                {
                    User = user,
                };

                _dataContext.AppUsers.Add(appUser);
                await _dataContext.SaveChangesAsync();

                var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                var tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                _mailHelper.SendMail(model.Username, "Confirmación de E-Mail", $"<h1>Confirmación de E-Mail</h1>" +
                    $"Para habilitar el Usuario, " +
                    $"por favor haga clic en el siguiente link: </br></br><a href = \"{tokenLink}\">Confirmar E-mail</a>");

                ViewBag.Message = "Las instrucciones para habilitar su Usuario han sido enviadas por mail.";
                return View(model);

            }

            model.UserTypes = _combosHelper.GetComboUserTypes();
            return View(model);
        }

        private async Task<UserEntity> AddUser(AddUserViewModel model)
        {
            var path = string.Empty;

            if (model.ImageFile != null)
            {
                path = await _imageHelper.UploadImageAsync(model.ImageFile, "Users");
            }

            var user = new UserEntity
            {
                Document = model.Document,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                PicturePath = path,
                Email = model.Username,
                UserName = model.Username,
            };

            var result = await _userHelper.AddUserAsync(user, model.Password);
            if (result != IdentityResult.Success)
            {
                return null;
            }
            var newUser = await _userHelper.GetUserAsync(model.Username);
            await _userHelper.AddUserToRoleAsync(newUser, "User");
            return newUser;
        }


        // GET: AppUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _dataContext.AppUsers
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (appUser == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Address = appUser.User.Address,
                Document = appUser.User.Document,
                FirstName = appUser.User.FirstName,
                Id = appUser.Id,
                LastName = appUser.User.LastName,
                PhoneNumber = appUser.User.PhoneNumber,
                PicturePath = appUser.User.PicturePath
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserTypeId = 1;

                var path = model.PicturePath;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\Users",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/Users/{file}";
                }


                var appUser = await _dataContext.AppUsers
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == model.Id);


                appUser.User.Document = model.Document;
                appUser.User.FirstName = model.FirstName;
                appUser.User.LastName = model.LastName;
                appUser.User.Address = model.Address;
                appUser.User.PhoneNumber = model.PhoneNumber;
                appUser.User.PicturePath = path;

                await _userHelper.UpdateUserAsync(appUser.User);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        // AppUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _dataContext.AppUsers
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (appUser == null)
            {
                return NotFound();
            }
            await _userHelper.DeleteUserAsync(appUser.User.Email);
            _dataContext.AppUsers.Remove(appUser);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction($"{nameof(Index)}");
        }



        private bool AppUserExists(int id)
        {
            return _dataContext.AppUsers.Any(e => e.Id == id);
        }
    }
}