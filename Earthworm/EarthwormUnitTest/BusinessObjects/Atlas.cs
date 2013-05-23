using System;
using Earthworm;

namespace EarthwormUnitTest
{
    public class Atlas : MappableFeature
    {
        [MappedField("LastName", 0)]
        public virtual string LastName { get; set; }

        [MappedField("FirstName", 0)]
        public virtual string FirstName { get; set; }

        [MappedField("MiddleName", 0)]
        public virtual string MiddleName { get; set; }

        [MappedField("NameSuffix", 0)]
        public virtual string NameSuffix { get; set; }

        [MappedField("Other__Specify", 0)]
        public virtual string Other__Specify { get; set; }

        [MappedField("Provincial_Health_Number", 0)]
        public virtual string Provincial_Health_Number { get; set; }

        [MappedField("DOB", 0)]
        public virtual string DOB { get; set; }

        [MappedField("Age")]
        public virtual int? Age { get; set; }

        [MappedField("Sex", 0)]
        public virtual string Sex { get; set; }

        [MappedField("HomePhone", 0)]
        public virtual string HomePhone { get; set; }

        [MappedField("WorkPhone", 0)]
        public virtual string WorkPhone { get; set; }

        [MappedField("StreetAddress", 0)]
        public virtual string StreetAddress { get; set; }

        [MappedField("Municipality", 0)]
        public virtual string Municipality { get; set; }

        [MappedField("Province", 0)]
        public virtual string Province { get; set; }

        [MappedField("Postal_Code", 0)]
        public virtual string Postal_Code { get; set; }

        [MappedField("CountyOfResidence", 0)]
        public virtual string CountyOfResidence { get; set; }

        [MappedField("CompleteAddress", 0)]
        public virtual string CompleteAddress { get; set; }

        [MappedField("CensusTractFromDI", 0)]
        public virtual string CensusTractFromDI { get; set; }

        [MappedField("Occupation", 0)]
        public virtual string Occupation { get; set; }

        [MappedField("Notes", 0)]
        public virtual string Notes { get; set; }

        [MappedField("Ethnicity", 0)]
        public virtual string Ethnicity { get; set; }

        [MappedField("Race", 0)]
        public virtual string Race { get; set; }

        [MappedField("DateOfDiagnosis", 0)]
        public virtual string DateOfDiagnosis { get; set; }

        [MappedField("DateOfDeath", 0)]
        public virtual string DateOfDeath { get; set; }

        [MappedField("CMRNumber", 0)]
        public virtual string CMRNumber { get; set; }

        [MappedField("DateOfOnset")]
        public virtual DateTime? DateOfOnset { get; set; }

        [MappedField("DateCreated", 0)]
        public virtual string DateCreated { get; set; }

        [MappedField("EpisodeDate", 0)]
        public virtual string EpisodeDate { get; set; }

        [MappedField("DateSent", 0)]
        public virtual string DateSent { get; set; }

        [MappedField("DateClosed", 0)]
        public virtual string DateClosed { get; set; }

        [MappedField("DateSubmitted", 0)]
        public virtual string DateSubmitted { get; set; }

        [MappedField("Disease", 0)]
        public virtual string Disease { get; set; }

        [MappedField("DiseaseGroups", 0)]
        public virtual string DiseaseGroups { get; set; }

        [MappedField("ProcessStatus", 0)]
        public virtual string ProcessStatus { get; set; }

        [MappedField("ResolutionStatus", 0)]
        public virtual string ResolutionStatus { get; set; }

        [MappedField("TransmissionStatus", 0)]
        public virtual string TransmissionStatus { get; set; }

        [MappedField("OutbreakNumber", 0)]
        public virtual string OutbreakNumber { get; set; }

        [MappedField("OutbreakType", 0)]
        public virtual string OutbreakType { get; set; }

        [MappedField("NurseInvestigator", 0)]
        public virtual string NurseInvestigator { get; set; }

        [MappedField("ReportingSource", 0)]
        public virtual string ReportingSource { get; set; }

        [MappedField("SpecimenTypes", 0)]
        public virtual string SpecimenTypes { get; set; }

        [MappedField("ExposureTypes", 0)]
        public virtual string ExposureTypes { get; set; }

        [MappedField("HepatitisStatus", 0)]
        public virtual string HepatitisStatus { get; set; }

        [MappedField("Pregnant", 0)]
        public virtual string Pregnant { get; set; }

        [MappedField("ExpDeliveryDate", 0)]
        public virtual string ExpDeliveryDate { get; set; }

