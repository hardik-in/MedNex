import { Component, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminService } from '../admin.service';
import { ToastService } from '../../../core/toast/toast.service';

@Component({
  selector: 'app-edit-patient',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './edit-patient.component.html',
  styleUrl: './edit-patient.component.css',
})
export class EditPatientComponent implements OnInit {
  private fb      = inject(FormBuilder);
  private route   = inject(ActivatedRoute);
  readonly router = inject(Router);
  private service = inject(AdminService);
  private toast   = inject(ToastService);

  // Store publicId separately — never rely on a disabled form control for navigation
  publicId = '';
  private originalValues: unknown = null;
  showConfirmPopup = false;

  form = this.fb.group({
    // Read-only display only
    referenceId: [{ value: '', disabled: true }],
    // Editable
    firstName:             [''],
    lastName:              [''],
    email:                 [''],
    phoneNumber:           [''],
    dateOfBirth:           [''],
    gender:                [''],
    address:               [''],
    bloodGroup:            [''],
    allergies:             [''],
    emergencyContactName:  [''],
    emergencyContactPhone: [''],
    medicalHistory:        [''],
  });

  ngOnInit(): void {
    // Route param is publicId (Guid string) — never parse as Number
    this.publicId = this.route.snapshot.paramMap.get('publicId') ?? '';

    this.service.getPatientByPublicId(this.publicId).subscribe({
      next: (res) => {
        const p = res;
        this.form.patchValue({
          referenceId:           p.referenceId,
          firstName:             p.firstName ?? '',
          lastName:              p.lastName  ?? '',
          email:                 p.email     ?? '',
          phoneNumber:           p.phoneNumber ?? '',
          dateOfBirth:           p.dateOfBirth
            ? p.dateOfBirth.substring(0, 10)
            : '',
          gender:                p.gender != null ? String(p.gender) : '',
          address:               p.address ?? '',
          bloodGroup:            p.bloodGroup ?? '',
          allergies:             (p as any).allergies ?? '',
          emergencyContactName:  p.emergencyContactName  ?? '',
          emergencyContactPhone: p.emergencyContactPhone ?? '',
          medicalHistory:        (p as any).medicalHistory ?? '',
        });
        this.originalValues = this.form.getRawValue();
      },
      error: () => this.toast.error('Failed to load patient profile.'),
    });
  }

  requestConfirm(): void {
    const hasChanges =
      JSON.stringify(this.form.getRawValue()) !== JSON.stringify(this.originalValues);
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
      phoneNumber:           raw.phoneNumber           || undefined,
      address:               raw.address               || undefined,
      allergies:             raw.allergies               || undefined,
      bloodGroup:            raw.bloodGroup            || undefined,
      emergencyContactName:  raw.emergencyContactName  || undefined,
      emergencyContactPhone: raw.emergencyContactPhone || undefined,
    };

    this.service.updatePatient(this.publicId, payload).subscribe({
      next: () => {
        this.toast.success('Patient updated successfully.');
        this.router.navigate(['/admin/patients', this.publicId]);
      },
      error: () => this.toast.error('Failed to update patient. Please try again.'),
    });
  }
}