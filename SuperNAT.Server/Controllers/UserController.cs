﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SuperNAT.Common;
using SuperNAT.Common.Bll;
using SuperNAT.Common.Models;
using SuperNAT.Server.Models;

namespace SuperNAT.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly JwtSetting _jwtSetting;
        public UserController(IOptions<JwtSetting> option)
        {
            _jwtSetting = option.Value;
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(User model)
        {
            using var bll = new UserBll();
            var rst = bll.Login(model);
            if (rst.Result)
            {
                //构造jwt token
                rst.Data.token = JwtHandler.GetToken(_jwtSetting, rst.Data);
            }
            return Json(rst);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Add(User model)
        {
            var rst = new ReturnResult<bool>();

            using var bll = new UserBll();
            if (model.id == 0)
            {
                model.user_id = EncryptHelper.CreateGuid();
                model.password = EncryptHelper.MD5Encrypt(model.password);
                if (string.IsNullOrEmpty(model.password))
                {
                    model.password = "123456";
                }
                rst = bll.Add(model);
            }
            else
            {
                if (!string.IsNullOrEmpty(model.password))
                {
                    model.password = EncryptHelper.MD5Encrypt(model.password);
                }
                rst = bll.Update(model);
            }

            return Json(rst);
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete(User model)
        {
            using var bll = new UserBll();
            var rst = bll.Delete(model);

            return Json(rst);
        }

        [HttpPost]
        [Route("Disable")]
        public IActionResult Disable(User model)
        {
            using var bll = new UserBll();
            model.is_disabled = !model.is_disabled;
            var rst = bll.Update(model);
            var text = model.is_disabled ? "禁用" : "启用";
            rst.Message = rst.Result ? $"{text}成功" : $"{text}失败";

            return Json(rst);
        }

        [HttpPost]
        [Route("GetOne")]
        public IActionResult GetOne(User model)
        {
            if (model.id == 0)
            {
                var defalut = new ReturnResult<User>()
                {
                    Result = true,
                    Data = new User()
                };
                return Json(defalut);
            }
            using var bll = new UserBll();
            var rst = bll.GetOne(model);
            return Json(rst);
        }

        [HttpPost]
        [Route("GetList")]
        public IActionResult GetList(User model)
        {
            using var bll = new UserBll();
            var rst = bll.GetList(model);
            return Json(rst);
        }
    }
}