        [MappedField("AgeInYears", 0)]
        public virtual string AgeInYears { get; set; }

        [MappedField("TransStatus", 0)]
        public virtual string TransStatus { get; set; }

        [MappedField("SpecimenCollectedDate", 0)]
        public virtual string SpecimenCollectedDate { get; set; }

        [MappedField("SpecimenReceivedDate", 0)]
        public virtual string SpecimenReceivedDate { get; set; }

        [MappedField("Result", 0)]
        public virtual string Result { get; set; }

        [MappedField("SpecimenNotes", 0)]
        public virtual string SpecimenNotes { get; set; }

        [MappedField("CreatedBy", 0)]
        public virtual string CreatedBy { get; set; }

        [MappedField("Doctor", 0)]
        public virtual string Doctor { get; set; }

        [MappedField("DoctorAddress", 0)]
        public virtual string DoctorAddress { get; set; }

        [MappedField("DoctorPhone", 0)]
        public virtual string DoctorPhone { get; set; }

        [MappedField("MedicalRecordNumber", 0)]
        public virtual string MedicalRecordNumber { get; set; }

        [MappedField("MostRecentLabResult", 0)]
        public virtual string MostRecentLabResult { get; set; }

        [MappedField("MostRecentLabResultValue", 0)]
        public virtual string MostRecentLabResultValue { get; set; }

        [MappedField("DateofLabReport", 0)]
        public virtual string DateofLabReport { get; set; }

        [MappedField("LabReportTestName", 0)]
        public virtual string LabReportTestName { get; set; }

        [MappedField("LabReportNotes", 0)]
        public virtual string LabReportNotes { get; set; }

        [MappedField("ZIPPlus4", 0)]
        public virtual string ZIPPlus4 { get; set; }

        [MappedField("CensusBlock", 0)]
        public virtual string CensusBlock { get; set; }

        [MappedField("CensusTract", 0)]
        public virtual string CensusTract { get; set; }

        [MappedField("CountyFIPS", 0)]
        public virtual string CountyFIPS { get; set; }

        [MappedField("AddressStandardized", 0)]
        public virtual string AddressStandardized { get; set; }

        [MappedField("County", 0)]
        public virtual string County { get; set; }

        [MappedField("ReportedBy", 0)]
        public virtual string ReportedBy { get; set; }

        [MappedField("DateReceived", 0)]
        public virtual string DateReceived { get; set; }

        [MappedField("DisShortName", 0)]
        public virtual string DisShortName { get; set; }

        [MappedField("TimeSubmitted", 0)]
        public virtual string TimeSubmitted { get; set; }

        [MappedField("Asymptomatic", 0)]
        public virtual string Asymptomatic { get; set; }

        [MappedField("PatientDiedofthisIllness", 0)]
        public virtual string PatientDiedofthisIllness { get; set; }

        [MappedField("PatientHospitalized", 0)]
        public virtual string PatientHospitalized { get; set; }

        [MappedField("IsIndexCase", 0)]
        public virtual string IsIndexCase { get; set; }

        [MappedField("ClusterId", 0)]
        public virtual string ClusterId { get; set; }

        [MappedField("DateAdmitted", 0)]
        public virtual string DateAdmitted { get; set; }

        [MappedField("DateDischarged", 0)]
        public virtual string DateDischarged { get; set; }

        [MappedField("InPatient", 0)]
        public virtual string InPatient { get; set; }

        [MappedField("OutPatient", 0)]
        public virtual string OutPatient { get; set; }

        [MappedField("Hospital", 0)]
        public virtual string Hospital { get; set; }

        [MappedField("HospitalDR", 0)]
        public virtual string HospitalDR { get; set; }

        [MappedField("SubmitterName", 0)]
        public virtual string SubmitterName { get; set; }

        [MappedField("PrimaryLanguage", 0)]
        public virtual string PrimaryLanguage { get; set; }

        [MappedField("RecordType", 0)]
        public virtual string RecordType { get; set; }

        [MappedField("Completed_appropriate_intervention_activities_", 0)]
        public virtual string Completed_appropriate_intervention_activities_ { get; set; }

        [MappedField("Completed_required_case_management_forms_", 0)]
        public virtual string Completed_required_case_management_forms_ { get; set; }

        [MappedField("Completed_required_supplemental_forms_", 0)]
        public virtual string Completed_required_supplemental_forms_ { get; set; }

        [MappedField("Investigated_possible_disease_transmission_", 0)]
        public virtual string Investigated_possible_disease_transmission_ { get; set; }

