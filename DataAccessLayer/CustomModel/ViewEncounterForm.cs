using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.CustomModel
{
    public class ViewEncounterForm
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Location { get; set; }

        public string Email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public DateOnly? DateOfService { get; set; }

        public string HistoryOfPresentIllness { get; set; }
        public string MedicalHistory { get; set; }
        public string Medications { get; set; }
        public string Allergies { get; set; }

        public string Temperature { get; set; }
        public string HR { get; set; } // Likely Heart Rate
        public string RR { get; set; } // Likely Respiratory Rate
        public string BPSystolic { get; set; } // Likely Blood Pressure Systolic
        public string BPDiastolic { get; set; } // Likely Blood Pressure Diastolic
        public string O2 { get; set; } // Likely Oxygen level
        public string Pain { get; set; }

        public string Heent { get; set; } // Likely Head, Eyes, Ears, Nose, Throat exam
        public string CV { get; set; }  // Likely Cardiovascular exam
        public string Chest { get; set; }
        public string ABD { get; set; }  // Likely Abdomen exam
        public string Extr { get; set; }  // Likely Extremities exam
        public string Skin { get; set; }
        public string Neuro { get; set; } // Likely Neurological exam
        public string Other { get; set; }

        public string Diagnosis { get; set; }
        public string TreatmentPlan { get; set; }

        public string MedicationsDispensed { get; set; }
        public string Procedures { get; set; }
        public string Followup { get; set; }

        public string IsFinalizied { get; set; }  
    }

}
