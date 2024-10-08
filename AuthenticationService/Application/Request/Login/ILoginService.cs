﻿using AuthenticationService.Application.Request.Login.Command;
using AuthenticationService.Models;
using Common;

namespace AuthenticationService.Application.Request.Login
{
    public interface ILoginService
    {
        Task<Result> AddUser(AddOrEditUser request);
        Task<Result> SaveUserDocument(SaveUserDocument request);
        Task<Result> UpdateUser(Updateuser request);
        Task<User> VerifyUser(LoginUser request);
        Task<Result> ResetPassword(ResetPassword request);
        Task<List<Models.Type>> GetAllDocumentType();
        Task<Result> SendOtp(SendOtp request);
        Task<Result> ValidateOTP(ValidateOtp request);

    }
}
