using Microsoft.AspNetCore.Mvc.Rendering;
using MiApp.Web.Data;
using System.Collections.Generic;
using System.Linq;
using MiApp.Common.Enums;

namespace MiApp.Web.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _context;

        public CombosHelper(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboItems()
        {
            List<SelectListItem> list = _context.Items.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = $"{t.Id}"
            })
                .OrderBy(t => t.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Elija un Item...]",
                Value = "0"
            });

            return list;
        }


        public IEnumerable<SelectListItem> GetComboUserTypes()
        {
            var list = _context.Items.Select(l => new SelectListItem
            {
                Text = l.Name,
                Value = $"{l.Id}"
            })
                .OrderBy(l => l.Text)
                .Where(l => l.Text == "zzz")
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione Rol del Usuario...]",
                Value = "0"
            });

            list.Insert(1, new SelectListItem
            {
                Text = UserType.Admin.ToString(),
                Value = "1"
            });

            list.Insert(2, new SelectListItem
            {
                Text = UserType.User.ToString(),
                Value = "2"
            });

            return list;
        }
    }
}