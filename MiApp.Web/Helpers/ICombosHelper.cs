using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MiApp.Web.Helpers
{
    public interface ICombosHelper
    {
        IEnumerable<SelectListItem> GetComboItems();

        IEnumerable<SelectListItem> GetComboUserTypes();
    }
}