import { Component, OnInit, inject } from '@angular/core';
import { DatePipe, NgClass } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { PatientService } from '../patient.service';
import { ToastService } from '../../../core/toast/toast.service';
import { Patient } from '../../../shared/models/patient.model';

@Component({
  selector: 'app-patient-profile',
  standalone: true,
  imports: [DatePipe, NgClass, ReactiveFormsModule],
  templateUrl: './patient-profile.component.html',
  styleUrl: './patient-profile.component.css',
})
export class PatientProfileComponent implements OnInit {
  private svc   = inject(PatientService);
  private toast = inject(ToastService);
  private fb    = inject(FormBuilder);

  loading          = true;
  saving           = false;
  isEditing        = false;
  showConfirmPopup = false;
  submitted        = false;

  profile:         Patient | null = null;
  private originalValues: unknown = null;

  form = this.fb.group({
    firstName:             ['', Validators.required],
    lastName:              ['', Validators.required],
    phoneNumber:           [''],
    address:               [''],
    allergies:             [''],
    medicalHistory:        [''],
    emergencyContactName:  [''],
    emergencyContactPhone: [''],
  });

  get f() { return this.form.controls; }

  get p(): Patient { return this.profile!; }
  
  ngOnInit(): void {
    this.svc.getMyProfile().subscribe({
      next: (p) => {
        this.profile = p;
        this.patchForm(p);
        this.loading = false;
      },
      error: () => {
        this.toast.error('Failed to load profile.');
        this.loading = false;
      },
    });
  }

  private patchForm(p: Patient): void {
    this.form.patchValue({
      firstName:             p.firstName             ?? '',
      lastName:              p.lastName              ?? '',
      phoneNumber:           p.phoneNumber           ?? '',
      address:               p.address               ?? '',
      allergies:             p.allergies             ?? '',
      medicalHistory:        p.medicalHistory        ?? '',
      emergencyContactName:  p.emergencyContactName  ?? '',
      emergencyContactPhone: p.emergencyContactPhone ?? '',
    });
    this.originalValues = this.form.getRawValue();
  }

  startEditing(): void {
    this.submitted = false;
    if (this.profile) this.patchForm(this.profile);
    this.isEditing = true;
  }

  cancelEditing(): void {
    this.isEditing = false;
    this.submitted = false;
  }

  requestSave(): void {
    this.submitted = true;
    if (this.form.invalid) return;
    const hasChanges =
      JSON.stringify(this.form.getRawValue()) !== JSON.stringify(this.originalValues);
    if (!hasChanges) {
      this.toast.warning('No changes were made.');
      return;
    }
    this.showConfirmPopup = true;
  }

  cancelSave(): void {
    this.showConfirmPopup = false;
  }

  save(): void {
    if (!this.profile) return;
    this.showConfirmPopup = false;
    this.saving           = true;

    const raw = this.form.getRawValue();
    const payload = {
      phoneNumber:           raw.phoneNumber           || undefined,
      address:               raw.address               || undefined,
      emergencyContactName:  raw.emergencyContactName  || undefined,
      emergencyContactPhone: raw.emergencyContactPhone || undefined,
    };

    // publicId (Guid string) — not internal int id
    this.svc.updateProfile(this.profile.publicId, payload).subscribe({
      next: (updated) => {
        this.profile        = updated;
        this.originalValues = this.form.getRawValue();
        this.isEditing      = false;
        this.toast.success('Profile updated successfully.');
        this.saving = false;
      },
      error: (err) => {
        this.toast.error(err?.error?.message ?? 'Failed to update profile.');
        this.saving = false;
      },
    });
  }

  formatBloodGroup(val: string | undefined): string {
    if (!val) return '—';
    const map: Record<string, string> = {
      APositive:  'A+',  ANegative:  'A−',
      BPositive:  'B+',  BNegative:  'B−',
      OPositive:  'O+',  ONegative:  'O−',
      ABPositive: 'AB+', ABNegative: 'AB−',
    };
    return map[val] ?? val;
  }
}