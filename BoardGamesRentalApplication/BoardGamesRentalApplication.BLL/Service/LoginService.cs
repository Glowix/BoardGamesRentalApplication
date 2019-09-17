﻿using System;
using System.Linq;
using BoardGamesRentalApplication.BLL.Enums;
using BoardGamesRentalApplication.BLL.IService;
using BoardGamesRentalApplication.DAL.Abstraction;
using BoardGamesRentalApplication.DAL.Models;

namespace BoardGamesRentalApplication.BLL.Service
{
    public class LoginService : ILoginService
    {
        private readonly IRepository<User> userRepository;
        private readonly ICryptographyService cryptographyService;

        public LoginService(IRepository<User> userRepository, ICryptographyService cryptographyService)
        {
            this.userRepository = userRepository;
            this.cryptographyService = cryptographyService;
        }

        public LoginServiceResponse Login(User user)
        {
            try
            {
                User matchingUser = userRepository.FindBy(u => u.Username == user.Username).FirstOrDefault();
                if (matchingUser == null)
                {
                    return LoginServiceResponse.UserDoesntExist;
                }
                else
                {
                    byte[] hash = cryptographyService.GenerateSHA512(user.Password, matchingUser.Salt);
                    byte[] hashForComparison = Convert.FromBase64String(matchingUser.Password);
                    for (int i = 0; i < hashForComparison.Length; i++)
                    {
                        if (hash[i] != hashForComparison[i])
                        {
                            return LoginServiceResponse.IncorrectPassword;
                        }
                    }
                    matchingUser.LastLogin = DateTime.Now;
                    userRepository.SaveChanges();
                    return LoginServiceResponse.LoginSuccessful;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
