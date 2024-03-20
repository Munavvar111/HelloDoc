using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class ProviderProfileVm
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Status { get; set; }
        public string? Role { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ConfirmEmail { get; set; }
        public string? MobileNo { get; set; }

        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public int? State { get; set; }
        public string? ZipCode { get; set; }
        public List<Region>? Regions { get; set; }
        public List<PhysicianRegion>? WorkingRegions { get; set; }
        public int physicianid { get;set; } 

        public string? MedicalLicense { get; set; }  
        public string? NPINumber { get; set; }  
        public string? SynchronizationEmail { get; set; }  

        public string? BusinessName { get; set; }   
        public string? BusinessWebsite { get; set; }
        public string? AdminNotes { get; set; }
        public IFormFile? File { get; set; }
        public IFormFile? signature { get; set; }

    }
}
