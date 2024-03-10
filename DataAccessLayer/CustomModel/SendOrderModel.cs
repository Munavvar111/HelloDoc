﻿using DataAccessLayer.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class SendOrderModel
    {

        public int requestid {  get; set; } 
        public List<Healthprofessional>? helthProfessional { get; set; } 

        public List<Healthprofessionaltype>? Healthprofessionaltypes { get; set; }

            
        [Required(ErrorMessage = "BusinessId is required.")]
        public int BusinessId { get; set; }

        [Required(ErrorMessage = "Contact is required.")]
        public string Contact { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "FaxNumber is required.")]
        public string FaxNumber { get; set; }

        [Required(ErrorMessage = "Prescription is required.")]
        public string Prescription { get; set; }

    }
}
