﻿using Autofac;
using BoardGamesRentalApplication.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BoardGamesRentalApplication.Autofac
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterAssemblyTypes(typeof(IRepository<>).Assembly).AsImplementedInterfaces().InstancePerRequest();
        }
    }
}