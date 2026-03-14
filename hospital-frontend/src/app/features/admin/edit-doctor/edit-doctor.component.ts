import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminAppointmentsService } from '../admin-appointments.service';

@Component({
  selector: 'app-edit-doctor',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './edit-doctor.component.html',
})
export class EditDoctorComponent implements OnInit {
  admins: any[] = [];
  filteredAdmins: any[] = [];
  selectedAdmin: any = null;
  showAdminPopup = false;
  adminSearch = '';

  form = this.fb.group({
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
    this.filteredAdmins = [];   
    this.showAdminPopup = true;
  }

  closeAdminPopup() {
    this.showAdminPopup = false;
  }

  onAdminSearch() {
    const term = this.adminSearch.toLowerCase().trim();
    if (!term) {
      this.filteredAdmins = [];
      return;
    }
    this.filteredAdmins = this.admins.filter(
      (a) =>
        `${a.firstName} ${a.lastName}`.toLowerCase().includes(term) ||
        a.employeeId?.toLowerCase().includes(term),
    );
  }

  selectAdmin(admin: any) {
    this.selectedAdmin = admin;
    this.form.patchValue({ assignedAdminId: String(admin.id) });
    this.closeAdminPopup();
  }

  submit() {
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

    this.service.updateDoctor(Number(id), payload).subscribe(() => {
      alert('Doctor updated');
      this.router.navigate(['/admin/doctors']);
    });
  }
}
