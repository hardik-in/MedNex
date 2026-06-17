import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PatientService } from '../patient.service';
import { ToastService } from '../../../core/toast/toast.service';
import { Doctor } from '../../../shared/models/doctor.model';
import { TimeSlot, SlotStatus } from '../../../shared/models/time-slot.model';

@Component({
  selector: 'app-book-appointment',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './book-appointment.component.html',
  styleUrl: './book-appointment.component.css',
})
export class BookAppointmentComponent implements OnInit {
  private route  = inject(ActivatedRoute);
  private router = inject(Router);
  private svc    = inject(PatientService);
  private toast  = inject(ToastService);

  // publicId is a Guid string — never parse as Number
  doctorPublicId = '';
  doctor:  Doctor | null = null;
  slots:   TimeSlot[]    = [];

  loadingDoctor = true;
  loadingSlots  = false;
  booking       = false;

  selectedDate:   string      = '';
  selectedSlotId: string | null = null; // publicId (Guid string)
  reason = '';

  get d(): Doctor { return this.doctor!; }

  get today(): string {
    return new Date().toISOString().split('T')[0];
  }

  get selectedSlot(): TimeSlot | null {
    return this.slots.find((s) => s.publicId === this.selectedSlotId) ?? null;
  }

  ngOnInit(): void {
    this.doctorPublicId = this.route.snapshot.paramMap.get('publicId') ?? '';

    this.svc.getDoctorByPublicId(this.doctorPublicId).subscribe({
      next: (d) => {
        this.doctor       = d;
        this.loadingDoctor = false;
      },
      error: () => {
        this.toast.error('Failed to load doctor details.');
        this.loadingDoctor = false;
      },
    });
  }

  onDateChange(): void {
    if (!this.selectedDate) return;
    this.selectedSlotId = null;
    this.slots          = [];
    this.loadingSlots   = true;

    this.svc.getAvailableSlots(this.doctorPublicId, this.selectedDate).subscribe({
      next: (slots) => {
        // Only show Available slots — use enum constant not magic number 1
        this.slots        = slots.filter((s) => s.status === SlotStatus.Available);
        this.loadingSlots = false;
      },
      error: () => {
        this.toast.error('Failed to load slots.');
        this.loadingSlots = false;
      },
    });
  }

  selectSlot(publicId: string): void {
    // Toggle: clicking the same slot deselects it
    this.selectedSlotId = this.selectedSlotId === publicId ? null : publicId;
  }

  book(): void {
    if (!this.selectedSlotId) {
      this.toast.warning('Please select a time slot.');
      return;
    }
    if (!this.reason.trim()) {
      this.toast.warning('Please enter a reason for the appointment.');
      return;
    }

    this.booking = true;
    this.svc.createAppointment({
      doctorId:        this.doctorPublicId,    // Guid string
      timeSlotId:      this.selectedSlotId,    // Guid string
      appointmentDate: this.selectedDate,
      reason:          this.reason.trim(),
    }).subscribe({
      next: () => {
        this.toast.success('Appointment booked successfully!');
        this.router.navigate(['/patient/appointments']);
      },
      error: (err) => {
        this.toast.error(err?.error?.message ?? 'Failed to book appointment.');
        this.booking = false;
      },
    });
  }

  getExperienceYears(careerStartDate: string | undefined): string {
    if (!careerStartDate) return '—';
    return `${new Date().getFullYear() - new Date(careerStartDate).getFullYear()} yrs`;
  }
}