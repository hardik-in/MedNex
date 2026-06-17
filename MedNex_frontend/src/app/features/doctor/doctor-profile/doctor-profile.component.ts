import { Component, OnInit, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { DoctorService } from '../doctor.service';
import { ToastService } from '../../../core/toast/toast.service';
import { Doctor } from '../../../shared/models/doctor.model';

@Component({
  selector: 'app-doctor-profile',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './doctor-profile.component.html',
  styleUrl: './doctor-profile.component.css',
})
export class DoctorProfileComponent implements OnInit {
  private doctorService = inject(DoctorService);
  private toast = inject(ToastService);
  private fb = inject(FormBuilder);

  profile: Doctor | null = null;
  loading = true;
  isEditMode = false;
  showConfirmPopup = false;
  saving = false;
  submitted = false;

  private originalValues: unknown = null;

  form = this.fb.group({
    phoneNumber: [
      '',
      [Validators.required, Validators.pattern(/^\+?[\d\s\-().]{7,20}$/)],
    ],
    consultationFee: [
      null as number | null,
      [Validators.required, Validators.min(0)],
    ],
    bio: [''],
    qualifications: [''],
  });

  get f() {
    return this.form.controls;
  }

  get p(): Doctor { return this.profile!; }

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.loading = true;
    this.doctorService.getMyProfile().subscribe({
      next: (profile) => {
        this.profile = profile;
        this.loading = false;
      },
      error: () => {
        this.toast.error('Failed to load profile.');
        this.loading = false;
      },
    });
  }

  enterEditMode(): void {
    if (!this.profile) return;
    this.form.patchValue({
      phoneNumber: this.profile.phoneNumber ?? '',
      consultationFee: this.profile.consultationFee ?? null,
      bio: this.profile.bio ?? '',
      qualifications: this.profile.qualifications ?? '',
    });
    this.originalValues = this.form.getRawValue();
    this.submitted = false;
    this.isEditMode = true;
  }

  cancelEdit(): void {
    this.isEditMode = false;
    this.submitted = false;
    this.showConfirmPopup = false;
  }

  requestConfirm(): void {
    this.submitted = true;
    if (this.form.invalid) return;
    const hasChanges =
      JSON.stringify(this.form.getRawValue()) !==
      JSON.stringify(this.originalValues);
    if (!hasChanges) {
      this.toast.warning('No changes were made.');
      return;
    }
    this.showConfirmPopup = true;
  }

  confirmSave(): void {
    if (!this.profile) return;
    this.showConfirmPopup = false;
    this.saving = true;

    const raw = this.form.getRawValue();
    const payload = {
      email: this.profile.email,
      phoneNumber: raw.phoneNumber || undefined,
      consultationFee: raw.consultationFee ?? undefined,
      bio: raw.bio || undefined,
      qualifications: raw.qualifications || undefined,
    };

    // publicId (Guid string) — not internal int id
    this.doctorService
      .updateMyProfile(this.profile.publicId, payload)
      .subscribe({
        next: () => {
          this.toast.success('Profile updated successfully.');
          this.saving = false;
          this.isEditMode = false;
          this.loadProfile();
        },
        error: () => {
          this.toast.error('Failed to update profile.');
          this.saving = false;
        },
      });
  }

  cancelConfirm(): void {
    this.showConfirmPopup = false;
  }

  // Compute years of experience from careerStartDate
  getExperienceYears(careerStartDate: string | undefined): string {
    if (!careerStartDate) return '—';
    const years =
      new Date().getFullYear() - new Date(careerStartDate).getFullYear();
    return `${years} yrs`;
  }

  getGender(gender: string | number | undefined): string {
    if (gender === null || gender === undefined) return '—';
    if (typeof gender === 'string') return gender;
    const map: Record<number, string> = { 0: 'Male', 1: 'Female', 2: 'Other' };
    return map[gender] ?? '—';
  }

  formatDate(val: string | null | undefined): string {
    if (!val) return '—';
    return new Date(val).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  }

  formatFee(fee: number | null | undefined): string {
    if (fee === null || fee === undefined) return '—';
    return `₹${fee.toLocaleString('en-IN', { minimumFractionDigits: 2 })}`;
  }

  getCareerYear(careerStartDate: string | undefined): string {
    if (!careerStartDate) return '—';
    return new Date(careerStartDate).getFullYear().toString();
  }
}
