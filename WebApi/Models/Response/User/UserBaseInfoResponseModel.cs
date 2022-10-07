using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models.CommonModels;
using WebApi.Models.Response.User.Dtos;
using WebApi.DataAccessLayer;
using WebApi.DataAccessLayer.Models;
using System.Net;
using Microsoft.EntityFrameworkCore;
using WebApi.Models.DTO;

namespace WebApi.Models.Response.User
{
    public class UserBaseInfoResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public int YearOfEducation { get; set; }
        public string Phone { get; set; }
        public UserRole Role { get; set; }
        public int? CompanyId { get; set; }
        //public List<UserCompanyDto> UserCompanies { get; set; }
        //public ApprovalState ApprovalState { get; set; }

        public Result<UserBaseInfoResponseModel> Init(int userId, DB ctx, DataAccessLayer.Models.User current)
        {
            /*
            var user = ctx.Users
                .Include(x => x.Company)
                .FirstOrDefault(x => x.Id == userId);

            if (user is null)
            {
                return new Result<UserBaseInfoResponseModel>(HttpStatusCode.NotFound, new Error("Пользователь не найден"));
            }

            var userResponse = new UserBaseInfoResponseModel
            {
                Id = user.Id,
                Name = user.Name,
                MiddleName = user.MiddleName,
                Surname = user.Surname,
                YearOfEducation = user.YearOfEducation,
                Role = user.Role,
                CompanyId = user.Company != null ? user.Company.Id : null,
                Phone = current.Role == UserRole.Student && current.Id != user.Id
                    ? null
                    : user.Phone,
                ApprovalState = user.ApprovalState,
                UserCompanies = ctx.UserCompanies
                    .Where(x => x.UserId == userId)
                    .Select(UserCompanyDto.Projection)
                    .ToList()
            };*/

            return new Result<UserBaseInfoResponseModel>(this);
        }
    }
}
