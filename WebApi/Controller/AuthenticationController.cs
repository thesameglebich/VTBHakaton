﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebApi.DataAccessLayer.Models;
using WebApi.Extensions.Attributes;
using WebApi.Extensions.HttpContextExtensions;
using WebApi.JWT;
using WebApi.Models.CommonModels;
using WebApi.Models.Requests;
using WebApi.Models.Response;

namespace WebApi.Controller
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly Authenticator _authenticator;
        private readonly RefreshTokenValidator _refreshTokenValidator;
        private readonly RefreshTokenRepository _refreshTokenRepository;

        public AuthenticationController(UserManager<User> userManager, Authenticator authenticator, RefreshTokenValidator refreshTokenValidator, RefreshTokenRepository refreshTokenRepository)
        {
            _userManager = userManager;
            _authenticator = authenticator;
            _refreshTokenValidator = refreshTokenValidator;
            _refreshTokenRepository = refreshTokenRepository;
        }

        [HttpPost("login")]
        [ValidateModel]
        public async Task<Result<AuthenticatedUserResponse>> Login([FromBody] LoginRequest request)
        {
            // ToDo: Behaviour for already authorized users

            var user = await _userManager.Users.IgnoreQueryFilters().SingleOrDefaultAsync(u => u.Email.Equals(request.Email));
            if (user == null)
            {
                return new Result<AuthenticatedUserResponse>(HttpStatusCode.NotFound, new Error("Пользователя с таким email не существует"));
            }

            var res = await _userManager.CheckPasswordAsync(user, request.Password);
            if (res)
            {
                return new Result<AuthenticatedUserResponse>(await _authenticator.AuthenticateAsync(user));
            }

            return new Result<AuthenticatedUserResponse>(HttpStatusCode.Unauthorized, new Error("Неверный пароль"));
        }

        [HttpPost("register")]
        [ValidateModel]
        public async Task<Result> Register([FromBody] RegisterRequest request)
        {
            var now = DateTime.UtcNow;
            // ToDo: Разобраться с именем пользователя
            User registrationUser = new()
            {
                CreatedOn = now,
                UpdatedOn = now,
                Role = UserRole.Worker,
                Name = request.Name,
                Surname = request.Surname,
                MiddleName = request.MiddleName,
                Email = request.Email,
            }; //добавить регистрацию по ссылку приглашению к группе
            registrationUser.UserName = "worker" + Guid.NewGuid().ToString();

            var res = await _userManager.CreateAsync(registrationUser, request.Password);

            if (!res.Succeeded)
            {
                return new Result(HttpStatusCode.BadRequest, res.Errors.Select(e => new Error(e.Description)).ToList());
            }

            return new Result(HttpStatusCode.OK);
        }

        [HttpPost("refresh")]
        [ValidateModel]
        public async Task<Result<AuthenticatedUserResponse>> Refresh([FromBody] RefreshRequest request)
        {
            bool refreshTokenIsValid = _refreshTokenValidator.Validate(request.RefreshToken);
            if (!refreshTokenIsValid)
            {
                return new Result<AuthenticatedUserResponse>(HttpStatusCode.BadRequest, new Error("Рефреш токен невалиден"));
            }

            RefreshToken tokenDBO = _refreshTokenRepository.FindByToken(request.RefreshToken);
            if (tokenDBO == null)
            {
                return new Result<AuthenticatedUserResponse>(HttpStatusCode.NotFound, new Error("Рефреш токен не найден"));
            }

            await _refreshTokenRepository.DeleteByIdAsync(tokenDBO.Id);

            User user = await _userManager.FindByIdAsync(tokenDBO.UserId.ToString());
            if (user == null)
            {
                return new Result<AuthenticatedUserResponse>(HttpStatusCode.NotFound, new Error("Рефреш токен не принадлежит этому пользователю"));
            }

            return new Result<AuthenticatedUserResponse>(await _authenticator.AuthenticateAsync(user));

        }

        [Authorize]
        [HttpDelete("logout")]
        public async Task<Result> Logout()
        {
            int? userId = User.Id();

            if (userId == null)
            {
                return new Result(HttpStatusCode.Unauthorized, new Error("Пользователь не авторизован"));
            }

            await _refreshTokenRepository.DeleteAllAsync(userId.Value);

            return new Result(HttpStatusCode.OK);
        }
    }
}
