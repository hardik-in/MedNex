import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminPatientsService } from '../admin-patients.service.ts.service';
import { ToastService } from '../../../core/toast/toast.service';

@Component({
  selector: 'app-edit-patient',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './edit-patient.component.html',
  styleUrl: './edit-patient.component.css',
})
export class EditPatientComponent implements OnInit {
  private toast = inject(ToastService);
  private originalValues: any = null;

  showConfirmPopup = false;

  form = this.fb.group({
    // Read-only
    id: [{ value: '', disabled: true }],
    userId: [{ value: '', disabled: true }],
    // Editable
    firstName: [''],
    lastName: [''],
    email: [''],
    phoneNumber: [''],
    dateOfBirth: [''],
    gender: [''],
    address: [''],
    bloodGroup: [''],
    allergies: [''],
    emergencyContactName: [''],
    emergencyContactPhone: [''],
    medicalHistory: [''],
  });

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    public router: Router,
    private service: AdminPatientsService,
  ) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.service.getPatient(id).subscribe((res: any) => {
      this.form.patchValue({
        id: String(res.id),
        userId: String(res.userId),
        firstName: res.firstName ?? '',
        lastName: res.lastName ?? '',
        email: res.email ?? '',
        phoneNumber: res.phoneNumber ?? '',
        dateOfBirth: res.dateOfBirth
          ? res.dateOfBirth.substring(0, 10) // format for date input: yyyy-MM-dd
          : '',
        gender: res.gender != null ? String(res.gender) : '',
        address: res.address ?? '',
        bloodGroup: res.bloodGroup ?? '',
        allergies: res.allergies ?? '',
        emergencyContactName: res.emergencyContactName ?? '',
        emergencyContactPhone: res.emergencyContactPhone ?? '',
        medicalHistory: res.medicalHistory ?? '',
      });

      this.originalValues = this.form.getRawValue();
    });
  }

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
    const id = Number(this.route.snapshot.paramMap.get('id'));
    const raw = this.form.getRawValue();

    const payload = {
      firstName: raw.firstName || undefined,
      lastName: raw.lastName || undefined,
      email: raw.email || undefined,
      phoneNumber: raw.phoneNumber || undefined,
      dateOfBirth: raw.dateOfBirth || undefined,
      gender:
        raw.gender != null && raw.gender !== ''
          ? Number(raw.gender)
          : undefined,
      address: raw.address || undefined,
      bloodGroup: raw.bloodGroup || undefined,
      allergies: raw.allergies || undefined,
      emergencyContactName: raw.emergencyContactName || undefined,
      emergencyContactPhone: raw.emergencyContactPhone || undefined,
      medicalHistory: raw.medicalHistory || undefined,
    };

    // TODO: add updatePatient(id, payload) to AdminPatientsService if not present
    this.service.updatePatient(id, payload).subscribe({
      next: () => {
        this.toast.success('Patient updated successfully.');
        this.router.navigate(['/admin/patients', id]);
      },
      error: () => {
        this.toast.error('Failed to update patient. Please try again.');
      },
    });
  }
}
