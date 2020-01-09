﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Web.Models
{
    public class GetRequestListRequest
    {
        public string Start { get; set; }

        public string End { get; set; }

        public string Url { get; set; }

        public string IP { get; set; }

        public string Node { get; set; }


        public int pageNumber { get; set; }

        public int pageSize { get; set; }

    }
}
