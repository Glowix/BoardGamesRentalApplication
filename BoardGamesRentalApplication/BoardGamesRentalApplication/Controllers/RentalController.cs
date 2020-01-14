﻿using BoardGamesRentalApplication.BLL.Enums;
using BoardGamesRentalApplication.BLL.IService;
using BoardGamesRentalApplication.DAL.Models;
using BoardGamesRentalApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BoardGamesRentalApplication.Controllers
{
    public class RentalController : Controller
    {
        private readonly IReservationService reservationService;
        private readonly IBoardGamesService boardGamesService;

        public RentalController(IReservationService reservationService, IBoardGamesService boardGamesService)
        {
            this.reservationService = reservationService;
            this.boardGamesService = boardGamesService;
        }

        [HttpGet]
        public ActionResult Index(DateTime rentalStartDate, DateTime rentalEndDate, int boardGameId, string boardGameName, int count, string discountCode, string rentalCostPerDay, string imagePath)
        {
            decimal totalCost = reservationService.CaculateTotalCostByBoardGameDiscountCode(discountCode, boardGameId, rentalStartDate, rentalEndDate, decimal.Parse(rentalCostPerDay));

            Session["TotalCost"] = totalCost; //brzydka lata

            ViewBag.ImagePath = imagePath;
            Models.Reservation reservation = new Models.Reservation
            {
                RentalStartDate = rentalStartDate,
                RentalEndDate = rentalEndDate,
                BoardGameId = boardGameId,
                Count = count,
                TotalCost = totalCost,
                BoardGameName = boardGameName,
                BoardGameIsAvailable = boardGamesService.IsAvailable(boardGameId),
                ImagePath = imagePath
            };
            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Models.Reservation model)
        {
            if (ModelState.IsValid)
            {
                DAL.Models.Reservation reservation = new DAL.Models.Reservation()
                {
                    RentalStartDate = model.RentalStartDate,
                    RentalEndDate = model.RentalEndDate,
                    Count = model.Count,
                    TotalCost = (decimal)Session["TotalCost"], //model.TotalCostString, nie mam pojecia dlaczego nawet w stringu niechce sie to przeslac, 
                                                               //bo w decimallu nie idzie bo ma problem konwertowac z przecinkiem na kropke, robie late
                    UserId = (int)Session["UserId"],
                    BoardGameId = model.BoardGameId,
                };
                switch (reservationService.AddReservation(reservation))
                {
                    case ReservationServiceResponse.SuccessReservation:
                        //string imagePath = model.ImagePath;
                        ModelState.Clear();
                        //ViewBag.ImagePath = imagePath;
                        TempData["SuccessReservation"] = $"Z powodzeniem dokonano rezerwacji: {model.BoardGameName}.";
                        return RedirectToAction("Login", "Login");
                    case ReservationServiceResponse.NotEnoughBoardGame:
                        ViewBag.NotEnoughBoardGameMessage = "Nie posiadamy wystarczającej liczby gier.";
                        return RedirectToAction("Details", "BoardGameDetailsOffer", new { boardGameId = model.BoardGameId });
                    default:
                        break;
                }
            }
            return View(model);
        }
    }
}