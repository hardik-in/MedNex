import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DoctorService } from '../doctor.service';
import { ToastService } from '../../../core/toast/toast.service';

const SLOT_STATUS_MAP: Record<number, { label: string; css: string }> = {
  1: { label: 'Available', css: 'slot-available' },
  2: { label: 'Booked', css: 'slot-booked' },
  3: { label: 'Blocked', css: 'slot-blocked' },
};

@Component({
  selector: 'app-doctor-timeslots',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './doctor-timeslots.component.html',
  styleUrl: './doctor-timeslots.component.css',
})
export class DoctorTimeslotsComponent implements OnInit {
  private service = inject(DoctorService);
  private toast = inject(ToastService);

  allSlots: any[] = [];
  filtered: any[] = [];
  loading = true;

  // Filters
  statusFilter = 0;
  dateFilter = '';
  showTodayOnly = true;
  today = new Date().toISOString().split('T')[0];

  // Delete confirmation
  slotToDelete: { id: number; label: string } | null = null;

  ngOnInit() {
    this.dateFilter = this.today;
    this.load();
  }

  load() {
    this.loading = true;
    this.service.getMyTimeslots().subscribe({
      next: (res) => {
        this.allSlots = res;
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  applyFilters() {
    let result = [...this.allSlots];

    if (this.showTodayOnly) {
      result = result.filter((s) => s.date?.startsWith(this.today));
    } else if (this.dateFilter) {
      result = result.filter((s) => s.date?.startsWith(this.dateFilter));
    }

    if (this.statusFilter !== 0) {
      result = result.filter((s) => s.status === this.statusFilter);
    }

    this.filtered = result.sort((a, b) => {
      const dateDiff = new Date(a.date).getTime() - new Date(b.date).getTime();
      if (dateDiff !== 0) return dateDiff;
      return a.startTime.localeCompare(b.startTime);
    });
  }

  onTodayToggle() {
    this.showTodayOnly = !this.showTodayOnly;
    if (this.showTodayOnly) {
      this.dateFilter = this.today;
    }
    this.applyFilters();
  }

  onDateChange() {
    this.showTodayOnly = false;
    this.applyFilters();
  }

  canDelete(status: number): boolean {
    return status !== 2; // not Booked
  }

  confirmDelete(slot: any) {
    this.slotToDelete = {
      id: slot.id,
      label: `${this.formatDate(slot.date)} ${slot.startTime} – ${slot.endTime}`,
    };
  }

  cancelDelete() {
    this.slotToDelete = null;
  }

  deleteSlot() {
    if (!this.slotToDelete) return;
    const { id } = this.slotToDelete;
    this.slotToDelete = null;
    this.service.deleteSlot(id).subscribe({
      next: () => {
        this.toast.danger('Timeslot deleted.');
        this.load();
      },
      error: () => {
        this.toast.error('Failed to delete slot.');
      },
    });
  }

  getSlotStatus(status: number) {
    return SLOT_STATUS_MAP[status] ?? { label: 'Unknown', css: '' };
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString('en-IN', {
      day: 'numeric',
      month: 'short',
      year: 'numeric',
    });
  }
}
