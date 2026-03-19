import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminAppointmentsService } from '../admin-appointments.service';
import { ToastService } from '../../../core/toast/toast.service';

const SLOT_STATUS_MAP: Record<number, { label: string; css: string }> = {
  1: { label: 'Available', css: 'status-available' },
  2: { label: 'Booked', css: 'status-booked' },
  3: { label: 'Blocked', css: 'status-blocked' },
};

@Component({
  selector: 'app-admin-timeslots',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './timeslots.component.html',
  styleUrl: './timeslots.component.css',
})
export class TimeslotsComponent implements OnInit {
  private toast = inject(ToastService);

  doctors: any[] = [];
  slots: any[] = [];
  filteredSlots: any[] = [];

  selectedDoctorId = '';
  statusFilter = '';
  loading = false;

  slotToDelete: { id: number; label: string } | null = null;

  readonly statusOptions = Object.entries(SLOT_STATUS_MAP).map(
    ([value, s]) => ({
      value,
      label: s.label,
    }),
  );

  constructor(
    private service: AdminAppointmentsService,
    public router: Router,
  ) {}

  ngOnInit() {
    this.service.getDoctors().subscribe((res) => (this.doctors = res));
  }

  onDoctorChange() {
    if (!this.selectedDoctorId) {
      this.slots = [];
      this.filteredSlots = [];
      return;
    }
    this.loading = true;
    this.statusFilter = '';
    this.service.getTimeSlots(Number(this.selectedDoctorId)).subscribe({
      next: (res) => {
        this.slots = res;
        this.filteredSlots = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toast.error('Failed to load time slots.');
      },
    });
  }

  applyFilter() {
    if (!this.statusFilter) {
      this.filteredSlots = this.slots;
      return;
    }
    this.filteredSlots = this.slots.filter(
      (s) => String(s.status) === this.statusFilter,
    );
  }

  confirmDelete(slot: any) {
    this.slotToDelete = {
      id: slot.id,
      label: `${slot.date ? new Date(slot.date).toLocaleDateString() : ''} ${slot.startTime} – ${slot.endTime}`,
    };
  }

  cancelDelete() {
    this.slotToDelete = null;
  }

  deleteSlot() {
    if (!this.slotToDelete) return;
    const id = this.slotToDelete.id;
    this.slotToDelete = null;

    this.service.deleteSlot(id).subscribe({
      next: () => {
        this.slots = this.slots.filter((s) => s.id !== id);
        this.filteredSlots = this.filteredSlots.filter((s) => s.id !== id);
        this.toast.danger('Time slot deleted.');
      },
      error: () => {
        this.toast.error('Failed to delete time slot.');
      },
    });
  }

  getStatus(status: number): { label: string; css: string } {
    return SLOT_STATUS_MAP[status] ?? { label: 'Unknown', css: '' };
  }
}
