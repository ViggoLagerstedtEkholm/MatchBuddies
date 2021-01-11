using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Application.Repositories;
using DataLayer.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;


namespace MatchBuddie.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;
        private readonly IInterestRepository _interestRepository;

        public DownloadPersonalDataModel(
            UserManager<User> userManager,
            ILogger<DownloadPersonalDataModel> logger,
            IInterestRepository interestRepository)
        {
            _userManager = userManager;
            _logger = logger;
            _interestRepository = interestRepository;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(User).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));

            // We had problem with the serialization method and therefore we created a step by step self-made serialization process
            //to xml this is done using two foreach loops to first get the profile info then later the intrest for the user
            var xmldoc = new XmlDocument();
            var xmlDecleration = xmldoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            var root = xmldoc.DocumentElement;
            xmldoc.InsertBefore(xmlDecleration, root);
            var body = xmldoc.CreateElement(string.Empty, "PersonalData", string.Empty);
            xmldoc.AppendChild(body);
            
            foreach (var p in personalDataProps)
            {
                if (p.GetValue(user) != null && p.GetValue(user).GetType() != typeof(bool)) {
                    //personalData.Add(p.Name, p.GetValue(user)?.ToString());
                    var node = xmldoc.CreateElement(p.Name);
                    node.InnerText = p.GetValue(user)?.ToString();
                    body.AppendChild(node);

                   }

            }
            var interests = _interestRepository.UsersInterest(user.Id).ToList();
            foreach (var i in interests)
            {               
                    var node = xmldoc.CreateElement("Interest");
                    node.InnerText = i.Interest;
                    body.AppendChild(node);
            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            // This code is left because we wanted to show that we earlier just deserialized the jsonObj that Identity already provided, this will work
            // equally good as our present method.

            //var intrests = _interestRepository.UsersInterest(user.Id).ToList();
            //var listofintrest = new List<string>();
            //var jsonObj = JsonConvert.SerializeObject(personalData);
            //jsonObj = jsonObj.Replace("}", ",");
            //jsonObj += "\"Interest\":[ ";
            //foreach (var item in intrests)
            //{
            //    jsonObj += $"    \"{item.Interest}\",";
            //}
            //jsonObj += "] }";

            //XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonObj, "PersonData");
            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.xml");
            return new FileContentResult(Encoding.UTF8.GetBytes(xmldoc.InnerXml/*doc.InnerXml*/), "application/xml");
        }
    }
}
