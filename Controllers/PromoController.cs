﻿using kopinang_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using kopinang_api.Attributes;

namespace kopinang_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [FirebaseAuthorize]
    public class PromoController : ControllerBase
    {
        private readonly FirestoreService _firestoreService;

        public PromoController(FirestoreService firestoreService)
        {
            _firestoreService = firestoreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPromos()
        {
            var promos = await _firestoreService.GetAllPromosAsync();
            return Ok(promos);
        }
    }
}
