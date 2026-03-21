using DoctorPatientApp.API.Models.Entities;
using DoctorPatientApp.API.Models.Enums;
using DoctorPatientApp.API.Services.Interfaces;
using DoctorPatientApp.API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DoctorPatientApp.API.Data
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public DataSeeder(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            // Guard — don't seed if data already exists
            if (await _context.Users.IgnoreQueryFilters().AnyAsync())
                return;

            var year = DateTime.UtcNow.Year;
            var password = _passwordHasher.Hash("Test@1234");
            var tomorrow = DateTime.Today.AddDays(1);

            // ── Admin ─────────────────────────────────────────────────────
            var adminUser = new User
            {
                FirstName = "Super",
                LastName = "Admin",
                Email = "admin@mednex.com",
                PhoneNumber = "+919876500000",
                PasswordHash = password,
                Role = UserRole.Admin,
                Gender = Gender.Male,
                DateOfBirth = new DateTime(1980, 1, 15),
                Address = "Mednex HQ, Mumbai",
                IsActive = true,
                ReferenceId = ReferenceIdGenerator.Generate("USR", year, 1)
            };
            await _context.Users.AddAsync(adminUser);
            await _context.SaveChangesAsync();

            var admin = new Admin
            {
                UserId = adminUser.Id,
                Department = "Administration",
                EmployeeId = "EMP-001",
                ReferenceId = ReferenceIdGenerator.Generate("ADM", year, 1)
            };
            await _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();

            // ── Doctors ───────────────────────────────────────────────────
            var doctorSeeds = new[]
            {
                new { First = "Rajesh",  Last = "Kumar",  Email = "rajesh.kumar@mednex.com",  Phone = "+919876500001", Spec = "Cardiology",       License = "LIC-CAR-001", Qual = "MBBS, MD Cardiology",       Bio = "Senior cardiologist with 15 years of experience.",     Fee = 800m,  Years = 15, DOB = new DateTime(1978, 3, 20),  Gender = Gender.Male   },
                new { First = "Priya",   Last = "Sharma", Email = "priya.sharma@mednex.com",   Phone = "+919876500002", Spec = "Neurology",        License = "LIC-NEU-001", Qual = "MBBS, DM Neurology",        Bio = "Specialist in neurological disorders and headaches.",   Fee = 900m,  Years = 12, DOB = new DateTime(1981, 7, 10),  Gender = Gender.Female },
                new { First = "Anil",    Last = "Mehta",  Email = "anil.mehta@mednex.com",     Phone = "+919876500003", Spec = "Orthopedics",      License = "LIC-ORT-001", Qual = "MBBS, MS Orthopedics",      Bio = "Expert in bone and joint surgery.",                    Fee = 700m,  Years = 10, DOB = new DateTime(1983, 11, 5),  Gender = Gender.Male   },
                new { First = "Sneha",   Last = "Patel",  Email = "sneha.patel@mednex.com",    Phone = "+919876500004", Spec = "Dermatology",      License = "LIC-DER-001", Qual = "MBBS, MD Dermatology",      Bio = "Specialist in skin and hair conditions.",              Fee = 600m,  Years = 8,  DOB = new DateTime(1985, 5, 22),  Gender = Gender.Female },
                new { First = "Vikram",  Last = "Singh",  Email = "vikram.singh@mednex.com",   Phone = "+919876500005", Spec = "General Medicine", License = "LIC-GEN-001", Qual = "MBBS, MD General Medicine", Bio = "General physician with broad clinical expertise.",     Fee = 500m,  Years = 6,  DOB = new DateTime(1988, 9, 14),  Gender = Gender.Male   },
            };

            var doctors = new List<Doctor>();

            for (int i = 0; i < doctorSeeds.Length; i++)
            {
                var d = doctorSeeds[i];

                var doctorUser = new User
                {
                    FirstName = d.First,
                    LastName = d.Last,
                    Email = d.Email,
                    PhoneNumber = d.Phone,
                    PasswordHash = password,
                    Role = UserRole.Doctor,
                    Gender = d.Gender,
                    DateOfBirth = d.DOB,
                    Address = "Mednex Hospital, Mumbai",
                    IsActive = true,
                    ReferenceId = ReferenceIdGenerator.Generate("USR", year, i + 2) // USR-YYYY-0002 to 0006
                };
                await _context.Users.AddAsync(doctorUser);
                await _context.SaveChangesAsync();

                var doctor = new Doctor
                {
                    UserId = doctorUser.Id,
                    AssignedAdminId = admin.Id,
                    Specialization = d.Spec,
                    LicenseNumber = d.License,
                    Qualifications = d.Qual,
                    Bio = d.Bio,
                    ConsultationFee = d.Fee,
                    CareerStartDate = DateTime.UtcNow.AddYears(-d.Years),
                    ReferenceId = ReferenceIdGenerator.Generate("DOC", year, i + 1)
                };
                await _context.Doctors.AddAsync(doctor);
                await _context.SaveChangesAsync();
                doctors.Add(doctor);
            }

            // ── Patients ──────────────────────────────────────────────────
            var patientSeeds = new[]
            {
                new { First = "Amit",   Last = "Joshi",  Email = "amit.joshi@gmail.com",   Phone = "+919876500006", DOB = new DateTime(1990, 2, 14),  Gender = Gender.Male,   Blood = BloodGroup.APositive  },
                new { First = "Neha",   Last = "Verma",  Email = "neha.verma@gmail.com",   Phone = "+919876500007", DOB = new DateTime(1995, 6, 28),  Gender = Gender.Female, Blood = BloodGroup.BPositive  },
                new { First = "Suresh", Last = "Reddy",  Email = "suresh.reddy@gmail.com", Phone = "+919876500008", DOB = new DateTime(1988, 11, 3),  Gender = Gender.Male,   Blood = BloodGroup.OPositive  },
                new { First = "Kavita", Last = "Nair",   Email = "kavita.nair@gmail.com",  Phone = "+919876500009", DOB = new DateTime(1992, 4, 17),  Gender = Gender.Female, Blood = BloodGroup.ABPositive },
                new { First = "Rohit",  Last = "Gupta",  Email = "rohit.gupta@gmail.com",  Phone = "+919876500010", DOB = new DateTime(1985, 8, 9),   Gender = Gender.Male,   Blood = BloodGroup.ANegative  },
            };

            var patients = new List<Patient>();

            for (int i = 0; i < patientSeeds.Length; i++)
            {
                var p = patientSeeds[i];

                var patientUser = new User
                {
                    FirstName = p.First,
                    LastName = p.Last,
                    Email = p.Email,
                    PhoneNumber = p.Phone,
                    PasswordHash = password,
                    Role = UserRole.Patient,
                    Gender = p.Gender,
                    DateOfBirth = p.DOB,
                    Address = "Mumbai, Maharashtra",
                    IsActive = true,
                    ReferenceId = ReferenceIdGenerator.Generate("USR", year, i + 7) // USR-YYYY-0007 to 0011
                };
                await _context.Users.AddAsync(patientUser);
                await _context.SaveChangesAsync();

                var patient = new Patient
                {
                    UserId = patientUser.Id,
                    BloodGroup = p.Blood,
                    Allergies = "None reported",
                    MedicalHistory = "No significant prior history",
                    EmergencyContactName = "Emergency Contact",
                    EmergencyContactPhone = "+919876599999",
                    ReferenceId = ReferenceIdGenerator.Generate("PAT", year, i + 1)
                };
                await _context.Patients.AddAsync(patient);
                await _context.SaveChangesAsync();
                patients.Add(patient);
            }

            // ── TimeSlots ─────────────────────────────────────────────────
            var slots = new List<TimeSlot>();

            for (int i = 0; i < 5; i++)
            {
                var slot = new TimeSlot
                {
                    DoctorId = doctors[i].Id,
                    Date = tomorrow,
                    StartTime = new TimeSpan(9 + i, 0, 0),
                    EndTime = new TimeSpan(9 + i, 30, 0),
                    Status = SlotStatus.Booked,
                    ReferenceId = ReferenceIdGenerator.Generate("SLT", year, i + 1)
                };
                await _context.TimeSlots.AddAsync(slot);
                await _context.SaveChangesAsync();
                slots.Add(slot);
            }

            // ── Appointments ──────────────────────────────────────────────
            var reasons = new[]
            {
                "Chest pain and shortness of breath",
                "Severe recurring headaches",
                "Lower back pain for 3 weeks",
                "Skin rash and itching",
                "Fever and sore throat"
            };

            var appointments = new List<Appointment>();

            for (int i = 0; i < 5; i++)
            {
                var appointment = new Appointment
                {
                    PatientId = patients[i].Id,
                    DoctorId = doctors[i].Id,
                    TimeSlotId = slots[i].Id,
                    AppointmentDate = tomorrow,
                    StartTime = slots[i].StartTime,
                    EndTime = slots[i].EndTime,
                    Status = AppointmentStatus.Completed,
                    Reason = reasons[i],
                    Notes = "Patient attended scheduled appointment.",
                    CompletedAt = DateTime.UtcNow,
                    ReferenceId = ReferenceIdGenerator.Generate("APT", year, i + 1)
                };
                await _context.Appointments.AddAsync(appointment);
                await _context.SaveChangesAsync();
                appointments.Add(appointment);
            }

            // ── Medical Records ───────────────────────────────────────────
            var medicalSeeds = new[]
            {
    new { Diagnosis = "Hypertension Stage 1",             Symptoms = "Chest tightness, dizziness, fatigue",          Treatment = "Antihypertensive medication, low sodium diet", LabResults = "Blood pressure: 145/92 mmHg. Cholesterol slightly elevated."     },
    new { Diagnosis = "Migraine with aura",               Symptoms = "Severe headache, visual disturbances, nausea", Treatment = "Triptan medication, dark room rest",           LabResults = "MRI brain: No structural abnormalities detected."                },
    new { Diagnosis = "Lumbar disc herniation (L4-L5)",   Symptoms = "Lower back pain, leg numbness",                Treatment = "Physiotherapy, NSAIDs, core strengthening",   LabResults = "MRI lumbar spine: L4-L5 disc herniation confirmed."              },
    new { Diagnosis = "Atopic dermatitis",                Symptoms = "Itchy red rash on arms and neck",              Treatment = "Topical corticosteroids, antihistamines",      LabResults = "Patch test: Positive for dust mites and synthetic fabrics."      },
    new { Diagnosis = "Viral upper respiratory infection", Symptoms = "Fever 101F, sore throat, runny nose",         Treatment = "Rest, fluids, symptomatic relief",             LabResults = "CBC: Mild leukocytosis. Throat swab: Negative for strep."       },
};

            var medicalRecords = new List<MedicalRecord>();

            for (int i = 0; i < 5; i++)
            {
                var m = medicalSeeds[i];

                var record = new MedicalRecord
                {
                    PatientId = patients[i].Id,
                    DoctorId = doctors[i].Id,
                    AppointmentId = appointments[i].Id,
                    Diagnosis = m.Diagnosis,
                    Symptoms = m.Symptoms,
                    Treatment = m.Treatment,
                    LabTestResults = m.LabResults,  
                    DoctorNotes = "Patient was cooperative. Advised follow-up in 2 weeks.",
                    Temperature = 98.4m + i * 0.2m,
                    BloodPressureSystolic = 120 + i * 4,
                    BloodPressureDiastolic = 80 + i * 2,
                    HeartRate = 72 + i * 3,
                    Weight = 62m + i * 4,
                    Height = 162m + i * 2,
                    Recommendations = "Adhere to medication, follow diet plan, avoid stress.",
                    FollowUpDate = DateTime.Today.AddDays(14),
                    ReferenceId = ReferenceIdGenerator.Generate("MED", year, i + 1)
                };
                await _context.MedicalRecords.AddAsync(record);
                await _context.SaveChangesAsync();
                medicalRecords.Add(record);
            }

            // ── Prescriptions ─────────────────────────────────────────────
            var prescriptionSeeds = new[]
            {
                new { Med = "Amlodipine",          Dosage = "5mg",   Freq = "Once daily",            Days = 30, Instructions = "Take in the morning with water."         },
                new { Med = "Sumatriptan",          Dosage = "50mg",  Freq = "As needed for migraine", Days = 14, Instructions = "Take at onset of migraine. Max 2/day." },
                new { Med = "Ibuprofen",            Dosage = "400mg", Freq = "Twice daily after meals", Days = 7, Instructions = "Do not take on empty stomach."          },
                new { Med = "Betamethasone cream",  Dosage = "0.1%",  Freq = "Apply twice daily",      Days = 14, Instructions = "Apply thin layer on affected area."     },
                new { Med = "Cetirizine",           Dosage = "10mg",  Freq = "Once daily at night",    Days = 7,  Instructions = "May cause drowsiness. Avoid driving."   },
            };

            for (int i = 0; i < 5; i++)
            {
                var rx = prescriptionSeeds[i];
                var startDate = DateTime.UtcNow;

                var prescription = new Prescription
                {
                    PatientId = patients[i].Id,
                    DoctorId = doctors[i].Id,
                    AppointmentId = appointments[i].Id,
                    MedicalRecordId = medicalRecords[i].Id,
                    MedicationName = rx.Med,
                    Dosage = rx.Dosage,
                    Frequency = rx.Freq,
                    DurationDays = rx.Days,
                    Instructions = rx.Instructions,
                    Notes = "Contact doctor if side effects occur.",
                    PrescribedDate = DateTime.UtcNow,
                    StartDate = startDate,
                    EndDate = startDate.AddDays(rx.Days),
                    IsActive = true,
                    ReferenceId = ReferenceIdGenerator.Generate("RX", year, i + 1)
                };
                await _context.Prescriptions.AddAsync(prescription);
                await _context.SaveChangesAsync();
            }
        }
    }
}