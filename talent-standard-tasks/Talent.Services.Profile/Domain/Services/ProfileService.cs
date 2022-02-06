using Talent.Common.Contracts;
using Talent.Common.Models;
using Talent.Services.Profile.Domain.Contracts;
using Talent.Services.Profile.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Talent.Services.Profile.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Talent.Common.Security;
using System.Linq.Expressions;

namespace Talent.Services.Profile.Domain.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserAppContext _userAppContext;
        IRepository<UserLanguage> _userLanguageRepository;
        IRepository<User> _userRepository;
        IRepository<Employer> _employerRepository;
        IRepository<Job> _jobRepository;
        IRepository<Recruiter> _recruiterRepository;
        IFileService _fileService;


        public ProfileService(IUserAppContext userAppContext,
                              IRepository<UserLanguage> userLanguageRepository,
                              IRepository<User> userRepository,
                              IRepository<Employer> employerRepository,
                              IRepository<Job> jobRepository,
                              IRepository<Recruiter> recruiterRepository,
                              IFileService fileService)
        {
            _userAppContext = userAppContext;
            _userLanguageRepository = userLanguageRepository;
            _userRepository = userRepository;
            _employerRepository = employerRepository;
            _jobRepository = jobRepository;
            _recruiterRepository = recruiterRepository;
            _fileService = fileService;
        }

        public async Task<List<AddLanguageViewModel>> GetLanguages(string id)
        {
            User user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }
            var languages = user.Languages.Select(x => ViewModelFromLanguage(x)).ToList();
            return languages;
        }
        public string AddNewLanguage(AddLanguageViewModel language)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<TalentProfileViewModel> GetTalentProfile(string Id)
        {
            User user = await _userRepository.GetByIdAsync(Id);
            if (user == null)
            {
                return null;
            }

            var videoUrl = string.IsNullOrWhiteSpace(user.VideoName)
                          ? ""
                          : await _fileService.GetFileURL(user.VideoName, FileType.UserVideo);

            var cvUrl = string.IsNullOrWhiteSpace(user.CvName)
                          ? ""
                          : await _fileService.GetFileURL(user.VideoName, FileType.UserCV);

            var language = user.Languages.Select(x => ViewModelFromLanguage(x)).ToList();
            var skill = user.Skills.Select(x => ViewModelFromSkill(x)).ToList();
            var education = user.Education.Select(x => ViewModelFromEducation(x)).ToList();
            var certification = user.Certifications.Select(x => ViewModelFromCertification(x)).ToList();
            var experience = user.Experience.Select(x => ViewModelFromExperience(x)).ToList();

            return new TalentProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Gender = user.Gender,
                Email = user.Email,
                Phone = user.Phone,
                MobilePhone = user.MobilePhone,
                IsMobilePhoneVerified = user.IsMobilePhoneVerified,
                Address = user.Address,
                Nationality = user.Nationality,
                VisaStatus = user.VisaStatus,
                VisaExpiryDate = user.VisaExpiryDate,
                ProfilePhoto = user.ProfilePhoto,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                VideoName = user.VideoName,
                VideoUrl = videoUrl,
                CvName = user.CvName,
                CvUrl = cvUrl,
                Summary = user.Summary,
                Description = user.Description,
                LinkedAccounts = user.LinkedAccounts,
                JobSeekingStatus = user.JobSeekingStatus,
                Languages = language,
                Skills = skill,
                Education = education,
                Certifications = certification,
                Experience = experience
            };
        }

        public async Task<bool> UpdateTalentProfile(TalentProfileViewModel user, string updaterId)
        {
            try
            {
                if (user.Id == null)
                {
                    return false;
                }
                User existingUser = await _userRepository.GetByIdAsync(user.Id);
                existingUser.FirstName = user.FirstName;
                existingUser.MiddleName = user.MiddleName;
                existingUser.LastName = user.LastName;
                existingUser.Gender = user.Gender;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                existingUser.IsMobilePhoneVerified = user.IsMobilePhoneVerified;
                existingUser.Address = user.Address;
                existingUser.Nationality = user.Nationality;
                existingUser.VisaStatus = user.VisaStatus;
                existingUser.JobSeekingStatus = user.JobSeekingStatus;
                existingUser.VisaExpiryDate = user.VisaExpiryDate;
                existingUser.Summary = user.Summary;
                existingUser.Description = user.Description;
                existingUser.LinkedAccounts = user.LinkedAccounts;
                existingUser.JobSeekingStatus = user.JobSeekingStatus;

                var newLanguages = new List<UserLanguage>();
                foreach (var item in user.Languages)
                {
                    var newItem = ExistOrDefaultById<UserLanguage>(existingUser.Languages, item);
                    if (newItem.UserId == null)
                    {
                        newItem.UserId = user.Id;
                    }
                    UpdateLanguageFromView(item, newItem);
                    newLanguages.Add(newItem);
                }
                existingUser.Languages = newLanguages;

                var newSkills = new List<UserSkill>();
                foreach (var item in user.Skills)
                {
                    var newItem = ExistOrDefaultById<UserSkill>(existingUser.Skills, item);
                    UpdateSkillFromView(item, newItem);
                    newSkills.Add(newItem);
                }
                existingUser.Skills = newSkills;

                var newEducations = new List<UserEducation>();
                foreach (var item in user.Education)
                {
                    var newItem = ExistOrDefaultById<UserEducation>(existingUser.Education, item);
                    UpdateEducationFromView(item, newItem);
                    newEducations.Add(newItem);
                }
                existingUser.Education = newEducations;

                var newCertifications = new List<UserCertification>();
                foreach (var item in user.Certifications)
                {
                    var newItem = ExistOrDefaultById<UserCertification>(existingUser.Certifications, item);
                    UpdateCertificationFromView(item, newItem);
                    newCertifications.Add(newItem);
                }
                existingUser.Certifications = newCertifications;

                var newExperiences = new List<UserExperience>();
                foreach (var item in user.Experience)
                {
                    var newItem = ExistOrDefaultById<UserExperience>(existingUser.Experience, item);
                    UpdateExperienceFromView(item, newItem);
                    newExperiences.Add(newItem);
                }
                existingUser.Experience = newExperiences;
                await _userRepository.Update(existingUser);
                return true;
            }
            catch (MongoException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<EmployerProfileViewModel> GetEmployerProfile(string Id, string role)
        {

            Employer profile = null;
            switch (role)
            {
                case "employer":
                    profile = (await _employerRepository.GetByIdAsync(Id));
                    break;
                case "recruiter":
                    profile = (await _recruiterRepository.GetByIdAsync(Id));
                    break;
            }

            var videoUrl = "";

            if (profile != null)
            {
                videoUrl = string.IsNullOrWhiteSpace(profile.VideoName)
                          ? ""
                          : await _fileService.GetFileURL(profile.VideoName, FileType.UserVideo);

                var skills = profile.Skills.Select(x => ViewModelFromSkill(x)).ToList();

                var result = new EmployerProfileViewModel
                {
                    Id = profile.Id,
                    CompanyContact = profile.CompanyContact,
                    PrimaryContact = profile.PrimaryContact,
                    Skills = skills,
                    ProfilePhoto = profile.ProfilePhoto,
                    ProfilePhotoUrl = profile.ProfilePhotoUrl,
                    VideoName = profile.VideoName,
                    VideoUrl = videoUrl,
                    DisplayProfile = profile.DisplayProfile,
                };
                return result;
            }

            return null;
        }

        public async Task<bool> UpdateEmployerProfile(EmployerProfileViewModel employer, string updaterId, string role)
        {
            try
            {
                if (employer.Id != null)
                {
                    switch (role)
                    {
                        case "employer":
                            Employer existingEmployer = (await _employerRepository.GetByIdAsync(employer.Id));
                            existingEmployer.CompanyContact = employer.CompanyContact;
                            existingEmployer.PrimaryContact = employer.PrimaryContact;
                            existingEmployer.ProfilePhoto = employer.ProfilePhoto;
                            existingEmployer.ProfilePhotoUrl = employer.ProfilePhotoUrl;
                            existingEmployer.DisplayProfile = employer.DisplayProfile;
                            existingEmployer.UpdatedBy = updaterId;
                            existingEmployer.UpdatedOn = DateTime.Now;

                            var newSkills = new List<UserSkill>();
                            foreach (var item in employer.Skills)
                            {
                                var skill = existingEmployer.Skills.SingleOrDefault(x => x.Id == item.Id);
                                if (skill == null)
                                {
                                    skill = new UserSkill
                                    {
                                        Id = ObjectId.GenerateNewId().ToString(),
                                        IsDeleted = false
                                    };
                                }
                                UpdateSkillFromView(item, skill);
                                newSkills.Add(skill);
                            }
                            existingEmployer.Skills = newSkills;

                            await _employerRepository.Update(existingEmployer);
                            break;

                        case "recruiter":
                            Recruiter existingRecruiter = (await _recruiterRepository.GetByIdAsync(employer.Id));
                            existingRecruiter.CompanyContact = employer.CompanyContact;
                            existingRecruiter.PrimaryContact = employer.PrimaryContact;
                            existingRecruiter.ProfilePhoto = employer.ProfilePhoto;
                            existingRecruiter.ProfilePhotoUrl = employer.ProfilePhotoUrl;
                            existingRecruiter.DisplayProfile = employer.DisplayProfile;
                            existingRecruiter.UpdatedBy = updaterId;
                            existingRecruiter.UpdatedOn = DateTime.Now;

                            var newRSkills = new List<UserSkill>();
                            foreach (var item in employer.Skills)
                            {
                                var skill = existingRecruiter.Skills.SingleOrDefault(x => x.Id == item.Id);
                                if (skill == null)
                                {
                                    skill = new UserSkill
                                    {
                                        Id = ObjectId.GenerateNewId().ToString(),
                                        IsDeleted = false
                                    };
                                }
                                UpdateSkillFromView(item, skill);
                                newRSkills.Add(skill);
                            }
                            existingRecruiter.Skills = newRSkills;
                            await _recruiterRepository.Update(existingRecruiter);

                            break;
                    }
                    return true;
                }
                return false;
            }
            catch (MongoException e)
            {
                return false;
            }
        }

        public async Task<bool> UpdateEmployerPhoto(string employerId, IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            List<string> acceptedExtensions = new List<string> { ".jpg", ".png", ".gif", ".jpeg" };

            if (fileExtension != null && !acceptedExtensions.Contains(fileExtension.ToLower()))
            {
                return false;
            }

            var profile = (await _employerRepository.Get(x => x.Id == employerId)).SingleOrDefault();

            if (profile == null)
            {
                return false;
            }

            var newFileName = await _fileService.SaveFile(file, FileType.ProfilePhoto);

            if (!string.IsNullOrWhiteSpace(newFileName))
            {
                var oldFileName = profile.ProfilePhoto;

                if (!string.IsNullOrWhiteSpace(oldFileName))
                {
                    await _fileService.DeleteFile(oldFileName, FileType.ProfilePhoto);
                }

                profile.ProfilePhoto = newFileName;
                profile.ProfilePhotoUrl = await _fileService.GetFileURL(newFileName, FileType.ProfilePhoto);

                await _employerRepository.Update(profile);
                return true;
            }

            return false;

        }

        public async Task<bool> AddEmployerVideo(string employerId, IFormFile file)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateTalentPhoto(string talentId, IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            List<string> acceptedExtensions = new List<string> { ".jpg", ".png", ".gif", ".jpeg" };

            if (fileExtension != null && !acceptedExtensions.Contains(fileExtension.ToLower()))
            {
                return false;
            }

            var profile = (await _userRepository.Get(x => x.Id == talentId)).SingleOrDefault();

            if (profile == null)
            {
                return false;
            }

            var newFileName = await _fileService.SaveFile(file, FileType.ProfilePhoto);

            if (!string.IsNullOrWhiteSpace(newFileName))
            {
                var oldFileName = profile.ProfilePhoto;

                if (!string.IsNullOrWhiteSpace(oldFileName))
                {
                    await _fileService.DeleteFile(oldFileName, FileType.ProfilePhoto);
                }

                profile.ProfilePhoto = newFileName;
                profile.ProfilePhotoUrl = await _fileService.GetFileURL(newFileName, FileType.ProfilePhoto);

                await _userRepository.Update(profile);
                return true;
            }

            return false;
        }

        public async Task<bool> AddTalentVideo(string talentId, IFormFile file)
        {
            //Your code here;
            throw new NotImplementedException();

        }

        public async Task<bool> RemoveTalentVideo(string talentId, string videoName)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateTalentCV(string talentId, IFormFile file)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetTalentSuggestionIds(string employerOrJobId, bool forJob, int position, int increment)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TalentSnapshotViewModel>> GetTalentSnapshotList(string employerOrJobId, bool forJob, int position, int increment)
        {
            //return new List<TalentSnapshotViewModel>();
            try
            {
                IEnumerable<User> users = await _userRepository.Get(x => FindTalentForJob(x, employerOrJobId, forJob));
                List<TalentSnapshotViewModel> snapshots = new List<TalentSnapshotViewModel>();
                foreach (var user in users)
                {
                    List<String> userSkill = new List<String>();
                    foreach (var skill in user.Skills)
                    {
                        userSkill.Add(skill.Skill);
                    }
                    snapshots.Add(new TalentSnapshotViewModel
                    {
                        Id = user.Id,
                        Name = user.FirstName + " " + user.LastName,
                        PhotoId = user.ProfilePhotoUrl,
                        Skills = userSkill,
                        Summary = user.Summary,
                        Visa = user.VisaStatus,
                        CurrentEmployment = "Software Developer at Dummy Company",
                        Level = "Dummy Level"
                    });
                }
                return snapshots;
            }
            catch (Exception e)
            {
                return new List<TalentSnapshotViewModel>();
            }
            
        }

        private bool FindTalentForJob(User talent, string employerOrJobId, bool forJob)
        {
            try
            {
                if (forJob)
                {
                    var job = _jobRepository.GetByIdAsync(employerOrJobId).Result;
                    if (job == null) return false;

                    var requirements = job.ApplicantDetails.Qualifications;
                    foreach (var skill in talent.Skills)
                    {
                        if (requirements.Any(skill.Skill.Contains)) return true;
                    }
                }
                else
                {
                    var employer = _employerRepository.GetByIdAsync(employerOrJobId).Result;
                    if (employer == null) return false;

                    foreach (var skill in talent.Skills)
                    {
                        if (employer.Skills.Exists(x => x == skill)) return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<TalentSnapshotViewModel>> GetTalentSnapshotList(IEnumerable<string> ids)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        private T ExistOrDefaultById<T>(List<T> list, Object item)
        {
            try
            {
                Type type = typeof(T);
                System.Reflection.PropertyInfo idPropertyInfo = type.GetProperty("Id");
                // Compare id
                var result = list.SingleOrDefault<T>(x => idPropertyInfo.GetValue(x) == item.GetType().GetProperty("Id").GetValue(item));
                if (result == null)
                {
                    // Create a new instance of T type and set id
                    result = (T)Activator.CreateInstance(type);
                    idPropertyInfo.SetValue(result, ObjectId.GenerateNewId().ToString());
                }
                return result;
            }
            catch (Exception)
            {
                return (T)Activator.CreateInstance(typeof(T));
            }


            //list.GetType().GetProperty("id");
            //Console.WriteLine(list[0].GetType().GetProperty("Id").GetValue(list[1]));
            //Console.WriteLine(list[0].GetType().GetProperties()[0]);
            //Console.WriteLine(list.GetType().GetProperties()[0]);
            //Object item = Activator.CreateInstance(list[0].GetType());
            //item.GetType().GetProperty("Id").SetValue(item, "0");
            //Console.WriteLine(item.GetType().GetProperty("Id").GetValue(item));

            //var newskills = new list<userskill>();
            //foreach (var item in employer.skills)
            //{
            //    var skill = existingemployer.skills.singleordefault(x => x.id == item.id);
            //    if (skill == null)
            //    {
            //        skill = new userskill
            //        {
            //            id = objectid.generatenewid().tostring(),
            //            isdeleted = false
            //        };
            //    }
            //    updateskillfromview(item, skill);
            //    newskills.add(skill);
            //}
            //existingemployer.skills = newskills;
        }

        #region TalentMatching

        public async Task<IEnumerable<TalentSuggestionViewModel>> GetFullTalentList()
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public IEnumerable<TalentMatchingEmployerViewModel> GetEmployerList()
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TalentMatchingEmployerViewModel>> GetEmployerListByFilterAsync(SearchCompanyModel model)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TalentSuggestionViewModel>> GetTalentListByFilterAsync(SearchTalentModel model)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TalentSuggestion>> GetSuggestionList(string employerOrJobId, bool forJob, string recruiterId)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<bool> AddTalentSuggestions(AddTalentSuggestionList selectedTalents)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        #endregion

        #region Conversion Methods

        #region Update from View

        protected void UpdateSkillFromView(AddSkillViewModel model, UserSkill original)
        {
            original.ExperienceLevel = model.Level;
            original.Skill = model.Name;
            //original.IsDeleted = false;
        }

        protected void UpdateLanguageFromView(AddLanguageViewModel model, UserLanguage original)
        {
            original.Language = model.Name;
            original.LanguageLevel = model.Level;
            //original.IsDeleted = false;
        }

        protected void UpdateEducationFromView(AddEducationViewModel model, UserEducation original)
        {
            original.Country = model.Country;
            original.Title = model.Title;
            original.Degree = model.Degree;
            original.YearOfGraduation = model.YearOfGraduation;
            //original.IsDeleted = false;
        }

        protected void UpdateCertificationFromView(AddCertificationViewModel model, UserCertification original)
        {
            original.CertificationName = model.CertificationName;
            original.CertificationFrom = model.CertificationFrom;
            original.CertificationYear = model.CertificationYear;
            //original.IsDeleted = false;
        }

        protected void UpdateExperienceFromView(ExperienceViewModel model, UserExperience original)
        {
            original.Company = model.Company;
            original.Position = model.Position;
            original.Responsibilities = model.Responsibilities;
            original.Start = model.Start;
            original.End = model.End;
        }

        #endregion

        #region Build Views from Model

        protected AddSkillViewModel ViewModelFromSkill(UserSkill skill)
        {
            return new AddSkillViewModel
            {
                Id = skill.Id,
                Level = skill.ExperienceLevel,
                Name = skill.Skill
            };
        }

        protected AddLanguageViewModel ViewModelFromLanguage(UserLanguage language)
        {
            return new AddLanguageViewModel
            {
                Id = language.Id,
                Level = language.LanguageLevel,
                Name = language.Language,
                CurrentUserId = language.UserId
            };
        }

        protected AddEducationViewModel ViewModelFromEducation(UserEducation education)
        {
            return new AddEducationViewModel
            {
                Id = education.Id,
                Country = education.Country,
                InstituteName = education.InstituteName,
                Title = education.Title,
                Degree = education.Degree,
                YearOfGraduation = education.YearOfGraduation
            };
        }

        protected AddCertificationViewModel ViewModelFromCertification(UserCertification certification)
        {
            return new AddCertificationViewModel
            {
                Id = certification.Id,
                CertificationName = certification.CertificationName,
                CertificationFrom = certification.CertificationFrom,
                CertificationYear = certification.CertificationYear
            };
        }

        protected ExperienceViewModel ViewModelFromExperience(UserExperience experience)
        {
            return new ExperienceViewModel
            {
                Id = experience.Id,
                Company = experience.Company,
                Position = experience.Position,
                Responsibilities = experience.Responsibilities,
                Start = experience.Start,
                End = experience.End
            };
        }

        #endregion

        #endregion

        #region ManageClients

        public async Task<IEnumerable<ClientViewModel>> GetClientListAsync(string recruiterId)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<ClientViewModel> ConvertToClientsViewAsync(Client client, string recruiterId)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<int> GetTotalTalentsForClient(string clientId, string recruiterId)
        {
            //Your code here;
            throw new NotImplementedException();

        }

        public async Task<Employer> GetEmployer(string employerId)
        {
            return await _employerRepository.GetByIdAsync(employerId);
        }
        #endregion

    }
}
