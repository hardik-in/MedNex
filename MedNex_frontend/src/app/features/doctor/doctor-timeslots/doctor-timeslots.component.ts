import { Component, OnInit, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DoctorService } from '../doctor.service';
import { ToastService } from '../../../core/toast/toast.service';
import { TimeSlot, SlotStatus, SLOT_STATUS_LABEL, SLOT_STATUS_CLASS } from '../../../shared/models/time-slot.model';

@Component({
  selector: 'app-doctor-timeslots',
  standalone: true,
  imports: [NgClass, FormsModule],
  templateUrl: './doctor-timeslots.component.html',
  styleUrl: './doctor-timeslots.component.css',
})
export class DoctorTimeslotsComponent implements OnInit {
  private service = inject(DoctorService);
  private toast   = inject(ToastService);

  allSlots: TimeSlot[] = [];
  filtered: TimeSlot[] = [];
  loading = true;
  error   = '';

  // Timeslot status IS numeric — backend does not use JsonStringEnumConverter here
  statusFilter  = 0; // 0 = all
  dateFilter    = '';
  showTodayOnly = true;
  readonly today = new Date().toISOString().split('T')[0];

  // Expose enum to template for status comparisons
  readonly SlotStatus = SlotStatus;

  // Delete confirmation — publicId (string Guid) not int id
  slotToDelete: { publicId: string; label: string } | null = null;

  ngOnInit(): void {
    this.dateFilter = this.today;
    this.load();
  }

  load(): void {
    this.loading = true;
    this.service.getMyTimeslots().subscribe({
      next: (res) => {
        this.allSlots = res;
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.error   = 'Failed to load timeslots.';
        this.loading = false;
      },
    });
  }

  applyFilters(): void {
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
      return dateDiff !== 0 ? dateDiff : a.startTime.localeCompare(b.startTime);
    });
  }

  onTodayToggle(): void {
    this.showTodayOnly = !this.showTodayOnly;
    if (this.showTodayOnly) this.dateFilter = this.today;
    this.applyFilters();
  }

  onDateChange(): void {
    this.showTodayOnly = false;
    this.applyFilters();
  }

  canDelete(status: SlotStatus): boolean {
    return status !== SlotStatus.Booked;
  }

  confirmDelete(slot: TimeSlot): void {
    this.slotToDelete = {
      publicId: slot.publicId,
      label: `${this.formatDate(slot.date)} ${slot.startTime} – ${slot.endTime}`,
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
        this.toast.warning('Timeslot deleted.');
        this.load();
      },
      error: () => {
        this.toast.error('Failed to delete slot.');
      },
    });
  }

  getSlotStatus(status: SlotStatus): { label: string; css: string } {
    return {
      label: SLOT_STATUS_LABEL[status] ?? 'Unknown',
      css:   SLOT_STATUS_CLASS[status] ?? '',
    };
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString('en-IN', {
      day: 'numeric', month: 'short', year: 'numeric',
    });
  }
}