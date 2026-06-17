using FluentAssertions;
using MedNex_Backend.API.DTOs.Appointment;
using MedNex_Backend.API.Models.Entities;
using MedNex_Backend.API.Models.Enums;
using MedNex_Backend.API.Repositories.Interfaces;
using MedNex_Backend.API.Services.Implementations;
using Moq;

namespace MedNex_Backend.Tests
{
    // xUnit creates a new instance of this class for EACH test.
    // That means each test starts with fresh mocks — no shared state between tests.
    public class AppointmentServiceTests
    {
        // ── Mocks (fake repositories) ─────────────────────────────────────
        private readonly Mock<IAppointmentRepository> _mockAppointmentRepo;
        private readonly Mock<ITimeSlotRepository> _mockTimeSlotRepo;
        private readonly Mock<IPatientRepository> _mockPatientRepo;
        private readonly Mock<IDoctorRepository> _mockDoctorRepo;

        // sut = System Under Test — the real service wired with fake repos
        private readonly AppointmentService _sut;

        public AppointmentServiceTests()
        {
            _mockAppointmentRepo = new Mock<IAppointmentRepository>();
            _mockTimeSlotRepo = new Mock<ITimeSlotRepository>();
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockDoctorRepo = new Mock<IDoctorRepository>();

            // Inject fakes into the REAL service — it doesn't know they're mocks
            _sut = new AppointmentService(
                _mockAppointmentRepo.Object,
                _mockTimeSlotRepo.Object,
                _mockPatientRepo.Object,
                _mockDoctorRepo.Object
            );
        }

        // ═══════════════════════════════════════════════════════════════════
        // UpdateAppointmentStatusAsync — Status Transition Tests
        // ═══════════════════════════════════════════════════════════════════

        [Fact]
        public async Task UpdateStatus_WhenCancelledToPending_ShouldThrowInvalidOperationException()
        {
            // Arrange — set up a cancelled appointment
            var appointment = BuildAppointment(AppointmentStatus.Cancelled);

            _mockAppointmentRepo
                .Setup(r => r.GetAppointmentWithDetailsAsync(1))
                .ReturnsAsync(appointment);

            // Act — try to move it to Pending (illegal transition)
            var act = async () => await _sut.UpdateAppointmentStatusAsync(
                1, AppointmentStatus.Pending);

            // Assert — should throw with a meaningful message
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Cannot transition*");
        }

        [Fact]
        public async Task UpdateStatus_WhenCancelledToConfirmed_ShouldThrowInvalidOperationException()
        {
            var appointment = BuildAppointment(AppointmentStatus.Cancelled);

            _mockAppointmentRepo
                .Setup(r => r.GetAppointmentWithDetailsAsync(1))
                .ReturnsAsync(appointment);

            var act = async () => await _sut.UpdateAppointmentStatusAsync(
                1, AppointmentStatus.Confirmed);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Cannot transition*");
        }

        [Fact]
        public async Task UpdateStatus_WhenCompletedToConfirmed_ShouldThrowInvalidOperationException()
        {
            var appointment = BuildAppointment(AppointmentStatus.Completed);

            _mockAppointmentRepo
                .Setup(r => r.GetAppointmentWithDetailsAsync(1))
                .ReturnsAsync(appointment);

            var act = async () => await _sut.UpdateAppointmentStatusAsync(
                1, AppointmentStatus.Confirmed);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Cannot transition*");
        }

        [Fact]
        public async Task UpdateStatus_WhenCompletedToCancelled_ShouldThrowInvalidOperationException()
        {
            var appointment = BuildAppointment(AppointmentStatus.Completed);

            _mockAppointmentRepo
                .Setup(r => r.GetAppointmentWithDetailsAsync(1))
                .ReturnsAsync(appointment);

            var act = async () => await _sut.UpdateAppointmentStatusAsync(
                1, AppointmentStatus.Cancelled);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Cannot transition*");
        }

