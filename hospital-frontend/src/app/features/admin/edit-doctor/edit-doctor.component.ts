import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminAppointmentsService } from '../admin-appointments.service';
import { ToastService } from '../../../core/toast/toast.service';

@Component({
  selector: 'app-edit-doctor',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './edit-doctor.component.html',
  styleUrl: './edit-doctor.component.css',
})
export class EditDoctorComponent implements OnInit {
  private toast = inject(ToastService);

  private originalValues: any = null;
  admins: any[] = [];
  filteredAdmins: any[] = [];
  selectedAdmin: any = null;
  showAdminPopup = false;
  showConfirmPopup = false;
  adminSearch = '';

  form = this.fb.group({
    id: [{ value: '', disabled: true }],
    userId: [{ value: '', disabled: true }],
    firstName: [{ value: '', disabled: true }],
    lastName: [{ value: '', disabled: true }],
    email: [''],
    phoneNumber: [''],
    specialization: [''],
    licenseNumber: [{ value: '', disabled: true }],
    yearsOfExperience: [''],
    qualifications: [''],
    bio: [''],
    consultationFee: [''],
    address: [''],
    assignedAdminId: [''],
  });

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    public router: Router,
    private service: AdminAppointmentsService,
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');

    this.service.getAdmins().subscribe((res) => {
      this.admins = res;
      this.filteredAdmins = res;
    });

    this.service.getDoctor(Number(id)).subscribe((d: any) => {
      this.form.patchValue({
        ...d,
        assignedAdminId: d.assignedAdminId ? String(d.assignedAdminId) : '',
      });

      // Snapshot the loaded values for dirty checking
      this.originalValues = this.form.getRawValue();

      if (d.assignedAdminId) {
        this.service.getAdmins().subscribe((admins) => {
          this.selectedAdmin =
            admins.find((a) => a.id === d.assignedAdminId) ?? null;
        });
      }
    });
  }

  openAdminPopup() {
    this.adminSearch = '';
    this.filteredAdmins = this.admins; // show all by default
    this.showAdminPopup = true;
  }

  closeAdminPopup() {
    this.showAdminPopup = false;
  }

  onAdminSearch() {
    const term = this.adminSearch.toLowerCase().trim();
    if (!term) {
      this.filteredAdmins = this.admins;
      return;
    }
    this.filteredAdmins = this.admins.filter(
      (a) =>
        a.firstName.toLowerCase().startsWith(term) ||
        a.lastName.toLowerCase().startsWith(term) ||
        a.employeeId?.toLowerCase().startsWith(term),
    );
  }

  selectAdmin(admin: any) {
    this.selectedAdmin = admin;
    this.form.patchValue({ assignedAdminId: String(admin.id) });
    this.closeAdminPopup();
  }

  // Called by the form submit — shows confirmation popup instead of saving immediately
  requestConfirm() {
    const current = this.form.getRawValue();
    const hasChanges =
      JSON.stringify(current) !== JSON.stringify(this.originalValues);
    if (!hasChanges) {
      this.toast.warning('No changes were made.');
      return;
    }
    this.showConfirmPopup = true;
  }

  cancelConfirm() {
    this.showConfirmPopup = false;
  }

  confirmSubmit() {
    this.showConfirmPopup = false;
    const id = this.route.snapshot.paramMap.get('id');
    const raw = this.form.getRawValue();

    const payload = {
      ...raw,
      phoneNumber: raw.phoneNumber || undefined,
      specialization: raw.specialization || undefined,
      qualifications: raw.qualifications || undefined,
      bio: raw.bio || undefined,
      address: raw.address || undefined,
      assignedAdminId: raw.assignedAdminId
        ? Number(raw.assignedAdminId)
        : undefined,
      consultationFee: raw.consultationFee
        ? Number(raw.consultationFee)
        : undefined,
      yearsOfExperience: raw.yearsOfExperience
        ? Number(raw.yearsOfExperience)
        : undefined,
    };

    this.service.updateDoctor(Number(id), payload).subscribe({
      next: () => {
        this.toast.success('Doctor updated successfully.');
        this.router.navigate(['/admin/doctors']);
      },
      error: () => {
        this.toast.error('Failed to update doctor. Please try again.');
      },
    });
  }
}
