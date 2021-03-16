﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using QuickSack.Server.Code;
using QuickSack.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QuickSack.Server.Controllers
{
    [Route("api/[controller]")]
    public class FeedController : Controller
    {
        private readonly IFeedFactory feedFactory;
        public FeedController(IFeedFactory factory) => feedFactory = factory;

        [HttpGet]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult Get()
        {
            List<FeedItem> feedItems = feedFactory.GetFeedItems();

            return Ok(feedItems);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            FeedItem feedItem = (feedFactory.GetFeedItems()).FirstOrDefault(x => x.Title == HttpUtility.UrlDecode(id));
            return Ok(feedItem);
        }
    }
}
