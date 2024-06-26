﻿global using System.Text;
global using System.Text.Json;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Options;
global using Microsoft.AspNetCore.Authorization;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.Utils.Models;
global using Masa.BuildingBlocks.Authentication.Identity;
global using Masa.BuildingBlocks.Data.Mapping;
global using MasaQSTemplate.Contracts.Auth.Dtos;
global using MasaQSTemplate.Contracts.Auth.Enums;
global using MasaQSTemplate.Contracts.Log.Dtos;
global using MasaQSTemplate.AuthModule;
global using MasaQSTemplate.AuthModule.Application.Events;
global using MasaQSTemplate.LogModule;
global using MasaQSTemplate.LogModule.Application.Events;