        [MappedField("Investigated_possible_source_s__of_exposure_", 0)]
        public virtual string Investigated_possible_source_s__of_exposure_ { get; set; }

        [MappedField("Validated_patient_demographics_", 0)]
        public virtual string Validated_patient_demographics_ { get; set; }

        [MappedField("Status_Date")]
        public virtual DateTime? Status_Date { get; set; }

        [MappedField("Status_Repeated", 0)]
        public virtual string Status_Repeated { get; set; }

        [MappedField("ClosedDate", 0)]
        public virtual string ClosedDate { get; set; }

        [MappedField("Closed_by_HDDate", 0)]
        public virtual string Closed_by_HDDate { get; set; }

        [MappedField("Completed_Local_InvestigationDate", 0)]
        public virtual string Completed_Local_InvestigationDate { get; set; }

        [MappedField("FinalDate", 0)]
        public virtual string FinalDate { get; set; }

        [MappedField("Investigation_CompletedDate", 0)]
        public virtual string Investigation_CompletedDate { get; set; }

        [MappedField("Local_AdministratorDate", 0)]
        public virtual string Local_AdministratorDate { get; set; }

        [MappedField("Lost_to_Follow_upDate", 0)]
        public virtual string Lost_to_Follow_upDate { get; set; }

        [MappedField("LTBI_Medication_refill_orderDate", 0)]
        public virtual string LTBI_Medication_refill_orderDate { get; set; }

        [MappedField("Medication_order_processedDate", 0)]
        public virtual string Medication_order_processedDate { get; set; }

        [MappedField("Needs_Follow_upDate", 0)]
        public virtual string Needs_Follow_upDate { get; set; }

        [MappedField("NewDate", 0)]
        public virtual string NewDate { get; set; }

        [MappedField("New_LTBI_medication_orderDate", 0)]
        public virtual string New_LTBI_medication_orderDate { get; set; }

        [MappedField("OpenDate", 0)]
        public virtual string OpenDate { get; set; }

        [MappedField("Open_Local_InvestigationDate", 0)]
        public virtual string Open_Local_InvestigationDate { get; set; }

        [MappedField("Received_by_HDDate", 0)]
        public virtual string Received_by_HDDate { get; set; }

        [MappedField("Received_By_StateDate", 0)]
        public virtual string Received_By_StateDate { get; set; }

        [MappedField("Refill_order_processsedDate", 0)]
        public virtual string Refill_order_processsedDate { get; set; }

        [MappedField("Resolution_Status_FinalDate", 0)]
        public virtual string Resolution_Status_FinalDate { get; set; }

        [MappedField("Returned_to_HDDate", 0)]
        public virtual string Returned_to_HDDate { get; set; }

        [MappedField("Sent_to_StateDate", 0)]
        public virtual string Sent_to_StateDate { get; set; }

        [MappedField("State_AdministratorDate", 0)]
        public virtual string State_AdministratorDate { get; set; }

        [MappedField("Under_InvestigationDate", 0)]
        public virtual string Under_InvestigationDate { get; set; }

        [MappedField("Under_SurveillanceDate", 0)]
        public virtual string Under_SurveillanceDate { get; set; }

        [MappedField("unknownDate", 0)]
        public virtual string unknownDate { get; set; }

        [MappedField("UpdatedDate", 0)]
        public virtual string UpdatedDate { get; set; }

        [MappedField("CSDUID", 0)]
        public virtual string CSDUID { get; set; }

        [MappedField("CSDNAME", 0)]
        public virtual string CSDNAME { get; set; }

        [MappedField("CSDTYPE", 0)]
        public virtual string CSDTYPE { get; set; }

        [MappedField("PRNAME", 0)]
        public virtual string PRNAME { get; set; }

        [MappedField("HRUID", 0)]
        public virtual string HRUID { get; set; }

        [MappedField("HRNAME", 0)]
        public virtual string HRNAME { get; set; }

        [MappedField("DAUID", 0)]
        public virtual string DAUID { get; set; }

        [MappedField("PRUID", 0)]
        public virtual string PRUID { get; set; }

        [MappedField("CTUID", 0)]
        public virtual string CTUID { get; set; }

        [MappedField("Latitude")]
        public virtual double? Latitude { get; set; }

        [MappedField("Longitude")]
        public virtual double? Longitude { get; set; }

        [MappedField("X_LCC")]
        public virtual double? X_LCC { get; set; }

        [MappedField("Y_LCC")]
        public virtual double? Y_LCC { get; set; }

        [MappedField("ID")]
        public virtual int? ID { get; set; }
    }
}
