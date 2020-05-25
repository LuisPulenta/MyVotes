using MiApp.Common.Models;
using MiApp.Web.Data.Entities;
using MiApp.Web.Models;
using System.Collections.Generic;

namespace MiApp.Web.Helpers
{
    public interface IConverterHelper
    {
        ItemEntity ToItemEntity(ItemViewModel model, string path, bool isNew);

        ItemViewModel ToItemViewModel(ItemEntity ItemEntity);

        ItemResponse ToItemResponse(ItemEntity itemEntity);

        List<ItemResponse> ToItemResponse(List<ItemEntity> itemEntities);

        UserResponse ToUserResponse(UserEntity user);
    }
}