using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClaimAPI.Models;

namespace ClaimAPI.Models
{
    public class ModelClass
    {
    }

    public class PostData
    {

        public string verse { get; set; }

        public string body { get; set; }

        public string login { get; set; }

       
    }


    public class Alldata
    {

        public RealPost dpost { get; set; }

        public List<RealPostsImage> allimage { get; set; }



    }




}