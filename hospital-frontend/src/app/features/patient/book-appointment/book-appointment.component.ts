import { Component, OnInit, inject } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PatientAppointmentsService } from '../patient-appointments.service';
import { ToastService } from '../../../core/toast/toast.service';

@Component({
  selector: 'app-book-appointment',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule, RouterLink],
  templateUrl: './book-appointment.component.html',
  styleUrl: './book-appointment.component.css',
})
export class BookAppointmentComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private svc = inject(PatientAppointmentsService);
  private toast = inject(ToastService);

  doctorId!: number;
  doctor: any = null;
  slots: any[] = [];

  loadingDoctor = true;
  loadingSlots = false;
  booking = false;

  selectedDate = '';
  selectedSlotId: number | null = null;
  reason = '';

  get today(): string {
    return new Date().toISOString().split('T')[0];
  }

  get selectedSlot(): any {
    return this.slots.find((s) => s.id === this.selectedSlotId) ?? null;
  }

  ngOnInit() {
    this.doctorId = Number(this.route.snapshot.paramMap.get('id'));

    this.svc.getDoctor(this.doctorId).subscribe({
      next: (d) => {
        this.doctor = d;
        this.loadingDoctor = false;
      },
      error: () => {
        this.toast.error('Failed to load doctor details.');
        this.loadingDoctor = false;
      },
    });
  }

  onDateChange() {
    if (!this.selectedDate) return;
    this.selectedSlotId = null;
    this.slots = [];
    this.loadingSlots = true;

    this.svc.getAvailableSlots(this.doctorId, this.selectedDate).subscribe({
      next: (slots) => {
        this.slots = slots.filter((s: any) => s.status === 1);
        this.loadingSlots = false;
      },
      error: () => {
        this.toast.error('Failed to load slots.');
        this.loadingSlots = false;
      },
    });
  }

  selectSlot(id: number) {
    this.selectedSlotId = this.selectedSlotId === id ? null : id;
  }

  book() {
    if (!this.selectedSlotId) {
      this.toast.warning('Please select a time slot.');
      return;
    }
    if (!this.reason.trim()) {
      this.toast.warning('Please enter a reason for the appointment.');
      return;
    }

    this.booking = true;
    this.svc
      .createAppointment({
        doctorId: this.doctorId,
        timeSlotId: this.selectedSlotId,
        appointmentDate: this.selectedDate,
        reason: this.reason.trim(),
      })
      .subscribe({
        next: () => {
          this.toast.success('Appointment booked successfully!');
          this.router.navigate(['/patient/appointments']);
        },
        error: (err) => {
          this.toast.error(
            err?.error?.message ?? 'Failed to book appointment.',
          );
          this.booking = false;
        },
      });
  }
}
