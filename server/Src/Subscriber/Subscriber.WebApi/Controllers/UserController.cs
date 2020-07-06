using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Subscriber.Services;
using Subscriber.Services.Models;
using Subscriber.WebApi.Models;
using Microsoft.AspNetCore.Routing;
using Serilog;
using System.Net.Mail;

namespace Subscriber.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        public void SendEmail()
        {
            //var smtpClient = new SmtpClient("smtp.gmail.com")
            //{
            //    Port = 587,
            //    UseDefaultCredentials = false,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //   Credentials = new NetworkCredential("tzippyfreedman1@gmail.com", "tf0583265366"),
            //    EnableSsl = true,
            //};

            //smtpClient.Send("tzippyfreedman1@gmail.com", "tali.freid@gmail.com", "subject", "body");
            //using (MailMessage mail = new MailMessage())
            //{
            //    mail.From = new MailAddress("tzippyfreedman1@gmail.com");
            //    mail.To.Add("tali.freid@gmail.com");
            //    mail.Subject = "Hello World";
            //    mail.Body = "<h1>Hello</h1>";
            //    mail.IsBodyHtml = true;
            //    //mail.Attachments.Add(new Attachment("C:\\file.zip"));

            //    using (SmtpClient smtp = new SmtpClient("tzippyfreedman1@gmail.com", 587))
            //    {
            //        smtp.Host = "smtp.gmail.com";
            //        smtp.UseDefaultCredentials = false;
            //        smtp.Credentials = new NetworkCredential("tzippyfreedman1@gmail.com", "tf0583265366");
            //        smtp.EnableSsl = true;
            //        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            //        smtp.Send(mail);
            //    }
            //}
            string emailAddress = "estherivka1@gmail.com";
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("rivkifreedman1@gmail.com"); //enter whatever email you are sending from here
                mail.To.Add(emailAddress); //Text box that the user enters their email address
                mail.Subject = "Email Vertification"; //enter whatever subject you would like
                mail.Body = $"Your Activation Code is: ";
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("rivkifreedman1@gmail.com", 587)) //enter the same email that the message is sending from along with port 587
                {
                    smtp.Host = "smtp.gmail.com";

                    smtp.Credentials = new NetworkCredential("rivkifreedman1@gmail.com", "er0533150865"); //Enter email with password
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    smtp.Send(mail);
                }


            }
            //var mailMessage = new MailMessage
            //{
            //    From = new MailAddress("email"),
            //    Subject = "subject",
            //    Body = "<h1>Hello</h1>",
            //    IsBodyHtml = true,
            //};
            //mailMessage.To.Add("recipient");

            //smtpClient.Send(mailMessage);
        }
        [HttpPost]
        public async Task<ActionResult<bool>> Register(SubscriberDTO userRegister)
        {

            UserModel userToRegister = _mapper.Map<UserModel>(userRegister);
            UserFileModel userFileToRegister = _mapper.Map<UserFileModel>(userRegister);
            UserModel userAdded = await _userService.RegisterAsync(userToRegister, userFileToRegister);
            if (userAdded == null)
            {
                Log.Information("User with email {@email} requested to create but already exists", userRegister.Email);
                throw new Exception("Bad Request: Patient with email ${ userRegister.Email } requested to create but already exists");
                //   throw new HttpResponseException(HttpStatusCode.NotFound);
                // return BadRequest($"patient with id:{newPatient.PatientId} already exists");
            }




            else
            {
                Log.Information("User with email {@email} created successfully", userAdded.Email);
                return StatusCode((int)HttpStatusCode.Created);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Login(LoginDTO userLogin)
        {

            Guid patientCardId = await _userService.LoginAsync(userLogin.Email, userLogin.Password);
            if (patientCardId.Equals(Guid.Empty))
            {
                return Unauthorized();
            }
            else
            {
                return patientCardId;
            }
        }

        [HttpGet]
        [Route("{userCardId:Guid}")]
        public async Task<ActionResult<UserFileDTO>> GetUserFileById(Guid userCardId)
        {

            UserFileModel file = await _userService.GetUserFileById(userCardId);
            if (file == null)
            {
                return NoContent();
            }
            

            return _mapper.Map<UserFileDTO>(file);
        }

    }
}
