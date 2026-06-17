import { Component, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastService } from '../../../core/toast/toast.service';
import { AdminService } from '../admin.service';
import { Doctor } from '../../../shared/models/doctor.model';

interface AdminOption {
  publicId: string;
  firstName: string;
  lastName: string;
  employeeId?: string;
}

@Component({
  selector: 'app-edit-doctor',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule],
  templateUrl: './edit-doctor.component.html',
  styleUrl: './edit-doctor.component.css',
})
export class EditDoctorComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  readonly router = inject(Router);
  private service = inject(AdminService);
  private toast = inject(ToastService);

  // Store publicId separately — never rely on disabled form controls for IDs
  publicId = '';
  private originalValues: unknown = null;

  admins: AdminOption[] = [];
  filteredAdmins: AdminOption[] = [];
  selectedAdmin: AdminOption | null = null;
  showAdminPopup = false;
  showConfirmPopup = false;
  adminSearch = '';

  form = this.fb.group({
    // Read-only display fields — never sent in payload
    referenceId: [{ value: '', disabled: true }],
    firstName: [{ value: '', disabled: true }],
    lastName: [{ value: '', disabled: true }],
    // Editable fields
    email: [''],
    phoneNumber: [''],
    specialization: [''],
    careerStartDate: [''], // replaces yearsOfExperience
    qualifications: [''],
    bio: [''],
    consultationFee: [''],
    address: [''],
    assignedAdminPublicId: [''],
  });

  ngOnInit(): void {
    // publicId is a Guid string — never parse as Number
    this.publicId = this.route.snapshot.paramMap.get('publicId') ?? '';

    // Load admins once — not twice
    this.service.getAdmins().subscribe({
      next: (res) => {
        this.admins = res as AdminOption[];
        this.filteredAdmins = res as AdminOption[];
      },
    });

    this.service.getDoctorByPublicId(this.publicId).subscribe({
      next: (d: Doctor) => {
        this.form.patchValue({
          referenceId: d.referenceId,
          firstName: d.firstName,
          lastName: d.lastName,
          email: d.email,
          phoneNumber: d.phoneNumber ?? '',
          specialization: d.specialization,
          careerStartDate: d.careerStartDate
            ? d.careerStartDate.substring(0, 10)
            : '',
          qualifications: d.qualifications ?? '',
          bio: d.bio ?? '',
          consultationFee:
            d.consultationFee != null ? String(d.consultationFee) : '',
          address: d.address ?? '',
          assignedAdminPublicId: d.assignedAdminPublicId ?? '',
        });

        this.originalValues = this.form.getRawValue();

        if (d.assignedAdminPublicId) {
          this.selectedAdmin =
            this.admins.find((a) => a.publicId === d.assignedAdminPublicId) ??
            null;
        }
      },
      error: () => this.toast.error('Failed to load doctor profile.'),
    });
  }

  openAdminPopup(): void {
    this.adminSearch = '';
    this.filteredAdmins = this.admins;
    this.showAdminPopup = true;
  }

  closeAdminPopup(): void {
    this.showAdminPopup = false;
  }

  onAdminSearch(): void {
    const term = this.adminSearch.toLowerCase().trim();
    this.filteredAdmins = !term
      ? this.admins
      : this.admins.filter(
          (a) =>
            a.firstName.toLowerCase().includes(term) ||
            a.lastName.toLowerCase().includes(term) ||
            a.employeeId?.toLowerCase().includes(term),
        );
  }

  selectAdmin(admin: AdminOption): void {
    this.selectedAdmin = admin;
    this.form.patchValue({ assignedAdminPublicId: admin.publicId });
    this.closeAdminPopup();
  }

  requestConfirm(): void {
    const current = this.form.getRawValue();
    const hasChanges =
      JSON.stringify(current) !== JSON.stringify(this.originalValues);
    if (!hasChanges) {
      this.toast.warning('No changes were made.');
      return;
    }
    this.showConfirmPopup = true;
  }

  cancelConfirm(): void {
    this.showConfirmPopup = false;
  }

  confirmSubmit(): void {
    this.showConfirmPopup = false;
    const raw = this.form.getRawValue();

    const payload = {
      email: raw.email ?? '',
      phoneNumber: raw.phoneNumber || undefined,
      specialization: raw.specialization || undefined,
      careerStartDate: raw.careerStartDate || undefined,
      qualifications: raw.qualifications || undefined,
      bio: raw.bio || undefined,
      address: raw.address || undefined,
      assignedAdminPublicId: raw.assignedAdminPublicId || undefined,
      consultationFee: raw.consultationFee
        ? Number(raw.consultationFee)
        : undefined,
    };

    this.service.updateDoctor(this.publicId, payload).subscribe({
      next: () => {
        this.toast.success('Doctor updated successfully.');
        this.router.navigate(['/admin/doctors']);
      },
      error: () =>
        this.toast.error('Failed to update doctor. Please try again.'),
    });
  }
}
