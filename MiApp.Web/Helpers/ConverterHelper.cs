using MiApp.Common.Models;
using MiApp.Web.Data.Entities;
using MiApp.Web.Models;
using System.Collections.Generic;

namespace MiApp.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public ItemEntity ToItemEntity(ItemViewModel model, string path, bool isNew)
        {
            return new ItemEntity
            {
                Id = isNew ? 0 : model.Id,
                LogoPath = path,
                Name = model.Name,
                Active=model.Active,
                Date=model.Date,
                Price = model.Price,
                Quantity = model.Quantity
            };
        }

        public ItemViewModel ToItemViewModel(ItemEntity ItemEntity)
        {
            return new ItemViewModel
            {
                Id = ItemEntity.Id,
                LogoPath = ItemEntity.LogoPath,
                Name = ItemEntity.Name,
                Active = ItemEntity.Active,
                Date = ItemEntity.Date,
                Price = ItemEntity.Price,
                Quantity = ItemEntity.Quantity
            };
        }

        public ItemResponse ToItemResponse(ItemEntity itemEntity)
        {
            return new ItemResponse
            {
                Active = itemEntity.Active,
                Date = itemEntity.Date,
                Id = itemEntity.Id,
                LogoPath = itemEntity.LogoPath,
                Name = itemEntity.Name,
                Price = itemEntity.Price,
                Quantity = itemEntity.Quantity,
            };
        }

        public List<ItemResponse> ToItemResponse(List<ItemEntity> itemEntities)
        {
            List<ItemResponse> list = new List<ItemResponse>();
            foreach (ItemEntity itemEntity in itemEntities)
            {
                list.Add(ToItemResponse(itemEntity));
            }

            return list;
        }

        public UserResponse ToUserResponse(UserEntity user)
        {
            if (user == null)
            {
                return null;
            }

            return new UserResponse
            {
                Address = user.Address,
                Document = user.Document,
                Email = user.Email,
                FirstName = user.FirstName,
                Id = user.Id,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                PicturePath = user.PicturePath,
                UserType = user.UserType
            };
        }
    }
}