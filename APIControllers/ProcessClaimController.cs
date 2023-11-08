using ClaimAPI.ModelClasses;
using ClaimAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;


namespace ClaimAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]

    [BasicAuthentication]
    public class ProcessClaimController : ApiController
    {
        REALESTATEEntities db = new REALESTATEEntities();



        [HttpGet]
        public List<RealPost> Showpost()
        {

            var alluser = (from e in db.RealPosts orderby e.id descending select e).ToList();


            return alluser;



        }


        // return Request.CreateResponse(HttpStatusCode.OK, dreg);}

        // return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "ERROR1 " + ex.Message);

        // return Request.CreateResponse(HttpStatusCode.BadRequest, pushmessage);

        //pushmessage.Message = responseinfo.Status;
        // pushmessage.Messageresponse = responseinfo.Response;

        // return Request.CreateResponse(HttpStatusCode.OK, pushmessage);


        public Task<IEnumerable<RealPost>> sendpost()
        {
            var alluser = (from e in db.RealPosts orderby e.id descending select e).ToList().Take(100); 

            return Task.FromResult(alluser);
        }


        [HttpGet]
        public async Task<HttpResponseMessage> Allpost()
        {
            APIPUSH pushmessage = new APIPUSH();


            try
            {

                var allinfo = await sendpost();


                pushmessage.apimessage = "Post Successful";
                pushmessage.apimessageresponse = "success";

                return Request.CreateResponse(HttpStatusCode.OK, allinfo);

            }
            catch (System.Exception ex)
            {


                RealAllError derror = new RealAllError();

                derror.daction = "post action";

                derror.errormessage = ex.Message;

                if (ex.InnerException != null)
                {
                    derror.errormessage = ex.InnerException.ToString();

                }

                derror.date = DateTime.Now.Date;

                db.RealAllErrors.Add(derror);
                db.SaveChanges();


                pushmessage.apimessage = "Error in sending data , do try again";

                return Request.CreateResponse(HttpStatusCode.BadRequest, pushmessage);
            }


            return Request.CreateResponse(HttpStatusCode.OK, "HELLO");
        }


        public Task<IEnumerable<RealComment>> sendcomment(int? comid)
        {
            var alluser = (from e in db.RealComments where e.postid == comid orderby e.id descending select e).ToList().Take(50);

            return Task.FromResult(alluser);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> AllComments(int? postid)
        {
            APIPUSH pushmessage = new APIPUSH();


            try
            {

                var allinfo = await sendcomment(postid);


                pushmessage.apimessage = "Post Successful";
                pushmessage.apimessageresponse = "success";

                return Request.CreateResponse(HttpStatusCode.OK, allinfo);

            }
            catch (System.Exception ex)
            {


                RealAllError derror = new RealAllError();

                derror.daction = "comment action";

                derror.errormessage = ex.Message;

                if (ex.InnerException != null)
                {
                    derror.errormessage = ex.InnerException.ToString();

                }

                derror.date = DateTime.Now.Date;

                db.RealAllErrors.Add(derror);
                db.SaveChanges();


                pushmessage.apimessage = "Error in sending data , do try again";

                return Request.CreateResponse(HttpStatusCode.BadRequest, pushmessage);
            }


            return Request.CreateResponse(HttpStatusCode.OK, "HELLO");
        }


        public Task<RealPost> Toaddcomment(int? comid)
        {
            var alluser = (from e in db.RealPosts where e.id == comid select e).FirstOrDefault();

            return Task.FromResult(alluser);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> AddComment()
        {
            APIPUSH pushmessage = new APIPUSH();


            RealComment dpost = new RealComment();


            try
            {

                dpost.postid= Convert.ToInt32(HttpContext.Current.Request.Params["postid"]);
                dpost.username = HttpContext.Current.Request.Params["username"];               
                dpost.userid = Convert.ToInt32(HttpContext.Current.Request.Params["userid"]);
                dpost.comment = HttpContext.Current.Request.Params["comment"];
                dpost.userimage = HttpContext.Current.Request.Params["userimage"];
                dpost.date = DateTime.Now.Date;
                dpost.datecomment = DateTime.Now.ToString("h:mm tt dd/MM/yyyy");


                var dpostinfo = await Toaddcomment(dpost.postid);

                dpostinfo.comments = dpostinfo.comments + 1;


                db.RealComments.Add(dpost);
                db.SaveChanges();


                pushmessage.apimessage = "Post Successful";
                pushmessage.apimessageresponse = "success";

                return Request.CreateResponse(HttpStatusCode.OK, pushmessage);
            }

            catch (System.Exception ex)
            {


                RealAllError derror = new RealAllError();

                derror.daction = "post comment action";

                derror.errormessage = ex.Message;

                if (ex.InnerException != null)
                {
                    derror.errormessage = ex.InnerException.ToString();

                }

                derror.date = DateTime.Now.Date;

                db.RealAllErrors.Add(derror);
                db.SaveChanges();


                pushmessage.apimessage = "Error in posting, do try again";

                return Request.CreateResponse(HttpStatusCode.BadRequest, pushmessage);
            }
        }



        [HttpPost]
        public async Task<HttpResponseMessage> AddPost()
        {
            APIPUSH pushmessage = new APIPUSH();

            string Imageurl = ConfigurationManager.AppSettings["imageurl"];


            Random random = new Random();
            int newnumber = random.Next(10000, 99999);

            int dnowsec = DateTime.Now.Second;

            int dnowmill = DateTime.Now.Millisecond;

            string postid = dnowmill.ToString() + dnowsec.ToString() + newnumber;


            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/uploads");

            var provider = new MultipartFormDataStreamProvider(root);


            string[] magefiles = new string[5];

            int i = 0;

            try
            {

                await Request.Content.ReadAsMultipartAsync(provider);


            

                foreach (MultipartFileData fileData in provider.FileData)
                {
                   

                    if (string.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName))
                    {
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted");
                    }
                    string fileName = fileData.Headers.ContentDisposition.FileName;
                    if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                    {
                        fileName = fileName.Trim('"');
                    }
                    if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                    {
                        fileName = Path.GetFileName(fileName);
                    }

                    string dex = Path.GetExtension(fileName).ToLower();



                    if (dex != ".jpg" && dex != ".jpeg" && dex != ".png")
                    {


                        pushmessage.apimessage = "Error- Wrong Image Format";

                        return Request.CreateResponse(HttpStatusCode.OK, pushmessage);


                    }



                    Random random1 = new Random();
                    int newnumber1 = random.Next(10000, 99999);

                    int dnowmill1 = DateTime.Now.Millisecond;

                    string postid1 = dnowmill1.ToString() + newnumber1;


                    fileName = postid1 + fileName;

                    File.Move(fileData.LocalFileName, Path.Combine(root, fileName));


                    magefiles[i] = Imageurl + fileName;



                    //PostsImage dpostmage = new PostsImage();


                    //dpostmage.imageurl = Imageurl + fileName;

                    //dpostmage.postid  = postid;

                    //dpostmage.date = DateTime.Now.Date;



                    //db.PostsImages.Add(dpostmage);

                    //db.SaveChanges();



                    if ((File.Exists(fileData.LocalFileName)))
                    {
                        File.Delete(fileData.LocalFileName);
                    }

                    i++;

                }


                RealPost dpost = new RealPost();

                dpost.postheader = HttpContext.Current.Request.Params["postheader"];
                dpost.userid  = Convert.ToInt32(HttpContext.Current.Request.Params["userid"]);
                dpost.postbody = HttpContext.Current.Request.Params["postbody"];
                dpost.postbody = HttpUtility.HtmlEncode(dpost.postbody);
                dpost.username = HttpContext.Current.Request.Params["username"];
                dpost.usermobile = HttpContext.Current.Request.Params["usermobile"];
                dpost.userimage = HttpContext.Current.Request.Params["userimage"];
                dpost.date = DateTime.Now.Date;
                dpost.userdate =  DateTime.Now.ToString("h:mm tt dd/MM/yyyy");
                dpost.comments = 0;
                dpost.likes = 0;
                dpost.likes = 0;
                dpost.share = 0;




                if (magefiles.ElementAt(0) != null)
                {
                    dpost.image1 = magefiles.ElementAt(0);
                }

                if (magefiles.ElementAt(1) != null)
                {
                    dpost.image2 = magefiles.ElementAt(1);
                }

                if (magefiles.ElementAt(2) != null)
                {
                    dpost.image3 = magefiles.ElementAt(2);
                }

                if (magefiles.ElementAt(3) != null)
                {
                    dpost.image4 = magefiles.ElementAt(3);
                }

                if (magefiles.ElementAt(4) != null)
                {
                    dpost.image5 = magefiles.ElementAt(4);
                }
                //dpost.postid = postid;


                db.RealPosts.Add(dpost);
                db.SaveChanges();


                pushmessage.apimessage = "Post Successful";
                pushmessage.apimessageresponse = "success";

                return Request.CreateResponse(HttpStatusCode.OK, pushmessage);
            }
            catch (System.Exception ex)
            {


                RealAllError derror = new RealAllError();

                derror.daction = "post action";


                derror.errormessage = ex.Message;

                if (ex.InnerException != null)
                {
                    derror.errormessage = ex.InnerException.ToString();

                }

                derror.date = DateTime.Now.Date;

                db.RealAllErrors.Add(derror);
                db.SaveChanges();


                pushmessage.apimessage = "Error in posting, do try again";

                return Request.CreateResponse(HttpStatusCode.BadRequest, pushmessage);
            }





            return Request.CreateResponse(HttpStatusCode.OK, "HELLO");
        }

      

        public Task<RealREGUSER> checkuser(string duser)
        {
            var alluser = (from e in db.RealREGUSERs where e.username == duser select e).FirstOrDefault();

            return Task.FromResult(alluser);
        }


        public Task<RealREGUSER> checkmobile(string dmobile)
        {
            var allmobile = (from e in db.RealREGUSERs where e.mobile == dmobile select e).FirstOrDefault();

            return Task.FromResult(allmobile);
        }


        [HttpPost]
        public async Task<HttpResponseMessage> RegisterUser()
        {
            APIPUSH pushmessage = new APIPUSH();

            string Imageurl = ConfigurationManager.AppSettings["imageurl"];


            Random random = new Random();
            int newnumber = random.Next(10000, 99999);

            int dnowsec = DateTime.Now.Second;

            int dnowmill = DateTime.Now.Millisecond;

            string postid = dnowmill.ToString() + dnowsec.ToString() + newnumber;


            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/uploads");

            var provider = new MultipartFormDataStreamProvider(root);

            RealREGUSER duser = new RealREGUSER();

            try
            {
              
                await Request.Content.ReadAsMultipartAsync(provider);


               
                duser.username = HttpContext.Current.Request.Params["username"].ToLower();
                duser.mobile = HttpContext.Current.Request.Params["mobile"];



                var checuser = await checkuser(duser.username);

                if (checuser != null)
                {
                    pushmessage.apimessage = "Error - Username already in Use";
                    return Request.CreateResponse(HttpStatusCode.OK, pushmessage);

                }


                var checuser1 = await checkmobile(duser.mobile);

                if (checuser1 != null)
                {
                    pushmessage.apimessage = "Error - Mobile number already in Use";
                    return Request.CreateResponse(HttpStatusCode.OK, pushmessage);

                }




                foreach (MultipartFileData fileData in provider.FileData)
                {
                    if (string.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName))
                    {
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted");
                    }
                    string fileName = fileData.Headers.ContentDisposition.FileName;
                    if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                    {
                        fileName = fileName.Trim('"');
                    }
                    if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                    {
                        fileName = Path.GetFileName(fileName);
                    }

                    string dex = Path.GetExtension(fileName).ToLower();



                    if(dex != ".jpg" && dex != ".jpeg" && dex != ".png")
                    {


                        pushmessage.apimessage = "Error- Wrong Image Format";
                      
                        return Request.CreateResponse(HttpStatusCode.OK, pushmessage);


                    }


                    fileName = postid + fileName;
  
                    //  string dex = root.Get
                    File.Move(fileData.LocalFileName, Path.Combine(root, fileName));

                    if ((File.Exists(fileData.LocalFileName)))
                    {
                        File.Delete(fileData.LocalFileName);
                    }

                    duser.imageurl = Imageurl + fileName;

                }


                duser.fullname = HttpContext.Current.Request.Params["fullname"];
                duser.password = HttpContext.Current.Request.Params["password"].ToLower();
                duser.password = GetHash(duser.password);
                duser.job_title = HttpContext.Current.Request.Params["job"];
                duser.date = DateTime.Now.Date;


                db.RealREGUSERs.Add(duser);
                db.SaveChanges();


                pushmessage.apimessage =  "Registration Successful";
                pushmessage.apimessage += "\r\nPlease Login";
                pushmessage.apimessageresponse = "success";

                return Request.CreateResponse(HttpStatusCode.OK, pushmessage);
            }
            catch (System.Exception ex)
            {


                RealAllError derror = new RealAllError();

                derror.daction = "registration action";


                derror.errormessage = ex.Message;

                if (ex.InnerException != null)
                {
                    derror.errormessage = ex.InnerException.ToString();

                }

                derror.date = DateTime.Now.Date;

                db.RealAllErrors.Add(derror);
                db.SaveChanges();


                pushmessage.apimessage = "Error in Registration, do try again";

                return Request.CreateResponse(HttpStatusCode.BadRequest, pushmessage);
            }





            return Request.CreateResponse(HttpStatusCode.OK, "HELLO");
        }


        public Task<RealREGUSER> dloginuser(string dusername, string dpassword)
        {
            var alluser = (from e in db.RealREGUSERs where (e.username == dusername  && e.password == dpassword) || (e.mobile == dusername && e.password == dpassword) select e).FirstOrDefault();

            return Task.FromResult(alluser);
        }




        [HttpPost]
        public async Task<HttpResponseMessage> LoginUser()
        {
            APIPUSH pushmessage = new APIPUSH();


            RealREGUSER duser = new RealREGUSER();

            try
            {



                duser.username = HttpContext.Current.Request.Params["username"].ToLower();
                duser.password = HttpContext.Current.Request.Params["password"].ToLower();
                duser.password = GetHash(duser.password);




                var checuser = await dloginuser(duser.username, duser.password);

                if (checuser == null)
                {
                    pushmessage.apimessage = "Error - No User Found";
                    pushmessage.apimessageresponse = "none";
                    return Request.CreateResponse(HttpStatusCode.OK, pushmessage);

                }

               

                pushmessage.apimessage = "Login Successful";
                pushmessage.apimessageresponse = "success";

                return Request.CreateResponse(HttpStatusCode.OK, checuser);
            }
            catch (System.Exception ex)
            {


                RealAllError derror = new RealAllError();

                derror.daction = "login action";


                derror.errormessage = ex.Message;

                if (ex.InnerException != null)
                {
                    derror.errormessage = ex.InnerException.ToString();

                }

                derror.date = DateTime.Now.Date;

                db.RealAllErrors.Add(derror);
                db.SaveChanges();


                pushmessage.apimessage = "Error in login, do try again";

                return Request.CreateResponse(HttpStatusCode.BadRequest, pushmessage);
            }





            return Request.CreateResponse(HttpStatusCode.OK, "HELLO");
        }





        [HttpPost]
        public async Task<HttpResponseMessage> PostImage()
        {

            Random random = new Random();
            int newnumber = random.Next(100000, 999999);

            int dnowsec = DateTime.Now.Second;

            int dnowmill = DateTime.Now.Millisecond;

            string postid = dnowmill.ToString() + dnowsec.ToString() +newnumber;

         



            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/uploads");

            var provider = new MultipartFormDataStreamProvider(root);



            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (MultipartFileData fileData in provider.FileData)
                {
                  if(string.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName))
                    {
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted");
                    }
                    string fileName = fileData.Headers.ContentDisposition.FileName;
                    if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                    {
                        fileName = fileName.Trim('"');
                    }
                    if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                    {
                        fileName = Path.GetFileName(fileName);
                    }
                    File.Move(fileData.LocalFileName, Path.Combine(root, fileName));


                    var verse = HttpContext.Current.Request.Params["dverse"];
                }


                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }




           // HttpFileCollection hfc = HttpContext.Current.Request.Files;

            //  var verse = HttpContext.Current.Request.Params["dverse"];

            //var dbody = HttpContext.Current.Request.Params["dbodyone"];

            //  for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            //  {
            //      HttpPostedFile hpf = hfc[iCnt];

            //      if (hpf.ContentLength > 0)
            //      {

            //      }
            //  }


            return Request.CreateResponse(HttpStatusCode.OK, "HELLO");
        }


     

        [HttpPost]
        public HttpResponseMessage SaveUserInfo([FromBody]RealPost dinfo)
        {
            string Companyname = ConfigurationManager.AppSettings["companyname"];

            string apikey = ConfigurationManager.AppSettings["apipasskey"];


           
            return Request.CreateResponse(HttpStatusCode.OK, dinfo);
        }


        public static string GetHash(string input)
        {
            return string.Join("", (new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input))).Select(x => x.ToString("X2")).ToArray());
        }

    }
}
