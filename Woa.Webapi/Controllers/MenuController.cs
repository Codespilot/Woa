﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Woa.Webapi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MenuController : ControllerBase
{
}