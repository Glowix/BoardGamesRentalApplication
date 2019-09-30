﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGamesRentalApplication.DAL.Models
{
    public class BoardGamePublisher
    {
        public int BoardGamePublisherId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BoardGame> BoardGames { get; set; }
    }
}
