import { Component, OnInit, inject } from '@angular/core';
import { NgClass, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AdminService } from '../admin.service';
import { ToastService } from '../../../core/toast/toast.service';
import { Doctor } from '../../../shared/models/doctor.model';
import { TimeSlot, SlotStatus, SLOT_STATUS_LABEL, SLOT_STATUS_CLASS } from '../../../shared/models/time-slot.model';

@Component({
  selector: 'app-admin-timeslots',
  standalone: true,
  imports: [NgClass, DatePipe, FormsModule, RouterModule],
  templateUrl: './timeslots.component.html',
  styleUrl: './timeslots.component.css',
})
export class TimeslotsComponent implements OnInit {
  private service = inject(AdminService);
  private toast   = inject(ToastService);
  readonly router = inject(Router);

  doctors:       Doctor[]    = [];
  slots:         TimeSlot[]  = [];
  filteredSlots: TimeSlot[]  = [];

  selectedDoctorPublicId = '';
  statusFilter           = '';
  loading                = false;

  slotToDelete: { publicId: string; label: string } | null = null;

  // Drive dropdown from model constants — no duplication
  readonly statusOptions = Object.entries(SLOT_STATUS_LABEL).map(
    ([value, label]) => ({ value, label }),
  );

  // Expose enum to template for comparison
  readonly SlotStatus = SlotStatus;

  ngOnInit(): void {
    // Load first page of doctors for the selector
    this.service.getDoctors(1, 100).subscribe({
      next: (res) => { this.doctors = res.items; },
    });
  }

  onDoctorChange(): void {
    if (!this.selectedDoctorPublicId) {
      this.slots = [];
      this.filteredSlots = [];
      return;
    }
    this.loading      = true;
    this.statusFilter = '';

    // selectedDoctorPublicId is already a string Guid — no Number() conversion
    this.service.getTimeSlots(this.selectedDoctorPublicId).subscribe({
      next: (res) => {
        this.slots         = res;
        this.filteredSlots = res;
        this.loading       = false;
      },
      error: () => {
        this.loading = false;
        this.toast.error('Failed to load time slots.');
      },
    });
  }

  applyFilter(): void {
    this.filteredSlots = !this.statusFilter
      ? this.slots
      : this.slots.filter((s) => String(s.status) === this.statusFilter);
  }

  confirmDelete(slot: TimeSlot): void {
    this.slotToDelete = {
      publicId: slot.publicId,
      label: `${slot.date ? new Date(slot.date).toLocaleDateString() : ''} ${slot.startTime} – ${slot.endTime}`,
    };
  }

  cancelDelete(): void {
    this.slotToDelete = null;
  }

  deleteSlot(): void {
    if (!this.slotToDelete) return;
    const { publicId } = this.slotToDelete;
    this.slotToDelete  = null;

    this.service.deleteSlot(publicId).subscribe({
      next: () => {
        // Remove by publicId — not by int id
        this.slots         = this.slots.filter((s) => s.publicId !== publicId);
        this.filteredSlots = this.filteredSlots.filter((s) => s.publicId !== publicId);
        this.toast.warning('Time slot deleted.');
      },
      error: () => this.toast.error('Failed to delete time slot.'),
    });
  }

  getStatusLabel(status: SlotStatus): string {
    return SLOT_STATUS_LABEL[status] ?? 'Unknown';
  }

  getStatusClass(status: SlotStatus): string {
    return SLOT_STATUS_CLASS[status] ?? '';
  }
}