        [Fact]
        public async Task UpdateStatus_WhenPendingToConfirmed_ShouldSucceed()
        {
            // Arrange — valid transition: Pending → Confirmed
            var appointment = BuildAppointment(AppointmentStatus.Pending);

            _mockAppointmentRepo
                .Setup(r => r.GetAppointmentWithDetailsAsync(1))
                .ReturnsAsync(appointment);

            _mockAppointmentRepo
                .Setup(r => r.UpdateAsync(It.IsAny<Appointment>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.UpdateAppointmentStatusAsync(
                1, AppointmentStatus.Confirmed);

            // Assert — status was updated, no exception thrown
            result.Should().NotBeNull();
            appointment.Status.Should().Be(AppointmentStatus.Confirmed);
        }

        [Fact]
        public async Task UpdateStatus_WhenSetToCompleted_ShouldSetCompletedAt()
        {
            // Completing an appointment should stamp CompletedAt with current time
            var appointment = BuildAppointment(AppointmentStatus.Confirmed);

            _mockAppointmentRepo
                .Setup(r => r.GetAppointmentWithDetailsAsync(1))
                .ReturnsAsync(appointment);

            _mockAppointmentRepo
                .Setup(r => r.UpdateAsync(It.IsAny<Appointment>()))
                .Returns(Task.CompletedTask);

            await _sut.UpdateAppointmentStatusAsync(1, AppointmentStatus.Completed);

            // CompletedAt should be set and close to now
            appointment.CompletedAt.Should().NotBeNull();
            appointment.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task UpdateStatus_WhenAppointmentNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange — repo returns null (appointment doesn't exist)
            _mockAppointmentRepo
                .Setup(r => r.GetAppointmentWithDetailsAsync(999))
                .ReturnsAsync((Appointment?)null);

            var act = async () => await _sut.UpdateAppointmentStatusAsync(
                999, AppointmentStatus.Confirmed);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        // ═══════════════════════════════════════════════════════════════════
        // CancelAppointmentAsync Tests
        // ═══════════════════════════════════════════════════════════════════

        [Fact]
        public async Task Cancel_WhenAlreadyCancelled_ShouldThrowInvalidOperationException()
        {
            var appointment = BuildAppointment(AppointmentStatus.Cancelled);

            _mockAppointmentRepo
                .Setup(r => r.GetAppointmentWithDetailsAsync(1))
                .ReturnsAsync(appointment);

            var act = async () => await _sut.CancelAppointmentAsync(1, "duplicate cancel");

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*already cancelled*");
        }

        [Fact]
        public async Task Cancel_WhenCompleted_ShouldThrowInvalidOperationException()
        {
            var appointment = BuildAppointment(AppointmentStatus.Completed);

            _mockAppointmentRepo
                .Setup(r => r.GetAppointmentWithDetailsAsync(1))
                .ReturnsAsync(appointment);

            var act = async () => await _sut.CancelAppointmentAsync(1, "too late");

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*completed*");
        }

        [Fact]
        public async Task Cancel_WhenPending_ShouldSetStatusAndReleaseTimeSlot()
        {
            // Cancelling a pending appointment should:
            // 1. Set status to Cancelled
            // 2. Set CancelledAt
            // 3. Store the reason
            // 4. Release the timeslot back to Available
            var timeSlot = new TimeSlot { Id = 5, Status = SlotStatus.Booked };
            var appointment = BuildAppointment(AppointmentStatus.Pending);
            appointment.TimeSlotId = 5;

            _mockAppointmentRepo
                .Setup(r => r.GetAppointmentWithDetailsAsync(1))
                .ReturnsAsync(appointment);

            _mockAppointmentRepo
                .Setup(r => r.UpdateAsync(It.IsAny<Appointment>()))
                .Returns(Task.CompletedTask);

            _mockTimeSlotRepo
                .Setup(r => r.GetByIdAsync(5))
                .ReturnsAsync(timeSlot);

            _mockTimeSlotRepo
                .Setup(r => r.UpdateAsync(It.IsAny<TimeSlot>()))
                .Returns(Task.CompletedTask);

            await _sut.CancelAppointmentAsync(1, "Patient request");

            // Appointment should be cancelled
            appointment.Status.Should().Be(AppointmentStatus.Cancelled);
            appointment.CancellationReason.Should().Be("Patient request");
            appointment.CancelledAt.Should().NotBeNull();

            // TimeSlot should be released
            timeSlot.Status.Should().Be(SlotStatus.Available);

            // Verify UpdateAsync was called on BOTH entities
            _mockAppointmentRepo.Verify(r => r.UpdateAsync(appointment), Times.Once);
            _mockTimeSlotRepo.Verify(r => r.UpdateAsync(timeSlot), Times.Once);
        }

        // ═══════════════════════════════════════════════════════════════════
        // CreateAppointmentAsync Tests
        // ═══════════════════════════════════════════════════════════════════

        [Fact]
        public async Task Create_WhenSlotNotAvailable_ShouldThrowInvalidOperationException()
        {
            // Arrange — slot exists but is Booked
            var dto = BuildCreateDto();
            var patient = new Patient { Id = 1 };
            var doctor = new Doctor { Id = 2 };
            var timeSlot = new TimeSlot { Id = 3, Status = SlotStatus.Booked };

            _mockPatientRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(patient);
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(doctor);
            _mockTimeSlotRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(timeSlot);

            var act = async () => await _sut.CreateAppointmentAsync(dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*not available*");
        }

        [Fact]
        public async Task Create_WhenConflictingAppointmentExists_ShouldThrowInvalidOperationException()
        {
            // Slot is Available but there's already an appointment for it
            var dto = BuildCreateDto();
            var patient = new Patient { Id = 1 };
            var doctor = new Doctor { Id = 2 };
            var timeSlot = new TimeSlot { Id = 3, Status = SlotStatus.Available };

            _mockPatientRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(patient);
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(doctor);
            _mockTimeSlotRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(timeSlot);
            _mockAppointmentRepo
                .Setup(r => r.HasConflictingAppointmentAsync(2, 3))
                .ReturnsAsync(true); // conflict exists

            var act = async () => await _sut.CreateAppointmentAsync(dto);

            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*already booked*");
        }

        [Fact]
        public async Task Create_WhenPatientNotFound_ShouldThrowKeyNotFoundException()
        {
            var dto = BuildCreateDto();

            _mockPatientRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Patient?)null);

            var act = async () => await _sut.CreateAppointmentAsync(dto);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task Create_WhenDoctorNotFound_ShouldThrowKeyNotFoundException()
        {
            var dto = BuildCreateDto();
            var patient = new Patient { Id = 1 };

            _mockPatientRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(patient);
            _mockDoctorRepo
                .Setup(r => r.GetByIdAsync(2))
                .ReturnsAsync((Doctor?)null);

            var act = async () => await _sut.CreateAppointmentAsync(dto);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task Create_WhenTimeSlotNotFound_ShouldThrowKeyNotFoundException()
        {
            var dto = BuildCreateDto();
            var patient = new Patient { Id = 1 };
            var doctor = new Doctor { Id = 2 };

            _mockPatientRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(patient);
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(doctor);
            _mockTimeSlotRepo
                .Setup(r => r.GetByIdAsync(3))
                .ReturnsAsync((TimeSlot?)null);

            var act = async () => await _sut.CreateAppointmentAsync(dto);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        // ═══════════════════════════════════════════════════════════════════
        // GetAppointmentByIdAsync Tests
        // ═══════════════════════════════════════════════════════════════════

        [Fact]
        public async Task GetById_WhenNotFound_ShouldThrowKeyNotFoundException()
        {
            _mockAppointmentRepo
                .Setup(r => r.GetAppointmentWithDetailsAsync(999))
                .ReturnsAsync((Appointment?)null);

            var act = async () => await _sut.GetAppointmentByIdAsync(999);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        // ═══════════════════════════════════════════════════════════════════
        // Private helpers — build test data without cluttering test methods
        // ═══════════════════════════════════════════════════════════════════

        private static Appointment BuildAppointment(AppointmentStatus status)
        {
            return new Appointment
            {
                Id = 1,
                PatientId = 1,
                DoctorId = 2,
                TimeSlotId = 5,
                AppointmentDate = DateTime.Today.AddDays(1),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(9, 30, 0),
                Status = status,
                Patient = new Patient
                {
                    Id = 1,
                    PublicId = Guid.NewGuid(),
                    User = new User
                    {
                        FirstName = "Test",
                        LastName = "Patient",
                        Email = "patient@test.com"
                    }
                },
                Doctor = new Doctor
                {
                    Id = 2,
                    PublicId = Guid.NewGuid(),
                    Specialization = "General",
                    User = new User
                    {
                        FirstName = "Test",
                        LastName = "Doctor",
                        Email = "doctor@test.com"
                    }
                },
                TimeSlot = new TimeSlot
                {
                    Id = 5,
                    PublicId = Guid.NewGuid(),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(9, 30, 0)
                }
            };
        }

        private static CreateAppointmentDto BuildCreateDto()
        {
            return new CreateAppointmentDto
            {
                PatientId = 1,
                DoctorId = 2,
                TimeSlotId = 3,
                AppointmentDate = DateTime.Today.AddDays(1),
                Reason = "Test appointment"
            };
        }
    }
}